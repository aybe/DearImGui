using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace im.NET.Generator;

public sealed class ConsoleGeneratorRewriter : CSharpSyntaxRewriter
{
    private const string Internal = "__Internal";

    private const string Internal32 = "__Internal32";

    private const string Internal64 = "__Internal64";

    public ConsoleGeneratorRewriter(SyntaxNode root32, SyntaxNode root64)
    {
        Root32 = root32;
        Root64 = root64;
    }

    private SyntaxNode Root32 { get; }

    private SyntaxNode Root64 { get; }

    public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        node = ReplaceImports(node, Root32, Root64);
        return base.VisitClassDeclaration(node);
    }

    public override SyntaxNode? VisitStructDeclaration(StructDeclarationSyntax node)
    {
        node = ReplaceImports(node, Root32, Root64);
        return base.VisitStructDeclaration(node);
    }

    public override SyntaxNode? VisitBlock(BlockSyntax node)
    {
        var expressions = node.DescendantNodes().OfType<ExpressionStatementSyntax>();

        foreach (var expression in expressions)
        {
            if (expression.Expression is not InvocationExpressionSyntax invocation)
                continue;

            var identifier = TryGetIdentifier(invocation, Internal);
            if (identifier is null)
                continue;

            var ancestors = identifier.Ancestors().ToArray();

            if (ancestors.OfType<PointerTypeSyntax>().Any())
                continue;

            var invocation32 = ReplaceIdentifier(invocation, identifier, Internal32);
            var invocation64 = ReplaceIdentifier(invocation, identifier, Internal64);

            var statement =
                IfStatement(GetCondition(),
                        Block(ExpressionStatement(invocation32)))
                    .WithElse(ElseClause(
                        Block(ExpressionStatement(invocation64))))
                    .WithTriviaFrom(expression);

            node = node.ReplaceNode(expression, statement);
        }

        var equals = node.DescendantNodes().OfType<EqualsValueClauseSyntax>();

        foreach (var equal in equals)
        {
            if (equal.Value is not InvocationExpressionSyntax invocation)
                continue;

            var identifier = TryGetIdentifier(invocation, Internal);
            if (identifier is null)
                continue;

            var ancestors = identifier.Ancestors().ToArray();

            if (ancestors.OfType<SizeOfExpressionSyntax>().Any())
                continue;

            if (ancestors.OfType<PointerTypeSyntax>().Any())
                continue;

            var invocation32 = ReplaceIdentifier(invocation, identifier, Internal32);
            var invocation64 = ReplaceIdentifier(invocation, identifier, Internal64);

            var expression = ConditionalExpression(GetCondition(), invocation32, invocation64);

            node = node.ReplaceNode(equal, equal.WithValue(expression));
        }

        return base.VisitBlock(node);
    }

    private static BinaryExpressionSyntax GetCondition()
    {
        var expression =
            BinaryExpression(
                SyntaxKind.EqualsExpression,
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(nameof(IntPtr)),
                    IdentifierName(nameof(IntPtr.Size))),
                LiteralExpression(
                    SyntaxKind.NumericLiteralExpression,
                    Literal(4)));

        return expression;
    }

    private static StructDeclarationSyntax? GetStruct(BaseTypeDeclarationSyntax node, SyntaxNode root)
    {
        var outer = root
            .DescendantNodes()
            .OfType<TypeDeclarationSyntax>()
            .Single(s => s.Identifier.ValueText == node.Identifier.ValueText);

        var inner = GetStruct(outer);

        return inner;
    }

    private static StructDeclarationSyntax? GetStruct(TypeDeclarationSyntax node)
    {
        if (node == null)
            throw new ArgumentNullException(nameof(node));

        var syntax = node
            .Members
            .OfType<StructDeclarationSyntax>()
            .SingleOrDefault(s => s.Identifier.ValueText == Internal);

        return syntax;
    }

    private static bool HasMethods(TypeDeclarationSyntax node)
    {
        return node.Members.OfType<MethodDeclarationSyntax>().Any();
    }

    private static StructDeclarationSyntax RemoveAttributes(StructDeclarationSyntax node)
    {
        return node.WithAttributeLists(List<AttributeListSyntax>());
    }

    private static StructDeclarationSyntax RemoveFields(StructDeclarationSyntax node)
    {
        return node.WithMembers(List(node.Members.Where(s => s is not FieldDeclarationSyntax)));
    }

    private static StructDeclarationSyntax RemoveMethods(StructDeclarationSyntax node)
    {
        return node.WithMembers(List(node.Members.Where(s => s is not MethodDeclarationSyntax)));
    }

    private static StructDeclarationSyntax Rename(StructDeclarationSyntax node, string name)
    {
        return node.WithIdentifier(Identifier(SyntaxTriviaList.Empty, name, node.GetTrailingTrivia()));
    }

    private static InvocationExpressionSyntax ReplaceIdentifier(InvocationExpressionSyntax invocation, IdentifierNameSyntax identifier, string name)
    {
        if (invocation is null)
            throw new ArgumentNullException(nameof(invocation));

        if (identifier is null)
            throw new ArgumentNullException(nameof(identifier));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

        var node = invocation.ReplaceNode(identifier, IdentifierName(name));

        return node;
    }

    private static T ReplaceImports<T>(T node, SyntaxNode root32, SyntaxNode root64) where T : TypeDeclarationSyntax
    {
        var internalStruct = GetStruct(node);
        if (internalStruct == null)
            return node;

        // TODO remove now meaningless Size from StructLayoutAttribute when Sequential

        if (!HasMethods(node))
            return node;

        // remove methods from struct as they're useless now

        internalStruct = GetStruct(node)!;

        node = node.ReplaceNode(internalStruct, RemoveMethods(internalStruct));

        if (!HasMethods(internalStruct))
            return node;

        internalStruct = GetStruct(node)!;

        // inject renamed private structs and without fields

        var internal32 = RemoveAttributes(RemoveFields(Rename(GetStruct(node, root32)!, Internal32)));
        var internal64 = RemoveAttributes(RemoveFields(Rename(GetStruct(node, root64)!, Internal64)));

        internal32 = WithPrivatePartialModifiers(internal32);
        internal64 = WithPrivatePartialModifiers(internal64);

        node = node.InsertNodesAfter(internalStruct, new[] { internal32, internal64 });

        return node;
    }

    private static IdentifierNameSyntax? TryGetIdentifier(InvocationExpressionSyntax invocation, string name)
    {
        if (invocation is null)
            throw new ArgumentNullException(nameof(invocation));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

        var identifier = invocation
            .DescendantNodes()
            .OfType<IdentifierNameSyntax>()
            .SingleOrDefault(s => s.Identifier.ValueText == name);

        return identifier;
    }

    private static StructDeclarationSyntax WithPrivatePartialModifiers(StructDeclarationSyntax node)
    {
        return node
            .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.PartialKeyword)))
            .NormalizeWhitespace();
    }
}
using CppSharp.AST;
using CppSharp.Passes;

namespace ImGuiNET.Generator.Passes;

internal sealed class CleanupEnumerations : RenamePass
{
    public override bool Rename(Declaration decl, out string newName)
    {
        if (decl is not Enumeration.Item declaration)
        {
            return base.Rename(decl, out newName);
        }

        var name = declaration.Name;

        var namespaceName = declaration.Namespace.Name;

        if (name.StartsWith(namespaceName))
        {
            newName = name[namespaceName.Length..];
        }
        else
        {
            var index = name.IndexOf('_');

            if (index != -1)
            {
                newName = name[(index + 1)..];
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        newName = newName.Trim('_');

        if (char.IsDigit(newName[0]))
        {
            newName = $"D{newName}";
        }

        var rename = newName != name;

        return rename;
    }
}
﻿using im.NET.Generator.Passes;

namespace implot.NET.Generator.Passes;

public sealed class ImPlotSummaryPass : ImSummaryPass
{
    protected override string HeaderName { get; } = "implot.h";

    protected override string HeaderUrl { get; } = @"https://github.com/epezent/implot/blob/15e494b76a78b44ae2c1b76608ff9bc39a661409/implot.h";
}
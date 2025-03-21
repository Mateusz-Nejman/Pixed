﻿using Pixed.Core;

namespace Pixed.Application.Models;

internal readonly struct GridSettingsResult(int width, int height, UniColor color)
{
    public int Width { get; } = width;
    public int Height { get; } = height;
    public UniColor Color { get; } = color;
}
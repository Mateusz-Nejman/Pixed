using System;

namespace Pixed.Common.Extensions;
public readonly struct ExtensionToolEntry(string name, Type type)
{
    public string Name { get; } = name;
    public Type Type { get; } = type;
}
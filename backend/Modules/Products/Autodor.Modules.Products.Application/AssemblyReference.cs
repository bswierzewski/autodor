using System.Reflection;

namespace Autodor.Modules.Products.Application;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
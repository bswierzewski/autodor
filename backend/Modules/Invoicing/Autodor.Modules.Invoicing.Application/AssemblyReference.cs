using System.Reflection;

namespace Autodor.Modules.Invoicing.Application;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
using System.Reflection;

namespace Autodor.Modules.Contractors.Application;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
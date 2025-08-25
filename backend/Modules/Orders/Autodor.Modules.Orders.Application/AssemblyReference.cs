using System.Reflection;

namespace Autodor.Modules.Orders.Application;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
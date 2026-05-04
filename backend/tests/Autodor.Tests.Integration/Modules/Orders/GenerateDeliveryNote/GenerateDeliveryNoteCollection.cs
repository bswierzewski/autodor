namespace Autodor.Tests.Integration.Modules.Orders.GenerateDeliveryNote;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class GenerateDeliveryNoteCollection : ICollectionFixture<GenerateDeliveryNoteEnvironment>
{
    public const string Name = "GenerateDeliveryNote";
}
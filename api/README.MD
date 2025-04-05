Database migration
dotnet ef migrations add "Init" --project src/Infrastructure --startup-project src/Web/ --output-dir Data/Migrations

TS generate
dotnet tool install --global Swashbuckle.AspNetCore.Cli
<Exec Command="swagger tofile --output ../../docs/snippetApi.yaml --yaml $(OutputPath)$(AssemblyName).dll v1" WorkingDirectory="$(ProjectDir)" />

<Target Name="OpenAPI" AfterTargets="Build" Condition="$(Configuration)=='Debug'">
<Exec Command="swagger tofile --output ../../docs/autodor.yaml --yaml $(OutputPath)$(AssemblyName).dll v1" WorkingDirectory="$(ProjectDir)" />
</Target>

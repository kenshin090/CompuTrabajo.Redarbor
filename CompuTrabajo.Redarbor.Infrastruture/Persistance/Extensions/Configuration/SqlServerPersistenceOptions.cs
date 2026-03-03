namespace CompuTrabajo.Redarbor.Infrastruture.Persistance.Extensions.Configuration
{
    public sealed record SqlServerPersistenceOptions
    {
        public string ConnectionStringName { get; init; } = default!;
        public bool EnableSensitiveDataLogging { get; init; } = false;
        public bool EnableDetailedErrors { get; init; } = true;
        public int CommandTimeoutSeconds { get; init; } = 30;
    }
}

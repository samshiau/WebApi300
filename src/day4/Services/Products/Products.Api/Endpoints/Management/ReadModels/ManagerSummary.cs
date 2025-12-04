namespace Products.Api.Endpoints.Management.ReadModels;

public record ManagerSummary
{
    public Guid Id { get; init; }
    public int Version { get; init; }
    public string Sub { get; set; }

    public Dictionary<string, ManagerProductItem> ProductsCreated { get; init; } = new();
}

public record ManagerProductItem(string Name, DateTimeOffset CreatedDate);
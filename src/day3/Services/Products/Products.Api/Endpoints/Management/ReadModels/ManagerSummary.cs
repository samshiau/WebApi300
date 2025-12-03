namespace Products.Api.Endpoints.Management.ReadModels;

public record ManagerSummary
{
    public Guid Id { get; init; }
    public int Version { get; init; }
    public string Sub { get; init; } = string.Empty;
    public List<Guid> CreatedProductIds { get; init; } = new();
}
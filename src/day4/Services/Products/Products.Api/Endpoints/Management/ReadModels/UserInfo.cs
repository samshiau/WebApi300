namespace Products.Api.Endpoints.Management.ReadModels;

public record UserInfo
{
    public required Guid Id { get; init; }
    public int Version { get; set; }
    public required string Sub { get; init; }
}
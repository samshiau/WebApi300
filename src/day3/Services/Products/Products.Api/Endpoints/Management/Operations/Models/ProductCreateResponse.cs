using Facet;
using Products.Api.Endpoints.Management.Handlers;

namespace Products.Api.Endpoints.Management.Operations.Models;

[Facet(typeof(CreateProduct))]
public partial record ProductCreateResponse
{
    public string Status => "Pending";
}
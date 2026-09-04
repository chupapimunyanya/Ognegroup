using Application.Interfaces;
using Domain;
using MediatR;

namespace Ognegroup.Application.Queries;

// Query model. Says: "I want to get a list of products".
public class GetAllProductsQuery : IRequest<IEnumerable<Product>>
{
}

// Handler. Knows where to get them from (calls the repository).
public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<Product>>
{
    private readonly IProductRepository _repository;

    public GetAllProductsQueryHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Product>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync();
    }
}
using Domain;

namespace Application.Interfaces;

public interface ISupplierRepository
{
    Task<Supplier?> GetByIdAsync(int id);
    Task AddAsync(Supplier supplier, CancellationToken cancellationToken = default);
}
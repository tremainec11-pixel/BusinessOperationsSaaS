using BusinessOperationsSaaS.Application.Products.DTOs;

namespace BusinessOperationsSaaS.Application.Products.Interfaces;

public interface IProductService
{
    Task<List<ProductResponse>> GetAllAsync(Guid companyId);

    Task<ProductResponse?> GetByIdAsync(
        Guid companyId,
        Guid productId);

    Task<ProductResponse> CreateAsync(
        Guid companyId,
        CreateProductRequest request);

    Task<ProductResponse?> UpdateAsync(
        Guid companyId,
        Guid productId,
        UpdateProductRequest request);

    Task<bool> DeleteAsync(
        Guid companyId,
        Guid productId);
}

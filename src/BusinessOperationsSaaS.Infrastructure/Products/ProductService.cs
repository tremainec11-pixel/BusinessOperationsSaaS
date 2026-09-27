using BusinessOperationsSaaS.Application.Products.DTOs;
using BusinessOperationsSaaS.Application.Products.Interfaces;
using BusinessOperationsSaaS.Application.Subscriptions.Services;
using BusinessOperationsSaaS.Domain.Entities;
using BusinessOperationsSaaS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperationsSaaS.Infrastructure.Products;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;
    private readonly ISubscriptionLimitService _subscriptionLimitService;

    public ProductService(
        AppDbContext context,
        ISubscriptionLimitService subscriptionLimitService)
    {
        _context = context;
        _subscriptionLimitService = subscriptionLimitService;
    }

    public async Task<List<ProductResponse>> GetAllAsync(Guid companyId)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(x => x.CompanyId == companyId)
            .OrderBy(x => x.Name)
            .ThenBy(x => x.CreatedAt)
            .Select(x => new ProductResponse
            {
                Id = x.Id,
                CompanyId = x.CompanyId,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price,
                Stock = x.Stock,
                SKU = x.SKU,
                Category = x.Category,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<ProductResponse?> GetByIdAsync(
        Guid companyId,
        Guid productId)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(x =>
                x.Id == productId &&
                x.CompanyId == companyId)
            .Select(x => new ProductResponse
            {
                Id = x.Id,
                CompanyId = x.CompanyId,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price,
                Stock = x.Stock,
                SKU = x.SKU,
                Category = x.Category,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ProductResponse> CreateAsync(
        Guid companyId,
        CreateProductRequest request)
    {
        var canCreate =
            await _subscriptionLimitService
                .CanCreateProductAsync(companyId);

        if (!canCreate)
        {
            throw new InvalidOperationException(
                "Product limit reached for the current subscription plan.");
        }

        var sku = request.SKU?.Trim();

        if (!string.IsNullOrWhiteSpace(sku))
        {
            var skuExists = await _context.Products
                .AnyAsync(x =>
                    x.CompanyId == companyId &&
                    x.SKU == sku);

            if (skuExists)
            {
                throw new InvalidOperationException(
                    "A product with this SKU already exists.");
            }
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            Price = request.Price,
            Stock = request.Stock,
            SKU = sku,
            Category = request.Category?.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);

        await _context.SaveChangesAsync();

        return new ProductResponse
        {
            Id = product.Id,
            CompanyId = product.CompanyId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            SKU = product.SKU,
            Category = product.Category,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt
        };
    }

    public async Task<ProductResponse?> UpdateAsync(
        Guid companyId,
        Guid productId,
        UpdateProductRequest request)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(x =>
                x.Id == productId &&
                x.CompanyId == companyId);

        if (product is null)
        {
            return null;
        }

        var sku = request.SKU?.Trim();

        if (!string.IsNullOrWhiteSpace(sku))
        {
            var skuExists = await _context.Products
                .AnyAsync(x =>
                    x.CompanyId == companyId &&
                    x.SKU == sku &&
                    x.Id != productId);

            if (skuExists)
            {
                throw new InvalidOperationException(
                    "A product with this SKU already exists.");
            }
        }

        product.Name = request.Name.Trim();
        product.Description = request.Description?.Trim();
        product.Price = request.Price;
        product.Stock = request.Stock;
        product.SKU = sku;
        product.Category = request.Category?.Trim();
        product.IsActive = request.IsActive;

        await _context.SaveChangesAsync();

        return new ProductResponse
        {
            Id = product.Id,
            CompanyId = product.CompanyId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            SKU = product.SKU,
            Category = product.Category,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt
        };
    }

    public async Task<bool> DeleteAsync(
        Guid companyId,
        Guid productId)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(x =>
                x.Id == productId &&
                x.CompanyId == companyId);

        if (product is null)
        {
            return false;
        }

        _context.Products.Remove(product);

        await _context.SaveChangesAsync();

        return true;
    }
}
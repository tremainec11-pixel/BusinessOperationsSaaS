namespace BusinessOperationsSaaS.Application.Products.DTOs;

public class ProductResponse
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public decimal Price { get; set; }
    public int Stock { get; set; }

    public string? SKU { get; set; }
    public string? Category { get; set; }

    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

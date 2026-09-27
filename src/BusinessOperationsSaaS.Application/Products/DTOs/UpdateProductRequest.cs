namespace BusinessOperationsSaaS.Application.Products.DTOs;

public class UpdateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public decimal Price { get; set; }
    public int Stock { get; set; }

    public string? SKU { get; set; }
    public string? Category { get; set; }

    public bool IsActive { get; set; }
}

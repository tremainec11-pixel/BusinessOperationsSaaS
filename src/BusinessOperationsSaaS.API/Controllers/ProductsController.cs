using BusinessOperationsSaaS.Application.Products.DTOs;
using BusinessOperationsSaaS.Application.Products.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BusinessOperationsSaaS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductResponse>>> GetAll()
    {
        var companyId = PublicCompanyId;

        

        var products = await _productService.GetAllAsync(companyId);

        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> GetById(Guid id)
    {
        var companyId = PublicCompanyId;

        

        var product = await _productService.GetByIdAsync(
            companyId,
            id);

        if (product is null)
        {
            return NotFound(new
            {
                message = "Product not found."
            });
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Create(
        CreateProductRequest request)
    {
        var companyId = PublicCompanyId;

        

        try
        {
            var product = await _productService.CreateAsync(
                companyId,
                request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = product.Id },
                product);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> Update(
        Guid id,
        UpdateProductRequest request)
    {
        var companyId = PublicCompanyId;

        

        try
        {
            var product = await _productService.UpdateAsync(
                companyId,
                id,
                request);

            if (product is null)
            {
                return NotFound(new
                {
                    message = "Product not found."
                });
            }

            return Ok(product);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var companyId = PublicCompanyId;

        

        var deleted = await _productService.DeleteAsync(
            companyId,
            id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Product not found."
            });
        }

        return NoContent();
    }

    private static readonly Guid PublicCompanyId = Guid.Parse("feb88165-b70c-429e-83a3-94d412dca312");
}

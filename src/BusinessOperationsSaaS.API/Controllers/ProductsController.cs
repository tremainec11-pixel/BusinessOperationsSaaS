using System.Security.Claims;
using BusinessOperationsSaaS.Application.Products.DTOs;
using BusinessOperationsSaaS.Application.Products.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessOperationsSaaS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
        var companyId = GetCompanyId();

        if (companyId is null)
        {
            return Unauthorized(new
            {
                message = "Company information is missing from the token."
            });
        }

        var products = await _productService.GetAllAsync(companyId.Value);

        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> GetById(Guid id)
    {
        var companyId = GetCompanyId();

        if (companyId is null)
        {
            return Unauthorized(new
            {
                message = "Company information is missing from the token."
            });
        }

        var product = await _productService.GetByIdAsync(
            companyId.Value,
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
        var companyId = GetCompanyId();

        if (companyId is null)
        {
            return Unauthorized(new
            {
                message = "Company information is missing from the token."
            });
        }

        try
        {
            var product = await _productService.CreateAsync(
                companyId.Value,
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
        var companyId = GetCompanyId();

        if (companyId is null)
        {
            return Unauthorized(new
            {
                message = "Company information is missing from the token."
            });
        }

        try
        {
            var product = await _productService.UpdateAsync(
                companyId.Value,
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
        var companyId = GetCompanyId();

        if (companyId is null)
        {
            return Unauthorized(new
            {
                message = "Company information is missing from the token."
            });
        }

        var deleted = await _productService.DeleteAsync(
            companyId.Value,
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

    private Guid? GetCompanyId()
    {
        var companyIdClaim = User.FindFirst("companyId")?.Value;

        if (Guid.TryParse(companyIdClaim, out var companyId))
        {
            return companyId;
        }

        return null;
    }
}
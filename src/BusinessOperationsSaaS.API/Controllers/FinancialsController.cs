using System.Security.Claims;
using BusinessOperationsSaaS.Application.Financials.DTOs;
using BusinessOperationsSaaS.Application.Financials.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessOperationsSaaS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FinancialsController : ControllerBase
{
    private readonly IFinancialService _financialService;

    public FinancialsController(IFinancialService financialService)
    {
        _financialService = financialService;
    }

    [HttpGet]
    public async Task<ActionResult<List<FinancialTransactionResponse>>> GetAll()
    {
        var companyId = GetCompanyId();

        if (companyId is null)
        {
            return Unauthorized(new
            {
                message = "Company information is missing from the token."
            });
        }

        var transactions = await _financialService.GetAllAsync(
            companyId.Value);

        return Ok(transactions);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FinancialTransactionResponse>> GetById(
        Guid id)
    {
        var companyId = GetCompanyId();

        if (companyId is null)
        {
            return Unauthorized(new
            {
                message = "Company information is missing from the token."
            });
        }

        var transaction = await _financialService.GetByIdAsync(
            companyId.Value,
            id);

        if (transaction is null)
        {
            return NotFound(new
            {
                message = "Financial transaction not found."
            });
        }

        return Ok(transaction);
    }

    [HttpPost]
    public async Task<ActionResult<FinancialTransactionResponse>> Create(
        CreateFinancialTransactionRequest request)
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
            var transaction = await _financialService.CreateAsync(
                companyId.Value,
                request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = transaction.Id },
                transaction);
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
    public async Task<ActionResult<FinancialTransactionResponse>> Update(
        Guid id,
        UpdateFinancialTransactionRequest request)
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
            var transaction = await _financialService.UpdateAsync(
                companyId.Value,
                id,
                request);

            if (transaction is null)
            {
                return NotFound(new
                {
                    message = "Financial transaction not found."
                });
            }

            return Ok(transaction);
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

        var deleted = await _financialService.DeleteAsync(
            companyId.Value,
            id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Financial transaction not found."
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
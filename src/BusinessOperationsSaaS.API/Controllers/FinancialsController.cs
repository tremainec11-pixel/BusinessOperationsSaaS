using BusinessOperationsSaaS.Application.Financials.DTOs;
using BusinessOperationsSaaS.Application.Financials.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BusinessOperationsSaaS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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
        var companyId = PublicCompanyId;

        

        var transactions = await _financialService.GetAllAsync(
            companyId);

        return Ok(transactions);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FinancialTransactionResponse>> GetById(
        Guid id)
    {
        var companyId = PublicCompanyId;

        

        var transaction = await _financialService.GetByIdAsync(
            companyId,
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
        var companyId = PublicCompanyId;

        

        try
        {
            var transaction = await _financialService.CreateAsync(
                companyId,
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
        var companyId = PublicCompanyId;

        

        try
        {
            var transaction = await _financialService.UpdateAsync(
                companyId,
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
        var companyId = PublicCompanyId;

        

        var deleted = await _financialService.DeleteAsync(
            companyId,
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

    private static readonly Guid PublicCompanyId = Guid.Parse("feb88165-b70c-429e-83a3-94d412dca312");
}

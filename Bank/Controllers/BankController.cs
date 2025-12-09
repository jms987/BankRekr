using Bank.Service.BankService;
using Bank.Service.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Controllers;

[ApiController]
[Route("Bank")]
public class BankController : Controller
{
    private readonly IBankService _bankService;
    private readonly ILogger<BankController> _logger;

    public BankController(ILogger<BankController> logger, IBankService bankService)
    {
        _logger = logger;
        _bankService = bankService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()

    {
        try
        {
            var result = await _bankService.GetAllBanksAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all banks");
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        if (id == Guid.Empty) return BadRequest("Invalid bank ID");

        try
        {
            var result = await _bankService.GetBankByIdAsync(id);

            if (result?.Bank == null) return NotFound($"Bank with ID {id} not found");

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting bank with ID {BankId}", id);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AddBank bank)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            await _bankService.CreateBankAsync(bank);
            return CreatedAtAction(nameof(GetById), new { id = Guid.NewGuid() }, bank);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating bank");
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBank bank)
    {
        if (id == Guid.Empty) return BadRequest("Invalid bank ID");

        if (bank.Id != id) return BadRequest("ID in URL does not match ID in body");

        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            await _bankService.UpdateBankAsync(bank);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Bank with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating bank with ID {BankId}", id);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (id == Guid.Empty) return BadRequest("Invalid bank ID");

        try
        {
            await _bankService.DeleteBankAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Bank with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting bank with ID {BankId}", id);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }
}
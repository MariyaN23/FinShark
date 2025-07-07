using api.Dtos.Stock;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/stock")]
[ApiController]
public class StockController : ControllerBase
{
    private readonly IStockRepository _stockRepo;

    public StockController(IStockRepository stockRepo)
    {
        _stockRepo = stockRepo;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll([FromQuery] QueryObject query)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var stock = await _stockRepo.GetAllAsync(query);
        var stockDto = stock.Select(s => s.ToStockDto()).ToList();
        return Ok(stockDto);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var stock = await _stockRepo.GetByIdAsync(id);
        if (stock == null) return NotFound();
        return Ok(stock.ToStockDto());
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStockRequestDto stockDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var stock = stockDto.ToStockFromCreateDto();
        await _stockRepo.CreateAsync(stock);
        return CreatedAtAction(nameof(GetById), new { id = stock.Id }, stock.ToStockDto());
    }

    //[HttpPut]
    //[Route("{id}")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStockRequestDto updateDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var stock = await _stockRepo.UpdateAsync(id, updateDto);
        if (stock == null) return NotFound();
        return Ok(stock.ToStockDto());
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var stock = await _stockRepo.DeleteAsync(id);
        if (stock == null) return NotFound();
        return NoContent();
    }
}
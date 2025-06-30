using api.Data;
using api.Dtos.Stock;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/stock")]
[ApiController]
public class StockController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public StockController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var stock = _context.Stocks.ToList().Select(s => s.ToStockDto());
        return Ok(stock);
    }

    [HttpGet("{id}")]
    public IActionResult GetById([FromRoute] int id)
    {
        var stock = _context.Stocks.Find(id);
        if (stock == null) return NotFound();
        return Ok(stock.ToStockDto());
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateStockRequestDto stockDto)
    {
        var stockModel = stockDto.ToStockFromCreateDto();
        _context.Stocks.Add(stockModel);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetById), new { id = stockModel.Id }, stockModel.ToStockDto());
    }

    [HttpPut]
    [Route("{id}")]
    public IActionResult Update([FromRoute] int id, [FromBody] UpdateStockRequestDto updateDto)
    {
        var stock = _context.Stocks.FirstOrDefault(x => x.Id == id);
        if (stock == null) return NotFound();

        stock.Symbol = updateDto.Symbol;
        stock.CompanyName = updateDto.CompanyName;
        stock.Purchase = updateDto.Purchase;
        stock.LastDiv = updateDto.LastDiv;
        stock.Industry = updateDto.Industry;
        stock.MarketCap = updateDto.MarketCap;

        _context.SaveChanges();
        return Ok(stock.ToStockDto());
    }
}
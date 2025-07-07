using api.Dtos.Comment;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/comment")]
[ApiController]
public class CommentController : ControllerBase
{
    private readonly ICommentRepository _commentRepo;
    private readonly IStockRepository _stockRepo;
    private readonly UserManager<AppUser> _userManager;
    private readonly IFMPService _fmpService;

    public CommentController(ICommentRepository commentRepo, IStockRepository stockRepo, UserManager<AppUser> userManager, IFMPService fmpService)
    {
        _commentRepo = commentRepo;
        _stockRepo = stockRepo;
        _userManager = userManager;
        _fmpService = fmpService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var comments = await _commentRepo.GetAllAsync();
        var commentsDto = comments.Select(s => s.ToCommentDto());
        return Ok(commentsDto);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var comment = await _commentRepo.GetByIdAsync(id);
        if (comment == null) return NotFound();
        return Ok(comment.ToCommentDto());
    }

    [HttpPost("{symbol:alpha}")]
    public async Task<IActionResult> Create([FromRoute] string symbol, CreateCommentDto commentDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var stock = await _stockRepo.GetBySymbolAsync(symbol);
        if (stock == null)
        {
            stock = await _fmpService.FindStockBySymbolAsync(symbol);
            if (stock == null) return BadRequest("Stock does not exist");
            await _stockRepo.CreateAsync(stock);
        }
        
        var userName = User.GetUsername();
        var appUser = await _userManager.FindByNameAsync(userName);
        
        var comment = commentDto.ToCommentFromCreateDto(stock.Id);
        comment.AppUserId = appUser.Id;
        await _commentRepo.CreateAsync(comment);
        return CreatedAtAction(nameof(GetById), new { id = comment.Id }, comment.ToCommentDto());
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCommentRequestDto updateDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var comment = await _commentRepo.UpdateAsync(id, updateDto);
        if (comment == null) return NotFound();
        return Ok(comment.ToCommentDto());
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var comment = await _commentRepo.DeleteAsync(id);
        if (comment == null) return NotFound();
        return NoContent();
    }
}
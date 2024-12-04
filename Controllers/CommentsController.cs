using api.Dtos.Comments;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/comments")]
public class CommentsController
(ICommentRepository commentRepository,
 IStockRepository stockRepository,
 UserManager<AppUser> userManager,
 IFMPService fmpService) : ControllerBase
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly ICommentRepository _commentRepository = commentRepository;
    private readonly IStockRepository _stockRepository = stockRepository;
    private readonly IFMPService _fmpService = fmpService;

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] CommentQueryObject queryObject)
    {
        var comments = await _commentRepository.GetAllAsync(queryObject);

        var commentDtos = comments.Select(s => s.ToCommentDto());

        return Ok(commentDtos);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
    {
        var comment = await _commentRepository.GetByIdAsync(id);

        if (comment == null)
        {
            return NotFound();
        }

        return Ok(comment.ToCommentDto());
    }

    [HttpPost]
    [Route("{symbol:alpha}")]
    public async Task<IActionResult> CreateAsync([FromRoute] string symbol, [FromBody] CreateCommentDto commentDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var stock = await _stockRepository.GetBySymbolAsync(symbol);

        if (stock == null)
        {
            stock = await _fmpService.FindStockBySymbolAsync(symbol);
            if (stock == null)
            {
                return BadRequest("Stock does not exist");
            }

            await _stockRepository.CreateAsync(stock);
        }

        var userName = User.GetUsername();
        var appUser = await _userManager.FindByNameAsync(userName);

        var commentModel = commentDto.ToCommentFromCreate(stock.Id);

        commentModel.AppUserId = appUser.Id;

        await _commentRepository.CreateAsync(commentModel);

        return Ok();
        // return CreatedAtAction(nameof(GetByIdAsync), new { id = commentModel.Id }, commentModel.ToCommentDto());
    }

    [HttpPut]
    [Route("{id:int}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] UpdateCommentRequestDto updateDto)
    {
        var comment = await _commentRepository.UpdateAsync(id, updateDto.ToCommentFromUpdate());

        if (comment == null)
        {
            return NotFound("Comment not found");
        }

        return Ok(comment.ToCommentDto());
    }

    [HttpDelete]
    [Route("{id:int}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id)
    {
        var comment = await _commentRepository.DeleteAsync(id);

        if (comment == null)
        {
            return NotFound("Comment not found");
        }

        return NoContent();
    }
}
using api.Dtos.Comments;
using api.Interfaces;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/comments")]
public class CommentsController
(ICommentRepository commentRepository,
 IStockRepository stockRepository) : ControllerBase
{
    private readonly ICommentRepository _commentRepository = commentRepository;
    private readonly IStockRepository _stockRepository = stockRepository;

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var comments = await _commentRepository.GetAllAsync();

        var commentDtos = comments.Select(s => s.ToCommentDto());

        return Ok(commentDtos);
    }

    [HttpGet("{id}")]
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
    [Route("{stockId}")]
    public async Task<IActionResult> CreateAsync([FromRoute] int stockId, [FromBody] CreateCommentDto commentDto)
    {
        if (!await _stockRepository.CheckExistingStock(stockId))
        {
            return BadRequest("Stock does not exist");
        }

        var commentModel = commentDto.ToCommentFromCreate(stockId);

        await _commentRepository.CreateAsync(commentModel);

        return Ok();
        // return CreatedAtAction(nameof(GetByIdAsync), new { id = commentModel.Id }, commentModel.ToCommentDto());
    }
}
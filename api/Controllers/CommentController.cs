using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.Extentions;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _CommentRepository;
        private readonly IStockRepository _StockRepository;
        private readonly UserManager<AppUser> _userManager;

        public CommentController(ICommentRepository commentRepository, IStockRepository stockRepository, UserManager<AppUser> userManager)
        {
            _CommentRepository = commentRepository;
            _StockRepository = stockRepository;
            _userManager = userManager;

        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var comments = await _CommentRepository.GetAllAsync();
            var commentsDto = comments.Select(c => c.ToCommentDto());
            return Ok(commentsDto);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var comment = await _CommentRepository.GetByIdAsync(id);
            if (comment is null)
            {
                return NotFound();
            }
            return Ok(comment.ToCommentDto());
        }
        [HttpPost("{stockId:int}")]
        public async Task<IActionResult> Create([FromRoute] int stockId,
        [FromBody] CreateCommentDto createCommentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!await _StockRepository.IsStockExistsAsync(stockId))
            {
                return BadRequest("Specified stock do not exist");
            }

            var user = await _userManager.FindByNameAsync(User.GetUserName());
            var commentModel = createCommentDto.ToCommentFromCreateCommentDto(stockId);
            commentModel.AppUserId = user.Id;
            await _CommentRepository.CreateAsync(commentModel);

            return CreatedAtAction(nameof(GetById), new { id = commentModel.Id }, commentModel.ToCommentDto());
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCommentDto updateCommentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var commentmodel = await _CommentRepository.UpdateAsync(id, updateCommentDto);
            if (commentmodel is null)
            {
                return NotFound();
            }
            return Ok(commentmodel.ToCommentDto());
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var comment = await _CommentRepository.DeleteAsync(id);
            if (comment is null)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
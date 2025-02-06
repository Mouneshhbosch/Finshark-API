using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Stock;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/stock")]
    public class StockController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IStockRepository _stockRepo;

        public StockController(ApplicationDbContext context, IStockRepository stockRepository)
        {
            _context = context;
            _stockRepo = stockRepository;

        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject queryObject)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var stocks = await _stockRepo.GetAllAsync(queryObject);
            var stocksDto = stocks.Select(s => s.ToStockDto()).ToList();
            return Ok(stocksDto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var stock = await _stockRepo.GetByIdAsync(id);
            if (stock is null)
            {
                return NotFound();
            }
            return Ok(stock.ToStockDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStockDto createStockDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var stockModel = createStockDto.ToStockFromCreateStockDto();
            await _stockRepo.CreateAsync(stockModel);
            return CreatedAtAction(nameof(GetById), new { id = stockModel.Id }, stockModel.ToStockDto());
        }


        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStockDto updateStockDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // var stockModel = await _context.Stocks.FirstOrDefaultAsync(s => s.Id == id);
            // if (stockModel is null)
            // {
            //     return NotFound();
            // }
            // stockModel.Symbol = updateStockDto.Symbol;
            // stockModel.CompanyName = updateStockDto.CompanyName;
            // stockModel.Purchase = updateStockDto.Purchase;
            // stockModel.LastDiv = updateStockDto.LastDiv;
            // stockModel.Industry = updateStockDto.Industry;
            // stockModel.MarketCap = updateStockDto.MarketCap;
            // await _context.SaveChangesAsync();
            var stockModel = await _stockRepo.UpdateAsync(id, updateStockDto);
            if (stockModel is null)
            {
                return NotFound();
            }

            return Ok(stockModel.ToStockDto());
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var stockModel = await _stockRepo.DeleteAsync(id);
            if (stockModel is null)
            {
                return NotFound();
            }
            return NoContent();
        }
    }

}
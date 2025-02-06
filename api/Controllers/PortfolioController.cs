using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Extentions;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class PortfolioController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IStockRepository _stockRepository;
        private readonly IPortfolioRepository _portfolioRepository;

        public PortfolioController(UserManager<AppUser> userManager, IStockRepository stockRepository, IPortfolioRepository portfolioRepository)
        {
            _userManager = userManager;
            _stockRepository = stockRepository;
            _portfolioRepository = portfolioRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetUserPortfolio()
        {
            var user = User.GetUserName();
            var appUser = await _userManager.FindByNameAsync(user);

            var usePortFoloio = await _portfolioRepository.GetUserPortfolio(appUser);
            return Ok(usePortFoloio);
        }

        [HttpPost]
        public async Task<IActionResult> AddStockToPortfolio(string symbol)
        {
            var user = User.GetUserName();
            var appUser = await _userManager.FindByNameAsync(user);

            var stock = await _stockRepository.GetBySymbolAsync(symbol);
            if (stock is null)
            {
                return BadRequest("Stock not found");
            }

            var userPortFolio = await _portfolioRepository.GetUserPortfolio(appUser);

            if (userPortFolio.Any(p => p.Symbol.ToLower() == stock.Symbol.ToLower()))
            {
                return BadRequest("Stock already in portfolio");
            }

            var portfolio = new Portfolio
            {
                AppUserId = appUser.Id,
                StockId = stock.Id,
            };

            await _portfolioRepository.CreateAsync(portfolio);

            if (portfolio is null)
            {
                return BadRequest("Failed to add stock to portfolio");
            }
            return Created();
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveStockFromPortfolio(string symbol)
        {
            var user = User.GetUserName();
            var appUser = await _userManager.FindByNameAsync(user);

            var stock = await _stockRepository.GetBySymbolAsync(symbol);
            if (stock is null)
            {
                return BadRequest("Stock not found");
            }

            var userPortFolio = await _portfolioRepository.GetUserPortfolio(appUser);

            var portfolio = userPortFolio.Where(x => x.Symbol.ToLower() == stock.Symbol.ToLower()).FirstOrDefault();

            if (portfolio is not null)
            {
                await _portfolioRepository.DeleteAync(appUser, stock.Symbol);
            }
            else
            {
                return BadRequest("Stock not found in portfolio");
            }

            return Ok();
        }
    }
}
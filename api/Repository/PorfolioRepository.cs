using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class PorfolioRepository : IPortfolioRepository
    {
        private readonly ApplicationDbContext _context;

        public PorfolioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Portfolio> CreateAsync(Portfolio portfolio)
        {
            await _context.Portfolios.AddAsync(portfolio);
            await _context.SaveChangesAsync();
            return portfolio;
        }

        public async Task<Portfolio?> DeleteAync(AppUser appUser, string symbol)
        {
            var portfolio = _context.Portfolios.FirstOrDefault(x => x.AppUserId == appUser.Id && x.Stock.Symbol == symbol);
            if (portfolio is null)
            {
                return null;
            }
            _context.Portfolios.Remove(portfolio);
            await _context.SaveChangesAsync();
            return portfolio;
        }

        public async Task<List<Stock>> GetUserPortfolio(AppUser user)
        {
            return await _context.Portfolios.Where(x => x.AppUserId == user.Id)
            .Select(x => new Stock
            {
                Id = x.Stock.Id,
                CompanyName = x.Stock.CompanyName,
                Purchase = x.Stock.Purchase,
                Symbol = x.Stock.Symbol,
                LastDiv = x.Stock.LastDiv,
                Industry = x.Stock.Industry,
                MarketCap = x.Stock.MarketCap,
            }).ToListAsync();
        }
    }
}
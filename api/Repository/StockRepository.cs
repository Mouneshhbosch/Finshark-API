using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Stock;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDbContext _context;
        public StockRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Stock> CreateAsync(Stock stockModel)
        {
            await _context.Stocks.AddAsync(stockModel);
            await _context.SaveChangesAsync();
            return stockModel;
        }

        public async Task<Stock?> DeleteAsync(int id)
        {
            var stockModel = await _context.Stocks.FirstOrDefaultAsync(s => s.Id == id);
            if (stockModel is null)
            {
                return null;
            }
            _context.Stocks.Remove(stockModel);
            await _context.SaveChangesAsync();
            return stockModel;
        }

        public async Task<List<Stock>> GetAllAsync(QueryObject queryObject)
        {
            var stocks = _context.Stocks.Include(c => c.Comments).ThenInclude(x => x.AppUser).AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryObject.Symbol))
            {
                stocks = stocks.Where(s => s.Symbol.Contains(queryObject.Symbol));
            }

            if (!string.IsNullOrWhiteSpace(queryObject.CompanyName))
            {
                stocks = stocks.Where(s => s.CompanyName.Contains(queryObject.CompanyName));
            }

            if (!string.IsNullOrWhiteSpace(queryObject.SortBy))
            {
                if (queryObject.SortBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
                {
                    stocks = queryObject.IsDescending
                    ? stocks.OrderByDescending(s => s.Symbol) : stocks.OrderBy(s => s.Symbol);
                }
            }

            var skipNumber = (queryObject.PageNumber - 1) * queryObject.PageSize;

            return await stocks.Skip(skipNumber).Take(queryObject.PageSize).ToListAsync();
        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            return await _context.Stocks.Include(c => c.Comments).ThenInclude(c => c.AppUser).FirstOrDefaultAsync(i => i.Id == id);
        }

        public Task<Stock?> GetBySymbolAsync(string symbol)
        {
            return _context.Stocks.FirstOrDefaultAsync(s => s.Symbol == symbol);
        }

        public async Task<bool> IsStockExistsAsync(int id)
        {
            return await _context.Stocks.AnyAsync(i => i.Id == id);
        }

        public async Task<Stock?> UpdateAsync(int id, UpdateStockDto stockDto)
        {
            var stockModel = await _context.Stocks.FirstOrDefaultAsync(s => s.Id == id);
            if (stockModel is null)
            {
                return null;
            }
            stockModel.Symbol = stockDto.Symbol;
            stockModel.CompanyName = stockDto.CompanyName;
            stockModel.Purchase = stockDto.Purchase;
            stockModel.LastDiv = stockDto.LastDiv;
            stockModel.Industry = stockDto.Industry;
            stockModel.MarketCap = stockDto.MarketCap;
            await _context.SaveChangesAsync();
            return stockModel;
        }
    }
}
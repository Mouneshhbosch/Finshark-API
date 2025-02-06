using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Stock;
using api.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace api.Mappers
{
    public static class StockMappers
    {
        public static StockDto ToStockDto(this Stock stockModel)
        {
            return new StockDto()
            {
                Id = stockModel.Id,
                Symbol = stockModel.Symbol,
                CompanyName = stockModel.CompanyName,
                Purchase = stockModel.Purchase,
                LastDiv = stockModel.LastDiv,
                Industry = stockModel.Industry,
                MarketCap = stockModel.MarketCap,
                Comments = stockModel.Comments.Select(c => c.ToCommentDto()).ToList()
            };
        }
        public static Stock ToStockFromCreateStockDto(this CreateStockDto createStockDto)
        {
            return new Stock()
            {
                Symbol = createStockDto.Symbol,
                CompanyName = createStockDto.CompanyName,
                Purchase = createStockDto.Purchase,
                LastDiv = createStockDto.LastDiv,
                Industry = createStockDto.Industry,
                MarketCap = createStockDto.MarketCap
            };
        }
        public static Stock UpdateStockFromUpdateStockDto(this UpdateStockDto updateStockDto)
        {
            return new Stock()
            {
                Symbol = updateStockDto.Symbol,
                CompanyName = updateStockDto.CompanyName,
                Purchase = updateStockDto.Purchase,
                LastDiv = updateStockDto.LastDiv,
                Industry = updateStockDto.Industry,
                MarketCap = updateStockDto.MarketCap
            };
        }
    }
}
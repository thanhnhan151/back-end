using api.Dtos.Stocks;
using api.Models;

namespace api.Mappers;
public static class StockMappers
{
    public static StockDto ToStockDto(this Stock stockModel)
    {
        return new StockDto
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

    public static Stock ToStockFromCreateDto(this CreateStockRequestDto request)
    {
        return new Stock
        {
            Symbol = request.Symbol,
            CompanyName = request.CompanyName,
            Purchase = request.Purchase,
            LastDiv = request.LastDiv,
            Industry = request.Industry,
            MarketCap = request.MarketCap
        };
    }

    public static Stock ToStockFromFMP(this FMPStock fmpStock)
    {
        return new Stock
        {
            Symbol = fmpStock.symbol,
            CompanyName = fmpStock.companyName,
            Purchase = (decimal)fmpStock.price,
            LastDiv = (decimal)fmpStock.lastDiv,
            Industry = fmpStock.industry,
            MarketCap = fmpStock.mktCap
        };
    }
}

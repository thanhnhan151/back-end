using api.Data;
using api.Dtos.Stocks;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories;
public class StockRepository(ApplicationDbContext context) : IStockRepository
{
    private readonly ApplicationDbContext _context = context;

    public Task<bool> CheckExistingStock(int id)
    => _context.Stocks.AnyAsync(x => x.Id == id);

    public async Task<Stock> CreateAsync(Stock stock)
    {
        await _context.Stocks.AddAsync(stock);

        await _context.SaveChangesAsync();

        return stock;
    }

    public async Task<Stock?> DeleteAsync(int id)
    {
        var stockModel = await _context.Stocks.FirstOrDefaultAsync(x => x.Id == id);

        if (stockModel == null)
        {
            return null;
        }

        _context.Stocks.Remove(stockModel);
        await _context.SaveChangesAsync();

        return stockModel;
    }

    public async Task<List<Stock>> GetAllAsync(QueryObject query)
    {
        var stocks = _context.Stocks
                   .Include(c => c.Comments)
                   .ThenInclude(x => x.AppUser)
                   .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.CompanyName))
        {
            stocks = stocks.Where(s => s.CompanyName.Contains(query.CompanyName));
        }

        if (!string.IsNullOrWhiteSpace(query.Symbol))
        {
            stocks = stocks.Where(s => s.Symbol.Contains(query.Symbol));
        }

        if (!string.IsNullOrWhiteSpace(query.SortBy))
        {
            if (query.SortBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
            {
                stocks = query.IsDescending ? stocks.OrderByDescending(s => s.Symbol) : stocks.OrderBy(s => s.Symbol);
            }
        }

        var skipNumber = (query.PageNumber - 1) * query.PageSize;

        return await stocks.Skip(skipNumber).Take(query.PageSize).ToListAsync();
    }

    public async Task<Stock?> GetByIdAsync(int id)
    => await _context.Stocks.Include(c => c.Comments)
                            .ThenInclude(x => x.AppUser)
                            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Stock?> GetBySymbolAsync(string symbol)
    => await _context.Stocks.FirstOrDefaultAsync(x => x.Symbol == symbol);

    public async Task<Stock?> UpdateAsync(int id, UpdateStockRequestDto stockDto)
    {
        var existingStock = await _context.Stocks.FirstOrDefaultAsync(x => x.Id == id);

        if (existingStock == null)
        {
            return null;
        }

        existingStock.Symbol = stockDto.Symbol;
        existingStock.CompanyName = stockDto.CompanyName;
        existingStock.Purchase = stockDto.Purchase;
        existingStock.LastDiv = stockDto.LastDiv;
        existingStock.Industry = stockDto.Industry;
        existingStock.MarketCap = stockDto.MarketCap;

        await _context.SaveChangesAsync();

        return existingStock;
    }
}

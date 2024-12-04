using api.Data;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories;
public class PortfolioRepository
(ApplicationDbContext context) : IPortfolioRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Portfolio> CreateAsync(Portfolio portfolio)
    {
        await _context.Portfolios.AddAsync(portfolio);

        await _context.SaveChangesAsync();

        return portfolio;
    }

    public async Task<List<Stock>> GetUserPortfolio(AppUser user)
    => await _context.Portfolios.Where(x => x.AppUserId == user.Id)
                                .Select(s => new Stock
                                {
                                    Id = s.StockId,
                                    Symbol = s.Stock.Symbol,
                                    CompanyName = s.Stock.CompanyName,
                                    Purchase = s.Stock.Purchase,
                                    LastDiv = s.Stock.LastDiv,
                                    Industry = s.Stock.Industry,
                                    MarketCap = s.Stock.MarketCap
                                }).ToListAsync();
}

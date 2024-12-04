using api.Extensions;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/portfolios")]
public class PortfoliosController
(UserManager<AppUser> userManager,
 IStockRepository stockRepository,
 IPortfolioRepository portfolioRepository) : ControllerBase
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly IStockRepository _stockRepository = stockRepository;
    private readonly IPortfolioRepository _portfolioRepository = portfolioRepository;

    [HttpGet]
    public async Task<IActionResult> GetUserPortfolioAsync()
    {
        var userName = User.GetUsername();
        var appUser = await _userManager.FindByNameAsync(userName);
        var userPortfolio = await _portfolioRepository.GetUserPortfolio(appUser);
        return Ok(userPortfolio);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(string symbol)
    {
        var userName = User.GetUsername();
        var appUser = await _userManager.FindByNameAsync(userName);
        var stock = await _stockRepository.GetBySymbolAsync(symbol);

        if (stock == null) return BadRequest("Stock not found");

        var userPortfolio = await _portfolioRepository.GetUserPortfolio(appUser);

        if (userPortfolio.Any(e => e.Symbol.ToLower() == symbol.ToLower())) return BadRequest("Cannot add same stock to portfolio");

        var portfolioModel = new Portfolio
        {
            StockId = stock.Id,
            AppUserId = appUser.Id
        };

        await _portfolioRepository.CreateAsync(portfolioModel);

        if (portfolioModel == null) return StatusCode(500, "Could not create");

        return Created();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(string symbol)
    {
        var userName = User.GetUsername();
        var appUser = await _userManager.FindByNameAsync(userName);

        var userPortfolio = await _portfolioRepository.GetUserPortfolio(appUser);

        var filteredStock = userPortfolio.Where(s => s.Symbol.ToLower() == symbol.ToLower()).ToList();

        if (filteredStock.Count() != 1) return BadRequest("Stock not in your portfolio");

        await _portfolioRepository.DeletePortfolioAsync(appUser, symbol);

        return Ok();
    }
}

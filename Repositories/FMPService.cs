using api.Dtos.Stocks;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Newtonsoft.Json;

namespace api.Repositories;

public class FMPService
(HttpClient httpClient,
 IConfiguration configuration) : IFMPService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IConfiguration _configuration = configuration;

    public async Task<Stock> FindStockBySymbolAsync(string symbol)
    {
        try
        {
            var result = await _httpClient.GetAsync($"https://financialmodelingprep.com/api/v3/profile/{symbol}?apikey={_configuration["FMPKey"]}");

            if (result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                var tasks = JsonConvert.DeserializeObject<FMPStock[]>(content);
                var stock = tasks[0];
                if (stock != null)
                {
                    return stock.ToStockFromFMP();
                }
                return null;
            }
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
}
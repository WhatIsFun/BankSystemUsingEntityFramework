using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem_Using_Entity_Framework
{
    internal class ExchangeRateService
    {
        private readonly HttpClient httpClient;
        private const string ExchangeRateApiUrl = "https://v6.exchangerate-api.com/v6/2d8754c1bf6d68b8bbea954d/latest/OMR";
        DateTime DateTime = DateTime.Now;
        public ExchangeRateService()
        {
            httpClient = new HttpClient();
        }

        public async Task ViewExchangeRates()
        {
            HomePage homePage = new HomePage();

            Console.WriteLine("Exchange Rates:\n");
            Console.WriteLine(DateTime);
            ExchangeRateData exchangeRates = await GetExchangeRatesAsync();

            if (exchangeRates != null)
            {
                Console.WriteLine($"\n\nBase Currency: {exchangeRates.base_code}\n");
                foreach (var conversion_rates in exchangeRates.conversion_rates)
                {
                    Console.WriteLine($"{conversion_rates.Key}: {conversion_rates.Value}");
                }
            }
            Console.WriteLine("\n\n\n\n\n\nPress any key to go back.....");
            Console.ReadLine();
            Console.Clear();
            homePage.mainMenu();
        }
        private async Task<ExchangeRateData> GetExchangeRatesAsync()
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(ExchangeRateApiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    ExchangeRateData exchangeRateData = JsonConvert.DeserializeObject<ExchangeRateData>(responseBody);
                    return exchangeRateData;
                }
                else
                {
                    Console.WriteLine($"API request failed with status code: {response.StatusCode}");
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Failed to retrieve data from the API: {ex.Message}");
                return null;
            }
        }
    }

    public class ExchangeRateData
    {
        public string base_code { get; set; }
        public Dictionary<string, decimal> conversion_rates { get; set; }
    }

}

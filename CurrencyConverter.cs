using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using Newtonsoft.Json;

namespace BankSystem_Using_Entity_Framework
{
    internal class CurrencyConverter
    {
        private readonly HttpClient httpClient;

        public CurrencyConverter()
        {
            httpClient = new HttpClient();
        }
        public async Task ViewCurrencyConverter()
        {
            HomePage homePage = new HomePage();

            Console.WriteLine("Welcome to Currency Converter Service\n\n\r\n" +
                "E.g:\n" +
                "Currency Code  Currency Name\n" +
                "OMR            Omani Rial\n" +
                "AED            UAE Dirham\n" +
                "USD            United States Dollar\n" +
                "EUR            Euro\n");
            Console.WriteLine("Enter the base currency: ");
            string Base = Console.ReadLine();
            Console.WriteLine("Enter the exchange currency: ");
            string exchangeTo = Console.ReadLine();

            CurrencyConverterData currencyConverter = await GetExCurrencyConverterAsync(Base, exchangeTo);

            if (currencyConverter != null)
            {
                Console.WriteLine($"Base Currency: {currencyConverter.base_code}");
                Console.WriteLine($"Target Currency: {currencyConverter.target_code}");
                Console.WriteLine("Exchange Rates:");
                foreach (var conversion_rate in currencyConverter.conversion_rate)
                {
                    Console.Write($"{currencyConverter.conversion_rate}");
                }
                Console.WriteLine("\n\n\n\n\n\nPress any key to go back.....");
                Console.ReadLine();
                Console.Clear();
                homePage.mainMenu();
            }
        }
        public async Task<CurrencyConverterData> GetExCurrencyConverterAsync(string Base, string exchangeTo)
        {
            string CurrencyConverterApiUrl = $"https://v6.exchangerate-api.com/v6/2d8754c1bf6d68b8bbea954d/pair/{Base}/{exchangeTo}";

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(CurrencyConverterApiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    CurrencyConverterData CurrencyConverterData = JsonConvert.DeserializeObject<CurrencyConverterData>(responseBody);
                    return CurrencyConverterData;
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

    public class CurrencyConverterData
    {
        public string base_code { get; set; }
        public string target_code { get; set; }
        public string conversion_rate { get; set; }
    }
}
    


using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace BankSystem_Using_Entity_Framework
{
    internal class HomePage
    {
        public void mainMenu()
        {
            Registeration registeration = new Registeration();
            LoginPage loginPage = new LoginPage();
            ExchangeRateService exchangeRateService = new ExchangeRateService();
            CurrencyConverter currencyConverter = new CurrencyConverter();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. View current exchange rates");
                Console.WriteLine("4. Currency converter");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("5. Exit");
                Console.ResetColor();
                Console.Write("Select an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        registeration.Register();
                        break;
                    case "2":
                        loginPage.Login();
                        break;
                    case "3":
                        exchangeRateService.ViewExchangeRates();
                        break;
                    case "4":
                        currencyConverter.ViewCurrencyConverter();
                        break;
                    case "5":
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Are you sure you want to exit? (y/n) "); // Check if the user want to exit the application
                        string ExitInput = Console.ReadLine();
                        ExitInput.ToLower();
                        Console.ResetColor();
                        if (ExitInput.Equals("y", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.Write("Thank You for using our services");
                            Environment.Exit(0);
                        }
                        else
                        {
                            Console.Clear();
                            mainMenu();
                        }
                        break;

                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
                Console.Clear();
            }
        }
    }
}

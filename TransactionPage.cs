using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem_Using_Entity_Framework
{
    public class TransactionPage
    {
        public void transactionMenu(List<Account> userAccounts, User authenticatedUser)
        {
            ProfilePage profilePage = new ProfilePage();
            //profilePage.userAccounts = userAccounts;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("$$$ $$ $ Transaction $ $$ $$$\n");
            Console.ResetColor();
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("1) Deposite\n2) Withdraw\n3) Transfer Money\n4) Go Back\n");
                Console.ResetColor();
                Console.Write("Select an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Clear();
                        deposit(userAccounts);
                        break;
                    case "2":
                        Console.Clear();
                        withdraw(userAccounts);
                        break;
                    case "3":
                        Console.Clear();
                        transfer(userAccounts);
                        break;
                    case "4":
                        Console.Clear();
                        profilePage.profileMenu(authenticatedUser, userAccounts);
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
                Console.Clear();
            }
        }
        void deposit(List<Account> userAccounts)
        {
            Console.Write("Enter the account number to deposit into: ");
            if (!int.TryParse(Console.ReadLine(), out int sourceAccountId))
            {
                Console.WriteLine("Invalid account number.");
                return;
            }

            if (userAccounts.Count == 0)
            {
                Console.WriteLine("Add account first");
                return;
            }
            else if (!userAccounts.Any(account => account.Account_Id == sourceAccountId))
            {
                Console.WriteLine("Enter a valid account number.");
                return;
            }
            Console.Write("Enter the amount to deposit: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
            {
                Console.WriteLine("Invalid deposit amount.");
                return;
            }
            using (var context = new ApplicationDbContext())
            {
                try
                {
                    string type = "Deposit";
                    DateTime Timestamp = DateTime.Now;
                    var dAccount = context.Accounts.FirstOrDefault(a => a.Account_Id == sourceAccountId);

                    if (dAccount != null)
                    {
                        dAccount.Balance += amount;
                        context.SaveChanges();

                        RecordTransaction(Timestamp, type, amount, sourceAccountId, sourceAccountId, sourceAccountId);
                        Console.WriteLine("Transaction deposit added");
                        Console.WriteLine("\n\nPress Enter to go back...");
                        Console.ReadLine();
                        Console.Clear();
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Invalid deposit.");
                        return;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
            }
        }
        void withdraw(List<Account> userAccounts)
        {
            if (!int.TryParse(Console.ReadLine(), out int sourceAccountId))
            {
                Console.WriteLine("Invalid account number.");
                return;
            }

            if (userAccounts.Count == 0)
            {
                Console.WriteLine("Add account first");
                return;
            }
            else if (!userAccounts.Any(account => account.Account_Id == sourceAccountId))
            {
                Console.WriteLine("Enter a valid account number.");
                return;
            }
            Console.Write("Enter the amount to withdraw: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
            {
                Console.WriteLine("Invalid withdrawal amount.");
                return;
            }
            using (var context = new ApplicationDbContext())
            {
                try
                {
                    string type = "Withdrawal";
                    DateTime Timestamp = DateTime.Now;
                    var dAccount = context.Accounts.FirstOrDefault(a => a.Account_Id == sourceAccountId);

                    if (dAccount != null)
                    {
                        dAccount.Balance -= amount;
                        context.SaveChanges();

                        RecordTransaction(Timestamp, type, amount, sourceAccountId, sourceAccountId, sourceAccountId);
                        Console.WriteLine("Transaction Withdrawal added");
                        Console.WriteLine("\n\nPress Enter to go back...");
                        Console.ReadLine();
                        Console.Clear();
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Invalid deposit.");
                        return;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
            }
        }

        void transfer(List<Account> userAccounts)
        {
            DateTime Timestamp = DateTime.Now;
            Console.Write("Enter the account number to transfer from: ");
            if (!int.TryParse(Console.ReadLine(), out int sourceAccountId))
            {
                Console.WriteLine("Invalid account number.");
                return;
            }

            if (userAccounts.Count == 0)
            {
                Console.WriteLine("Add account first");
                return;
            }
            else if (!userAccounts.Any(account => account.Account_Id == sourceAccountId))
            {
                Console.WriteLine("Enter a valid account number.");
                return;
            }
            else
            {
                Console.Write("Enter the account number you want to transfer to: ");
                if (!int.TryParse(Console.ReadLine(), out int targetAccountId))
                {
                    Console.WriteLine("Invalid target account number.");
                    return;
                }

                using (var context = new ApplicationDbContext())
                {
                    try
                    {
                        var sourceAccount = context.Accounts.FirstOrDefault(a => a.Account_Id == sourceAccountId);
                        var targetAccount = context.Accounts.FirstOrDefault(a => a.Account_Id == targetAccountId);

                        if (sourceAccount == null || targetAccount == null)
                        {
                            Console.WriteLine("Source or target account not found.");
                            return;
                        }
                        Console.Write("Enter the amount to transfer: ");
                        if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
                        {
                            Console.WriteLine("Invalid withdrawal amount.");
                            return;
                        }
                        if (sourceAccount.Balance >= amount)
                        {
                            // Perform the transfer
                            string type = "Transfer";
                            //string Timestamp1 = time;
                           

                            sourceAccount.Balance -= amount;
                            targetAccount.Balance += amount;

                            context.SaveChanges();

                            RecordTransaction(Timestamp, type, amount, sourceAccountId, targetAccountId, sourceAccountId);
                            Console.WriteLine("Transaction added");
                            Console.WriteLine("\n\nPress Enter to go back...");
                            Console.ReadLine();
                            Console.Clear();
                            return;
                        }
                        else
                        {
                            Console.WriteLine("Insufficient funds for the transfer.");
                            return;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }

        private void RecordTransaction(DateTime Timestamp1, string type, decimal amount, int sourceAccountId, int targetAccountId, int accountNum)
        {
            using (var context = new ApplicationDbContext())
            {
                try
                {
                    var transaction = new Transaction
                    {
                        Timestamp = Timestamp1,
                        Type = type,
                        Amount = amount,
                        SorAccId = sourceAccountId,
                        TarAccId = targetAccountId,
                        User_Id = accountNum
                    };

                    context.Transactions.Add(transaction);
                    int rowsAffected = context.SaveChanges();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Transaction recorded successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Failed to record the transaction.");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred while recording the transaction: " + e.Message);
                }
            }
        }

        public void ViewTransactionHistory(User authenticatedUser, string period)
        {
            DateTime startDate;
            switch (period.ToLower())
            {
                case "last transaction":
                    startDate = DateTime.MinValue; // Set to minimum date
                    break;
                case "last day":
                    startDate = DateTime.Now.AddDays(-1);
                    break;
                case "last 5 days":
                    startDate = DateTime.Now.AddDays(-5);
                    break;
                case "last 1 month":
                    startDate = DateTime.Now.AddMonths(-1);
                    break;
                case "last 2 months":
                    startDate = DateTime.Now.AddMonths(-2);
                    break;
                default:
                    Console.WriteLine("Invalid period. Showing all transactions.");
                    startDate = DateTime.MinValue; // Set to minimum date
                    break;
            }

            using (var context = new ApplicationDbContext())
            {
                try
                {
                    var userId = authenticatedUser.User_Id;

                    var transactions = context.Transactions
                        .Where(t => (t.SorAccId == userId || t.TarAccId == userId) && t.Timestamp >= startDate)
                        .OrderByDescending(t => t.Timestamp)
                        .ToList();

                    if (transactions.Count > 0)
                    {
                        Console.WriteLine($"Transaction History (Last {period}):");
                        foreach (var transaction in transactions)
                        {
                            Console.WriteLine($"Transaction ID: {transaction.T_Id}");
                            Console.WriteLine($"Timestamp: {transaction.Timestamp}");
                            Console.WriteLine($"Type: {transaction.Type}");
                            Console.WriteLine($"Amount: {transaction.Amount} OMR");
                            Console.WriteLine($"Source Account: {transaction.SorAccId}");
                            Console.WriteLine($"Target Account: {transaction.TarAccId}");
                            Console.WriteLine("---------------------------");
                        }
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }
                    else
                    {
                        Console.WriteLine("No transaction history found.");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }

    }
}


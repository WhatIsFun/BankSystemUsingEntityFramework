using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem_Using_Entity_Framework
{
    internal class ProfilePage
    {
        public List<Account> userAccounts = new List<Account>();
        public void profileMenu(User authenticatedUser, List<Account> userAccounts)
        {
            var _context = new ApplicationDbContext();
            TransactionPage transaction = new TransactionPage();
            HomePage homePage = new HomePage();
            if (authenticatedUser != null)
            {
                Console.WriteLine($"Welcome, {authenticatedUser.Name}\n\n");

                userAccounts = GetUserAccounts(authenticatedUser.User_Id);

                    if (userAccounts.Count > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("Your Accounts:");
                        Console.ResetColor();

                        foreach (var account in userAccounts)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.WriteLine($"Account Number:      {account.Account_Id}");
                            Console.WriteLine($"Account Holder Name: {account.HolderName}");
                            Console.WriteLine($"Account Balance:     {account.Balance} OMR");
                            Console.ResetColor();
                            Console.WriteLine("____________________________________");
                            Console.WriteLine();
                        }
                    }
                    else { Console.WriteLine("You dont have any accounts. Please add one\n\n"); }
                
            }
            else
            {
                Console.WriteLine("Login failed");
                homePage.mainMenu();
            }
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("1) Create new account\n2) Make a transaction\n3) View Transaction History\n4) Delete account\n5) Delete User\n");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("6) Logout");
                Console.ResetColor();
                Console.Write("\n\nSelect an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Clear();
                        createAccount(authenticatedUser);
                        break;
                    case "2":
                        Console.Clear();
                        transaction.transactionMenu(userAccounts, authenticatedUser);
                        break;
                    case "3":
                        Console.Clear();
                        ViewTransactionHistoryMenu(authenticatedUser);
                        break;
                    case "4":
                        Console.Clear();
                        deleteAccount(authenticatedUser, userAccounts);
                        break;
                    case "5":
                        Console.Clear();
                        deleteUser(authenticatedUser);
                        break;
                    case "6":
                        authenticatedUser = null;
                        Console.Clear();
                        homePage.mainMenu();
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }
        private void createAccount(User authenticatedUser)
        {
            Console.Write("Enter initial balance: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal initialBalance))
            {
                decimal balance = initialBalance;
                int UserID = authenticatedUser.User_Id;
                string AccountHolderName = authenticatedUser.Name;
                insertAccount(balance, UserID, AccountHolderName);

            }
            else
            {
                Console.WriteLine("Invalid initial balance.");
                return;
            }
            Console.WriteLine("Press Enter to go back...");
            Console.ReadLine();
            Console.Clear();
            profileMenu(authenticatedUser, userAccounts);

        }
        private static void insertAccount(decimal balance, int UserID, string AccountHolderName)
        {
            try
            {
                using (var _context = new ApplicationDbContext())
                {
                    var acc = new Account { User_Id = UserID, HolderName = AccountHolderName, Balance = balance};
                    _context.Add(acc);
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public static List<Account> GetUserAccounts(int userId)
        {
            using (var _context = new ApplicationDbContext())
            {
                List<Account> accounts = _context.Accounts
                    .Where(a => a.User_Id == userId)
                    .ToList();

                return accounts;
            }
        }

        private void deleteAccount(User authenticatedUser, List<Account> userAccount)
        {
            Console.Clear();
            Console.WriteLine("Delete Account");
            Console.Write("Enter the account number to delete: ");
            int accountIdToDelete;
            if (!int.TryParse(Console.ReadLine(), out accountIdToDelete))
            {
                Console.WriteLine("Invalid account ID. Account deletion failed.");
                Console.WriteLine("Press Enter to go back ...");
                Console.ReadLine();
                return;
            }
            // Check if the provided account ID exists in the user's accounts
            if (userAccount != null)
            {

                foreach (var account in userAccount)
                {
                    if (userAccount.Any(account => account.Account_Id == accountIdToDelete))
                    {
                        // Verify the provided email and password against the authenticated user's credentials
                        Console.Write("Enter your password to confirm deletion: ");
                        string passwordInput = Console.ReadLine();

                        if (VerifyPassword(passwordInput, authenticatedUser.Password))
                        {
                            deleteAccountServer(accountIdToDelete);
                        }
                        else
                        {
                            Console.WriteLine("Invalid password. Account deletion failed.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Account with the specified ID does not exist in your accounts. Account deletion failed.");
                    }

                    Console.WriteLine("Press Enter to go back...");
                    Console.ReadLine();
                    Console.Clear();
                    return;
                }
            }
        }
        private void deleteAccountServer(int accountIdToDelete)
        {
            try
            {
                using (var _context = new ApplicationDbContext())
                {
                    var accountToDelete = _context.Accounts.Find(accountIdToDelete);

                    if (accountToDelete != null)
                    {
                        _context.Accounts.Remove(accountToDelete);
                        _context.SaveChanges();

                        Console.WriteLine($"Account with ID {accountIdToDelete} deleted successfully.\nVisit nearest ATM to withdraw your balance");
                    }
                    else
                    {
                        Console.WriteLine($"Account with ID {accountIdToDelete} not found.");
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        private void deleteUser(User authenticatedUser)
        {
            Console.WriteLine("Delete User\n\nSoryy to hear that");
            int userToDelete = authenticatedUser.User_Id;
            if (authenticatedUser != null)
            {
                Console.Write("Enter your password to confirm deletion: ");
                string passwordInput = Console.ReadLine();

                if (VerifyPassword(passwordInput, authenticatedUser.Password))
                {
                    deleteUserServer(userToDelete);
                }
                else
                {
                    Console.WriteLine("Invalid password. Account deletion failed.");
                }
            }
            Console.WriteLine("Press Enter to go back...");
            Console.ReadLine();
            Console.Clear();
            return;
        }
        private void deleteUserServer(int userIdToDelete)
        {
            try
            {
                using (var _context = new ApplicationDbContext())
                {
                    var userDelete = _context.Users.Find(userIdToDelete);

                    if (userDelete != null)
                    {
                        _context.Users.Remove(userDelete);
                        _context.SaveChanges();

                        Console.WriteLine($"User with ID {userIdToDelete} deleted successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"User with ID {userIdToDelete} not found.");
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }
        private static bool VerifyPassword(string inputPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(inputPassword, hashedPassword);
        }
        private void ViewTransactionHistoryMenu(User authenticatedUser)
        {
            TransactionPage transaction = new TransactionPage();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Enter the account number to view history: ");
            if (!int.TryParse(Console.ReadLine(), out int viewAccId))
            {
                Console.WriteLine("Invalid account number.");
                return;
            }

            using (var _context = new ApplicationDbContext())
            {
                try
                {
                    var userAccount = _context.Accounts
                        .Where(a => a.Account_Id == viewAccId && a.User.User_Id == authenticatedUser.User_Id)
                        .FirstOrDefault();

                    if (userAccount != null)
                    {
                        Console.Clear();
                        Console.WriteLine("View Transaction History:\n");

                        Console.ResetColor();
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("1) Last transaction");
                        Console.WriteLine("2) Last day");
                        Console.WriteLine("3) Last 5 days");
                        Console.WriteLine("4) Last 1 month");
                        Console.WriteLine("5) Last 2 months");
                        Console.ResetColor();

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\n\n* If you want more than two months, please contact or visit the nearest branch");
                        Console.ResetColor();

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("6) Back to Menu");
                        Console.ResetColor();
                        Console.WriteLine("Enter an option: ");

                        if (int.TryParse(Console.ReadLine(), out int choice))
                        {
                            string period;
                            switch (choice)
                            {
                                case 1:
                                    period = "last transaction";
                                    break;
                                case 2:
                                    period = "last day";
                                    break;
                                case 3:
                                    period = "last 5 days";
                                    break;
                                case 4:
                                    period = "last 1 month";
                                    break;
                                case 5:
                                    period = "last 2 months";
                                    break;
                                case 6:
                                    profileMenu(authenticatedUser, userAccounts);
                                    return; // Back to the main menu
                                default:
                                    period = "Enter a valid option";
                                    break;
                            }

                            transaction.ViewTransactionHistory(authenticatedUser, period, viewAccId);
                        }
                        else
                        {
                            Console.WriteLine("Invalid input. Please enter a valid number.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("The entered account number does not belong to you.");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    profileMenu(authenticatedUser, userAccounts);
                }
            }
        }
    }
}

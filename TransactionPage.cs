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
    internal class TransactionPage
    {
        public void transactionMenu(List<Account> userAccounts, User authenticatedUser)
        {
            ProfilePage profilePage = new ProfilePage();
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
                        deposit(userAccounts);
                        break;
                    case "2":
                        withdraw(userAccounts);
                        break;
                    case "3":
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
            if (!int.TryParse(Console.ReadLine(), out int accountNum))
            {
                Console.WriteLine("Invalid account number.");
                return;
            }

            if (userAccounts.Count == 0)
            {
                Console.WriteLine("There are no accounts.");
                return;
            }

            Console.Write("Enter the amount to deposit: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
            {
                Console.WriteLine("Invalid deposit amount.");
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string type = "Deposit";
                    string Timestamp1 = Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
                    string Updatebalance = "Update Account set Balance = Balance + @Amount where Account_Id = @Account_Id;";
                    SqlCommand Command = new SqlCommand(Updatebalance, connection);
                    Command.Parameters.AddWithValue("@Amount", amount);
                    Command.Parameters.AddWithValue("@Account_Id", accountNum);

                    int rowaffected = Command.ExecuteNonQuery();

                    if (rowaffected > 0)
                    {
                        RecordTransaction(Timestamp1, type, amount, accountNum, accountNum, accountNum);
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
                }
                finally
                {
                    connection.Close();
                }
            }
            Console.WriteLine("\n\nPress Enter to go back...");
            Console.ReadLine();
            Console.Clear();
            return;
        }
        void withdraw(List<Account> userAccounts)
        {
            using (var context = new ApplicationDbContext())
            {
                var account = context.Accounts.FirstOrDefault(a => a.Account_Id == accountId);
                Console.Write("Enter the account number to withdraw from: ");
                if (!int.TryParse(Console.ReadLine(), out int accountNum))
                {
                    Console.WriteLine("Invalid account number.");
                    return;
                }

                if (account == null)
                {
                    Console.WriteLine("Invalid account number.");
                    return;
                }

                Console.Write("Enter the amount to withdraw: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
                {
                    Console.WriteLine("Invalid withdrawal amount.");
                    return;
                }

                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var type = "Withdrawal";
                        var timestamp = DateTime.Now;

                        // Update account balance
                        account.Balance -= amount;
                        context.SaveChanges();

                        // Create a transaction record
                        var transactionRecord = new Transaction
                        {
                            Timestamp = timestamp,
                            Type = type,
                            Amount = amount,
                            SourceAccountId = accountId,
                            TargetAccountId = accountId
                        };

                        context.Transactions.Add(transactionRecord);
                        context.SaveChanges();

                        transaction.Commit();

                        Console.WriteLine("Withdrawal successful.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                        transaction.Rollback();
                    }
                }
            }

            Console.WriteLine("\n\nPress Enter to go back...");
            Console.ReadLine();
            Console.Clear();
            return;
        }

        void transfer(List<Account> userAccounts)
        {
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
            else if (userAccounts.Any(account => account.Account_Id != sourceAccountId))
            {
                Console.WriteLine("Enter a valid account number ");
                return;
            }
            else
            {
                Console.Write("Enter the account number want to transfer to: ");
                if (!int.TryParse(Console.ReadLine(), out int targetAccountId))
                {
                    Console.WriteLine("Invalid target account number.");
                    return;
                }
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        string selectSql = "SELECT Account_Id, AccountHolderName FROM Account WHERE Account_Id = @Account_Id";
                        using (SqlCommand command = new SqlCommand(selectSql, connection))
                        {
                            command.Parameters.AddWithValue("@Account_Id", targetAccountId);

                            object targetAccountResult = command.ExecuteScalar();

                            if (targetAccountResult != null)
                            {
                                targetAccountId = (int)targetAccountResult;
                                //string targetName = command["AccountHolderName"].ToString(); // Retrieve AccountHolderName from the result
                            }
                            else
                            {
                                Console.WriteLine("Target account not found.");
                                return;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                }

                Console.Write("Enter the amount to transfer: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
                {
                    Console.WriteLine("Invalid transfer amount.");
                    return;
                }
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        string type = "Transfer";
                        string Timestamp1 = Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
                        //int targetAccountid = targetAccountId;
                        // Source
                        string UpdateSourBalance = "Update Account set Balance = Balance - @Amount where Balance>@Amount and Account_Id = @Account_Id;";
                        SqlCommand Command = new SqlCommand(UpdateSourBalance, connection);
                        Command.Parameters.AddWithValue("@Amount", amount);
                        Command.Parameters.AddWithValue("@Account_Id", sourceAccountId);

                        Console.WriteLine("Transering successful");
                        int rowaffected = Command.ExecuteNonQuery();
                        if (rowaffected > 0)
                        {

                            RecordTransaction(Timestamp1, type, amount, sourceAccountId, targetAccountId, sourceAccountId);
                            Console.WriteLine("Transaction added");
                            Console.ReadLine();
                            return;
                        }
                        // Target 
                        string UpdateTargBalance = "Update Account set Balance = Balance + @Amount where Account_Id = @Account_Id;";
                        SqlCommand Command1 = new SqlCommand(UpdateTargBalance, connection);
                        Command1.Parameters.AddWithValue("@Amount", amount);
                        Command1.Parameters.AddWithValue("@Account_Id", targetAccountId);


                        int rowaffected1 = Command1.ExecuteNonQuery();
                        if (rowaffected1 > 0)
                        {
                            RecordTransaction(Timestamp1, type, amount, sourceAccountId, targetAccountId, sourceAccountId);
                            Console.WriteLine("Transaction added");
                            Console.WriteLine("\n\nPress Enter to go back...");
                            Console.ReadLine();
                            Console.Clear();
                            return;
                        }
                        else
                        {
                            Console.WriteLine("Invalid withdrawal amount or insufficient funds.");
                            return;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                    Console.WriteLine("\n\nPress Enter to go back...");
                    Console.ReadLine();
                    Console.Clear();
                    return;
                }
            }
        }
        private void RecordTransaction(string Timestamp1, string type, decimal amount, int sourceAccountId, int targetAccountId, int accountNum)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    sqlConnection.Open();
                    string insertTransactionQuery = "INSERT INTO dbo.Transactions (Timestamp, Type, Amount, SourceAccountId, TargetAccountId, Account_Id) " +
    "values (@Timestamp, @Type, @Amount, @SourceAccountId, @TargetAccountId, @Account_Id);";

                    using (SqlCommand insertTransactionCommand = new SqlCommand(insertTransactionQuery, sqlConnection))
                    {
                        insertTransactionCommand.Parameters.AddWithValue("@Timestamp", Timestamp1);
                        insertTransactionCommand.Parameters.AddWithValue("@Type", type);
                        insertTransactionCommand.Parameters.AddWithValue("@Amount", amount);
                        insertTransactionCommand.Parameters.AddWithValue("@SourceAccountId", sourceAccountId); // Corrected parameter name
                        insertTransactionCommand.Parameters.AddWithValue("@TargetAccountId", targetAccountId); // Corrected parameter name
                        insertTransactionCommand.Parameters.AddWithValue("@Account_Id", accountNum);

                        int rowsAffected = insertTransactionCommand.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Transaction recorded successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Failed to record the transaction.");
                        }
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
            DateTime minSqlDate = new DateTime(1753, 1, 1);
            DateTime startDate;
            switch (period.ToLower())
            {
                case "last transaction":
                    startDate = minSqlDate; // Set to minimum date
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
                    startDate = minSqlDate; // Set to minimum date
                    break;
            }
            string connectionString = "Data Source=(local);Initial Catalog=BankSystem; Integrated Security=true";
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    sqlConnection.Open();
                    string selectQuery = "SELECT Transaction_Id, Timestamp, Type, Amount, SourceAccountId, TargetAccountId FROM dbo.Transactions " +
                        "WHERE (SourceAccountId IN (SELECT Account_Id FROM dbo.Account WHERE User_ID = @userId) " +
                        "OR TargetAccountId IN (SELECT Account_Id FROM dbo.Account WHERE User_ID = @userId)) " +
                        "AND Timestamp >= @startDate " +
                        "ORDER BY Timestamp DESC";
                    using (SqlCommand sqlCommand = new SqlCommand(selectQuery, sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@userId", authenticatedUser.UserId);
                        sqlCommand.Parameters.AddWithValue("@startDate", startDate);
                        // Execute the query and read the results
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                Console.WriteLine($"Transaction History (Last {period}):");
                                while (reader.Read())
                                {
                                    int Transaction_Id = reader.GetInt32(0);
                                    DateTime Timestamp = reader.GetDateTime(1);
                                    string Type = reader.GetString(2);
                                    decimal Amount = reader.GetDecimal(3);
                                    int SourceAccountId = reader.GetInt32(4);
                                    int TargetAccountId = reader.GetInt32(5);
                                    Console.WriteLine($"Transaction ID: {Transaction_Id}");
                                    Console.WriteLine($"Timestamp: {Timestamp}");
                                    Console.WriteLine($"Type: {Type}");
                                    Console.WriteLine($"Amount: {Amount} OMR");
                                    Console.WriteLine($"Source Account: {SourceAccountId}");
                                    Console.WriteLine($"Target Account: {TargetAccountId}");
                                    Console.WriteLine("---------------------------");
                                }
                                Console.WriteLine("Press any key to continue...");
                                Console.ReadKey();
                                Console.Clear();
                                return;
                            }
                            else
                            {
                                Console.WriteLine("No transaction history found.");
                                Console.WriteLine("Press any key to continue...");
                                Console.ReadKey();
                                return;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    return;
                }
            }
        }
    } 
}


using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;

namespace BankSystem_Using_Entity_Framework
{
    internal class LoginPage
    {
        public LoginPage()
        {
        }

        public void Login()
        {
            ProfilePage profilePage = new ProfilePage();
            // Prompt the user for email and password
            Console.Write("Enter your email: ");
            string email = Console.ReadLine();

            Console.Write("Enter your password: ");
            string password = Console.ReadLine();

            // Authenticate the user
            User authenticatedUser = AuthenticateUser(email, password);

            if (authenticatedUser == null)
            {
                Console.WriteLine("Invalid email or password. Please try again.");
                Console.WriteLine("\n\n\n\n\n\nPress any key to try again.....");
                Console.ReadLine();
                return;
            }
            else
            {
                // User is authenticated; grant access to the program
                Console.WriteLine("Login successful! Welcome, " + authenticatedUser.Name);
                loading();
                Console.Clear();
                profilePage.profileMenu(authenticatedUser);
            }
        }
        private static User AuthenticateUser(string email, string password)
        {
            using (var _context = new ApplicationDbContext())
            {
                // Find the user by email
                User user = _context.Users.SingleOrDefault(u => u.Email == email);

                if (user != null)
                {
                    if (VerifyPassword(password, user.Password))
                    {
                        // Password is correct; return the user
                        return user;
                    }
                }
                // No matching user or incorrect password; return null
                return null;
            }
        }

        // Implement a password verification method
        private static bool VerifyPassword(string inputPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(inputPassword, hashedPassword);
        }
        public void loading()
        {
            string[] spinner = { "-", "\\", "|", "/" };

            Console.Write("Loading ");
            for (int i = 0; i < 10; i++)
            {
                Console.Write(spinner[i % spinner.Length]);
                Thread.Sleep(200);
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
            }
        }
    }
}

namespace BankSystem_Using_Entity_Framework
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("      Welcome To    ");
            Console.WriteLine(" +-+-+-+-+-+-+-+-+-+\r\n |W|h|a|t|I|s|F|u|n|\r\n +-+-+-+-+-+-+-+-+-+");
            Console.WriteLine("    Banking System      \n");
            Console.ResetColor();
            Console.WriteLine("\n\n\n\n\n\nPress any key to start.....");
            Console.ReadLine();
            HomePage homePage = new HomePage();
            homePage.mainMenu();
        }
    }
}
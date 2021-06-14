using System;

namespace MapCreations
{
    class Program
    {
        static void Main(string[] args)
        {
            Run();
        }


        private static void Run()
        {
            try
            {
                Creator.CreateLookup(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
            }
            catch(Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("ERROR: ");
                Console.Write(e.Message);
                Console.ResetColor();

                Environment.Exit(-1);
            }

            Console.WriteLine("Successfully created.");
        }
    }
}

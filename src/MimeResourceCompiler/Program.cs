using System;

namespace MapCreations
{
    class Program
    {
        static void Main(string[] args)
        {
            string outDir;
            try
            {
                var compiler = new Compiler();
                outDir = compiler.CreateResources(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
            }
            catch(Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("ERROR: ");
                Console.Write(e.Message);
                Console.ResetColor();

                Environment.Exit(-1);
                return;
            }

            Console.WriteLine($"Mime resources successfully created at {outDir}.");
        }

    }
}

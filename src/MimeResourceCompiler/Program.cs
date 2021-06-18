using System;

namespace MapCreations
{
    class Program
    {
        static void Main(string[] args)
        {
            Compiler compiler;
            try
            {
                compiler = new Compiler(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));

                compiler.CreateResources();
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

            Console.WriteLine($"Mime resources successfully created at {compiler.OutputDirectory}.");
        }

    }
}

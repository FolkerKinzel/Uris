using System;

namespace MimeResourceCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            using var factory = new Factory(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));

            try
            {
                Compiler compiler = factory.ResolveCompiler();
                compiler.CompileResources();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("ERROR: ");
                Console.Write(e.Message);
                Console.ResetColor();

                Environment.Exit(-1);
                return;
            }

            Console.WriteLine($"Mime resources successfully created at {factory.ResolveOutputDirectory().FullName}.");
        }

    }
}

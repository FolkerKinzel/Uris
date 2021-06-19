using System;

namespace MimeResourceCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using var factory = new Factory(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));

                using (Compiler compiler = factory.ResolveCompiler())
                {
                    compiler.CompileResources();
                }

                factory.ResolveReadmeFile().Create();

                Console.WriteLine($"Mime resources successfully created at {factory.ResolveOutputDirectory().FullName}.");

            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("ERROR: ");
                Console.Write(e.Message);
                Console.ResetColor();

                Environment.Exit(-1);
            }
        }

    }
}

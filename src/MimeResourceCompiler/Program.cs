using System;
using System.Collections.Generic;
using CommandLine;

namespace MimeResourceCompiler
{
    internal class Program
    {
        private static void Main(string[] args) => _ = Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options => RunCompiler(options))
                .WithNotParsed(errs => OnCommandLineParseErrors(errs));


        private static void RunCompiler(Options options)
        {
            try
            {
                using var factory = new Factory(options);

                using (Compiler compiler = factory.ResolveCompiler())
                {
                    compiler.CompileResources();
                }

                if (options.CreateReadme)
                {
                    factory.ResolveReadmeFile().Create();
                }

                Console.WriteLine($"Mime resources successfully created at {factory.ResolveOutputDirectory().FullName}.");

            }
            catch (Exception e)
            {
                WriteError(e.Message);
                Environment.Exit(-1);
            }
        }


        private static void OnCommandLineParseErrors(IEnumerable<Error> errs)
        {
            bool hasError = false;

            foreach (Error err in errs)
            {
                switch (err.Tag)
                {
                    case ErrorType.HelpRequestedError:
                    case ErrorType.VersionRequestedError:
                        continue;

                    default:
                        break;
                }

                WriteError(err.ToString());
                hasError = true;
            }

            if (hasError)
            {
                Environment.Exit(-1);
            }
        }


        private static void WriteError(string? error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("ERROR: ");
            Console.WriteLine(error);
            Console.ResetColor();
        }

    }
}

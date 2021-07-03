using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace MimeResourceCompiler
{
    internal class Program
    {
        private const string LOG_FILE_NAME = "LastBuild.log";

        private static void Main(string[] args)
        {
            _ = Parser.Default.ParseArguments<Options>(args)
                                .WithParsed(options => RunCompiler(options))
                                .WithNotParsed(errs => OnCommandLineParseErrors(errs));
        }


        private static void RunCompiler(Options options)
        {
            string? logFilePath = Path.Combine(Environment.CurrentDirectory, LOG_FILE_NAME);
            using Logger log = InitializeLogger(options.CreateLogFile ? logFilePath : null, options.LogToConsole);

            try
            {

                using var factory = new Factory(options, log);

                using (Compiler compiler = factory.ResolveCompiler())
                {
                    compiler.CompileResources();
                }

                if (options.CreateReadme)
                {
                    factory.ResolveReadmeFile().Create();
                }

                log.Information("Mime resources successfully created at {outDir}.", factory.ResolveOutputDirectory().FullName);
                log.Information("A log file has been created at {logFilePath}.", logFilePath);
            }
            catch (Exception e)
            {
                log.Fatal(e.Message);
                log.Debug(e, "");
                Environment.ExitCode = -1;
            }
        }


        private static void OnCommandLineParseErrors(IEnumerable<Error> errs)
        {
            using Logger log = InitializeLogger(null, false);

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

                log.Fatal(err.ToString());
                Environment.ExitCode = -1;
            }
        }


        private static Logger InitializeLogger(string? logFilePath, bool logToConsole)
        {
            LogEventLevel consoleLogEventLevel = logToConsole ? LogEventLevel.Debug : LogEventLevel.Information;

            LoggerConfiguration config = new LoggerConfiguration()
                                        .MinimumLevel.Debug()
                                        .WriteTo.Console(restrictedToMinimumLevel: consoleLogEventLevel);

            if (logFilePath is not null)
            {
                if(File.Exists(logFilePath))
                {
                    try
                    {
                        File.Delete(logFilePath);
                    }
                    catch
                    { }
                }
                _ = config.WriteTo.File(logFilePath);
            }

            return config.CreateLogger();
        }

    }
}

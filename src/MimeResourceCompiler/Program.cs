using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace MimeResourceCompiler
{
    /// <summary>
    /// Console application, which compiles the resource files needed in the FolkerKinzel.Uris.dll to parse
    /// MIME types and file type extensions in a fast way.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The main part of the data used to find appropriate file type extensions for MIME types or to find an appropriate
    /// MIME type for a file type extension comes from http://svn.apache.org/repos/asf/httpd/httpd/trunk/docs/conf/mime.types .
    /// The program loads the current version of this file from the internet each time it runs and enriches these data with the
    /// entries from the resource file Resources\Addendum.csv. The data in this file is collected at https://wiki.selfhtml.org/wiki/MIME-Type/%C3%9Cbersicht ,
    /// at https://mimesniff.spec.whatwg.org/  and from several articles in WIKIPEDIA.
    /// </para>
    /// <para>
    /// The resource file Resources\Addendum.csv allows to add entries that are missing in the Apache file. Entries in Addendum.csv don't produce
    /// duplicates even if they are already present in the Apache file.
    /// </para>
    /// <para>
    /// The resource file Resources\Default.csv allows to determine the order in which entries are selected. The Apache file is mostly in alphabetical
    /// order: That produces not always the expected results.
    /// </para>
    /// </remarks>
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

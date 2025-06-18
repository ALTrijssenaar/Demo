using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Xunit;

namespace ConsoleApp.Tests
{
    public class ProgramTests
    {
        private string RunConsoleApp(params string[] args)
        {
            var exePath = Path.Combine("..", "ConsoleApp", "bin", "Debug", "net8.0", "ConsoleApp.exe");
            var psi = new ProcessStartInfo(exePath)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = string.Join(" ", args)
            };
            using var proc = Process.Start(psi);
            var output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            return output.Trim();
        }

        [Fact]
        public void NoArguments_PrintsUsage()
        {
            var output = RunConsoleApp();
            Assert.Contains("No arguments provided", output);
        }

        [Fact]
        public void HelpCommand_PrintsHelp()
        {
            var output = RunConsoleApp("help");
            Assert.Contains("Usage: ConsoleApp", output);
            Assert.Contains("greet <name>", output);
        }

        [Fact]
        public void InvalidCommand_PrintsUnknown()
        {
            var output = RunConsoleApp("foo");
            Assert.Contains("Unknown command", output);
        }

        [Fact]
        public void GreetCommand_MissingArg_PrintsError()
        {
            var output = RunConsoleApp("greet");
            Assert.Contains("Error: 'greet' needs exactly one argument", output);
        }

        [Fact]
        public void AddCommand_MissingArgs_PrintsError()
        {
            var output = RunConsoleApp("add", "1");
            Assert.Contains("Error: 'add' needs two numeric arguments", output);
        }

        [Fact]
        public void AddCommand_NonNumeric_PrintsError()
        {
            var output = RunConsoleApp("add", "a", "b");
            Assert.Contains("Error: Both arguments for 'add' must be numbers", output);
        }
    }
}

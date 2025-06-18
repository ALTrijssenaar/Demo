using MediatR;
using ConsoleApp;
using Microsoft.Extensions.DependencyInjection;
using ConsoleApp.Application;

 public class Program
 {
     public static void Main(string[] args)
     {
         try
         {
             var services = new ServiceCollection();
             services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
             var provider = services.BuildServiceProvider();
             var mediator = provider.GetRequiredService<IMediator>();

             // Handle no arguments
             if (args.Length == 0)
             {
                 Console.WriteLine("No arguments provided. Usage: ConsoleApp <command> [options]");
                 Console.WriteLine("Try 'help' for more info.");
                 return;
             }

             // Discover commands dynamically
             var commandTypes = typeof(Program).Assembly.GetTypes()
                 .Where(t => typeof(IRequest<string>).IsAssignableFrom(t) && t.Name.EndsWith("Command"))
                 .ToDictionary(
                     t => t.Name.Substring(0, t.Name.Length - "Command".Length).ToLower(),
                     t => t);
             var cmd = args[0].ToLower();

             // Help output
             if (cmd == "help")
             {
                 Console.WriteLine("Usage: ConsoleApp <command> [options]");
                 Console.WriteLine("Commands:");
                 foreach (var kv in commandTypes)
                 {
                     var ctor = kv.Value.GetConstructors()[0];
                     var ps = ctor.GetParameters();
                     var usage = kv.Key;
                     foreach (var p in ps) usage += $" <{p.Name}>";
                     Console.WriteLine($"  {usage}   - {kv.Key} command");
                 }
                 return;
             }

             // Unknown command
             if (!commandTypes.TryGetValue(cmd, out var type))
             {
                 Console.WriteLine($"Unknown command: {args[0]}");
                 Console.WriteLine("Try 'help' for a list of commands.");
                 return;
             }

             var ci = type.GetConstructors()[0];
             var paramInfos = ci.GetParameters();
             // Missing args
             if (args.Length - 1 != paramInfos.Length)
             {
                 if (paramInfos.Length == 1)
                     Console.WriteLine($"Error: '{cmd}' needs exactly one argument ({paramInfos[0].Name}).");
                 else if (paramInfos.Length == 2 && paramInfos.All(p => p.ParameterType == typeof(int)))
                     Console.WriteLine($"Error: '{cmd}' needs two numeric arguments.");
                 else
                     Console.WriteLine($"Error: '{cmd}' needs {paramInfos.Length} arguments.");
                 return;
             }

             var ctorArgs = new object[paramInfos.Length];
             bool bad = false;
             for (int i = 0; i < paramInfos.Length; i++)
             {
                 var p = paramInfos[i];
                 var raw = args[i + 1];
                 if (p.ParameterType == typeof(int))
                 {
                     if (int.TryParse(raw, out var val)) ctorArgs[i] = val;
                     else
                     {
                         Console.WriteLine($"Error: Both arguments for '{cmd}' must be numbers.");
                         bad = true; break;
                     }
                 }
                 else if (p.ParameterType == typeof(string))
                     ctorArgs[i] = raw;
                 else
                 {
                     Console.WriteLine($"Error: unsupported parameter type {p.ParameterType.Name}.");
                     bad = true; break;
                 }
             }
             if (bad) return;

             // Execute command
             var cmdObj = ci.Invoke(ctorArgs);
             var resultTask = (Task<string>)mediator.Send((dynamic)cmdObj);
             Console.WriteLine(resultTask.Result);
         }
         catch (Exception ex)
         {
             Console.WriteLine("A critical dependency or configuration is missing. Please check your setup.");
             Console.WriteLine($"Error details: {ex.Message}");
             Environment.Exit(1);
         }
     }
 }

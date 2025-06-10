using MediatR;
using ConsoleApp;
using Microsoft.Extensions.DependencyInjection;
using ConsoleApp.Application;

 public class Program
 {
     public static void Main(string[] args)
     {
         var services = new ServiceCollection();
         services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
         var provider = services.BuildServiceProvider();
         var mediator = provider.GetRequiredService<IMediator>();

         if (args.Length == 0)
         {
             Console.WriteLine("No arguments provided. Usage: ConsoleApp <command> [options]");
             Console.WriteLine("Try 'help' for more info.");
             return;
         }

         if (args[0].ToLower() == "help")
         {
             Console.WriteLine("Usage: ConsoleApp <command> [options]");
             Console.WriteLine("Commands:");
             Console.WriteLine("  greet <name>   - Greet the user by name");
             Console.WriteLine("  add <a> <b>    - Add two numbers");
             return;
         }

         switch (args[0].ToLower())
         {
             case "greet":
                 if (args.Length != 2)
                 {
                     Console.WriteLine("Error: 'greet' needs exactly one argument (name).");
                 }
                 else
                 {
                     var result = mediator.Send(new GreetCommand(args[1])).Result;
                     Console.WriteLine(result);
                 }
                 break;
             case "add":
                 if (args.Length != 3)
                 {
                     Console.WriteLine("Error: 'add' needs two numeric arguments.");
                 }
                 else if (int.TryParse(args[1], out int a) && int.TryParse(args[2], out int b))
                 {
                     var result = mediator.Send(new AddCommand(a, b)).Result;
                     Console.WriteLine(result);
                 }
                 else
                 {
                     Console.WriteLine("Error: Both arguments for 'add' must be numbers.");
                 }
                 break;
             default:
                 Console.WriteLine($"Unknown command: {args[0]}");
                 Console.WriteLine("Try 'help' for a list of commands.");
                 break;
         }
     }
 }

using MediatR;

namespace ConsoleApp.Application;

 public record GreetCommand(string Name) : IRequest<string>;

using MediatR;

namespace ConsoleApp.Application;

 public record AddCommand(int A, int B) : IRequest<string>;

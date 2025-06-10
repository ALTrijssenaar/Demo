using MediatR;

namespace ConsoleApp.Application
{
    public class GreetCommandHandler : IRequestHandler<GreetCommand, string>
    {
        public Task<string> Handle(GreetCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult($"Hello, {request.Name}!");
        }
    }
}

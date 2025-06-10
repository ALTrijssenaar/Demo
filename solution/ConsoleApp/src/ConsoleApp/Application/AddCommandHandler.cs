using MediatR;

namespace ConsoleApp.Application
{
    public class AddCommandHandler : IRequestHandler<AddCommand, string>
    {
        public Task<string> Handle(AddCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult($"Sum: {request.A + request.B}");
        }
    }
}

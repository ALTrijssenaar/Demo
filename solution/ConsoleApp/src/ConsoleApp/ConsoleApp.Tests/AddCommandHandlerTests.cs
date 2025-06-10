using Xunit;
using ConsoleApp.Application;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp.Tests
{
    public class AddCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ReturnsSum()
        {
            var handler = new AddCommandHandler();
            var command = new AddCommand(2, 3);
            var result = await handler.Handle(command, CancellationToken.None);
            Assert.Equal("Sum: 5", result);
        }
    }
}

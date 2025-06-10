using System.Threading;
using System.Threading.Tasks;
using ConsoleApp.Application;
using Xunit;

namespace ConsoleApp.Tests.Application
{
    public class GreetCommandHandlerTests
    {
        [Theory]
        [InlineData("Alice", "Hello, Alice!")]
        [InlineData("Bob", "Hello, Bob!")]
        [InlineData("", "Hello, !")]
        [InlineData(null, "Hello, !")]
        public async Task Handle_ReturnsCorrectGreeting(string name, string expected)
        {
            var handler = new GreetCommandHandler();
            var command = new GreetCommand(name);
            var result = await handler.Handle(command, CancellationToken.None);
            Assert.Equal(expected, result);
        }
    }
}

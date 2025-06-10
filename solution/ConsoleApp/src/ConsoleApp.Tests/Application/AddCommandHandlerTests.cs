using Xunit;
using ConsoleApp.Application;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp.Tests
{
    public class AddCommandHandlerTests
    {
        [Theory]
        [InlineData(1, 2, "Sum: 3")]
        [InlineData(-1, 5, "Sum: 4")]
        [InlineData(0, 0, "Sum: 0")]
        [InlineData(100, 200, "Sum: 300")]
        public async Task Handle_ReturnsCorrectSum(int a, int b, string expected)
        {
            var handler = new AddCommandHandler();
            var command = new AddCommand(a, b);
            var result = await handler.Handle(command, CancellationToken.None);
            Assert.Equal(expected, result);
        }
    }
}

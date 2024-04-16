using Xunit;

namespace WebApplication1Tests.Controllers
{
    public class CalculatorTests
    {
        [Fact]
        public void Add_ReturnsCorrectSum()
        {
            // Arrange
            var calculator = new Calculator();
            int a = 3;
            int b = 5;
            int expectedSum = 8;

            // Act
            int result = calculator.Add(a, b);

            // Assert
            Assert.Equal(expectedSum, result);
        }
    }
}

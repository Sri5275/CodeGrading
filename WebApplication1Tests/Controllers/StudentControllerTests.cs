using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SQLRepository.Repository;
using WebAppService.Service;
using Common.Models;
using System.Threading.Tasks;
using Xunit;
using Moq; // To mock dependencies

namespace YourNamespace.Tests
{
    public class StudentServiceTests
    {
        // Mock dependencies
        private readonly Mock<SqlConnection> _mockConnection;
        private readonly Mock<IStudentRepository> _mockStudentRepository;
        private readonly Mock<SqlCommand> _mockCommand;
        private readonly StudentService _studentService;

        public StudentServiceTests()
        {
            // Initialize mocks
            _mockConnection = new Mock<SqlConnection>();
            _mockStudentRepository = new Mock<IStudentRepository>();
            _mockCommand = new Mock<SqlCommand>();

            // Initialize the service with the mock dependencies
            // Initialize the service with the mock IStudentRepository
            _studentService = new StudentService(_mockStudentRepository.Object);

        }

        [Fact]
        public async Task GetNames_ReturnsListOfStudents()
        {
            // Arrange
            var expectedStudents = new List<Student>
            {
                new Student { id = 1, username = "JohnDoe", email = "john@example.com" },
                new Student { id = 2, username = "JaneDoe", email = "jane@example.com" }
            };

            // Set up the mock SqlConnection to open successfully
            _mockConnection.Setup(c => c.Open()).Verifiable();

            // Set up the mock SqlCommand and SqlDataReader to return the expected students
            var mockReader = new Mock<SqlDataReader>();
            mockReader.SetupSequence(r => r.ReadAsync())
                .ReturnsAsync(true)
                .ReturnsAsync(true)
                .ReturnsAsync(false); // End of results
            mockReader.Setup(r => r["id"]).Returns(expectedStudents[0].id);
            mockReader.Setup(r => r["username"]).Returns(expectedStudents[0].username);
            mockReader.Setup(r => r["email"]).Returns(expectedStudents[0].email);

            mockReader.Setup(r => r.ReadAsync()).ReturnsAsync(true);
            mockReader.Setup(r => r["id"]).Returns(expectedStudents[1].id);
            mockReader.Setup(r => r["username"]).Returns(expectedStudents[1].username);
            mockReader.Setup(r => r["email"]).Returns(expectedStudents[1].email);

            _mockCommand.Setup(c => c.ExecuteReaderAsync())
                .ReturnsAsync(mockReader.Object);

            // Act
            var result = await _studentService.getnames();

            // Assert
            Assert.Equal(expectedStudents.Count, result.Count);
            Assert.Equal(expectedStudents[0].id, result[0].id);
            Assert.Equal(expectedStudents[0].username, result[0].username);
            Assert.Equal(expectedStudents[0].email, result[0].email);
            Assert.Equal(expectedStudents[1].id, result[1].id);
            Assert.Equal(expectedStudents[1].username, result[1].username);
            Assert.Equal(expectedStudents[1].email, result[1].email);

            // Verify that the connection was opened and the command was executed
            _mockConnection.Verify(c => c.Open(), Times.Once);
            _mockCommand.Verify(c => c.ExecuteReaderAsync(), Times.Once);
        }
    }
}

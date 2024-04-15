using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SQLRepository.Repository;
using WebAppService.Service;
using Common.Models;
using System.Threading.Tasks;
using Xunit;
using Moq;
using WebAppService.Interface;

namespace WebApplication1Tests.Controllers
{
    public interface IStudentDataAccess
    {
        Task<IEnumerable<Student>> GetStudentsAsync();
    }

    public class StudentService
    {
        private readonly IStudentDataAccess _dataAccess;

        public StudentService(IStudentDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task<IEnumerable<Student>> GetStudentsAsync()
        {
            var students = await _dataAccess.GetStudentsAsync();
            return students;
        }
    }

    public class StudentServiceTests
    {
        private readonly Mock<IStudentDataAccess> _mockDataAccess;
        private readonly Mock<SqlConnection> _mockConnection;
        private readonly Mock<SqlCommand> _mockCommand;

        public StudentServiceTests()
        {
            _mockConnection = new Mock<SqlConnection>();
            _mockCommand = new Mock<SqlCommand>();
            _mockDataAccess = new Mock<IStudentDataAccess>();
        }

        [Fact]
        public async Task GetNames_ReturnsListOfStudents()
        {
            // Arrange
            var expectedStudents = new List<Student>
              {
                new Student { id = 1, username = "Nithish", email = "thop@gmail.com" },
                new Student { id = 2, username = "Naveen", email = "nav@gmail.com" }
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
            var _studentService = new StudentService(_mockDataAccess.Object);

            // Act
            var result = await _studentService.getnamesAsync();
            //var result = await _studentService.GetNamesAsync(); // Corrected method name

            // Assert
            Assert.Equal(expectedStudents.Count, result[0].Count);
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

using Common.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using WebApplication1.Controllers;
using Microsoft.Extensions.Configuration;
using WebAppService.Interface;

namespace WebApplication1.Tests.Controllers
{
    [TestClass]
    public class StudentControllerTests
    {
        public SqlConnection _connection;
        public StudentController _controller;
        public IStudentservice _std;
        [TestInitialize]
        public void TestInitialize()
        {
            // Initialize the SqlConnection for testing
            // Provide your connection string here
            _connection = new SqlConnection("Server=tcp:vnrsserver.database.windows.net,1433;Initial Catalog=StudentDatabase;Persist Security Info=False;User ID=Saketh;Password=Sri@5275;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

            // Initialize the StudentController with the connection
            _controller = new StudentController(_std);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // Clean up the connection after the tests
            _connection?.Dispose();
        }

        [TestMethod]
        public async Task UpdateStd_ValidData_ShouldReturnTrueAndUpdateRecord()
        {
            // Arrange
            var studentId = 1;
            var updatedStudent = new Student
            {
                id = studentId,
                username = "UpdatedUsername",
                email = "updatedemail@example.com"
            };

            // Act
            bool result = await _controller.updateStd(studentId, updatedStudent);

            // Assert
            Assert.IsTrue(result);

            // Verify the student record was updated in the database
            // You would execute a SQL query to retrieve the updated record and verify the changes
            var checkQuery = "SELECT username, email FROM students WHERE id = @id";
            SqlCommand cmd = new SqlCommand(checkQuery, _connection);
            cmd.Parameters.AddWithValue("@id", studentId);
            _connection.Open();
            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            Assert.IsTrue(reader.Read());
            Assert.AreEqual(updatedStudent.username, reader["username"].ToString());
            Assert.AreEqual(updatedStudent.email, reader["email"].ToString());

            reader.Close();
            _connection.Close();
        }
    }
}

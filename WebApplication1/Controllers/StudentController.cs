using Common.Models;
using Microsoft.AspNetCore.Mvc;
using WebAppService.Interface;
using WebAppService.Service;
namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentController:ControllerBase
    {
        
      public   IStudentservice _std; 
        public StudentController(IStudentservice stdser) {
            _std= stdser;
        }
        [HttpGet]
        [Route("getAll")]
        public async Task<List<Student>> getnames()
        {
            return await _std.getnames();
        }
        [HttpPost]
        [Route("addNewStd")]
        public async Task<bool> addNewStd(Student student)
        {
            return await _std.addNewStd(student);
        }
        [HttpPut]
        [Route("updateStd")]
        //How to provide the fixed input value
        public async Task<bool> updateStd(int id, Student student)
        {
            return await _std.updateStd(id, student);
        }
        [HttpDelete]
        [Route("deleteStd")]
        public async Task<bool> deleteStd(int id)
        {
            return await _std.deleteStd(id);
        }
        [HttpPost("test-repository")]
        public async Task<IActionResult> TestRepository([FromBody] string repositoryUrl)
        {
            // Download and extract the repository
            string repoPath = await DownloadAndExtractRepository(repositoryUrl);

            // Run tests and capture results
            string testResults = RunTests(repoPath);

            // Return test results as response
            return Ok(testResults);
        }

        private async Task<string> DownloadAndExtractRepository(string repositoryUrl)
        {
            // Create a unique temp directory to download and extract the repository
            string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);

            using (HttpClient client = new HttpClient())
            {
                // Download the repository archive
                byte[] archiveData = await client.GetByteArrayAsync(repositoryUrl);

                // Save the archive data as a tarball or zip file
                string archivePath = Path.Combine(tempDir, "repository.tar.gz");
                await System.IO.File.WriteAllBytesAsync(archivePath, archiveData);

                // Extract the archive (you may need to handle different formats, e.g., tar.gz, zip)
                // Here we assume a .tar.gz format
                ProcessStartInfo psi = new ProcessStartInfo("tar", $"-xzf {archivePath} -C {tempDir}")
                {
                    RedirectStandardError = true
                };
                var process = Process.Start(psi);
                process.WaitForExit();

                // Return the path to the extracted repository
                return Path.Combine(tempDir, "extracted-repo");
            }
        }

        private string RunTests(string repoPath)
        {
            // Change the current working directory to the repository path
            Directory.SetCurrentDirectory(repoPath);

            // Execute the test command (adjust the command according to the project's language and testing framework)
            // Example: Running 'dotnet test' for a .NET project
            ProcessStartInfo psi = new ProcessStartInfo("dotnet", "test")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var process = Process.Start(psi);
            process.WaitForExit();

            // Capture the output and error streams
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            // Return the test results
            return $"Output: {output}\nError: {error}";
        }


        /*// StudentService stdser = new StudentService();
         [HttpGet]
         [Route("StudentsStartsWith")]
         public  Task< List<Student> > GetStudents1(char ch)
         {
             return  _std.GetStudentByAlpha(ch);
         }

         [HttpGet]
         [Route("GetAllStudents")]
         public List<string> GetAllStudents()
         {
             return _std.Getstudents();
         }
         [HttpGet]
         [Route("NotStartsWith")]
         public List<string> GetStudentsnotstartswithc(char ch)
         {
             return _std.GetStudentNotStartWith(ch);
         }


         [HttpGet]
         [Route("IscontainsString")]

         public ActionResult<bool> iscontains1(string str)
         {
             bool t=_std.iscontains(str);
             if (t)
             {
                 return _std.iscontains(str);
             }
             return NotFound();
         }
        */


    }
}

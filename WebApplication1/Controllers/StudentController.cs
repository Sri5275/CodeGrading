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

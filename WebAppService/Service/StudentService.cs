using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
//using WebAppService.Data;
using Common.Models;
using SQLRepository.Repository;
using WebAppService.Interface;
namespace WebAppService.Service
{   public class StudentService:IStudentservice
    {
     
        public readonly IStudentRepository std2;
        public StudentService(IStudentRepository std1)
        {
            std2 = std1;
        }
        public async Task<List<Student>> getnames()
        {
            List<Student> names = await std2.getnames();
            return names;
        }
        public async Task<bool> addNewStd(Student student)
        {
            return await std2.addNewStd(student);
        }
        public async Task<bool> updateStd(int id, Student student)
        {
            return await std2.updateStd(id, student);
        }
        public async Task<bool> deleteStd(int id)
        {
            // Delegates the responsibility of deleting the student record to std2.
            return await std2.deleteStd(id);
        }

        /*  public List<string> Getstudents()
          {
              for (int i = 0; i < std2.getStudentNamesAll().Length;i++)
              {
                  std.Add(std2.getStudentNamesAll()[i]);
              }
              return std;
          }
          public List<string>  GetStudentByAlpha(char ch)
          {
              List<string> std1= new List<string>();
              for(int i=0; i < std2.getStudentNamesAll().Length; i++)
              {
                  if (std2.getStudentNamesAll()[i][0]==ch)
                  {
                      std1.Add(std2.getStudentNamesAll()[i]);
                  }
              }
              return std1;
          }
          public List<string> IsContains( char ch)
          {
              List<string> std3= new List<string>();
              for(int i=0;i< std2.getStudentNamesAll().Length;i++)
              {
                  if (std2.getStudentNamesAll()[i].Contains(ch))
                  {
                      std3.Add(std2.getStudentNamesAll()[i]);
                  }
              }
              return std3;
          }
          public List<string> GetStudentNotStartWith(char ch)
          {
              List<string> std4 = new List<string>();
              for (int i = 0; i < std2.getStudentNamesAll().Length; i++)
              {
                  if (std2.getStudentNamesAll()[i][0] !=ch)
                  {
                      std4.Add(std2.getStudentNamesAll()[i]);
                  }
              }
              return std4;
          }
          public bool iscontains(string str)
          {
              return std2.getStudentNamesAll().Contains(str);
          }*/

    }
}



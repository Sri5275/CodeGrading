using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models;
using SQLRepository.Repository;

namespace SQLRepository.Repository
{
    public interface IStudentRepository
    {
        //public string[] getStudentNamesAll();
        public  Task<List<Student>> getnames();

        public Task<bool> addNewStd(Student student);
        public Task<bool> updateStd(int id, Student student);
        public Task<bool> deleteStd(int id);

    }
}

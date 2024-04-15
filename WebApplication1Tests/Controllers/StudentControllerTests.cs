using Common.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using WebApplication1.Controllers;
using Microsoft.Extensions.Configuration;
using WebAppService.Interface;
using WebAppService.Service;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace WebApplication1.Tests.Controllers
{
    [TestClass()]
    public class StudentControllerTests
    {
        [TestMethod()]
        public void StudentControllerTest()
        {
            string s = "HelloWorld!";
            Assert.IsTrue(s.Contains("Hello"));


        }
    }



}

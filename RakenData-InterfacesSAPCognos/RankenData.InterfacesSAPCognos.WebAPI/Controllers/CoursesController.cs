using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RankenData.InterfacesSAPCognos.WebAPI.Controllers
{
    public class CoursesController : ApiController
    {
        static List<Course> courses = InitCourses();

        public IEnumerable<Course> GetCourse()
        {
            return courses;
        }

        private static List<Course> InitCourses()
        {
            var ret = new List<Course>();
            ret.Add(new Course { Id = 1, Tittle = "Colombia Negociante" });
            ret.Add(new Course { Id = 2, Tittle = "Desarrollo econimico" });
            return ret;
        }
    }

    public class Course
    {
        public int Id { get; set; }
        public string Tittle { get; set; }
    }
}

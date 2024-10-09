using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attendance_Management_System_CNPM.Models
{
    public class Courses
    {
        public int CourseID { get; set; }
        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public string GroupName { get; set; }
        public string SessionTime { get; set; }
        public int TeacherID { get; set; }
        public int ClassID { get; set; }
        public string ClassName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web_App_for_Image_Service.Models
{
    public class HomePageModel
    {
        private ClientTCP client;
        private int pics;
        private List<Student> _students;
        public List<Student> students
        {
            get
            {
                if (_students == null) _students = loadStudents();
                return _students;
            }
            private set { _students = value; }
        }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "status")]
        public string serviceStatus
        {
            get
            {
                if (client.isConnected) return "Running";
                return "Off";
            }
        }

        [Required]
        [Display(Name = "numOfPics")]
        public string numOfPictures
        {
            get
            {
                if (pics < 0) return "Waiting for server";
                return pics.ToString();
            }
        }
        public HomePageModel(int numOfPics)
        {
            pics = numOfPics;
            if (pics < 0) pics = -1;
            client = ClientTCP.getInstance();
        }

        List<Student> loadStudents()
        {
            List<Student> result = new List<Student>();
            string[] lines = System.IO.File.ReadAllLines(@"Students.txt");
            foreach (string line in lines)
            {
                Student student = new Student();
                student.Id = int.Parse(GetData(line, "Id"));
                student.FirstName = GetData(line, "First Name");
                student.LastName = GetData(line, "Last Name");
                result.Add(student);
            }
            return result;
        }

        public void UpdatePicCounter(object sender, int num)
        {
            pics = num;
        }

        public string GetData(string line, string field)
        {
            if (string.IsNullOrEmpty(line) || !line.Contains(field)) return null;
            string[] attributes = line.Split(",");
            foreach (string s in attributes)
            {
                if (s.Contains(field))
                {
                    return s.TrimStart((field + ":").ToCharArray());
                }
            }
            return null;
        }
    }

}

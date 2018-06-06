using System.ComponentModel.DataAnnotations;

namespace Web_App_for_Image_Service.Models
{
    public class Student
    {
        public Student() { }

        [Required]
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
    }
}

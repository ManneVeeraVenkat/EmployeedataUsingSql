using Microsoft.EntityFrameworkCore;
using NSwag.Annotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeedataUsingSql.Model
{
    public class EmployeeData
    {
        [Key]
        public int id { get; set; }
        public int employeeId { get; set; }

        //[JsonProperty("firstName")]
        public string firstName { get; set; }

        //[JsonProperty("lastName")]
        public string lastName { get; set; }

        //[JsonProperty("position")]
        public string desigination { get; set; }

        //[JsonProperty("department")]
        public string department { get; set; }

        //[JsonProperty("salary")]
        public string location { get; set; }

        public string skill { get; set; }
        public int salary { get; set; }

        //public string ProfilePicturePath { get; set; }


    }
}

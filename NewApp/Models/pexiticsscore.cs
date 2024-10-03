using System;
using System.ComponentModel.DataAnnotations;

namespace NewApp.Models
{
    public class pexiticsscore
    {
        [Key]
        public int id { get; set; }

        public string Report_Subattribute { get; set; }

        public string Question_Sub_Attribute { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }

        public int Maximum_Marks_For_Question { get; set; }

        public int Marks_For_Answer_Chosen { get; set; }

        public int Candidate_ID { get; set; }

        public string Name_Of_Candidate { get; set; }

        public string Start_Time_Stamp_Of_Test { get; set; }

        public string End_Time_Stamp_Of_Test { get; set; }

        public string Name_Of_Test { get; set; }
    }
}

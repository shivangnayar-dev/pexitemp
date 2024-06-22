using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewApp.Models
{
    public class AssessmentResults
    {
    
        public int Id { get; set; }

    
        public int candidate_id  { get; set; }

 
        public string candidate_name { get; set; }

  
        public string AssessmentSubAttributeId { get; set; }


        public string AssessmentSubAttribute { get; set; }
        
        public decimal sumofmarks { get; set; }

        public int CountofQuestiontoDisplay { get; set; }


        public decimal total_marks { get; set; }


        public decimal percentage { get; set; }

        public string Matrix { get; set; }

        public string ReportSubAttributeId { get; set; }
      
        public string ReportSubattribute { get; set; }




    }
}

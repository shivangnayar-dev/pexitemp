using System;
using System.ComponentModel.DataAnnotations;

namespace NewApp.Models
{
    public class CandidateSelectedOptions
    {
      
        public int candidate_id { get; set; }

        // Define other properties as per your table schema
        public string selected_option { get; set; }
        public string candidate_name { get; set; }
        public string question_id { get; set; }
        public int maxmarks { get; set; }
        public string AssessmentSubAttributeId { get; set; }
        public string AssessmentSubAttribute { get; set; }
        public int CountofQuestiontoDisplay { get; set; }
        public string SubjectiveCorrectAnswer { get; set; }
    }
}


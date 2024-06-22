using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewApp.Models
{
    public class QuestionData
    {
        [Key]
        public int SerialNumber { get; set; } // Assuming SerialNumber is the primary key

        // Removed Id property as it's not the primary key anymore

        public string AssessmentSubAttribute { get; set; }

        // Foreign key to CandidateDetails
        public int Id { get; set; } // Foreign key to CandidateDetails

        // Candidate details
        public string Email { get; set; }
        public string Adhar { get; set; }
        public string Mobile { get; set; }
        public string Name { get; set; }

        // Navigation property

    }

}
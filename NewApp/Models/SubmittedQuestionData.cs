using System;
using System.ComponentModel.DataAnnotations;

namespace NewApp.Models
{
    public class SubmittedQuestion
    {
        public int Id { get; set; }


        public string QuestionId { get; set; } = "0";


        public string Email { get; set; } = "0";


        public string Adhar { get; set; } = "0";


        public string Mobile { get; set; } = "0";

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}


using System;
namespace NewApp.Models
{
    public class DatabaseTable
    {
        public int Id { get; set; }
        public int candidate_id { get; set; }
        public string name { get; set; }
        public string gender { get; set; }
        public DateTime dob { get; set; }
        public int age { get; set; }
        public string qualification { get; set; }
        public DateTime time { get; set; }
      
    }
}


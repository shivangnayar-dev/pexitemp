using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewApp.Models
{
    public class Result
    {

        public int Id { get; set; }

        public int CandidateId { get; set; }

  
        public string TableName { get; set; }

     
        public string FieldName { get; set; }

 
        public string Value { get; set; }

        public string ReportName { get; set; }





    }
}


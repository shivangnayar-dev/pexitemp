using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewApp.Models;

namespace NewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionDataController : ControllerBase
    {
        private readonly CandidateDbContext _context;

        public QuestionDataController(CandidateDbContext context)
        {
            _context = context;
        }

        // POST: api/QuestionData/AddQuestionData

        [HttpPost("AddQuestionData")]
        public async Task<IActionResult> AddQuestionData([FromBody] QuestionData[] questionData)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    foreach (var data in questionData)
                    {
                        // Fetch the corresponding candidate_id from CandidateDetails based on email, Aadhar, or Mobile
                        int candidateId = FetchCandidateId(data.Email, data.Adhar, data.Mobile);

                        // Set the fetched candidate_id as the Id in QuestionData
                        data.Id = candidateId;

                        // Add QuestionData to the context
                        _context.QuestionData.Add(data);
                    }
                    await _context.SaveChangesAsync();
                    return Ok("Question data added successfully");
                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                // Log the error and return an error response
                Console.WriteLine($"Error adding question data: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("CheckCandidateSubmission")]
        public IActionResult CheckCandidateSubmission(string email, string adhar, string mobile)
        {
            try
            {
                // Fetch the candidate ID based on email, Aadhar, or Mobile
                int candidateId = FetchCandidateId(email, adhar, mobile);

                // Check if the candidate has submitted any data in QuestionData
                bool hasSubmitted = _context.QuestionData.Any(q => q.Id == candidateId);

                return Ok(new { hasSubmitted });
            }
            catch (Exception ex)
            {
                // Log the error and return an error response
                Console.WriteLine($"Error checking candidate submission: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("FetchAssessmentSubAttributes/{candidateId}")]
        public IActionResult FetchAssessmentSubAttributes(int candidateId)
        {
            try
            {
                // Fetch AssessmentSubAttributes for the given candidate ID
                var assessmentSubAttributes = _context.QuestionData
                    .Where(q => q.Id == candidateId)
                    .Select(q => q.AssessmentSubAttribute)
                    .Distinct()
                    .ToList();

                return Ok(assessmentSubAttributes);
            }
            catch (Exception ex)
            {
                // Log the error and return an error response
                Console.WriteLine($"Error fetching assessment subattributes: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("CheckCandidateId")]
        public IActionResult CheckCandidateId(int candidateId)
        {
            try
            {
                // Check if the candidate ID is present in the QuestionData table
                bool exists = _context.QuestionData.Any(q => q.Id == candidateId);

                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                // Log the error and return an error response
                Console.WriteLine($"Error checking candidate ID: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }


        // Method to fetch candidate_id from CandidateDetails based on email, Aadhar, or Mobile
        private int FetchCandidateId(string email, string adhar, string mobile)
        {
            int candidateId = 0; // Initialize candidateId to 0 as a default value

            // Query CandidateDetails table based on email, Aadhar, or Mobile
            var candidate = _context.Candidates.FirstOrDefault(c => c.email_address == email || c.Adhar_No == adhar || c.Mobile_No == mobile);

            // If candidate is found, set candidateId to candidate_id
            if (candidate != null)
            {
                candidateId = candidate.candidate_id;
            }

            return candidateId;
        }

    }
}
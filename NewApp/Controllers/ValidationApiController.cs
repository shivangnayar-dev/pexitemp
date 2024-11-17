using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewApp.Models;
using System;
using System.Threading.Tasks;

namespace NewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValidationApiController : ControllerBase
    {
        private readonly CandidateDbContext _context;

        public ValidationApiController(CandidateDbContext context)
        {
            _context = context;
        }

        // This endpoint will be responsible for updating the login status (login_page) and other fields of a candidate.
        [HttpPost("UpdateLoginStatus")]
        public async Task<IActionResult> UpdateLoginStatus([FromBody] UpdateLoginStatusRequest request)
        {
            if (request == null || request.candidateid <= 0)
            {
                return BadRequest("Invalid CandidateId");
            }

            try
            {
                // Check if the candidate already exists in the database
                var candidate = await _context.Validationtable
                                              .FirstOrDefaultAsync(c => c.candidateid == request.candidateid);

                if (candidate == null)
                {
                    // If candidate doesn't exist, create a new candidate entry
                    candidate = new Validationtable
                    {
                        candidateid = request.candidateid,
                        login_page = true,  // Set login_page to true
                        register_page = false,  // Default to false for new candidates
                        info_page = false,      // Default to false for new candidates
                        candidateinfo_page = false,  // Default to false for new candidates
                        teststatus = "Not Started",  // Default to "Not Started" for new candidates
                        timestamp = DateTime.Now // Set current timestamp for new candidates
                    };

                    // Add new candidate to the database
                    _context.Validationtable.Add(candidate);
                }
                else
                {
                    // If candidate exists, update their login status and other fields
                    candidate.login_page = true;  // Ensure login_page is set to true
                    candidate.register_page = false;
                    candidate.info_page = false;
                    candidate.candidateinfo_page = false;
                    candidate.teststatus = request.teststatus;  // Update test status if provided
                    candidate.timestamp = request.timestamp;  // Update timestamp
                }

                // Save changes to the database (either new candidate or updated candidate)
                await _context.SaveChangesAsync();

                return Ok(new { success = true });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new { success = false, message = $"Database error: {dbEx.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpPost("GetAndUpdateCandidateInfoPage")]
        public async Task<IActionResult> GetAndUpdateCandidateInfoPage([FromBody] UpdateLoginStatusRequest request)
        {
            if (request == null || request.candidateid <= 0)
            {
                return BadRequest("Invalid CandidateId");
            }

            try
            {
                // Fetch the candidate from the database based on the provided candidateid
                var candidate = await _context.Validationtable
                                              .FirstOrDefaultAsync(c => c.candidateid == request.candidateid);

                if (candidate == null)
                {
                    return NotFound("Candidate not found");
                }

                // Update the candidateinfo_page to true
                candidate.candidateinfo_page = true;
                candidate.info_page = true;

                // Save changes to update the candidate's candidateinfo_page field in the database
                await _context.SaveChangesAsync();

                // Return the candidate's updated information
                return Ok(new
                {
                    success = true,
                    message = "Candidate info updated successfully",
                    candidateid = candidate.candidateid,
                    candidateinfo_page = candidate.candidateinfo_page
                });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new { success = false, message = $"Database error: {dbEx.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpPost("UpdateTestStatus")]
        public async Task<IActionResult> UpdateTestStatus([FromBody] UpdateLoginStatusRequest request)
        {
            if (request == null || request.candidateid <= 0)
            {
                return BadRequest("Invalid CandidateId");
            }

            try
            {
                // Fetch the candidate from the database based on the provided candidateid
                var candidate = await _context.Validationtable
                                              .FirstOrDefaultAsync(c => c.candidateid == request.candidateid);

                if (candidate == null)
                {
                    return NotFound("Candidate not found");
                }

                // Update the candidateinfo_page to true
                candidate.teststatus = "completed";

                // Save changes to update the candidate's candidateinfo_page field in the database
                await _context.SaveChangesAsync();

                // Return the candidate's updated information
                return Ok(new
                {
                    success = true,
                    message = "Candidate info updated successfully",
                    candidateid = candidate.candidateid,
                    candidateinfo_page = candidate.candidateinfo_page
                });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new { success = false, message = $"Database error: {dbEx.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpGet("GetCandidateStatus")]
        public async Task<IActionResult> GetCandidateStatus([FromQuery] int candidateid)
        {
            if (candidateid <= 0)
            {
                return BadRequest("Invalid CandidateId");
            }

            try
            {
                // Fetch the candidate from the database based on the provided candidateid
                var candidate = await _context.Validationtable
                                              .FirstOrDefaultAsync(c => c.candidateid == candidateid);

                if (candidate == null)
                {
                    return NotFound("Candidate not found");
                }

                // Return the candidate's status information
                return Ok(new
                {
                    success = true,
                    candidateid = candidate.candidateid,
                    login_page = candidate.login_page,
                    register_page = candidate.register_page,
                    info_page = candidate.info_page,
                    candidateinfo_page = candidate.candidateinfo_page,
                    teststatus = candidate.teststatus,
                    timestamp = candidate.timestamp
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }


        // Define a class to represent the request payload for updating the login status and other fields
        public class UpdateLoginStatusRequest
        {
            public int id { get; set; }  // Primary Key, auto-incremented (as per your database schema)
            public int candidateid { get; set; }  // Candidate ID (maps to candidateid in database)

            // Default to false for all validation statuses
            public bool login_page { get; set; } = false;  // Default to false
            public bool register_page { get; set; } = false;  // Default to false
            public bool info_page { get; set; } = false;  // Default to false
            public bool candidateinfo_page { get; set; } = false;  // Default to false

            // Default to "Not Started" or any other default string value for test status
            public string teststatus { get; set; } = "Not Started";  // Default to "Not Started"

            // Default to the current date and time (or any value you want) for the timestamp
            public DateTime timestamp { get; set; } = DateTime.Now;  // Default to current date/time
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NewApp.Controllers
{
    [Route("api/[controller]")]
    public class CandidateController : Controller
    {
        private readonly CandidateDbContext _context;
        private readonly ILogger<CandidateController> _logger;

        public CandidateController(CandidateDbContext context, ILogger<CandidateController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<CandidateDetails> Get()
        {
            return _context.Candidates.ToList();
        }

        private async Task<(bool exists, string? identifierType)> CheckForDuplicateAsync(CandidateDetails candidate)
        {
            if (!string.IsNullOrEmpty(candidate.Mobile_No) && candidate.Mobile_No != "0")
            {
                bool mobileExists = await _context.Candidates.AnyAsync(c => c.Mobile_No == candidate.Mobile_No);
                return (mobileExists, "Mobile");
            }
            else if (!string.IsNullOrEmpty(candidate.Adhar_No) && candidate.Adhar_No != "0")
            {
                bool aadharExists = await _context.Candidates.AnyAsync(c => c.Adhar_No == candidate.Adhar_No);
                return (aadharExists, "Aadhar");
            }
            else if (!string.IsNullOrEmpty(candidate.email_address) && candidate.email_address != "0")
            {
                bool emailExists = await _context.Candidates.AnyAsync(c => c.email_address == candidate.email_address);
                return (emailExists, "Email");
            }

            return (false, string.Empty);
        }

        [HttpPost("CheckDuplicate")]
        public async Task<IActionResult> CheckDuplicate([FromBody] CandidateDetails candidate)
        {
            try
            {
                _logger.LogInformation($"Checking duplicate for email: {candidate.email_address}");

                var (exists, identifierType) = await CheckForDuplicateAsync(candidate);
                string? password = null;
                string? name = null;
                string? gender = null;
                DateTime? dob = null;
                string? country = null;
                string? location = null;
                string? organization = null;
                string? qualification = null;



                if (exists)
                {
                    // Retrieve password based on the existing entit
                    (password,
    name,
    gender,
    dob, country, location, organization, qualification) = await GetPasswordAndNameByUserAsync(candidate, identifierType);
                }

                return Json(new
                {
                    exists,
                    Password = password,
                    Name = name,
                    Gender = gender,
                    Dob = dob,
                    Country = country,
                    Location = location,
                    Organization = organization,
                    Qualification = qualification

                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking duplicate: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        private async Task<(string? password, string? name, string? gender, DateTime? dob, string? country, string? location, string? organization, string? qualification)> GetPasswordAndNameByUserAsync(CandidateDetails candidate, string identifierType)
        {
            try
            {
                _logger.LogInformation($"Retrieving password and name for {identifierType}");

                var query = _context.Candidates
                    .Where(c =>
                        (identifierType == "Mobile" && c.Mobile_No == candidate.Mobile_No) ||
                        (identifierType == "Aadhar" && c.Adhar_No == candidate.Adhar_No) ||
                        (identifierType == "Email" && c.email_address == candidate.email_address));

                var result = await query
                    .OrderByDescending(c =>
                        identifierType == "Mobile" ? c.Mobile_No :
                        identifierType == "Aadhar" ? c.Adhar_No :
                        identifierType == "Email" ? c.email_address :
                        null)
                    .ThenByDescending(c => c.candidate_id)// Default ordering by null in case of unknown identifier
                    .FirstOrDefaultAsync();

                if (result != null)
                {
                    return (
                        result.password,
                        result.name,
                        result.gender,
                        result.dob,
                        result.country,
                        result.location,
                        result.qualification,
                        result.organization
                    );
                }
                else
                {
                    // Log a message if the candidate is not found
                    _logger.LogInformation($"No candidate found for {identifierType}");

                    return (null, null, null, null, null, null, null, null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving password and name: {ex}");
                return (null, null, null, null, null, null, null, null);
            }
        }



        [HttpPost("submit")]
        public async Task<IActionResult> SubmitUser([FromBody] CandidateDetails candidate)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Check if the candidate with the same Mobile_No, Adhar_No, or email_address already exists
                    var existingCandidate = await _context.Candidates.FirstOrDefaultAsync(c =>
                        (c.Mobile_No != "0" && c.Mobile_No == candidate.Mobile_No) ||
                        (c.Adhar_No != "0" && c.Adhar_No == candidate.Adhar_No) ||
                        (c.email_address != "0" && c.email_address == candidate.email_address));

                    if (existingCandidate != null)
                    {
                        // Ensure unique values in SelectedOptions and SelectedOptionTimestamps
                        var existingOptions = existingCandidate.SelectedOptions.Split(',').ToList();
                        var newOptions = candidate.SelectedOptions.Split(',').ToList();
                        var combinedOptions = existingOptions.Union(newOptions).Distinct().ToList();
                        existingCandidate.SelectedOptions = string.Join(",", combinedOptions);

                        var existingTimestamps = existingCandidate.SelectedOptionTimestamps.Split(',').ToList();
                        var newTimestamps = candidate.SelectedOptionTimestamps.Split(',').ToList();
                        var combinedTimestamps = existingTimestamps.Union(newTimestamps).Distinct().ToList();
                        existingCandidate.SelectedOptionTimestamps = string.Join(",", combinedTimestamps);

                        // Update other candidate details
                        existingCandidate.name = candidate.name;
                        existingCandidate.gender = candidate.gender;
                        existingCandidate.dob = candidate.dob;
                        existingCandidate.country = candidate.country;
                        existingCandidate.location = candidate.location;
                        existingCandidate.organization = candidate.organization;
                        existingCandidate.qualification = candidate.qualification;
                        existingCandidate.otherOrganization = candidate.otherOrganization;
                        existingCandidate.selectedSpecializations = candidate.selectedSpecializations;
                        existingCandidate.selectedIndustries = candidate.selectedIndustries;
                        existingCandidate.interest = candidate.interest;
                        existingCandidate.pursuing = candidate.pursuing;
                        existingCandidate.transactionId = candidate.transactionId;
                        existingCandidate.rating = candidate.rating;
                        existingCandidate.storedTestCode = candidate.storedTestCode;
                        existingCandidate.upiPhoneNumber = candidate.upiPhoneNumber;
                        existingCandidate.amountPaid = candidate.amountPaid;
                        existingCandidate.mathScience = candidate.mathScience;
                        existingCandidate.testProgress = candidate.testProgress;

                        _context.Candidates.Update(existingCandidate);
                    }
                    else
                    {
                        // Ensure unique values in new candidate's SelectedOptions and SelectedOptionTimestamps
                        candidate.SelectedOptions = string.Join(",", candidate.SelectedOptions.Split(',').Distinct());
                        
                        _context.Candidates.Add(candidate);
                    }

                    await _context.SaveChangesAsync();
                    return Ok("Data submitted successfully");
                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error submitting user data: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }


        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var candidate = _context.Candidates.Find(id);

            if (candidate == null)
            {
                return NotFound();
            }

            return Ok(candidate);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] CandidateDetails candidate)
        {
            if (id != candidate.candidate_id)
            {
                return BadRequest();
            }

            _context.Entry(candidate).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Candidates.Any(c => c.candidate_id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        [HttpGet("FetchCandidateId")]
        public IActionResult FetchCandidateId(string email, string adhar, string mobile)
        {
            try
            {
                // Query CandidateDetails table based on email, Aadhar, or Mobile
                var candidate = _context.Candidates.FirstOrDefault(c => c.email_address == email || c.Adhar_No == adhar || c.Mobile_No == mobile);

                // If candidate is found, return the candidate_id
                if (candidate != null)
                {
                    return Ok(new { candidateId = candidate.candidate_id });
                }

                // If no candidate is found, return NotFound
                return NotFound();
            }
            catch (Exception ex)
            {
                // Log the error and return an error response
                Console.WriteLine($"Error fetching candidate ID: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    



    [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var candidate = _context.Candidates.Find(id);

            if (candidate == null)
            {
                return NotFound();
            }

            _context.Candidates.Remove(candidate);
            _context.SaveChanges();

            return Ok(candidate);
        }
    }
}


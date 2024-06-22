using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NewApp.Models;
using Newtonsoft.Json;

namespace NewApp.Controllers
{
    [Route("api/[controller]")]
    public class AcademicController : Controller
    {
        private readonly CandidateDbContext _context;

        public AcademicController(CandidateDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> GetAcademicAreas()
        {
            try
            {
                var academicAreas = _context.AcademicStream
                    .Select(a => a.AcademicArea)
                    .ToList();

                Console.WriteLine($"Retrieved Academic Areas: {JsonConvert.SerializeObject(academicAreas)}");

                return Ok(academicAreas);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving academic areas: {ex.Message}");
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("GetIdByAcademicArea")]
        public ActionResult<Guid?> GetIdByAcademicArea(string academicArea)
        {
            try
            {
                var academicStream = _context.AcademicStream
                    .FirstOrDefault(a => a.AcademicArea == academicArea);

                return Ok(academicStream?.AcademicAreaID);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving academic area ID: {ex.Message}");
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using NewApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace NewApp.Controllers
{
    [Route("api/[controller]")]
    public class BenchmarkController : Controller
    {
        private readonly CandidateDbContext _context;

        public BenchmarkController(CandidateDbContext context)
        {
            _context = context;
        }
        [HttpGet("GetAllBenchmarks")]
        public ActionResult<IEnumerable<Benchmarkmodel>> GetAllBenchmarks()
        {
           return _context.benchmarkmodel.ToList();
        }

        // POST api/Benchmark/CheckBenchmarkValidity


        // GET api/Benchmark/GetAllBenchmarks

    }
}

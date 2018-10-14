using KzsRest.Engine.Keys;
using KzsRest.Engine.Services.Abstract;
using KzsRest.Models;
using KzsRest.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KzsRest.Controllers
{
    [Route("api/standings")]
    [ApiController]
    public class StandingsController : ControllerBase
    {
        readonly IKzsParser kzsParser;
        readonly ICacheService cacheService;
        public StandingsController(IKzsParser kzsParser, ICacheService cacheService)
        {
            this.kzsParser = kzsParser;
            this.cacheService = cacheService;
        }
        [HttpGet("{competition:regex(^u\\d{{1,2}}$)}/{gender}/{division}")]
        public async Task<ActionResult<Standings[]>> Get(string competition, Gender gender, DivisionType division)
        {
            if (!int.TryParse(competition.TrimStart('u', 'U'), out int uRating))
            {
                return BadRequest($"Invalid U rating");
            }
            var topLevel = await kzsParser.GetTopLevelAsync(CancellationToken.None);
            var query = from ml in topLevel.MinorLeagues
                        where ml.ULevel == uRating && ml.Gender == gender
                        from d in ml.Divisions
                        where d.DivisionType == division
                        select d;
            var current = query.SingleOrDefault();
            if (current == null)
            {
                return NotFound();
            }
            var result = await cacheService.GetDataAsync<string, Standings[]>(
                current.Url,
                TimeSpan.FromMinutes(15),
                (k, ct) => kzsParser.GetStandingsAsync(k, ct), 
                CancellationToken.None);
            return result;
        }
    }
}
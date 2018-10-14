using KzsRest.Engine.Services.Abstract;
using KzsRest.Models;
using Microsoft.AspNetCore.Mvc;
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
        public StandingsController(IKzsParser kzsParser)
        {
            this.kzsParser = kzsParser;
        }
        [HttpGet("{competition:regex(^u\\d{{1,2}}$)}/{gender}/{division}")]
        public async Task<ActionResult<Standings[]>> Get(string competition, Gender gender, DivisionType division)
        {
            var topLevel = await kzsParser.GetTopLevelAsync(CancellationToken.None);
            if (!int.TryParse(competition.TrimStart('u', 'U'), out int uRating))
            {
                return BadRequest($"Invalid U rating");
            }
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
            var result = await kzsParser.GetStandingsAsync(current.Url, CancellationToken.None);
            return result;
        }
    }
}
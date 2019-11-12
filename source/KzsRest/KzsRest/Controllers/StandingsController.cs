using KzsRest.Engine;
using KzsRest.Engine.Services.Abstract;
using KzsRest.Models;
using Microsoft.AspNetCore.Http;
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
        [HttpGet("minor-league/{competition:regex(^u\\d{{1,2}}$)}/{gender}/{division}")]
        public async Task<ActionResult<LeagueOverview>> GetMinorLeague(string competition, Gender gender, DivisionType division)
        {
            try
            {
                if (!int.TryParse(competition.TrimStart('u', 'U'), out int uRating))
                {
                    return BadRequest($"Invalid U rating");
                }
                var topLevel = await kzsParser.GetTopLevelAsync(CancellationToken.None);
                var query = from ml in topLevel.MinorLeagues
                            where ml.ULevel == uRating && ml.Gender == gender
                            where ml.DivisionType == division
                            select ml;
                var current = query.SingleOrDefault();
                if (current == null)
                {
                    return NotFound();
                }
                var result = await cacheService.GetDataAsync(
                    current,
                    TimeSpan.FromMinutes(15),
                    (k, ct) => kzsParser.GetLeagueOverviewAsync(current.Id, ct),
                    CancellationToken.None);
                return result;
            }
            catch (DomParsingException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        [HttpGet("{gender}/{division:regex(^(1|2|3|4)$)}")]
        public async Task<ActionResult<LeagueOverview>> GetMajorLeague(Gender gender, int division)
        {
            try
            {
                var topLevel = await kzsParser.GetTopLevelAsync(CancellationToken.None);

                var query = from m in topLevel.MajorLeagues
                            where m.Gender == gender && m.Division == division
                            select m;
                var current = query.SingleOrDefault();
                if (current == null)
                {
                    return NotFound();
                }
                var result = await cacheService.GetDataAsync(
                    current,
                    TimeSpan.FromMinutes(15),
                    (k, ct) => kzsParser.GetLeagueOverviewAsync(current.Id, ct),
                    CancellationToken.None);
                return result;
            }
            catch (DomParsingException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        [HttpGet("{gender}/pokal")]
        public async Task<ActionResult<LeagueOverview>> GetMajorCup(Gender gender)
        {
            try
            {
                var topLevel = await kzsParser.GetTopLevelAsync(CancellationToken.None);
                var query = from m in topLevel.MajorCupLeagues
                            where m.Gender == gender
                            select m;
                var current = query.SingleOrDefault();
                if (current == null)
                {
                    return NotFound();
                }
                var result = await cacheService.GetDataAsync(
                    current,
                    TimeSpan.FromMinutes(15),
                    (k, ct) => kzsParser.GetLeagueOverviewAsync(current.Id, ct),
                    CancellationToken.None);
                return result;
            }
            catch (DomParsingException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        [HttpGet("{gender}/mini-pokal")]
        public async Task<ActionResult<LeagueOverview>> GetMinorCup(Gender gender)
        {
            try
            {
                var topLevel = await kzsParser.GetTopLevelAsync(CancellationToken.None);
                var query = from m in topLevel.MinorCupLeagues
                            where m.Gender == gender
                            select m;
                var current = query.SingleOrDefault();
                if (current == null)
                {
                    return NotFound();
                }
                var result = await cacheService.GetDataAsync(
                    current,
                    TimeSpan.FromMinutes(15),
                    (k, ct) => kzsParser.GetLeagueOverviewAsync(current.Id, ct),
                    CancellationToken.None);
                return result;
            }
            catch (DomParsingException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
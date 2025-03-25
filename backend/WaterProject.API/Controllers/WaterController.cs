using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WaterProject.API.Data;

namespace WaterProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WaterController : ControllerBase
    {
        private WaterDbContext _waterContext;

        public WaterController(WaterDbContext temp) 
        {
            _waterContext = temp;
        }

        [HttpGet("AllProjects")]
        public IActionResult GetProjects(int pageSize = 10, int pageNum = 1, [FromQuery] List<string>? projectTypes = null)
        {

            var query = _waterContext.Projects.AsQueryable();

            if (projectTypes != null && projectTypes.Any()) 
            {
                query = query.Where(p => projectTypes.Contains(p.ProjectType));
            }

            string? FavProjectType = Request.Cookies["FavoriteProjectType"];
            Console.WriteLine("*******COOKIE****** " + FavProjectType);

            HttpContext.Response.Cookies.Append("FavoriteProjectType", "Borehole Well and Hand Pump", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.Now.AddMinutes(1)
            });
            
            
            var totalNumProjects = query.Count();

            var list = query
            .Skip((pageNum-1) * pageSize)
            .Take(pageSize)
            .ToList();


            var newObject = new
            {
                Projects = list,
                TotalNumProjects = totalNumProjects
            };

            return Ok(newObject);
        }

        [HttpGet("GetProjectTypes")]
        public IActionResult GetProjectTypes()
        {
            var projectTypes = _waterContext.Projects
                .Select(p => p.ProjectType)
                .Distinct()
                .ToList();

            return Ok(projectTypes);
        }

        //[HttpGet("FunctionalProjects")]
        //public IEnumerable<Project> GetFunctionalProjects()
        //{
        //    var list = _waterContext.Projects.Where(p => p.ProjectFunctionalityStatus == "Functional").ToList();

        //    return list;
        //}

    }
}

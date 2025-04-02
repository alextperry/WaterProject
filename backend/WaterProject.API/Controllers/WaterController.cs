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
        [HttpPost("addproject")]

        public IActionResult AddProject([FromBody]Project newProject) {
            _waterContext.Projects.Add(newProject);
            _waterContext.SaveChanges();
            return Ok(newProject);
        }


        [HttpPut("updateproject/{projectId}")]
        public IActionResult UpdateProject(int projectId, [FromBody] Project updatedProject) {
            var existingProject = _waterContext.Projects.Find(projectId);
            
            if (existingProject == null) {
                return NotFound($"Project with ID {projectId} not found.");
            }

            // Update the project fields
            existingProject.ProjectName = updatedProject.ProjectName;
            existingProject.ProjectType = updatedProject.ProjectType;
            existingProject.ProjectRegionalProgram = updatedProject.ProjectRegionalProgram;
            existingProject.ProjectImpact = updatedProject.ProjectImpact;
            existingProject.ProjectPhase = updatedProject.ProjectPhase;
            existingProject.ProjectFunctionalityStatus = updatedProject.ProjectFunctionalityStatus;

            // Save the changes to the database
            _waterContext.SaveChanges();

            // Return a success response
            return Ok(existingProject);
        }


        [HttpDelete("deleteproject/{projectId}")]
        public IActionResult DeleteProject(int projectId) {
            var project = _waterContext.Projects.Find(projectId);

            if (project == null) {
                return NotFound(new {message = "Project not found"});
            }
            _waterContext.Projects.Remove(project);
            _waterContext.SaveChanges();

            return NoContent();
        }

        //[HttpGet("FunctionalProjects")]
        //public IEnumerable<Project> GetFunctionalProjects()
        //{
        //    var list = _waterContext.Projects.Where(p => p.ProjectFunctionalityStatus == "Functional").ToList();

        //    return list;
        //}

    }
}

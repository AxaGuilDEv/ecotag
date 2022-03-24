using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ml.Cli.WebApp.Server.Datasets;
using Ml.Cli.WebApp.Server.Datasets.Database;
using Ml.Cli.WebApp.Server.Datasets.Database.FileStorage;
using Ml.Cli.WebApp.Server.Oidc;
using Ml.Cli.WebApp.Server.Projects.Cmd;
using Ml.Cli.WebApp.Server.Projects.Database.Project;

namespace Ml.Cli.WebApp.Server.Projects
{
    [Route("api/server/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.DataAnnoteur)]
    public class ProjectsController : Controller
    {
        [HttpGet("{projectId}/files/{id}")]
        [ResponseCache(Duration = 1)]
        public async Task<IActionResult> GetProjectFile([FromServices] GetProjectFileCmd getProjectFileCmd, string projectId, string id)
        {
            var nameIdentifier = User.Identity.GetSubject();
            var result = await getProjectFileCmd.ExecuteAsync(projectId, id, nameIdentifier);

            if (!result.IsSuccess)
            {
                var errorKey = result.Error.Key;
                return errorKey switch
                {
                    FileService.FileNameMissing => NotFound(),
                    GetProjectFileCmd.DatasetNotFound => NotFound(),
                    DatasetsRepository.FileNotFound => NotFound(),
                    ProjectsRepository.NotFound => NotFound(),
                    _ => Forbid()
                };
            }

            var file = result.Data;
            return File(file.Stream, file.ContentType, file.Name);
        }
        
        [HttpGet("{id}/{datasetId}", Name = "GetProjectDatasetById")]
        [ResponseCache(Duration = 1)]
        public async Task<ActionResult<GetDataset>> GetDataset([FromServices] GetProjectDatasetCmd getprojectDatasetCmd, string id, string datasetId)
        {
            var nameIdentifier = User.Identity.GetSubject();
            var getDatasetResult = await getprojectDatasetCmd.ExecuteAsync(datasetId, id, nameIdentifier);

            if (!getDatasetResult.IsSuccess)
            {
                var errorKey = getDatasetResult.Error.Key;
                return errorKey switch
                {
                    GetProjectDatasetCmd.DatasetNotFound => NotFound(),
                    ProjectsRepository.NotFound => NotFound(),
                    _ => Forbid()
                };
            }

            return Ok(getDatasetResult.Data);
        }

        [HttpGet]
        [ResponseCache(Duration = 1)]
        public async Task<ActionResult<IEnumerable<ProjectDataModel>>> GetAllProjects(
            [FromServices] GetAllProjectsCmd getAllProjectsCmd)
        {
            var nameIdentifier = User.Identity.GetSubject();
            var result = await getAllProjectsCmd.ExecuteAsync(nameIdentifier);
            return Ok(result);
        }

        [HttpGet("{id}", Name = "GetProjectById")]
        public async Task<ActionResult<ProjectDataModel>> GetProject([FromServices] GetProjectCmd getProjectCmd, string id)
        {
            var nameIdentifier = User.Identity.GetSubject();
            var commandResult = await getProjectCmd.ExecuteAsync(id, nameIdentifier);
            if (!commandResult.IsSuccess)
            {
                return commandResult.Error.Key == ProjectsRepository.Forbidden ? Forbid() : BadRequest(
                    commandResult.Error);
            }
            return Ok(commandResult.Data);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> Create([FromServices] CreateProjectCmd createProjectCmd,
            CreateProjectInput createProjectInput)
        {
            var creatorNameIdentifier = User.Identity.GetSubject();
            var commandResult = await createProjectCmd.ExecuteAsync(new CreateProjectWithUserInput{CreateProjectInput = createProjectInput, CreatorNameIdentifier = creatorNameIdentifier});
            if (!commandResult.IsSuccess)
            {
                return BadRequest(commandResult.Error);
            }

            return Created(commandResult.Data, commandResult.Data);
        }
        
        /*[HttpPost("{projectId}/annotations/{fileId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Annotation([FromServices]DatasetContext datasetContext,string projectId, string fileId, AnnotationInput annotationInput)
        {
            var id = Guid.NewGuid().ToString();
            var creatorNameIdentifier = User.Identity.GetSubject();
            var annotation = new AnnotationModel()
            {
                FileId = new Guid(fileId) , ProjectId = new Guid(projectId), 
                ExpectedOutput = annotationInput.ExpectedOutput,
                TimeStamp = DateTime.Now.Ticks,
                CreatorNameIdentifier = creatorNameIdentifier
            };
            datasetContext.Annotations.Add(annotation);
            await datasetContext.SaveChangesAsync();
            
            return Created($"{projectId}/annotations/{fileId}/{id}", annotation.Id);
        }
        
        [HttpPut("{projectId}/annotations/{fileId}/{annotationId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Annotation([FromServices]DatasetContext datasetContext, string projectId, string fileId, string annotationId, AnnotationInput annotationInput)
        {
            var annotation =  await datasetContext.Annotations.FirstOrDefaultAsync(a => a.Id == new Guid(annotationId));
            annotation.ExpectedOutput = annotationInput.ExpectedOutput;
            await datasetContext.SaveChangesAsync();
            return Ok();
        }*/
        
        [HttpPost("{projectId}/annotations/{fileId}/{annotationId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Annotation([FromServices]SaveAnnotationCmd saveAnnotationCmd, string projectId, string fileId, string annotationId, AnnotationInput annotationInput)
        {
            var creatorNameIdentifier = User.Identity.GetSubject();
            var commandResult = await saveAnnotationCmd.ExecuteAsync(new SaveAnnotationInput()
            {
                ProjectId = projectId,
                FileId = fileId,
                AnnotationId = annotationId == "null" ? null : annotationId,
                AnnotationInput = annotationInput,
                CreatorNameIdentifier = creatorNameIdentifier
            });
            if (!commandResult.IsSuccess)
            {
                return commandResult.Error.Key == ProjectsRepository.Forbidden
                    ? Forbid()
                    : BadRequest(commandResult.Error);
            }

            return annotationId == "null" ?
                Created($"{projectId}/annotations/{fileId}/{commandResult.Data}", commandResult.Data) :
                NoContent();
        }
        
        [HttpPost("{projectId}/reserve")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IList<ReserveOutput>>> Reserve([FromServices] ReserveCmd reserveCmd, string projectId, ReserveInput fileInput)
        {
            var creatorNameIdentifier = User.Identity.GetSubject();
           var reservations = await reserveCmd.ExecuteAsync(projectId, fileInput.FileId, creatorNameIdentifier);

           return Ok(reservations.Data);
        }
        
    }
}

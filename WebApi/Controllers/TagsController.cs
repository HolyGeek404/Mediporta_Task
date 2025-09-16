using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Features.Commands.RefreshTags;
using Model.Features.Queries.GetTags;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TagsController(IMediator mediator, ILogger<TagsController> logger) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "GetTags")]
    public async Task<IActionResult> Get([FromQuery] GetTagsQuery query)
    {
        logger.LogInformation("GET /tags called with query: {@Query}", query);

        try
        {
            var result = await mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while fetching tags for query: {@Query}", query);
            return StatusCode(500, "An error occurred while fetching tags.");
        }
    }

    [HttpPost]
    [Route("refresh")]
    [Authorize(Roles = "RefreshTags")]
    public async Task<IActionResult> RefreshTags()
    {
        logger.LogInformation("POST /tags/refresh called");

        try
        {
            var command = new RefreshTagsCommand();
            var result = await mediator.Send(command);

            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while refreshing tags");
            return StatusCode(500, "An error occurred while refreshing tags.");
        }
    }
}
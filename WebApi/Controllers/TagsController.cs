using MediatR;
using Microsoft.AspNetCore.Mvc;
using Model.Features.Commands.RefreshTags;
using Model.Features.Queries.GetTags;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TagsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery]GetTagsQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }
    
    [HttpPost]
    [Route("refresh")]
    public async Task<IActionResult> RefreshTags()
    {
        var result = await mediator.Send(new RefreshTagsCommand());
        return Ok(result);
    }
}
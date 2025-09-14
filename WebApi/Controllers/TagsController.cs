using MediatR;
using Microsoft.AspNetCore.Mvc;
using Model.Features.Queries.GetTags;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TagsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Get([FromBody] GetTagsQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }
}
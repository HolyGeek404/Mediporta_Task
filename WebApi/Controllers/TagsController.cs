using Microsoft.AspNetCore.Mvc;
using Model.Services;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TagsController(ITagsService tagsService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTags()
    {
        var result = await tagsService.GetTags();
        return Ok(result);
    }
}
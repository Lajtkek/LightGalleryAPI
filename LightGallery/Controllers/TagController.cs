using LightGallery.Models.Requests;
using LightGallery.Models.Results;
using LightGallery.Service;
using Microsoft.AspNetCore.Mvc;

namespace LightGallery.Controllers;

public class TagController : ControllerBase
{
    private readonly ITagService _tagService;

    public TagController(ITagService tagService)
    {
        _tagService = tagService;
    }

    [HttpPost("Gallery/{idGallery}/Tag/Create")]
    public async Task<ActionResult<TagGrid>> CreateTag([FromRoute] Guid idGallery, [FromBody] TagCreateRequest tagCreateRequest)
    {
        var result = await _tagService.CreateTag(idGallery, tagCreateRequest);
        if (!result.IsSuccess) return StatusCode(500);
        return Ok(result.Tag);
    }

    [HttpGet("Gallery/{idGallery}/Tags")]
    public async Task<ActionResult<IEnumerable<TagGrid>>> GetTags(Guid idGallery)
    {
        return Ok(await _tagService.GetTagGrid(idGallery, new TagGridRequest()
        {
            IncludeChildren = true,
            IncludePrivate = true
        }));
    }
}
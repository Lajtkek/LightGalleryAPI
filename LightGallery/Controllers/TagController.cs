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

    [HttpPost("Create")]
    public async Task<ActionResult<TagGrid>> CreateTag(Guid idGallery, TagCreateRequest tagCreateRequest)
    {
        var result = await _tagService.CreateTag(idGallery, tagCreateRequest);
        if (!result.IsSuccess) return StatusCode(500);
        return Ok(result.Tag);
    }

    [HttpGet("")]
    public async Task<ActionResult<IEnumerable<TagGrid>>> GetTags(Guid idGallery)
    {
        return Ok(await _tagService.GetTagGrid(idGallery, new TagGridRequest()
        {
            IncludeChildren = true,
            IncludePrivate = true
        }));
    }
}
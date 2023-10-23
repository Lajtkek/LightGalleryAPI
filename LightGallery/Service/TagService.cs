using AutoMapper;
using LightGallery.Models;
using LightGallery.Models.Requests;
using LightGallery.Models.Results;
using LightGallery.Models.Results.Service;
using Microsoft.EntityFrameworkCore;

namespace LightGallery.Service;

public interface ITagService
{
    public Task<TagCreateResult> CreateTag(Guid idGallery, TagCreateRequest request);
    public Task UpdateTag(Guid idGallery, Guid idTag, TagUpdateRequest request);
    public Task<IEnumerable<TagGrid>> GetTagGrid(Guid idGallery, TagGridRequest request);
}

public class TagService : ITagService
{
    private readonly DefaultDatabaseContext _context;
    private readonly IMapper _mapper;

    public TagService(DefaultDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<TagCreateResult> CreateTag(Guid idGallery, TagCreateRequest request)
    {
        if (await _context.Tags.AnyAsync(x => x.Code == request.Code))
        {
            return new TagCreateResult()
            {
                IsSuccess = false
            };
        }

        var tag = _mapper.Map<Tag>(request);
        if (request.Children != null && request.Children.Any())
        {
            tag.Children = await _context.Tags.Where(x => request.Children.Contains(x.Id)).ToListAsync();
        }

        tag.GalleryId = idGallery;
        
        await _context.Tags.AddAsync(tag);
        await _context.SaveChangesAsync();

        return new TagCreateResult()
        {
            IsSuccess = true,
            Tag = _mapper.Map<TagGrid>(tag)
        };
    }

    public Task UpdateTag(Guid idGallery, Guid idTag, TagUpdateRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<TagGrid>> GetTagGrid(Guid idGallery, TagGridRequest request)
    {
        // TODO: reflect, include children and only public
        var tags = await _context.Tags.Include(x => x.Children).ToListAsync();

        return _mapper.Map<IEnumerable<TagGrid>>(tags);
    }
}
using Microsoft.EntityFrameworkCore;
using Model.DataAccess.Context;
using Model.DataAccess.Entities;
using Model.DataAccess.Interfaces;

namespace Model.DataAccess;

public class TagsDao(TagsContext context) : ITagsDao
{
    public async Task<List<Tag>?> GetTags()
    {
        var isCreated = await context.Database.EnsureCreatedAsync();
        if (!isCreated) return null;
        
        var tagList = await context.Tags.ToListAsync();
        return tagList;
    }
}
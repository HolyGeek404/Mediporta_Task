using Microsoft.EntityFrameworkCore;
using Model.DataAccess.Context;
using Model.DataAccess.Entities;
using Model.DataAccess.Interfaces;

namespace Model.DataAccess;

public class TagsDao(TagsContext context) : ITagsDao
{
    public async Task<List<Tag>?> GetTags()
    {
        var tagList = await context.Tags.ToListAsync();
        return tagList;
    }

    public async Task SaveTags(List<Tag> tags)
    {
        await context.Tags.AddRangeAsync(tags);
        await context.SaveChangesAsync();
    }
}
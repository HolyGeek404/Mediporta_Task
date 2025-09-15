using Microsoft.EntityFrameworkCore;
using Model.DataAccess.Context;
using Model.DataAccess.Entities;
using Model.DataAccess.Interfaces;
using Model.Features.Queries.GetTags;

namespace Model.DataAccess;

public class TagsDao(TagsContext context) : ITagsDao
{
    public async Task<List<Tag>> GetTags(GetTagsQuery  query)
    {
        var queryable = context.Tags.AsNoTracking();

        switch (query.Sort)
        {
            case "name":
                queryable = query.Order == "desc"
                    ? queryable.OrderByDescending(x => x.Name)
                    : queryable.OrderBy(x => x.Name);
                break;
            case "percentage":
                queryable = query.Order == "desc"
                    ? queryable.OrderByDescending(x => x.Percentage)
                    : queryable.OrderBy(x => x.Percentage);
                break;
            default:
                queryable = queryable.OrderBy(x => x.Name);
                break;
        }

        var tagList = await queryable
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return tagList;
    }
    public async Task<List<Tag>> GetAllTags()
    {
        return await context.Tags.ToListAsync();
    }
    public async Task SaveTags(List<Tag> tags)
    {
        await context.Tags.AddRangeAsync(tags);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAllTags()
    {
       await context.Database.ExecuteSqlRawAsync("DELETE FROM Tags");
    }
}
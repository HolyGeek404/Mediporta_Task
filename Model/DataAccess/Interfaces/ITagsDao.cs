using Model.DataAccess.Entities;
using Model.Features.Queries.GetTags;

namespace Model.DataAccess.Interfaces;

public interface ITagsDao
{
    Task<List<Tag>> GetTags(GetTagsQuery query);
    Task SaveTags(List<Tag> tags);
}
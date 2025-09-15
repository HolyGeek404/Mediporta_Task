using Model.DataAccess.Entities;
using Model.Features.Queries.GetTags;

namespace Website.Api;

public interface ITagClient
{
    Task<List<Tag>> GetTags(GetTagsQuery query);
}
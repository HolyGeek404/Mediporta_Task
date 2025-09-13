using Model.DataAccess.Entities;

namespace Model.Services;

public interface ITagsService
{
    Task<List<Tag>?> GetTags();
}
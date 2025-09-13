using Model.DataAccess.Entities;

namespace Model.DataAccess.Interfaces;

public interface ITagsDao
{
    Task<List<Tag>?> GetTags();
    Task SaveTags(List<Tag> tags);
}
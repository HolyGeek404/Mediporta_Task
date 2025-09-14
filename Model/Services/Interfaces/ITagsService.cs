using Model.DataAccess.Entities;
using Model.Features.Commands.RefreshTags;
using Model.Features.Queries.GetTags;

namespace Model.Services.Interfaces;

public interface ITagsService
{
    Task<List<Tag>> GetTags(GetTagsQuery request, CancellationToken cancellationToken);
    Task UpdateTags();
    Task<List<Tag>> RefreshTags(RefreshTagsCommand  refreshTagsCommand);
}
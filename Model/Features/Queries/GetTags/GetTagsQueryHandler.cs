using MediatR;
using Model.DataAccess.Entities;
using Model.Services.Interfaces;

namespace Model.Features.Queries.GetTags;

public class GetTagsQueryHandler(ITagsService tagsService): IRequestHandler<GetTagsQuery, List<Tag>>
{
    public Task<List<Tag>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
    {
        return tagsService.GetTags(request, cancellationToken);
    }
}
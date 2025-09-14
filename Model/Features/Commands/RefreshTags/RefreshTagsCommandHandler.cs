using MediatR;
using Model.DataAccess.Entities;
using Model.Services.Interfaces;

namespace Model.Features.Commands.RefreshTags;

public class RefreshTagsCommandHandler(ITagsService tagsService): IRequestHandler<RefreshTagsCommand, List<Tag>>
{
    public Task<List<Tag>> Handle(RefreshTagsCommand request, CancellationToken cancellationToken)
    {
        return tagsService.RefreshTags(request);
    }
}
using MediatR;
using Model.DataAccess.Entities;

namespace Model.Features.Commands.RefreshTags;

public class RefreshTagsCommand : IRequest<List<Tag>>
{
    public const string BaseEndpoint = "tags";
    public const string Site = "stackoverflow";
    public string Order  = "desc";
    public int Page = 1;
    public int PageSize  = 100;
    public string Sort = "name";
}
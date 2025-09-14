using MediatR;
using Model.DataAccess.Entities;

namespace Model.Features.Queries.GetTags;

public record GetTagsQuery : IRequest<List<Tag>>
{
    public const string BaseEndpoint = "tags";
    public const string Site = "stackoverflow";
    public string Order { get; set; } = "desc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string Sort { get; set; } = "name";
}
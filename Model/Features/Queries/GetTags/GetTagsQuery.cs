using MediatR;
using Model.DataAccess.Entities;

namespace Model.Features.Queries.GetTags;

public record GetTagsQuery : IRequest<List<Tag>>
{
    public string Order { get; set; } = string.Empty;
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string Sort { get; set; } = string.Empty;
}
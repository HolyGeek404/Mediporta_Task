using FluentValidation;
using Model.Features.Queries.GetTags;

public class GetTagsQueryValidator : AbstractValidator<GetTagsQuery>
{
    public GetTagsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("PageSize must be between 1 and 100.");

        RuleFor(x => x.Sort)
            .Must(x => x is null || x.ToLower() == "name" || x.ToLower() == "percentage")
            .WithMessage("Sort must be either 'name' or 'percentage'.");

        RuleFor(x => x.Order)
            .Must(x => x is null || x.ToLower() == "asc" || x.ToLower() == "desc")
            .WithMessage("Order must be either 'asc' or 'desc'.");
    }
}
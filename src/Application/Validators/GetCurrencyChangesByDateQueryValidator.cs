using Application.ExchangeRates.Queries;
using FluentValidation;

namespace Application.Validators;

public class GetCurrencyChangesByDateQueryValidator : AbstractValidator<GetCurrencyChangesByDateQuery>
{
    public GetCurrencyChangesByDateQueryValidator()
    {
        RuleFor(value => value.Date)
            .NotEmpty()
            .Must(BeNotAfter2014).WithMessage("The date cannot be after 2014/12/31");
    }
    private bool BeNotAfter2014(DateTime date)
    {
        return date.Year < 2014;
    }
}

using FluentValidation;

namespace Application.Validators;

public class DateValidator : AbstractValidator<DateTime>
{
    public DateValidator()
    {
        RuleFor(date => date)
            .NotEmpty()
            .Must(BeNotAfter2014).WithMessage("The date cannot be after 2014/12/31");
    }
    private bool BeNotAfter2014(DateTime date)
    {
        return date.Year < 2014;
    }
}

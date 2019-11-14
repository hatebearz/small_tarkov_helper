using FluentValidation;
using TarkovHelper.Models;

namespace TarkovHelper.Validators
{
    public class ItemValidator: AbstractValidator<Item>
    {
        public static ItemValidator Instance { get; } = new ItemValidator();

        public ItemValidator()
        {
            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0);
        }
    }
}
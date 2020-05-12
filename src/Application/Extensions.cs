using CleanArchitecture.Application.Common.Exceptions;
using System.Linq;

namespace CleanArchitecture.Application
{
    public static class Extensions
    {
        public static void Validate2<T>(this FluentValidation.AbstractValidator<T> validator, T value)
        {
            var result = validator.Validate(value);
            if(result.Errors.Any())
                throw new ValidationException(result.Errors);
        }
    }
}

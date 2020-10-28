using CleanArchitecture.Application.Common.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CleanArchitecture.Application
{
    public static class Extensions
    {
        // Required because we throw our own ValidationException
        public static void Validate2<T>(this FluentValidation.AbstractValidator<T> validator, T value)
        {
            var result = validator.Validate(value);
            if(result.Errors.Any())
                throw new ValidationException(result.Errors);
        }

        public static Task<T> SingleOrDefault<T>(this IQueryable<T> queryable, Func<T, bool> predicate)
        {
            return Task.Run(() => queryable.SingleOrDefault(predicate));
        }

        public static Task<bool> All<T>(this IQueryable<T> queryable, Func<T, bool> predicate)
        {
            return Task.Run(() => queryable.All(predicate));
        }
    }
}

using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application
{
    public class UpdateTodoList
    {
        public class Command
        {
            public int Id { get; set; }
            public string Title { get; set; }
        }

        public class Handler
        {
            private readonly IApplicationDbContext _context;
            private readonly Validator _validator;

            // TODO: remove this dependency, use IPorts
            public Handler(IApplicationDbContext context, Validator validator)
            {
                _context = context;
                _validator = validator;
            }

            public async Task Handle(Command request)
            {
                _validator.Validate2(request);

                var entity = await _context.TodoLists.FindAsync(request.Id);

                if(entity == null)
                    throw new NotFoundException(nameof(TodoList), request.Id);

                entity.Title = request.Title;

                await _context.SaveChangesAsync();
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            public interface IPorts
            {
                Task<bool> IsUniqueTitle(int modelId, string title);
            }

            private readonly IPorts _ports;

            public Validator(IPorts ports)
            {
                _ports = ports;

                RuleFor(v => v.Title)
                    .NotEmpty().WithMessage("Title is required.")
                    .MaximumLength(200).WithMessage("Title must not exceed 200 characters.")
                    .MustAsync(BeUniqueTitle).WithMessage("The specified title already exists.");
            }

            public async Task<bool> BeUniqueTitle(Command model, string title, CancellationToken cancellationToken)
            {
                return await _ports.IsUniqueTitle(model.Id, title);
            }
        }
    }

}

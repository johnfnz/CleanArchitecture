using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using FluentValidation;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.TodoItems.Commands.UpdateTodoItem
{
    public class UpdateTodoItem
    {
        public partial class Command : IRequest
        {
            public int Id { get; set; }

            public string Title { get; set; }

            public bool Done { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IApplicationDbContext _context;

            public Handler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var entity = await _context.TodoItems.FindAsync(request.Id);

                if (entity == null)
                {
                    throw new NotFoundException(nameof(TodoItem), request.Id);
                }

                entity.Title = request.Title;
                entity.Done = request.Done;

                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }

        public class UpdateTodoItemCommandValidator : AbstractValidator<UpdateTodoItem.Command>
        {
            public UpdateTodoItemCommandValidator()
            {
                RuleFor(v => v.Title)
                    .MaximumLength(200)
                    .NotEmpty();
            }
        }
    }
}

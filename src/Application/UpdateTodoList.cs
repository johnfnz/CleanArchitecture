using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Domain.Entities;
using FluentValidation;
using System;
using System.Linq;
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

        public interface IPorts
        {
            IQueryable<TodoList> TodoLists { get; }
            Task Update(int id, Action<TodoList> action);
        }

        public class Handler
        {
            private readonly IPorts _ports;

            public Handler(IPorts ports)
            {
                _ports = ports;
            }

            public async Task Handle(Command request)
            {
                new Validator(_ports).Validate2(request);
                await _ports.Update(request.Id, entity =>
                {
                    entity.Title = request.Title;
                });
              }
        }

        public class Validator : AbstractValidator<Command>
        {
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
                return await _ports.TodoLists.Where(l => l.Id != model.Id).All(l => l.Title != title);
            }
        }
    }
}

using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.TodoLists.Commands
{
    public class UpdateTodoListCommandValidator : AbstractValidator<UpdateTodoListCommand>
    {
        public interface IPorts
        {
            Task<bool> IsUniqueTitle(int modelId, string title);
        }

        private readonly IPorts _ports;

        public UpdateTodoListCommandValidator(IPorts ports)
        {
            _ports = ports;

            RuleFor(v => v.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.")
                .MustAsync(BeUniqueTitle).WithMessage("The specified title already exists.");
        }

        public async Task<bool> BeUniqueTitle(UpdateTodoListCommand model, string title, CancellationToken cancellationToken)
        {
            return await _ports.IsUniqueTitle(model.Id, title);
        }
    }
}

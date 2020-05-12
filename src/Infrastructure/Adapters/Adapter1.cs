using System.Threading.Tasks;
using static CleanArchitecture.Application.TodoLists.Commands.UpdateTodoListCommandValidator;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CleanArchitecture.Infrastructure.Adapters
{
    class Adapter1 : IPorts
    {
        private readonly IApplicationDbContext _context;

        // Ideally, maybe, ApplicationDbContext would be static
        public Adapter1(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsUniqueTitle(int modelId, string title)
        {
            return await _context.TodoLists
                .Where(l => l.Id != modelId)
                .AllAsync(l => l.Title != title);

        }
    }
}

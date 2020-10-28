using System.Threading.Tasks;
using static CleanArchitecture.Application.UpdateTodoList;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using CleanArchitecture.Domain.Entities;
using System;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Infrastructure.Persistence;

namespace CleanArchitecture.Infrastructure.Adapters
{
    class UpdateTodoListAdapter : IPorts
    {
        private readonly ApplicationDbContext _context;

        // Ideally, maybe, ApplicationDbContext would be static
        public UpdateTodoListAdapter(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<TodoList> TodoLists => _context.TodoLists;

        public Task SaveChanges() => _context.SaveChangesAsync();

        public async Task Update(int id, Action<TodoList> action)
        {
            await _context.TodoLists.Update2(id, action);
            await _context.SaveChangesAsync();
        }
    }

    static class Extensions
    {
        public static async Task Update2<T>(this DbSet<T> dbset, int id, Action<T> action) where T : class
        {
            var entity = await dbset.FindAsync(id);

            if(entity == null)
                throw new NotFoundException(nameof(TodoList), id);

            action(entity);
        }
    }
}

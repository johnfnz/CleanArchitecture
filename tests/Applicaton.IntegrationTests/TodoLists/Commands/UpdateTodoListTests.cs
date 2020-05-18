using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Domain.Entities;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using static CleanArchitecture.Application.UpdateTodoList;

namespace CleanArchitecture.Application.IntegrationTests.TodoLists.Commands
{
    using static Testing;

    public class UpdateTodoListTests : TestBase
    {
        [Test]
        public void ShouldRequireValidTodoListId()
        {
            var command = new Command
            {
                Id = 99,
                Title = "New Title"
            };

            FluentActions.Invoking(async () => await
                GetService<Handler>().Handle(command)).Should().Throw<NotFoundException>();
        }

        [Test]
        public async Task ShouldRequireUniqueTitle()
        {
            var listId = await SendAsync(new CreateTodoList.Command
            {
                Title = "New List"
            });

            await SendAsync(new CreateTodoList.Command
            {
                Title = "Other List"
            });

            var command = new Command
            {
                Id = listId,
                Title = "Other List"
            };

            FluentActions.Invoking(async () => await
                GetService<Handler>().Handle(command))
                    .Should().Throw<ValidationException>().Where(ex => ex.Errors.ContainsKey("Title"))
                    .And.Errors["Title"].Should().Contain("The specified title already exists.");
        }

        [Test]
        public async Task ShouldUpdateTodoList()
        {
            var userId = await RunAsDefaultUserAsync();

            var listId = await SendAsync(new CreateTodoList.Command
            {
                Title = "New List"
            });

            var command = new Command
            {
                Id = listId,
                Title = "Updated List Title"
            };

            await GetService<Handler>().Handle(command);

            var list = await FindAsync<TodoList>(listId);

            list.Title.Should().Be(command.Title);
            list.LastModifiedBy.Should().NotBeNull();
            list.LastModifiedBy.Should().Be(userId);
            list.LastModified.Should().NotBeNull();
            list.LastModified.Should().BeCloseTo(DateTime.Now, 1000);
        }
    }
}

using Functionland.FxFiles.Api.Models.TodoItem;
using Functionland.FxFiles.Shared.Dtos.TodoItem;

namespace Functionland.FxFiles.Api.Mappers;

public class TodoItemMapperConfiguration : Profile
{
    public TodoItemMapperConfiguration()
    {
        CreateMap<TodoItem, TodoItemDto>().ReverseMap();
    }
}

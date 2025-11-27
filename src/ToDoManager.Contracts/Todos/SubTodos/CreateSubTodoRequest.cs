namespace ToDoManager.Contracts.Todos.SubTodos;

public record class CreateSubTodoRequest(string Title, string Description, bool IsComplete);
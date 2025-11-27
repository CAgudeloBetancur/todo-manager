namespace ToDoManager.Contracts.Todos.SubTodos;

public record UpdateSubTodoRequest(string Title,  string Description, bool IsComplete);
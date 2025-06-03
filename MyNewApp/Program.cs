using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Rewrite;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseRewriter(new RewriteOptions().AddRedirect("tasks/(.*)", "todos/$1"));

app.Use(async (context, next) =>
{
    Console.WriteLine($"[{context.Request.Method} {context.Request.Path} {DateTime.UtcNow}] Started.");
    await next(context);
    Console.WriteLine($"[{context.Request.Method} {context.Request.Path} {DateTime.UtcNow}] Finished.");
});

// app.MapGet("/", () => "Hello World!");

// app.MapGet("/1", () => "Hello World 1");

var todos = new List<Todo>();

// Get all todos
app.MapGet("/todos", () => todos);

// Add todo to list
app.MapPost("/todos", (Todo task) =>
{
    todos.Add(task);
    return TypedResults.Created("/todos/{id}", task);
});

// Find todo by id
app.MapGet("/todos/{id}", Results<Ok<Todo>, NotFound> (int id) =>
{
    var targetTodo = todos.SingleOrDefault(t => id == t.Id);
    return targetTodo is null ? TypedResults.NotFound() : TypedResults.Ok(targetTodo);
});

// Delete all todos
app.MapDelete("/todos/{id}", (int id) =>
{
    todos.RemoveAll(t => id == t.Id);
    return TypedResults.NoContent();
});

app.Run();

// Type of Todo item in a Todo list (think of it as a class in Spring)
public record Todo(int Id, string Name, DateTime DueDate, bool IsCompleted)
{

}
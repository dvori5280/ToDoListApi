using Microsoft.EntityFrameworkCore;
using TodoApi;


var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ToDoDB");

builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

app.UseCors("AllowAll");

app.UseSwagger();
app.UseSwaggerUI();



app.MapGet("/items", (ToDoDbContext db) => db.Items.ToList());     

// הוספת משימה חדשה

app.MapPost("/items",(Item item,ToDoDbContext db) => {
    db.Items.Add(item);
    db.SaveChanges();
    return Results.Created($"/items/{item.Id}", item);
    });

// עדכון משימה

app.MapPut("/items/{id}", (int id, Item updated, ToDoDbContext db) =>
{
    Item? item = db.Items.Find(id);
    if (item == null) return Results.NotFound();

    item.IsComplete = updated.IsComplete;
    db.SaveChanges();

    return Results.Ok(item);
});

// מחיקת משימה
app.MapDelete("/items/{id}",(ToDoDbContext db, int id)=>
{
    Item? item = db.Items.Find(id);
    if (item == null) return Results.NotFound();

    db.Items.Remove(item);
    db.SaveChanges();

    return Results.NoContent();
});
app.Run();
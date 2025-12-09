using Bank.Command.Infrastructure;
using Bank.Infrastructure.CommandDatabase;
using Bank.Infrastructure.QueryDatabase;
using Bank.Query.Infrastructure;
using Bank.Service.BankService;
using Bank.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<AddBankValidator>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CommandDatabaseContext>(optionsBuilder =>
    optionsBuilder.UseSqlite(
        "Data Source=database.db",
        sqliteOptions => sqliteOptions.MigrationsAssembly("Bank")
    )
);
builder.Services.AddDbContext<QueryDatabaseContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase("MyDatabase"));

builder.Services.AddCommandsHandler();
builder.Services.AddQueriesHandler();
builder.Services.AddServiceRegister();
builder.Services.AddLogging();


var app = builder.Build();

// Ensure database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<CommandDatabaseContext>();
        context.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
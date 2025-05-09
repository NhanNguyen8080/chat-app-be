using ChatService.API.DataService;
using ChatService.API.Extensions;
using ChatService.API.Hubs;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddDatabaseContext()
                .AddSwaggerConfiguration()
                .AddJwtAuth()
                .AddDependencyInjection();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwaggerDocumentation();
app.UseErrorHandlingMiddleware();
app.UseJwtMiddleware();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/chat");

app.Run();

using ChatService.API.Converters;
using ChatService.API.DataService;
using ChatService.API.Extensions;
using ChatService.API.Hubs;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddDatabaseContext()
                .AddRedis()
                .AddSwaggerConfiguration()
                .AddJwtAuth()
                .AddDependencyInjection()
                .AddCorsExtensions();

builder.Services.AddControllers()
                 .AddJsonOptions(options =>
                     {
                         options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
                     });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.SeedData();
app.UseSwaggerDocumentation();
app.UseErrorHandlingMiddleware();
app.UseJwtMiddleware();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/chat");

app.Run();

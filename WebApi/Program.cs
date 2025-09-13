using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Model.DataAccess.Context;
using WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddServices();
builder.Services.AddStackOverflowApiClient(builder.Configuration);

builder.Configuration.AddAzureKeyVault(
    new Uri(builder.Configuration.GetSection("AzureAd")["KvUrl"]!),
    new DefaultAzureCredential());

builder.Services.AddDbContext<TagsContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Local")));

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Model.DataAccess.Context;
using Model.Features.Queries.GetTags;
using Model.Services.Interfaces;
using WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddServices();
builder.Services.AddMediatRConfig();
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

using (var scope = app.Services.CreateScope())
{
    var tagsService = scope.ServiceProvider.GetRequiredService<ITagsService>();
    await tagsService.UpdateTags();
}

app.Run();
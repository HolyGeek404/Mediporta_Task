using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
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

var azureAd = builder.Configuration.GetSection("AzureAd");
builder.Configuration.AddAzureKeyVault(
    new Uri(azureAd["KvUrl"]!),
    new DefaultAzureCredential());

builder.Services.AddDbContext<TagsContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("Local")));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddMicrosoftIdentityWebApi(azureAd);
 
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
    var db = scope.ServiceProvider.GetRequiredService<TagsContext>();
    db.Database.EnsureCreated();
    var tagsService = scope.ServiceProvider.GetRequiredService<ITagsService>();
    await tagsService.UpdateTags();
}

app.Run();
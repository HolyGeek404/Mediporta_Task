using Azure.Identity;
using Website;
using Website.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Configuration.AddAzureKeyVault(
    new Uri(builder.Configuration.GetSection("AzureAd")["KvUrl"]!),
    new DefaultAzureCredential());

builder.Services.AddTagsApiClient(builder.Configuration);
builder.Services.AddServices();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
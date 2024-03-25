using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using IMS.Client;
using Radzen;
using BlazorPro.BlazorSize;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();

builder.Services.AddRadzenComponents();
builder.Services.AddMediaQueryService();
builder.Services.AddPWAUpdater();
await builder.Build().RunAsync();


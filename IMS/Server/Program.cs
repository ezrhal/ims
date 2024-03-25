
using IMS.Server.Services;
using Microsoft.AspNetCore.ResponseCompression;
using AspNetCore.Identity.Mongo;
using AspNetCore.Identity.Mongo.Model;
using Microsoft.Extensions.Configuration;
using IMS.Server.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Telerik.Reporting.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Telerik.Reporting.Cache.File;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

IConfigurationRoot configuration = new ConfigurationBuilder()
           .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
           .AddJsonFile("appsettings.json")
           .Build();


builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IMongoDBService, MongoDBService>();

// change builder.Services.AddRazorPages();
builder.Services.AddRazorPages().AddNewtonsoftJson();
builder.Services.AddControllers().AddNewtonsoftJson();

//REST Service
builder.Services.TryAddSingleton<IReportServiceConfiguration>(sp => new ReportServiceConfiguration
{

    ReportingEngineConfiguration = ConfigurationHelper.ResolveConfiguration(sp.GetService<IWebHostEnvironment>()),
    HostAppId = "IMS",
    Storage = new FileStorage(),
    ReportSourceResolver = new UriReportSourceResolver(
                        System.IO.Path.Combine(sp.GetService<IWebHostEnvironment>().ContentRootPath, "Reports"))
});

//builder.Services.TryAddSingleton<IReportServiceConfiguration>(sp =>
//     new ReportServiceConfiguration
//     {
//         // The default ReportingEngineConfiguration will be initialized from appsettings.json or appsettings.{EnvironmentName}.json:
//         ReportingEngineConfiguration = sp.GetService<IConfiguration>(),

//         // In case the ReportingEngineConfiguration needs to be loaded from a specific configuration file, use the approach below:
//         // ReportingEngineConfiguration = ResolveSpecificReportingConfiguration(WebHostEnvironment),
//         HostAppId = "ReportingNet5",
//         Storage = new FileStorage(),
//         ReportSourceResolver = new TypeReportSourceResolver()
//             .AddFallbackResolver(new UriReportSourceResolver("Reports"))
//     });

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})  
.AddJwtBearer(x =>
{
   

    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.MapInboundClaims = false;
    
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("KEY1312312312RANDOMARISHISOLUTIONSAMEOLDSHIRTKEY1312312312RANDOMARISHISOLUTIONSAMEOLDSHIRT")),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidIssuer = configuration.GetSection("JwtIssuer").ToString()
        //SignatureValidator = (token, _) => JsonWebToken(token) ,


    };
});

builder.Services.AddAuthorization();

//builder.Services.AddIdentityMongoDbProvider<MongoIdentityUser, MongoIdentityRole>(identity =>
//{
//    identity.Password.RequiredLength = 8;


//}, mongo =>
//{
//    IConfigurationRoot configuration = new ConfigurationBuilder()
//            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
//            .AddJsonFile("appsettings.json")
//            .Build();

//    mongo.ConnectionString = configuration.GetConnectionString("MongoConnection");
//    mongo.UsersCollection = "Mongos";
//    mongo.RolesCollection = "Roles";
//});

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
//    options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
//    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();



app.MapRazorPages();
app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();


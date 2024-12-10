using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BaseballCalcASP.Data;
using Microsoft.AspNetCore.Identity;
using BaseballCalcASP.Models;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using GroupSacePrep.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using NETCore.MailKit.Infrastructure.Internal;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<BaseballCalcASPContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BaseballCalcASPContext") ?? throw new InvalidOperationException("Connection string 'BaseballCalcASPContext' not found.")));

builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<BaseballCalcASPContext>()
    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Localizatie toevoegen
builder.Services.AddLocalization(options => options.ResourcesPath = "Localizing");

builder.Services.AddMvc()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

builder.Services.AddTransient<IEmailSender, MailKitEmailSender>();
builder.Services.Configure<MailKitOptions>(options =>
{
    options.Server = builder.Configuration["ExternalProviders:MailKit:SMTP:Address"];
    options.Port = Convert.ToInt32(builder.Configuration["ExternalProviders:MailKit:SMTP:Port"]);
    options.Account = builder.Configuration["ExternalProviders:MailKit:SMTP:Account"];
    options.Password = builder.Configuration["ExternalProviders:MailKit:SMTP:Password"];
    options.SenderEmail = builder.Configuration["ExternalProviders:MailKit:SMTP:SenderEmail"];
    options.SenderName = builder.Configuration["ExternalProviders:MailKit:SMTP:SenderName"];

    // Set it to TRUE to enable ssl or tls, FALSE otherwise
    options.Security = false;  // true zet ssl or tls aan
});

// Configure supported cultures
var supportedCultures = new[]
{
        new CultureInfo("en-US"),
        new CultureInfo("nl-BE"),
        new CultureInfo("fr-FR")
        // Add more cultures as needed
    };

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});



// toevoegen Swagger voor API
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "BaseballCalcASP", Version = "v1" });
});

// Read the API base URL from the settings file
var apiBaseUrl = builder.Configuration["ApiBaseUrl"];

// Register HttpClientFactory and configure base address using appsettings
// -> required for call from login page to AccountController (JWT-token)
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl); // Set the base address for HttpClient
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    handler.CookieContainer = new System.Net.CookieContainer();
    return handler;
});;

//toevoegen authentication voor JWT-token
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.HttpContext.Request.Headers.TryGetValue("Authorization", out var token))
            {
                context.Token = token.ToString().Replace("Bearer ", "");;
            }
            return Task.CompletedTask;
        }
    };
});

/* adding this results in MAUI issues with APIControllers

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});*/

/*builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});*/

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();
 //app.UseCors("AllowAll");
// Als er nog geen database is maar wel een bestaande migration, dan wordt de database hier automatisch aangemaakt
using (var serviceScope = app.Services.CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<BaseballCalcASPContext>();
    context.Database.Migrate();
}



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// toevoegen nodige ondersteuning bij de ontwikkeling van de applicatie
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApplication1 v1"));
}

//added
builder.WebHost.UseUrls("http://0.0.0.0:5204");

//Commented out app.UseHttpsRedirection() to switch from https to http.
//  + additionally changed applicationUrl in launchSettings.json from https to http for BaseballCalcASP
//app.UseHttpsRedirection();
app.UseStaticFiles();

// zonder deze lijn werkt de localizatie niet
app.UseRequestLocalization();

app.UseSession(); // Add before authentication middleware

app.UseRouting();

app.Use(async (context, next) =>
{
    var token = context.Session.GetString("JwtToken");
    if (!string.IsNullOrEmpty(token))
    {
        if (!context.Request.Headers.ContainsKey("Authorization"))
        {
            context.Request.Headers.Add("Authorization", $"Bearer {token}");
        }
    }
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();


// endpoint toevoegen
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});


app.Use(async (context, next) =>
{
    Console.WriteLine($"Incoming request: {context.Request.Method} {context.Request.Path}");
    await next();
});

app.Run();

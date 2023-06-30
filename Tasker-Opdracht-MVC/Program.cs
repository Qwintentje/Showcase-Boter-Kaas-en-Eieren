using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tasker_Opdracht_MVC.Areas.Identity.Data;
using Tasker_Opdracht_MVC.Data;
using Tasker_Opdracht_MVC.Hubs;
using Microsoft.OpenApi.Models;
using System;
using System.Reflection;
using System.IO;
using Microsoft.AspNetCore.CookiePolicy;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DatabaseConnectionString");

/*builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

builder.Services.AddDbContext<EmailDBContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

builder.Services.AddDbContext<GameDBContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});*/


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
	options.UseSqlServer(connectionString);
});

builder.Services.AddDbContext<EmailDBContext>(options =>
{
	options.UseSqlServer(connectionString);
});

builder.Services.AddDbContext<GameDBContext>(options =>
{
	options.UseSqlServer(connectionString);

});



builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
	.AddRoles<IdentityRole>()
	.AddEntityFrameworkStores<ApplicationDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
	options.Cookie.Name = ".Session-DevProf";
	options.IdleTimeout = TimeSpan.FromMinutes(30);
});





builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
	var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
});

//CORS
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowGoogleFontsAndRecaptcha", builder =>
	{
		builder.WithOrigins(
			"https://www.google.com",
			"https://www.gstatic.com/recaptcha/",
			"https://fonts.googleapis.com",
			"https://fonts.gstatic.com")
		.AllowAnyHeader()
		.AllowAnyMethod();
	});
});


//CookiePolicy
builder.Services.Configure<CookiePolicyOptions>(options =>
{
	options.Secure = CookieSecurePolicy.Always;
	//options.HttpOnly = HttpOnlyPolicy.Always;
	options.MinimumSameSitePolicy = SameSiteMode.Strict;
	options.CheckConsentNeeded = context => true;
	options.MinimumSameSitePolicy = SameSiteMode.Strict;
	options.ConsentCookie.MaxAge = TimeSpan.FromDays(365);
});

//HSTS header
builder.Services.AddHsts(options =>
{
	options.IncludeSubDomains = true;
	options.MaxAge = TimeSpan.FromDays(365);
});




AddAuthorizationPolicies();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
	c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication(); ;
app.UseAuthorization();
app.MapRazorPages();


//CSP HEADERS
app.Use(async (context, next) =>
{
	// Remove server header to prevent information disclosure
	context.Response.Headers.Remove("Server");

	// Set CSP header
	context.Response.Headers.Add("Content-Security-Policy",
		"default-src 'self' https://optimizationguide-pa.googleapis.com; " +
		"style-src 'self' fonts.googleapis.com; " +
		"img-src 'self' data:; " +
		"connect-src 'self' https://www.google.com https://www.gstatic.com/recaptcha/; " +
		"frame-src 'self' https://www.google.com https://www.gstatic.com/recaptcha/; " +
		"frame-ancestors 'none'; " +
		"font-src 'self' https://fonts.gstatic.com; " +
		"media-src 'self'; " +
		"manifest-src 'self'; " +
		"worker-src 'self'; " +
		"form-action 'self' https://www.google.com/recaptcha/; " +
		"script-src 'self' https://www.google.com/recaptcha/ https://www.gstatic.com/recaptcha/;");
	// Set Content-Type-Options header
	context.Response.Headers.Add("Content-Type-Options", "nosniff");

	await next.Invoke();
});



//USE CORS
app.UseCors("AllowGoogleFontsAndRecaptcha");


//Cookie
app.UseCookiePolicy();


app.UseHsts();




app.UseEndpoints(endpoints =>
{
	endpoints.MapHub<GameHub>("/game"); // Use the GameHub
	endpoints.MapControllerRoute(
		name: "default",
		pattern: "{controller=Home}/{action=AboutMe}/{id?}");
	app.Run();
});


void AddAuthorizationPolicies()
{
	builder.Services.AddAuthorization(options =>
	{
		options.AddPolicy("RequireManager", policy => policy.RequireRole("Manager"));
		options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Administrator"));
	});

}

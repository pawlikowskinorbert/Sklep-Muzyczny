using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Extensions;
using Project.Repositories;
using Project.Helpers;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddAplicationServices(builder.Configuration);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddIdentity<AppUser, IdentityRole>(options => 
{
	options.Password.RequireDigit = true;
	options.Password.RequiredLength = 6;

	options.SignIn.RequireConfirmedAccount = false;
	
})
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddDefaultTokenProviders();


builder.Services.AddControllersWithViews();

builder.Logging.AddConsole();


builder.Services.AddRazorPages();

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

var app = builder.Build();

using(var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	try
	{
		var context = services.GetRequiredService<ApplicationDbContext>();
		
		
		await SeedData.InitializeAsync(services);
	}
	catch(Exception ex)
	{
		Console.WriteLine($"Error podczas seedowania: ", ex.Message );
		Console.WriteLine(ex.StackTrace);
	}
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
}
else
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

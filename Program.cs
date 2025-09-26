using Microsoft.EntityFrameworkCore;
using VybeCheck.Models;
using VybeCheck.Services;

var builder = WebApplication.CreateBuilder(args);
var dbPassword = builder.Configuration["DbPassword"];
var connect = $"server=localhost;user=root;password={dbPassword};database=vybe_check_db";

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationContext>((options) => { options.UseMySql(connect, ServerVersion.AutoDetect(connect)); });
builder.Services.AddSession();
builder.Services.AddScoped<IPasswordService, BcryptPasswordService>();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error/500");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseStatusCodePagesWithReExecute("/error/{0}");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();
app.Run();

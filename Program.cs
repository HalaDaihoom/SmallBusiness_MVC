using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using SmallBusiness.DB_context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<StorDbContext>(option =>
option.UseSqlServer(builder.Configuration.GetConnectionString("ConnetionDb")));


//add session
builder.Services.AddSession(
           options => {
               options.IdleTimeout = TimeSpan.FromMinutes(30);
               options.Cookie.HttpOnly = true;
               options.Cookie.IsEssential = true;
           }
           );

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication(); ;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.UseSession();
app.Run();

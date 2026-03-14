using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using HackathonMvcSqlite.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=database.db"));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o => { o.LoginPath = "/Account/Login"; o.LogoutPath = "/Account/Logout"; });
//builder.Services.AddScoped<HackathonMvcSqlite.Services.InventoryService>();

var app = builder.Build();

//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//    db.Database.EnsureCreated();
//    if (!db.Warehouses.Any())
//    {
//        db.Warehouses.Add(new HackathonMvcSqlite.Models.Warehouse { Name = "Main Warehouse", Code = "WH01", Location = "Default" });
//        db.ProductCategories.Add(new HackathonMvcSqlite.Models.ProductCategory { Name = "General" });
//        db.SaveChanges();
//    }
//}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Dashboard}/{action=Index}/{id?}");
app.Run();
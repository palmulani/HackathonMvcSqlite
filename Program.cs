
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

var app = builder.Build();

var conn = new SqliteConnection("Data Source=database.db");
conn.Open();



app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
name:"default",
pattern:"{controller=Dashboard}/{action=Index}/{id?}");

app.Run();

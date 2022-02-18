using Microsoft.EntityFrameworkCore;
using Minitwit.DatabaseUtil;
using Minitwit.Models.Context;
using Minitwit.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<MinitwitContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Minitwit"));
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEntityAccessor, EntityAccessor>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
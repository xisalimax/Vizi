using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ViziLogin.Data;
using ViziLogin.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Index";
        options.AccessDeniedPath = "/Home/Index";
    });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

// 👇 SEED DO BANCO (REGIONAIS)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    db.Database.Migrate();

    if (!db.Areas.Any())
    {
        db.Areas.AddRange(
            new Area { Nome = "Barreiro" },
            new Area { Nome = "Centro-Sul" },
            new Area { Nome = "Leste" },
            new Area { Nome = "Nordeste" },
            new Area { Nome = "Noroeste" },
            new Area { Nome = "Norte" },
            new Area { Nome = "Oeste" },
            new Area { Nome = "Pampulha" },
            new Area { Nome = "Venda Nova" }
        );

        db.SaveChanges();
    }
}

app.Run();
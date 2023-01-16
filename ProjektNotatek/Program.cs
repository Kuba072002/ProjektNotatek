using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjektNotatek.Data;
using ProjektNotatek.Utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<DataContext>(
    o => o.UseNpgsql(builder.Configuration.GetConnectionString("postgresDbConnection"))
    );

builder.Services.AddTransient<IPasswordHasher<ApplicationUser>, CustomPasswordHasher>();
//builder.Services.AddScoped<IPasswordHasher<ApplicationUser>, CustomPasswordHasher<ApplicationUser>>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();
builder.Services.Configure<IdentityOptions>(options => {
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredUniqueChars = 1;

    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.AllowedForNewUsers = true;

    options.User.RequireUniqueEmail = true;

    options.SignIn.RequireConfirmedEmail = true;
});
builder.Services.Configure<DataProtectionTokenProviderOptions>(option =>
    option.TokenLifespan = TimeSpan.FromMinutes(10));
builder.Services.ConfigureApplicationCookie(option => {
    option.LoginPath = "/Identity/Signin";
    option.AccessDeniedPath = "/Identity/AccessDenied";
    option.ExpireTimeSpan = TimeSpan.FromHours(2);
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
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

using (var scope = app.Services.CreateScope()) {
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<DataContext>();
    if (context.Database.GetPendingMigrations().Any()) {
        context.Database.Migrate();
    }
}

app.Run();

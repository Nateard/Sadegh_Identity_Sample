using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sadegh_Identity_Sample.Data;
using Sadegh_Identity_Sample.Helper;
using Sadegh_Identity_Sample.Helpers;
using Sadegh_Identity_Sample.Models.Entities;
using static Sadegh_Identity_Sample.Helper.AddMyClaims;

var builder = WebApplication.CreateBuilder(args);
var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
string connectionString = configuration.GetConnectionString("DefaultConnection");

// Add services to the container.
builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<DataBaseContext>()
    .AddDefaultTokenProviders()
    .AddRoles<Role>()
    .AddErrorDescriber<CustomIdentityError>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Buyer", policy =>
    {
        policy.RequireClaim("Buyer");
    });
    options.AddPolicy("BloodType", policy =>
    {
        policy.RequireClaim("Ap", "O-");
    });

    options.AddPolicy("Cradit", policy =>
    {
        policy.Requirements.Add(new UserCreditRequirement(10000)); 
    });
    options.AddPolicy("IsBlogForUser", policy =>
    {
        policy.AddRequirements(new BlogRequirement());
    });
});

builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = "";
                    options.ClientSecret = "";
                }
                );

builder.Services.Configure<IdentityOptions>(option =>
{
    // user Settings
    option.User.RequireUniqueEmail = true;

    //passwords
    option.Password.RequireDigit = true;
    option.Password.RequireLowercase = true;
    option.Password.RequireUppercase = false;
    option.Password.RequireNonAlphanumeric = false;
    option.Password.RequiredLength = 8;
    option.Password.RequiredUniqueChars = 1;

    //lockout settings 
    option.Lockout.MaxFailedAccessAttempts = 5;
    option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

    //sign in
    option.SignIn.RequireConfirmedAccount = false;
});

//builder.Services.AddScoped<IUserClaimsPrincipalFactory<User>, AddMyClaims>();
builder.Services.AddScoped<IClaimsTransformation, AddClaim>();
builder.Services.AddSingleton<IAuthorizationHandler, UserCreditHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, IsBlogForUserAuthorizationHandler>();

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DataBaseContext>(
    p=> p.UseSqlServer(connectionString)
    );
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
   
}

app.UseHttpsRedirection();
app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();


app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllerRoute(
//      name: "areas",
//      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
//    );
//});
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}")
//    .WithStaticAssets();


app.Run();

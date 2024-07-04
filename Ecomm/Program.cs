using Ecomm.DataAccess.Data;
using Ecomm.DataAccess.Repository;
using Ecomm.DataAccess.Repository.IRepository;
using Ecomm.Models;
using Ecomm.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("conStr");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
//.AddDefaultTokenProviders().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("TwilioSettings"));
builder.Services.AddScoped<ISMSSenderService, SMSSenderService>();
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("StripeSettings"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
.AddDefaultTokenProviders().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddRazorPages();
builder.Services.AddScoped<IEmailSender,EmailSender>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
    options.LogoutPath = $"/Identity/Account/Logout";
});

//builder.Services.AddScoped<ICoverTypeRepository, CoverTypeRepository>();  //ctrl+k+c
//builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();      //ctrk+k+u
builder.Services.AddScoped<IUnitOfWork, UnitOfWorkd>();
builder.Services.AddAuthentication().AddFacebook(options =>
{
    options.AppId = "449218894340740";
    options.AppSecret = "9941ac1a66ac028551bf1d3f2c9f4cf9";
});
builder.Services.AddAuthentication().AddGoogle(options =>
    {
        options.ClientId = "699611435699-m1gfg4jmhpcdva01fj47lbipbvsca71f.apps.googleusercontent.com";
        options.ClientSecret = "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";
    });
builder.Services.AddAuthentication().AddTwitter(options =>
{
    options.ConsumerKey = "f3HOMsxqXApWDj0QwT9ydeKJA";
    options.ConsumerSecret = "N57BOp9hiNCdpDDKzvrfFZ0CZeIMs5wGDeaQCZOxXLkQFwZ7JV";
});
//Recovery Twilio GMBW9RP6YKV8GRALRYP5D5BZ.. ForX
// Recovery D73PKWBYUP3KZHMEWUYHJ167 For.
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

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
StripeConfiguration.ApiKey = builder.Configuration.GetSection("StripeSettings")["Secretkey"];
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

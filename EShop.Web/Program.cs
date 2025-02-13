using EShop.Repository;
using EShop.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using EShop.Service.Interface;
using EShop.Service.Implementation;
using EShop.Repository.Implementation;
using EShop.Repository.Interface;
using EShop.Domain.Payment;
using EShop.Domain.Email;
using EShop.Repository.Migrations;

var builder = WebApplication.CreateBuilder(args);
//var configuration = new ConfigurationBuilder()
//    .SetBasePath(Directory.GetCurrentDirectory())
//    .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true)
//    .AddEnvironmentVariables();

// Map Stripe Public and Secret Keys

builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("StripeSettings"));
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<EShopApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(IUserRepository), typeof(UserRepository));

builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddTransient<IShoppingCartService, ShoppingCartService>();
builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services.AddScoped<IDataInitializer, DataInitializer>();

var app = builder.Build();
using (var scope = app?.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    IDataInitializer dataInitializer = scope.ServiceProvider.GetRequiredService<IDataInitializer>();

    
        dataInitializer.Migrate();

   
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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Online_Book_Store.Utility;
using Online_Book_Store.Utility.DbInitializer;

namespace Online_Book_Store
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // DB Configurations
            builder.Services.AddDbContext<ApplicationDbContext>(
                option => option.UseSqlServer("Data Source=.;Initial Catalog=Onilne Book Store; Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;")
            );

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(option =>
            {
                option.Password.RequiredLength = 4;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();


            builder.Services.AddTransient<IEmailSender, EmailSender>();

            builder.Services.AddScoped<IRepository<Book>,Repository<Book>>();
            builder.Services.AddScoped<IRepository<Category>, Repository<Category>>();
            builder.Services.AddScoped<IRepository<PublishingHouse>, Repository<PublishingHouse>>();
            builder.Services.AddScoped<IRepository<Author>, Repository<Author>>();
            builder.Services.AddScoped<IRepository<BookFile>, Repository<BookFile>>();
            builder.Services.AddScoped<IRepository<AuthorFile>, Repository<AuthorFile>>();
            builder.Services.AddScoped<IRepository<CategoryFile>, Repository<CategoryFile>>();
            builder.Services.AddScoped<IRepository<PublishingHouseFile>, Repository<PublishingHouseFile>>();
            builder.Services.AddScoped<IRepository<ApplicationUserOTP>, Repository<ApplicationUserOTP>>();

            // Configure request size limits
            builder.Services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = int.MaxValue;
            });

            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = long.MaxValue;
            });

            var app = builder.Build();

            // Add this middleware
            app.Use(async (context, next) =>
            {
                context.Features.Get<IHttpMaxRequestBodySizeFeature>()!.MaxRequestBodySize = null;
                await next.Invoke();
            });

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{search?}")
                .WithStaticAssets();



            app.Run();
        }
    }
}

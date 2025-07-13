using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Online_Book_Store.Utility.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDbContext applicationDbContext;

        public DbInitializer(UserManager<ApplicationUser> userManager,RoleManager<IdentityRole>roleManager,ApplicationDbContext applicationDbContext)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.applicationDbContext = applicationDbContext;
        }
        public void Initialize()
        {
            if(applicationDbContext.Database.GetPendingMigrations().Any())
                applicationDbContext.Database.Migrate();

            if(roleManager.Roles.IsNullOrEmpty())
            {
                //Roles Creation
                roleManager.CreateAsync(new(SD.SuperAdmin)).GetAwaiter().GetResult();
                roleManager.CreateAsync(new(SD.Admin)).GetAwaiter().GetResult();
                roleManager.CreateAsync(new(SD.Employer)).GetAwaiter().GetResult();
                roleManager.CreateAsync(new(SD.Company)).GetAwaiter().GetResult();
                roleManager.CreateAsync(new(SD.Customer)).GetAwaiter().GetResult();

                //Super Admin User Creation
                userManager.CreateAsync(new()
                {
                    UserName="Mekheimer",
                    FirstName="Ahmed",
                    LastName="Mekheimer",
                    Email="dodoxdmekheimer@gmail.com",
                    EmailConfirmed=true
                },"Admin123$").GetAwaiter().GetResult();

                var admin = userManager.FindByNameAsync("Mekheimer").GetAwaiter().GetResult();
                userManager.AddToRoleAsync(admin!,SD.SuperAdmin).GetAwaiter().GetResult();
            }
        }
    }
}

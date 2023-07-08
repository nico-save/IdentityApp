using Api.Data;
using Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Api.Services
{
    public class ContextSeedService
    {
        private readonly Context context;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public ContextSeedService(Context context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public async Task InitializeContextAsync()
        {
            if(this.context.Database.GetPendingMigrationsAsync().GetAwaiter().GetResult().Count() > 0) {
             // apply any pending migration into our database
             await this.context.Database.MigrateAsync();
            }

            if (!this.roleManager.Roles.Any()) {
                await this.roleManager.CreateAsync(new IdentityRole { Name = SD.AdminRole });
                await this.roleManager.CreateAsync(new IdentityRole { Name = SD.ManagerRole });
                await this.roleManager.CreateAsync(new IdentityRole { Name = SD.PlayerRole });
            }

            if( !this.userManager.Users.AnyAsync().GetAwaiter().GetResult()) {

                var admin = new User
                {
                    FirstName = "admin",
                    LastName = "jackson",
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    EmailConfirmed = true,
                };

                await this.userManager.CreateAsync(admin, "123456");

                await this.userManager.AddToRolesAsync(admin, new[] {SD.AdminRole, SD.ManagerRole,SD.PlayerRole});
                await this.userManager.AddClaimsAsync(admin, new Claim[]
                {
                    new Claim(ClaimTypes.Email, admin.Email),
                    new Claim(ClaimTypes.Surname, admin.LastName)
                });
                    };
            var manager = new User
            {
                FirstName = "manager",
                LastName = "wilson",
                UserName = "manager@example.com",
                Email = "manager@example.com",
                EmailConfirmed = true,
            };

            await this.userManager.CreateAsync(manager, "123456");

            await this.userManager.AddToRoleAsync(manager, SD.ManagerRole);
            await this.userManager.AddClaimsAsync(manager, new Claim[]
            {
                    new Claim(ClaimTypes.Email, manager.Email),
                    new Claim(ClaimTypes.Surname, manager.LastName)
            });
            var player = new User
            {
                FirstName = "player",
                LastName = "miller",
                UserName = "player@example.com",
                Email = "player@example.com",
                EmailConfirmed = true,
            };

            await this.userManager.CreateAsync(player, "123456");

            await this.userManager.AddToRoleAsync(player, SD.PlayerRole);
            await this.userManager.AddClaimsAsync(player, new Claim[]
            {
                    new Claim(ClaimTypes.Email, player.Email),
                    new Claim(ClaimTypes.Surname, player.LastName)
            });

            var vipPlayer = new User
            {
                FirstName = "vipplayer",
                LastName = "tomson",
                UserName = "vipplayer@example.com",
                Email = "vipplayer@example.com",
                EmailConfirmed = true,
            };

            await this.userManager.CreateAsync(vipPlayer, "123456");

            await this.userManager.AddToRoleAsync(vipPlayer, SD.PlayerRole);
            await this.userManager.AddClaimsAsync(vipPlayer, new Claim[]
            {
                    new Claim(ClaimTypes.Email, vipPlayer.Email),
                    new Claim(ClaimTypes.Surname, vipPlayer.LastName)
            });
        }
     
    }
}

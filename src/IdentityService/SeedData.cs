using System.Security.Claims;
using IdentityModel;
using IdentityService.Data;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace IdentityService;

public class SeedData
{
    public static void EnsureSeedData(WebApplication app)
    {
        using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
        context.Database.Migrate();

        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        
        if(userMgr.Users.Any()) return;

        var matiur = userMgr.FindByNameAsync("matiur").Result;
        if (matiur == null)
        {
            matiur = new ApplicationUser
            {
                UserName = "matiur",
                Email = "matiurrasel1002@email.com",
                EmailConfirmed = true,
            };
            var result = userMgr.CreateAsync(matiur, "Pa$$w0rd").Result;
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            result = userMgr.AddClaimsAsync(matiur, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Matiur Rasel"),
                        }).Result;
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }
            Log.Debug("matiur created");
        }
        else
        {
            Log.Debug("matiur already exists");
        }

        var badhon = userMgr.FindByNameAsync("badhon").Result;
        if (badhon == null)
        {
            badhon = new ApplicationUser
            {
                UserName = "badhon",
                Email = "badhon1503183@email.com",
                EmailConfirmed = true
            };
            var result = userMgr.CreateAsync(badhon, "Pa$$w0rd").Result;
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            result = userMgr.AddClaimsAsync(badhon, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Badhon Mahmud"),
                        }).Result;
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }
            Log.Debug("badhon created");
        }
        else
        {
            Log.Debug("badhon already exists");
        }

        var manha = userMgr.FindByNameAsync("manha").Result;
        if (manha == null)
        {
            manha = new ApplicationUser
            {
                UserName = "manha",
                Email = "manha2022@email.com",
                EmailConfirmed = true
            };
            var result = userMgr.CreateAsync(manha, "Pa$$w0rd").Result;
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            result = userMgr.AddClaimsAsync(manha, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Manha Rahman"),
                        }).Result;
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }
            Log.Debug("manha created");
        }
        else
        {
            Log.Debug("manha already exists");
        }
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Utilities;
using WhiteLagoon.Domain.Entites;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Infrastructure.Repository
{
    public class DbInitializer : IDbInitializer
    {
        private readonly AppDbContext _context;
        private readonly  RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        public DbInitializer(AppDbContext context, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public void Initilaize()
        {
            try
            {
                if (_context.Database.GetPendingMigrations().Count() > 0)
                {
                    _context.Database.Migrate();    
                }

                // Selecting Role in the Register is a Security Issues 
                // but we will handel that in the Deployement
                if (!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
                {

                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).Wait();
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).Wait();

                    _userManager.CreateAsync(new AppUser
                    {
                        Name = "Agag",
                        Email = "Agag@gmail.com",
                        UserName = "Agag@gmail.com",
                        PhoneNumber = "010284148882",
                        NormalizedEmail = "AGAG@GMAIL.COM",
                        NormalizedUserName= "AGAG@GMAIL.COM",
                        CreatedAt = DateTime.Now
                    },"123456agag").GetAwaiter().GetResult();

                    var user =  _context.AppUsers.FirstOrDefault(x=>x.UserName == "Agag@gmail.com");
                    _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();   
                }
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}

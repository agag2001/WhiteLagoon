using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NuGet.Protocol.Plugins;
using System.Runtime.InteropServices;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Utilities;
using WhiteLagoon.Domain.Entites;
using WhiteLagoon.ViewModels;

namespace WhiteLagoon.Controllers
{
    public class AccountController : Controller
    {
     
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController( UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
          
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }


        #region Register
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            returnUrl ??= Url.Content("/");
         
            RegisterVM registerVM = new()
            {
                RoleList = _roleManager.Roles.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Name
                }),
                RedirectURL = returnUrl 
               
               
            };
            return View(registerVM);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new()
                {
                    Name = model.Name,
                    NormalizedEmail = model.Email.ToUpper(),
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    EmailConfirmed = true,
                    UserName = model.Email,
                    CreatedAt = DateTime.Now,

                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.Role))
                    {
                        await _userManager.AddToRoleAsync(user, model.Role);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, SD.Role_Customer);
                    }
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    if (string.IsNullOrEmpty(model.RedirectURL))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return LocalRedirect(model.RedirectURL);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }

            model.RoleList = _roleManager.Roles.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Name
            });
            return View(model);
        } 
        #endregion

        #region Login
        public IActionResult Login([FromQuery]string returnUrl = null)
        {
			// if url is null >> put default
			returnUrl ??= Url.Content("~/");
            LoginVM loginVM = new LoginVM()
            {
                RedirectURL = returnUrl
			};
            return View(loginVM);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var userDB = await _userManager.FindByEmailAsync(model.Email);
                if (userDB is not null)
                {
                    var result = await _signInManager.PasswordSignInAsync(userDB, model.Password, model.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        var user = await _userManager.FindByEmailAsync(model.Email);
                        if( await _userManager.IsInRoleAsync(user, SD.Role_Admin))
                        {
                            return RedirectToAction("Index", "Dashboard");
                        }
                        else
                        {
							if (string.IsNullOrEmpty(model.RedirectURL))
							{
								return RedirectToAction("Index", "Home");
							}
							else
							{
								return LocalRedirect(model.RedirectURL);
							}
						}
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid Email or Password");

                    }


                }


            }
            return View(model);
        } 
        #endregion
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }
        public IActionResult AccessDenied()
        {
            return View();  
        }
    }
}

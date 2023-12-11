using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pronia.Helpers;
using Pronia.Models;
using Pronia.ViewModels.Account;

namespace Pronia.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM, string? ReturnUrl)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }

            AppUser user = new AppUser()
            {
                Name = registerVM.Name,
                Surname = registerVM.Surname,
                Email = registerVM.Email,
                UserName = registerVM.Username,
            };

            var result = await _userManager.CreateAsync(user, registerVM.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();

            }
            await _signInManager.SignInAsync(user, false);
            await _userManager.AddToRoleAsync(user,UserRole.Member.ToString());
            return RedirectToAction(nameof(Index),"Home");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }
            var user = await _userManager.FindByEmailAsync(loginVM.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Not Found");
                return View();
            }

            var result = _signInManager.CheckPasswordSignInAsync(user, loginVM.Password, true).Result;
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Not Found");
                return View();
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Waiting");
                return View();
            }

            await _signInManager.SignInAsync(user, true);

            return RedirectToAction(nameof(Index), "Home");
        }

        public async  Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index), "Home");
        }

        public async Task<IActionResult> CreateRole()
        {
            foreach(var item in Enum.GetValues(typeof(UserRole)))
            {
                if(await _roleManager.FindByNameAsync(item.ToString()) == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole() 
                    { 
                        Name=item.ToString(),
                    });

                }
            }

            return RedirectToAction(nameof(Index), "Home");
        }
    }
}

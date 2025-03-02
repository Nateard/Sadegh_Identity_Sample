using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Sadegh_Identity_Sample.Models.Dto;
using Sadegh_Identity_Sample.Models.DTO;
using Sadegh_Identity_Sample.Models.Entities;
using Sadegh_Identity_Sample.Services.EmailServices;

namespace Sadegh_Identity_Sample.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly EmailService _emailService;
        private readonly EmailContentService _emailContentService;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = new EmailService();
            _emailContentService = new EmailContentService();
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto input)
        {
            if (!ModelState.IsValid)
            {
                return View(input);
            }
            User newUser = new User()
            {
                FirstName = input.FirstName,
                LastName = input.LastName,
                Email = input.Email,
                UserName = input.Email,
            };
            var result = await _userManager.CreateAsync(newUser, input.Password);
            if (result.Succeeded)
            {
                var confirmationCode = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                string callBackUrl = Url.Action("ConfirmEmail", "Account", new { UserId = newUser.Id, token = confirmationCode }, protocol: Request.Scheme);
                var emailContent = _emailContentService.GenerateEmailConfirmationEmail(callBackUrl);
                _emailService.Execute(newUser.Email, emailContent.Body, emailContent.Subject);

                return RedirectToAction("DisplayEmail");
            }
            string message = "";
            foreach (var item in result.Errors.ToList())
            {
                message += item.Description + Environment.NewLine;

            }
            TempData["Message"] = message;


            return View(input);
        }

        public IActionResult Register()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            return View(new LogInDto
            {
                ReturnUrl = returnUrl,
            });

        }

        [HttpPost]
        public async Task<IActionResult> LogIn(LogInDto logIn)
        {
            if (!ModelState.IsValid)
            {
                return View(logIn);
            }
            var user = await _userManager.FindByEmailAsync(logIn.Email);
            await _signInManager.SignOutAsync();
            var result = await _signInManager.PasswordSignInAsync(user.UserName, logIn.Password, logIn.IsPersistent, true);
            if (result.Succeeded)
            {
                return Redirect(logIn.ReturnUrl);
            }

            if (result.RequiresTwoFactor == true)
            {
                //Redirect();
            }
            if (!result.IsLockedOut)
            {
                //
            }
            ModelState.AddModelError(string.Empty, "LogIn Error");
            return View();
        }
        public IActionResult LogOut()
        {
            _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return BadRequest();
            }
            var user = _userManager.FindByIdAsync(userId).Result;
            if (user == null)
            {
                return View("Error");
            }
            var result = _userManager.ConfirmEmailAsync(user, token).Result;
            if (result.Succeeded)
            {
                // return
            }
            else
            {
                return base.View("Error");
            }
            return RedirectToAction("Login");
        }

        public IActionResult DisplayEmail()
        {
            return View();
        }

    }
}

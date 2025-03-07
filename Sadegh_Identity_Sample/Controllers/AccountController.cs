using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.Identity.Client;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Sadegh_Identity_Sample.Models.Dto;
using Sadegh_Identity_Sample.Models.Dto.Account;
using Sadegh_Identity_Sample.Models.DTO;
using Sadegh_Identity_Sample.Models.Entities;
using Sadegh_Identity_Sample.Services.EmailServices;
using Sadegh_Identity_Sample.Services.SMSServices;
using System.Security.Claims;

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
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            MyAccountInfoDto accountInfo = new MyAccountInfoDto()
            {
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                FullName = $"{user.FirstName} {user.LastName}",
                Id = user.Id,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled,
                UserName = user.UserName
            };
            return View(accountInfo);
        }


        [Authorize]
        public IActionResult TwoFactorEnabled()
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            var result = _userManager.SetTwoFactorEnabledAsync(user,!user.TwoFactorEnabled).Result;

            return RedirectToAction(nameof(Index));
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
                return RedirectToAction("TwoFactorLogIn", new { logIn.Email, logIn.IsPersistent });
            }
            if (!result.IsLockedOut)
            {
                //
            }
            ModelState.AddModelError(string.Empty, "LogIn Error");
            return View();
        }

        public IActionResult TwoFactorLogIn(string email , bool isPersistent)
        {
            var user = _userManager.FindByEmailAsync(email).Result;
            if (user==null)
            {
                return BadRequest();
            }
            var providers = _userManager.GetValidTwoFactorProvidersAsync(user).Result;

            TwoFactorLoginDto model = new TwoFactorLoginDto();
            
             if (providers.Contains("Email"))
            {
                string emailCode = _userManager.GenerateTwoFactorTokenAsync(user, "Email").Result;
                EmailService emailService = new EmailService();
                emailService.Execute(user.Email, $"Two Factor Code:{emailCode}", "Two Factor Login");

                model.Provider = "Email";
                model.IsPersistent = isPersistent;
            }
            else if(providers.Contains("Phone"))
            {
                string smsCode = _userManager.GenerateTwoFactorTokenAsync(user, "Phone").Result;

                SmsService smsService = new SmsService();
                smsService.Send(user.PhoneNumber, smsCode);
                model.Provider = "Phone";
                model.IsPersistent = isPersistent;

            }
            return View(model);
        }


        [HttpPost]
        public IActionResult TwoFactorLogin(TwoFactorLoginDto twoFactor)
        {
            if (!ModelState.IsValid)
            {
                return View(twoFactor);
            }

            var user = _signInManager.GetTwoFactorAuthenticationUserAsync().Result;
            if (user == null)
            {
                return BadRequest();
            }

            var result = _signInManager.TwoFactorSignInAsync(twoFactor.Provider, twoFactor.Code, twoFactor.IsPersistent, false).Result;

            if (result.Succeeded) 
            {
                return RedirectToAction( "index", "Home");
            }
            else if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "حساب کاربری شما قفل شده است");
                return View();
            }
            else
            {
                ModelState.AddModelError("", "کد وارد شده صحیح نیست ");
                return View();
            }
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

        public IActionResult ForgetPassword()
        {

            return View();
        }

        [HttpPost]
        public IActionResult ForgetPassword(ForgetPasswordConfirmationDto forgot)
        {
            if (!ModelState.IsValid)
            {

                return View(forgot);
            }

            var user = _userManager.FindByEmailAsync(forgot.Email).Result;
            if (user == null || _userManager.IsEmailConfirmedAsync(user).Result == false)
            {
                ViewBag.meesage = "ممکن است ایمیل وارد شده معتبر نباشد! و یا اینکه ایمیل خود را تایید نکرده باشید";
                return View();
            }

            string token = _userManager.GeneratePasswordResetTokenAsync(user).Result;
            string callBackUrl = Url.Action("ResetPassword", "Account", new
            {
                UserId = user.Id,
                token = token
            }, protocol: Request.Scheme);

            string body = $"برای تنظیم مجدد کلمه عبور بر روی لینک زیر کلیک کنید <br/> <a href={callBackUrl}> link reset Password </a>";
            _emailService.Execute(user.Email, body, "فراموشی رمز عبور");
            ViewBag.meesage = "لینک تنظیم مجدد کلمه عبور برای ایمیل شما ارسال شد";
            return View();
        }

        public IActionResult ResetPassword(string userId, string token)
        {

            return View(new ResetPasswordDto
            {
                UserId = userId,
                Token = token

            });
        }

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordDto reset)
        {
            if (!ModelState.IsValid)
                return View(reset);
            if (reset.Password != reset.ConfirmPassword)
            {
                return BadRequest();
            }
            var user = _userManager.FindByIdAsync(reset.UserId).Result;
            if (user == null)
            {
                return BadRequest();
            }
            var result = _userManager.ResetPasswordAsync(user, reset.Token, reset.Password).Result;
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirm));
            }
            ViewBag.Errors = result.Errors.ToList();
            return View(result);
        }

        public IActionResult ResetPasswordConfirm()
        {
            return View();
        }

        public IActionResult SetPhoneNumber()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult SetPhoneNumber(SetPhoneNumberDto phoneNumberDto)
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            var setResult = _userManager.SetPhoneNumberAsync(user, phoneNumberDto.PhoneNumber).Result;
            string code = _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumberDto.PhoneNumber).Result;
            SmsService smsService = new SmsService();
            //smsService.Send(phoneNumberDto.PhoneNumber, code);
            TempData["PhoneNumber"] = phoneNumberDto.PhoneNumber;
            return RedirectToAction(nameof(VerifyPhoneNumber));
        }

        public IActionResult VerifyPhoneNumber()
        {
            return View(new VerifyPhoneNumberDto
            {
                PhoneNumber = TempData["PhoneNumber"].ToString()
            });
        }

        [Authorize]
        [HttpPost]
        public IActionResult VerifyPhoneNumber(VerifyPhoneNumberDto verify)
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            bool resultVerify = _userManager.VerifyChangePhoneNumberTokenAsync(user, verify.Code, verify.PhoneNumber).Result;
            if (resultVerify == false)
            {
                ViewData["Message"] = $"کد وارد شده برای شماره {verify.PhoneNumber} اشتباه اشت";
            }

            user.PhoneNumberConfirmed = true;
            _userManager.UpdateAsync(user);
            return RedirectToAction(nameof(VerifySuccess));
        }

        public IActionResult VerifySuccess()
        {

            return View();
        }


        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }


        public IActionResult ExternalLogin(string ReturnUrl)
        {
            string url = Url.Action(nameof(CallBack), "Account", new
            {
                ReturnUrl
            });

            var propertis = _signInManager
                .ConfigureExternalAuthenticationProperties("Google", url);

            return new ChallengeResult("Google", propertis);
        }

        public IActionResult CallBack(string ReturnUrl)
        {
            var loginInfo = _signInManager.GetExternalLoginInfoAsync().Result;

            string email = loginInfo.Principal.FindFirst(ClaimTypes.Email)?.Value ?? null;
            if (email == null)
            {
                return BadRequest();
            }
            string FirstName = loginInfo.Principal.FindFirst(ClaimTypes.GivenName)?.Value ?? null;
            string LastName = loginInfo.Principal.FindFirst(ClaimTypes.Surname)?.Value ?? null;

            var signin = _signInManager.ExternalLoginSignInAsync("Google", loginInfo.ProviderKey
                , false, true).Result;
            if (signin.Succeeded)
            {
                if (Url.IsLocalUrl(ReturnUrl))
                {
                    return Redirect("~/");

                }
                return RedirectToAction("Index", "Home");
            }
            else if (signin.RequiresTwoFactor)
            {
                //
            }

            var user = _userManager.FindByEmailAsync(email).Result;
            if (user == null)
            {
                User newUser = new User()
                {
                    UserName = email,
                    Email = email,
                    FirstName = FirstName,
                    LastName = LastName,
                    EmailConfirmed = true,
                };
                var resultAdduser = _userManager.CreateAsync(newUser).Result;
                user = newUser;
            }
            var resultAddlogin = _userManager.AddLoginAsync(user, loginInfo).Result;
            _signInManager.SignInAsync(user, false).Wait();


            return Redirect("/");
        } 

    }
    

}

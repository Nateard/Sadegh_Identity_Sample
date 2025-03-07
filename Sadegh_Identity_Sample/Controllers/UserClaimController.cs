using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sadegh_Identity_Sample.Models.Entities;
using System.Security.Claims;

namespace Sadegh_Identity_Sample.Controllers
{
    public class UserClaimController : Controller
    {
        private readonly UserManager<User> _userManager;

        public UserClaimController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [Authorize]
        public IActionResult Indzex()
        {

            return View(User.Claims);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(string ClaimType, string ClaimValue)
        {
            var user = _userManager.GetUserAsync(User).Result;

            Claim newClaim = new Claim(ClaimType, ClaimValue, ClaimValueTypes.String);
            var result = _userManager.AddClaimAsync(user, newClaim).Result;
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            return View();
        }


        public IActionResult Delete(string ClaimType)
        {
            var user = _userManager.GetUserAsync(User).Result;
            Claim claim = User.Claims.Where(p => p.Type == ClaimType).FirstOrDefault();
            if (claim != null)
            {
                var result = _userManager.RemoveClaimAsync(user, claim).Result;
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }

            }
            return RedirectToAction("Index");

        }


    }
}


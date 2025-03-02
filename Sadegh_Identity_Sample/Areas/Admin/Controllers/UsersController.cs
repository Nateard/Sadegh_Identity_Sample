using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sadegh_Identity_Sample.Areas.Admin.Models.Dto.Users;
using Sadegh_Identity_Sample.Migrations;
using Sadegh_Identity_Sample.Models.DTO;
using Sadegh_Identity_Sample.Models.Entities;

namespace Sadegh_Identity_Sample.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        public UsersController(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            var users = _userManager.Users.Select(p => new UserListDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                Email = p.Email,
                LastName = p.LastName,
                EmailConfirmed = p.EmailConfirmed,
                PhoneNumber = p.PhoneNumber,
                UserName = p.UserName,
                AccessFailedCount = p.AccessFailedCount,
            }).ToList();
            return View(users);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return View("Error");
            }
            UserEditDto userEditDto = new UserEditDto()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                UserName = user.UserName
            };
            return View(userEditDto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserEditDto userEditDto)
        {
            var user = await _userManager.FindByIdAsync(userEditDto.Id);
            user.FirstName = userEditDto.FirstName;
            user.LastName = userEditDto.LastName;
            user.Email = userEditDto.Email;
            user.PhoneNumber = userEditDto.PhoneNumber;
            user.UserName = userEditDto.UserName;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(userEditDto);
        }
        public async Task<IActionResult> Create(RegisterDto register)
        {
            if (!ModelState.IsValid)
            {
                return View(register);
            }
            User newUser = new User()
            {
                FirstName = register.FirstName,
                LastName = register.LastName,
                Email = register.Email,
                UserName = register.Email,
            };
            var result = await _userManager.CreateAsync(newUser, register.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "users", new { area = "Admin" });
            }
            string message = "";
            foreach (var item in result.Errors.ToList())
            {
                message += item.Description + Environment.NewLine;
            }
            TempData["Message"] = message;
            return View(register);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return RedirectToAction();
            }
            UserDeleteDto userDeleteDto = new UserDeleteDto()
            {
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                Id = user.Id,
                UserName = user.UserName
            };
            return View(userDeleteDto); 
        }

        [HttpPost]
        public async Task<IActionResult> Delete(UserDeleteDto input)
        {
 
            var user = await _userManager.FindByIdAsync(input.Id);

            if (user == null)
            {
                return RedirectToAction();
            }
           var result= await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Index","users");          
            }

            return View(result.Errors.ToList());
        }



        public IActionResult AddUserRole(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;

            var roles = new List<SelectListItem>(
                _roleManager.Roles.Select(p => new SelectListItem
                {
                    Text = p.Name,
                    Value = p.Name,
                }
                ).ToList());
           
            return View(new AddUserRoleDto
            {
                Id = id,
                Roles = roles,
                FullName = $"{user.FirstName}{user.LastName}",
                Email = user.Email,
            });  
        }

        [HttpPost]
        public async Task<IActionResult> AddUserRole(AddUserRoleDto newRole)
        {
            var user =await _userManager.FindByIdAsync(newRole.Id);

            var result = await _userManager.AddToRoleAsync(user,newRole.Role);

            return RedirectToAction("UserRoles", "users", new {Id= user.Id ,area = "Admin" });
        }


        public async Task<IActionResult> UserRoles(string id)
        {
             var user =await _userManager.FindByIdAsync(id);

            var result = await _userManager.GetRolesAsync(user);

            ViewBag.UserName = user.UserName;
            return View(result);
        }
    }
}

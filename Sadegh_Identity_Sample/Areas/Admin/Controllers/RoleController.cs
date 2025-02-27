using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sadegh_Identity_Sample.Areas.Admin.Models.Dto.Roles;
using Sadegh_Identity_Sample.Models.Entities;

namespace Sadegh_Identity_Sample.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<Role> _roleManager;
        public RoleController(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            var roles = _roleManager.Roles.Select(p => new RoleListDto
            {
                Description = p.Description,
                Id = p.Id,
                Name = p.Name
            }).ToList();
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create (AddNewRoleDto newRole)
        {
            Role role = new Role()
            {
                Description = newRole.Description,
                Name = newRole.Name,
            };
            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Role" , new {area = "Admin"});
            }
             ViewBag.Errors = result.Errors.ToList();
            return View(newRole);
        }
    }
}

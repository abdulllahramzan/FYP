using Chat.Web.Areas.Identity.Pages.Account;
using Chat.Web.Data;
using Chat.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AdminController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        public AdminController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpGet("Users")]
        public IActionResult Users()
        {
            string senderId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault().Value.ToString();
            var admin = _context.AppUsers.FirstOrDefault(c => c.Id == senderId);
            if(admin.UserName != "Admin")
            {
                return LocalRedirect("/");
            }
            var users = _context.AppUsers.Where(c => c.Id != senderId).ToList();
            return View(users);
        }

        [HttpGet("Save")]
        public IActionResult Save(string Id)
        {
            string senderId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault().Value.ToString();
            var admin = _context.AppUsers.FirstOrDefault(c => c.Id == senderId);
            if (admin.UserName != "Admin")
            {
                return LocalRedirect("/");
            }
            var users = _context.AppUsers.FirstOrDefault(c => c.Id == Id);
            if(users != null)
            {
                if (users.Locked == false)
                    users.Locked = true;
                else
                    users.Locked = false;
                _context.SaveChanges();
            }
            return Redirect("/Admin/Users");
        }

        [HttpGet("LogoutAdmin")]
        public IActionResult LogoutAdmin(string Id)
        {
             _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

                return LocalRedirect("/");

        }
    }
}

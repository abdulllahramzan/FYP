using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Chat.Web.Data;
using Chat.Web.ViewModels;
using Chat.Web.Models;

namespace Chat.Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MessengerrController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MessengerrController(ApplicationDbContext context,   IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet("privateUsers")]
        public IActionResult Index()
        {
            List<ApplicationUser> users = new List<ApplicationUser>();
            string senderId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault().Value.ToString();
            var friends = _context.Friends.Where(c => c.RequestedBy.Id == senderId || c.RequestedTo.Id == senderId && c.FriendStatus == Enums.Enum.FriendStatus.Approved).ToList();
            foreach (var items in friends)
            {
                var userRequestsBy = _context.AppUsers.FirstOrDefault(c => items.RequestedBy.Id != senderId && c.Id == items.RequestedBy.Id);
                var userRequestsTo = _context.AppUsers.FirstOrDefault(c => items.RequestedTo.Id != senderId && c.Id == items.RequestedTo.Id);
                if (userRequestsBy != null)
                {
                    users.Add(userRequestsBy);
                }
                if (userRequestsTo != null)
                {
                    users.Add(userRequestsTo);
                }
            }

            return View(users);
        }
        //[HttpGet("privateUsers")]
        //public IActionResult Index()
        //{
        //    string senderId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault().Value.ToString();
        //    var users = _context.AppUsers.Where(c => c.Id != senderId).ToList();
        //    return View(users);
        //}

        [HttpPost]
        public IActionResult AddFriend(string recieverId )
        {
            Friends friends = new Friends();
            string senderId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault().Value.ToString();
            friends.RequestedBy.Id = senderId;
            friends.RequestedTo.Id = recieverId;
            friends.RequestTime = DateTime.UtcNow;
            friends.FriendStatus = Enums.Enum.FriendStatus.Pending;
            _context.Friends.Add(friends);
            _context.SaveChanges();
            return View();
        }

        [HttpGet]
        public IActionResult FriendRequests()
        {
            string senderId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault().Value.ToString();
            var req = _context.Friends.Where(c => c.RequestedTo.Id == senderId && c.FriendStatus == Enums.Enum.FriendStatus.Pending).ToList();
            return View(req);
        }

        [HttpPost]
        public IActionResult ActOnRequest(string recieverId)
        {
            string senderId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault().Value.ToString();
            var req = _context.Friends.Where(c => c.RequestedBy.Id == recieverId && c.FriendStatus == Enums.Enum.FriendStatus.Pending).FirstOrDefault();
            req.FriendStatus = Enums.Enum.FriendStatus.Approved;
            req.BecameFriendsTime = DateTime.UtcNow;
            _context.Friends.Update(req);
            _context.SaveChanges();
            return View(req);
        }
        [HttpGet("Chat")]
        public IActionResult Chat(string receiverId)
        {
            string senderId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault().Value.ToString();
            VmMessage model = new VmMessage();
            model.User = _context.AppUsers.Find(receiverId);
            _httpContextAccessor.HttpContext.Session.SetString("Rid", model.User.Id);
            model.Messages = _context.PrivateMessages.Where(m => m.SenderId == senderId && m.ReceiverId == receiverId || m.SenderId == receiverId && m.ReceiverId == senderId).ToList();
            model.SenderId = senderId;
            return View(model);



        }
    }
}

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

        public MessengerrController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        //[HttpGet("privateUsers")]
        //public IActionResult Index()
        //{
        //    string senderId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault().Value.ToString();
        //    var users = _context.AppUsers.Where(c => c.Id != senderId).ToList();
        //    return View(users);
        //}

        [HttpGet("privateUsers")]
        public IActionResult Index()
        {
            List<ApplicationUser> users = new List<ApplicationUser>();
            string senderId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault().Value.ToString();
            var friends = _context.Friends.Where(c => c.FriendStatus == "Approved" &&
            c.RequestedById == senderId && c.BecameFriendsTime != null || c.RequestedToId == senderId && c.BecameFriendsTime != null && c.FriendStatus == "Approved").ToList();
            foreach (var items in friends)
            {
                var userRequestsBy = _context.AppUsers.FirstOrDefault(c => items.RequestedById != senderId && c.Id == items.RequestedById);
                var userRequestsTo = _context.AppUsers.FirstOrDefault(c => items.RequestedToId != senderId && c.Id == items.RequestedToId);
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
        [HttpGet("AllUsers")]
        public IActionResult AllUsers()
        {
            List<ApplicationUser> userss = new List<ApplicationUser>();
            string senderId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault().Value.ToString();
            var users = _context.AppUsers.Where(c => c.Id != senderId).ToList();
            var friends = _context.Friends.Where(c => c.RequestedById == senderId && c.FriendStatus == "Approved" || c.RequestedToId == senderId && c.FriendStatus == "Approved").ToList();
            foreach (var items in friends)
            {
                var userRequestsBy = _context.AppUsers.FirstOrDefault(c => items.RequestedById != senderId && c.Id == items.RequestedById);
                var userRequestsTo = _context.AppUsers.FirstOrDefault(c => items.RequestedToId != senderId && c.Id == items.RequestedToId);
                if (userRequestsBy != null)
                {
                    userss.Add(userRequestsBy);
                }
                if (userRequestsTo != null)
                {
                    userss.Add(userRequestsTo);
                }
            }
            var ids = userss.Select(c => c.Id).ToList();
            users = users.Except(userss).ToList();
            return View(users);
        }

        [HttpGet("AddFriend")]
        public IActionResult AddFriend(string Id)
        {
            Friends friends = new Friends();
            string senderId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault().Value.ToString();
            friends.RequestedById = senderId;
            friends.RequestedToId = Id;
            friends.RequestTime = DateTime.UtcNow;
            friends.FriendStatus = Enums.Enum.FriendStatus.Pending.ToString();
            _context.Friends.Add(friends);
            _context.SaveChanges();
            return Redirect("/Messengerr/AllUsers");
        }

        [HttpGet("FriendRequests")]
        public IActionResult FriendRequests()
        {
            List<ApplicationUser> users = new List<ApplicationUser>();
            string senderId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault().Value.ToString();
            var req = _context.Friends.Where(c => c.BecameFriendsTime == null &&
                                                c.FriendStatus == Enums.Enum.FriendStatus.Pending.ToString() &&
                                                c.RequestedTo.Id == senderId || c.RequestedById == senderId).ToList();
            if(req != null)
            {
                foreach(var items in req)
                {
                    var userRequestsBy = _context.AppUsers.FirstOrDefault(c => items.RequestedById != senderId && c.Id == items.RequestedById);
                    //var userRequestsTo = _context.AppUsers.FirstOrDefault(c => items.RequestedToId != senderId && c.Id == items.RequestedToId);
                    if (userRequestsBy != null)
                    {
                        users.Add(userRequestsBy);
                    }
                    //if (userRequestsTo != null)
                    //{
                    //    users.Add(userRequestsTo);
                    //}
                }
            }
            return View(users);
        }
            
        [HttpGet("ActOnRequest")]
        public IActionResult ActOnRequest(string Id, string status)
        {
            string senderId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault().Value.ToString();
            var req = _context.Friends.Where(c => c.RequestedById == Id || c.RequestedToId == Id 
             && c.RequestedToId == senderId || c.RequestedById == senderId && c.BecameFriendsTime == null && c.FriendStatus == Enums.Enum.FriendStatus.Pending.ToString()).FirstOrDefault();
            if(status == "approve")
            {
                req.FriendStatus = Enums.Enum.FriendStatus.Approved.ToString();
            }
            else if(status == "reject")
            {
                req.FriendStatus = Enums.Enum.FriendStatus.Rejected.ToString();
            }
            req.BecameFriendsTime = DateTime.UtcNow;
            _context.Friends.Update(req);
            _context.SaveChanges();
            return Redirect("/Messengerr/privateUsers");
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

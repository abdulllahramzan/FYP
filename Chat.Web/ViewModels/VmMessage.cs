using Chat.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Web.ViewModels
{
    public class VmMessage
    {
        public List<PrivateMessage> Messages { get; set; }
        public string SenderId { get; set; }
        public ApplicationUser User { get; set; }
    }
}

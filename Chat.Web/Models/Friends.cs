using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Chat.Web.Enums.Enum;

namespace Chat.Web.Models
{
    public class Friends
    {
        public int FriendsId { get; set; }

        [ForeignKey("RequestedById")]
        public ApplicationUser RequestedBy { get; set; }
        public string RequestedById { get; set; }
        [ForeignKey("RequestedToId")]
        public ApplicationUser RequestedTo { get; set; }
        public string RequestedToId { get; set; }
        public DateTime? RequestTime { get; set; }

        public DateTime? BecameFriendsTime { get; set; }

        public string FriendStatus { get; set; }
    }
}

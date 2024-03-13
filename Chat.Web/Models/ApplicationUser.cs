using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public string Discriminator { get; set; } 
        public ICollection<Room> Rooms { get; set; }
        public ICollection<Message> Messages { get; set; }
        [InverseProperty("RequestedBy")]
        public ICollection<Friends> Friend1 { get; set; }
        [InverseProperty("RequestedTo")]
        public ICollection<Friends> Friend2 { get; set; }
        public bool Locked { get; set; }
        
    }
}

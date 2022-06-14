using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Web.Enums
{
    public class Enum
    {
        public enum FriendStatus
        {

            [Description("Pending")]
            Pending = 'P',

            [Description("Approved")]
            Approved = 'A',

            [Description("Rejected")]
            Rejected = 'R',

            [Description("Blocked")]
            Blocked = 'B'
        }
    }
}

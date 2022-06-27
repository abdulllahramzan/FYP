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
            None = 'N',

            Pending = 'P',

            Approved = 'A',

            Rejected = 'R',

            Blocked = 'B'
        }
    }
}

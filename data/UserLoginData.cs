using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraMaster_ver2._0.data
{
    class UserLoginData
    {
        private static Account userdata;

        public static Account Userdata { get => userdata; set => userdata = value; }
    }
}

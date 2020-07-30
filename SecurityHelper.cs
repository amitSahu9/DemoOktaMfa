using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OktaDemo
{
    public class SecurityHelper
    {
        private readonly string userName;
        private readonly string password;
        public SecurityHelper(string username, string password)
        {
            this.userName = username;
            this.password = password;
        }
    }
}

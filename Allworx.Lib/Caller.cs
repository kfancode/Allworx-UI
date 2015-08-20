using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allworx.Lib
{
    public class Caller
    {
        private string phonenumber;
        private bool registered;

        public Caller(string m, bool r)
        {
            this.phonenumber = m;
            this.registered = r;
        }

        public string PhoneNumber
        {
            get { return phonenumber; }
        }
        

        public bool Registered
        {
            get { return registered; }
        }


        
    }
}

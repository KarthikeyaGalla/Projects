using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Utils
{
    internal class MemberIDGenerator
    {
        private static int studentCounter = 1;
        private static int facultyCounter = 1; 
        public static string random_Member_Generator(string role)
        {
            if (role.ToLower().StartsWith('s'))
            {
                return $"ST25_{studentCounter++.ToString("D3")}";
            }
            else if (role.ToLower().StartsWith('f'))
            {
                return $"FA25_{facultyCounter++.ToString("D3")}";
            }
            else {
                throw new ArgumentException("Invalid Role Specified");
            }

        }
    }
}

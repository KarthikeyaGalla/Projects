using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Utils;

namespace LibraryManagementSystem.Services
{
    internal class MemberManager
    {
        public static List<Member> Members = new List<Member>();
        
        //adding a class
        public static void AddMembers(Member member)
        {
            Members.Add(member);
            Console.WriteLine("Member Data added Successfully.");
        }

        public static void displayAllMembers()
        {
            if (Members.Count == 0)
            {
                Console.WriteLine("No Details Found.");
                return;
            }
            else
            {
                foreach (var member in Members)
                {
                    member.PrintInfo();
                }
            }
        }
    }
}

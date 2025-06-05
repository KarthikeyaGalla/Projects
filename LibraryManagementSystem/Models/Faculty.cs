using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    internal class Faculty: Member
    {

        public static List<Faculty> faculties = new List<Faculty>();
        public Faculty(string memberID, string name, string position): base(memberID, name, "Facutly")
        {

        }

        public override void PrintInfo()
        {
            Console.WriteLine($"[Faculty] ID: {MemberID}, Name: {Name}");
        }

        public override bool BorrowBook(Book book)
        {
            if (BorrowedBooks.Count >= 5)
            {
                return false;
            }
            return base.BorrowBook(book);
        }

        public static void DisplayFaculties()
        {
            if (faculties.Count == 0)
            {
                Console.WriteLine("No Data Found.");
                return;
            }
            else
            {
                foreach (var faculty in faculties)
                {
                    faculty.PrintInfo();
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    class Student:Member
    {

        public static List<Student> students = new List<Student>();
        public Student(string memberID, string name, string Position) : base(memberID, name, "Faculty")
        {

        }
        public override bool BorrowBook(Book book)
        {
            if (BorrowedBooks.Count >= 3)
            {
                return false;
            }
            return base.BorrowBook(book);
        }

        public override void PrintInfo()
        {

            Console.WriteLine($"[Student] ID: {MemberID}, Name: {Name}");
        }

        public static void DisplayStudents()
        {
            if (students.Count == 0)
            {
                Console.WriteLine("No Data Found.");
                return;
            }
            else
            {
                foreach (var student in students)
                {
                    student.PrintInfo();
                }
            }
        }


    }
}

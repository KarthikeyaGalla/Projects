using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Utils;

namespace LibraryManagementSystem.Models
{
    class Member
    {
        public string MemberID { get; set; }
        public string Name { get; set; }
        protected List<BorrowedRecord> BorrowedBooks = new List<BorrowedRecord>();
        public int MaxBookLimit { get; set; }
        public string? Position { get; set; }
        public int? BooksTaken { get; set; }

        public Member(string MemberID, string Name, string Position)
        {
            this.MemberID = MemberID;
            this.Name = Name;
            this.Position = Position;
            this.BooksTaken = null;
             //BorrowedBooks = new List<BorrowedRecord>();
        }

        public virtual void PrintInfo()
        {
            Console.WriteLine($"ID: {MemberID}, Name: {Name}, Position: {Position}");
        }

        public static void AddMember(string MemberID, string Name, string Position)
        {
            int BooksTaken = 0;
            MemberID = MemberID;
            Name = Name;
            if (Position[0] == 's')
            {
                Position = "Student";
            }
            else if (Position[0] == 'f')
            {
                Position = "Faculty";
            }
            else
            {
                Position = null;
            }
        }

        // Virtual Method to borrow a book
        public virtual bool BorrowBook(Book book)
        {
            if (book.AvailableCopies > 0)
            {
                book.AvailableCopies--;
                BorrowedBooks.Add(new BorrowedRecord(MemberID, book.BookID));

                foreach(var Books in BorrowedBooks)
                {
                    Console.WriteLine(Books.BookID);
                    Console.WriteLine(Books.MemberID);
                    //Console.WriteLine(Books.ToString());
                }
                return true;
            }
            return false;
        }


    }
}

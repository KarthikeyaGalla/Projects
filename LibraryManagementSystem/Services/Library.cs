using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Utils;

namespace LibraryManagementSystem.Services
{
    internal class Library : ILibrary
    {
        public List<Book> Books = new List<Book>();
        public List<Member> Members = new List<Member>();
        public List<BorrowedRecord> borrowedRecords = new List<BorrowedRecord>();
        //public List<BorrowedRecord> borrowedBooks = new List<BorrowedRecord>();

        //public static string GenerateUniqueIssueID()
        //{
        //    const string chars = "ABCDEFGHIJKLMNOPRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        //    var random = new Random();
        //    return new string(Enumerable.Repeat(chars, 15).Select(s=> s[random.Next(s.Length)]).ToArray() );
        //}

        public void AddBook(Book book)
        {
            Books.Add(book);
        }

        public void RemoveBook(string BookID)
        {
            Books.RemoveAll(b => b.BookID == BookID);
        }

        public string IssueBook(string IssueID, string MemberID)
        {
            Book book = Books.FirstOrDefault(b => b.BookID == IssueID);

            if (book == null)
            {
                Console.WriteLine("Book Not Found");
                return "Not Issued";
            }
            if(book.AvailableCopies <= 0)
            {
                Console.WriteLine("No Available Copies.");
                return "Not Issued";
            }
            book.AvailableCopies--;

            return IssueIDGenerator.GenerateUniqueIssueID();

        }

        public bool ReturnBook(string ReturnID, string IssuedID)
        {
            // Find the record that matches BOTH BookID and IssueID
            var record = BorrowedRecord.borrowedRecords
                .FirstOrDefault(r => r.BookID == ReturnID && r.IssueID == IssuedID);

            if (record != null)
            {
                // Find the book in the book list
                Book book = Books.FirstOrDefault(b => b.BookID == ReturnID);
                if (book != null)
                {
                    book.AvailableCopies++;
                    record.ReturnDate = DateTime.Now;


                    Console.WriteLine("Book Returned to the Library Successfully.");
                    return true;
                }
                else
                {
                    Console.WriteLine("Book record not found in library.");
                }
            }
            else
            {
                Console.WriteLine("No matching borrowed record found for that Book ID and Issue ID.");
            }

            return false;
        }

        public List<Book> ListAvailableBooks()
        {
            return Books.Where(book => book.AvailableCopies > 0).ToList();
        }

        public List<BorrowedRecord> BorrowedBook()
        {
            return borrowedRecords.ToList();
        }
    }
}

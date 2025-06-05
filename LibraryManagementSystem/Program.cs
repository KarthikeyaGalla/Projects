using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Utils;

namespace LibraryManagementSystem
{
    class Program
    {
        static List<BorrowedRecord> borrowedRecords = new List<BorrowedRecord>();
        static List<Book> books = new List<Book>(); 
        static List<Member> members = new List<Member>();

        static void Main(string[] args)
        {
            Library library = new Library();
            Librarian librarian = new Librarian();
            string memberid, m_name, position;
            Console.WriteLine("Enter Your Name: ");
            m_name = Console.ReadLine();
            Console.WriteLine("Enter your Position: ");
            position = Console.ReadLine().ToLower(); //stue
            memberid = MemberIDGenerator.random_Member_Generator(position);
            Member new_member = new Member(memberid, m_name, position);
            MemberManager.displayAllMembers();

            while (true)
            {
                Console.WriteLine("\n=== Library Management System ===");
                Console.WriteLine("1. Add Book");
                Console.WriteLine("2. Remove Book");
                Console.WriteLine("3. Issue Book");
                Console.WriteLine("4. Return Book");
                Console.WriteLine("5. List Available Books");
                Console.WriteLine("6. Borrowed Books");
                Console.WriteLine("7. Register the User.");
                Console.WriteLine("8. Display the Users ");
                Console.WriteLine("9. Exit");
                Console.Write("Enter your Choice: ");
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        // Adding Book with user input
                        Console.Write("Enter Book ID: ");
                        string bookID = Console.ReadLine();

                        Console.Write("Enter Book Title: ");
                        string title = Console.ReadLine();

                        Console.Write("Enter Author Name: ");
                        string author = Console.ReadLine();

                        Console.Write("Enter Total Copies: ");
                        int totalCopies;
                        while (!int.TryParse(Console.ReadLine(), out totalCopies))
                        {
                            Console.Write("Please enter a valid number for total copies: ");
                        }
                        Book newBook = new Book(bookID, title, author, totalCopies);
                        librarian.AddBookToLibrary(library, newBook);
                        Console.WriteLine("Book added Successfully.");
                        break;

                    case "2":
                        // Remove Book
                        Console.Write("Enter Book ID to remove: ");
                        string removeID = Console.ReadLine();
                        librarian.RemoveBookFromLibrary(library, removeID);
                        Console.WriteLine("{0} Book removed from the Library...", removeID);
                        break;

                    case "3":
                        // Issue Book
                        Console.Write("Enter Book ID to Issue: ");
                        string issueID = Console.ReadLine();
                        Console.Write("Enter Member ID: ");
                        string memberID = Console.ReadLine();
                        BorrowedRecord record = new BorrowedRecord(issueID, memberID);
                        string Issue_ID_generated = library.IssueBook(issueID, memberID);
                        record.update_IssueID(Issue_ID_generated);
                        Console.WriteLine("Your Book is Issued the IssuedID is "+ Issue_ID_generated);
                        //BorrowedRecord.borrowedRecords.Add(record);
                        //foreach(var rec in BorrowedRecord.borrowedRecords)
                        //{
                        //    Console.WriteLine($"ID: {rec.BookID}, MemberID: {rec.MemberID}, IssueID: {rec.IssueID}, BorrowedDate: {rec.BorrowedDate}, DueDate: {rec.DueDate}, Return Date: {rec.ReturnDate}");
                        //}
                        break;

                    case "4":
                        // Return Book
                        Console.Write("Enter Book ID to Return: ");
                        string returnID = Console.ReadLine();
                        Console.Write("Enter Issued ID: ");
                        string IssuedID = Console.ReadLine();
                        library.ReturnBook(returnID, IssuedID);
                        break;

                    case "5":
                        // List Available Books
                        List<Book> availableBooks = library.ListAvailableBooks();
                        Console.WriteLine("\nAvailable Books: ");
                        foreach(var book in availableBooks)
                        {
                            Console.WriteLine($"ID: {book.BookID}, Title: {book.Title}, Available: {book.AvailableCopies}");
                        }
                        break;

                    case "6":
                        //Borrowed Books
                        //List<BorrowedRecord> BorrowedBooks = library.BorrowedBook();
                        Console.WriteLine("\nBorrowed Books: ");
                        foreach (var book in BorrowedRecord.borrowedRecords)
                        {
                            Console.WriteLine($"ID: {book.BookID}, MemberID: {book.MemberID}, IssueID: {book.IssueID}, BorrowedDate: {book.BorrowedDate}, DueDate: {book.DueDate}, Return Date: {book.ReturnDate}");
                        }
                        break;

                    case "7":
                        // Adding Members
                        Console.WriteLine("Enter Your Name: ");
                        string name = Console.ReadLine();
                        Console.WriteLine("Enter your Position(Student/Faculty): ");
                        string Position = Console.ReadLine().ToLower();
                        memberID = MemberIDGenerator.random_Member_Generator(Position);
                        Member.AddMember(memberID, name, Position);
                        break;

                    case "8":
                        // Print users
                        Console.WriteLine("Select (Student/Facutly/ALL) to display? ");
                        string select = Console.ReadLine().ToLower();
                        if (select[0] == 's')
                        {
                            Student.DisplayStudents();
                        }
                        else if (select[0] == 'f')
                        {
                            Faculty.DisplayFaculties();
                        }
                        else
                        {
                            MemberManager.displayAllMembers();
                        }
                        break;

                    case "9":
                        // Exiting
                        Console.WriteLine("Exiting....");
                        return;

                    default:
                        Console.WriteLine("Invalid Choice. Try Again.");
                        break;
                    }
            }
        }
    }
}
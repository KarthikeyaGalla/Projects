using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services
{
    internal class Librarian
    {
        public void AddBookToLibrary(Library library, Book book)
        {
            library.AddBook(book);
        }

        public void RemoveBookFromLibrary(Library library, string BookID)
        {
            library.RemoveBook(BookID);
        }
    }
}

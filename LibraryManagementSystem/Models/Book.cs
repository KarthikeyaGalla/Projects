using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Models
{
    public class Book
    {
        public string BookID {  get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }


        public Book(string BookID, string Title, string author, int TotalCopies)
        {
            this.BookID = BookID;
            this.Title = Title;
            this.Author = author;
            this.TotalCopies = TotalCopies;
            this.AvailableCopies = TotalCopies;
        }
    }
}

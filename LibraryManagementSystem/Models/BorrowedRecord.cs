using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Utils;

namespace LibraryManagementSystem.Models
{
    internal class BorrowedRecord
    {
        public static List<Member> Members = new List<Member>();
        public static List<BorrowedRecord> borrowedRecords = new List<BorrowedRecord>();
        
        public string MemberID {  get; set; }
        public string BookID { get; set; }
        public DateTime BorrowedDate { get; set;}
        public DateTime DueDate { get; set; }
        public string? IssueID { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int? BooksTaken { get; set; }


        public BorrowedRecord(string BookID, string MemberID)
        {
            this.BookID = BookID;
            this.MemberID = MemberID;
            this.IssueID = null;
            this.BorrowedDate = DateTime.Now;
            this.DueDate = BorrowedDate.AddDays(15);
            this.ReturnDate = null;
            this.BooksTaken = 0;
        }

        public static void BookTaken(string MemberID)
        {
            var member = Members.FirstOrDefault(m => m.MemberID == MemberID);
            foreach (var records in borrowedRecords)
            {
                Console.WriteLine(records);
            }
        }

        public void update_IssueID(string IssueID)
        {
            this.IssueID = IssueID;
        }
    }
}


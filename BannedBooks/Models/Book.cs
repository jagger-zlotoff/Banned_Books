using System;
using System.ComponentModel.DataAnnotations;


namespace BannedBooks.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string? UserId { get; set; }

        [Required]
        public string Title { get; set; }

        public string Author { get; set; }
        public string Reason { get; set; }
        public string Genre { get; set; }


        public bool IsBanned { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

    }
}

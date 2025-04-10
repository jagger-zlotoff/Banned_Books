using System;
using System.ComponentModel.DataAnnotations;


namespace BannedBooks.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        [Required]
        public string Title { get; set; }

        public string Author { get; set; }
        public string State { get; set; }
        public string District { get; set; }
        public string Reason { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Date of Ban")]
        public DateTime? DateOfBan { get; set; }
        public string SecondaryAuthor { get; set; }
        public string Illustrator { get; set; }
        public string Translator { get; set; }

        

    }
}

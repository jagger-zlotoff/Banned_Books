using System;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using BannedBooks.Models;
using BannedBooks.Data;

namespace BannedBooks.Areas.Identity.Data.Helpers
{

    // Mapping between CSV columns and the Book model properties
    public sealed class BookMap : ClassMap<Book>
    {
        public BookMap()
        {
            // Adjust these "Name" values to match your CSV column headers exactly.
            Map(m => m.Title).Name("Title");
            Map(m => m.Author).Name("Author");
            // For example, if your CSV uses "Type of Ban" to indicate the ban reason:
            Map(m => m.Reason).Name("Type of Ban");
            
            Map(m => m.State).Name("State");
            Map(m => m.District).Name("District");
            // If your CSV includes a date column, adjust the name and formatting as needed.
            Map(m => m.DateOfBan).Name("Date of Challenge/Removal").Optional();

            // If your CSV file contains a description, map it accordingly.
            // Map(m => m.Description).Name("Description").Optional();

            Map(m => m.SecondaryAuthor).Name("Secondary Author(s)").Optional();
            Map(m => m.Illustrator).Name("Illustrator(s)").Optional();
            Map(m => m.Translator).Name("Translator(s)").Optional();
        }
    }

    public class CsvImporter
    {
        private readonly BannedBooksContext _context;

        public CsvImporter(BannedBooksContext context)
        {
            _context = context;
        }

        // Call this method with the full file path to your CSV.
        public void ImportBooks(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                // Register the mapping class so CsvHelper knows how to convert CSV rows to Book objects.
                csv.Context.RegisterClassMap<BookMap>();

                var records = csv.GetRecords<Book>().ToList();

                // Optional: Process or clean each book record if needed.
                foreach (var book in records)
                {
                    // For example, set a default description if none is provided:
                    if (string.IsNullOrWhiteSpace(book.Description))
                    {
                        book.Description = "No description provided.";
                    }
                }

                _context.Books.AddRange(records);
                _context.SaveChanges();
            }
        }
    }
}

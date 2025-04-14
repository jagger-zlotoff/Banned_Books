using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BannedBooks.Data;
using BannedBooks.Areas.Identity.Data.Helpers; 
using System;
using System.IO;

namespace BannedBooks.Pages.Admin
{
    public class ImportDataModel : PageModel
    {

        private readonly BannedBooksContext _context;
        private readonly IWebHostEnvironment _env;

        public string Message { get; set; }

        public ImportDataModel(BannedBooksContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            try
            {
                var filePath = Path.Combine(_env.ContentRootPath, "Areas", "Identity", "Data", "merged_dataset.csv");

                // Use the CSV importer to import data.
                var importer = new CsvImporter(_context);
                importer.ImportBooks(filePath);
                Message = "CSV data imported successfully!";
            }
            catch (Exception ex)
            {
                Message = $"Error importing CSV data: {ex.Message}";
            }

            return Page();
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BannedBooks.Data;
using BannedBooks.Models;
using Microsoft.AspNetCore.Authorization;

namespace BannedBooks.Pages.Books
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly BannedBooksContext _context;
        public const int PageSize = 100;  // Display 100 books per page

        public IndexModel(BannedBooksContext context)
        {
            _context = context;
        }

        // Bind the search term from query string
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        // Bind the current page from query string; defaults to 1
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        // Total count of books matching the current filter
        public int BookCount { get; set; }

        // List of books to display
        public IList<Book> Book { get; set; } = new List<Book>();

        public async Task OnGetAsync()
        {
            // Start with the base query from the database.
            var query = _context.Books.AsQueryable();

            // If a search term is provided, filter by Title or Author.
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(b => b.Title.Contains(SearchTerm) || b.Author.Contains(SearchTerm));
            }

            // Get the total number of matching books (for pagination)
            BookCount = await query.CountAsync();

            // Calculate how many records to skip based on the current page.
            int skip = (PageNumber - 1) * PageSize;

            // Retrieve only the records for the current page.
            Book = await query
                .OrderBy(b => b.Id) // Ensure a consistent order (you can adjust the ordering as needed)
                .Skip(skip)
                .Take(PageSize)
                .ToListAsync();
        }
    }
}

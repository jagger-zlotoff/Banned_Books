using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BannedBooks.Data;     // for BannedBooksContext
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BannedBooks.Models;
using Microsoft.AspNetCore.Mvc;

namespace BannedBooks.Pages
{
    public class IndexModel : PageModel
    {
        private readonly BannedBooksContext _context;

        public IndexModel(BannedBooksContext context)
        {
            _context = context;
        }

        // The text the user typed into the search bar
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        // The list of books we show in the table
        public IList<Book> Book { get; set; }

        public async Task OnGetAsync()
        {
            // Base query for all books
            var query = _context.Books.AsQueryable();

            // If user typed something, filter by Title or Author
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                query = query.Where(b =>
                    b.Title.Contains(SearchTerm) ||
                    b.Author.Contains(SearchTerm));
            }

            Book = await query.ToListAsync();
        }
    }
}

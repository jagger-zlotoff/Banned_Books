using System;
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
        private readonly BannedBooks.Data.BannedBooksContext _context;

        public IndexModel(BannedBooks.Data.BannedBooksContext context)
        {
            _context = context;
        }

        public IList<Book> Book { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Book = await _context.Books.ToListAsync();
        }
    }
}

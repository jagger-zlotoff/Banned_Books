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
using BannedBooks.Services;

namespace BannedBooks.Pages.Books
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly BannedBooks.Data.BannedBooksContext _context;
        private readonly AiService _aiService;

        public DetailsModel(BannedBooks.Data.BannedBooksContext context, AiService aiService)
        {
            _context = context;
            _aiService = aiService;
        }

        public Book Book { get; set; } = default!;
        public string AiResult { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            Book = await _context.Books.FindAsync(id);
            if (Book == null) return NotFound();

            // Call the AI service to classify the description.
            AiResult = await _aiService.GetClassificationAsync(Book.Description);

            return Page();
        }
    }
}

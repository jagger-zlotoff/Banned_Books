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
using Microsoft.AspNetCore.Identity;

namespace BannedBooks.Pages.Books
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly BannedBooks.Data.BannedBooksContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DeleteModel(BannedBooks.Data.BannedBooksContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Book Book { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            Book = await _context.Books.FindAsync(id);

            if (Book == null)
            {
                return NotFound();
            }

            // 👇 Only the owner can edit
            if (Book.UserId != _userManager.GetUserId(User))
            {
                return Forbid(); // or some error
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null) return NotFound();

            Book = await _context.Books.FindAsync(id);

            if (Book == null)
            {
                return NotFound();
            }
            // 👇 Only the owner can delete
            if (Book.UserId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            _context.Books.Remove(Book);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}

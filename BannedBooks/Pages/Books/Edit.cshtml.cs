using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BannedBooks.Data;
using BannedBooks.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BannedBooks.Pages.Books
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly BannedBooks.Data.BannedBooksContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EditModel(BannedBooks.Data.BannedBooksContext context, UserManager<IdentityUser> userManager)
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

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Check whether the current user is the owner of the Book.
            var currentUserId = _userManager.GetUserId(User);
            if (Book.UserId != currentUserId)
            {
                // If not, we return a Forbid result.
                return Forbid();
            }

            // Tell EF that this Book has been modified.
            _context.Attach(Book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(Book.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}

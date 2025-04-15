using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
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
        private readonly BannedBooksContext _context;
        private readonly AiService _aiService;

        public DetailsModel(BannedBooksContext context, AiService aiService)
        {
            _context = context;
            _aiService = aiService;
        }

        public Book Book { get; set; } = default!;

        // This will hold the friendly formatted AI result.
        public string AiResult { get; set; }

        // A helper class to parse the JSON response from the AI service.
        public class ClassificationResult
        {
            public string label { get; set; }
            public float score { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Book = await _context.Books.FindAsync(id);
            if (Book == null)
            {
                return NotFound();
            }

            // Call the AI service to classify the book description.
            // The AI service returns a JSON string.
            string rawAiResult = await _aiService.GetClassificationAsync(Book.Description ?? "");

            try
            {
                // Deserialize the JSON into a list of classification results.
                var results = JsonSerializer.Deserialize<List<ClassificationResult>>(rawAiResult);
                if (results != null && results.Count > 0)
                {
                    // Take the top result.
                    var best = results[0];
                    // Convert the confidence score to a percentage.
                    float confPct = best.score * 100;
                    AiResult = $"Predicted Category: {best.label} (Confidence: {confPct:F2}%)";
                }
                else
                {
                    AiResult = "No classification result returned.";
                }
            }
            catch (Exception ex)
            {
                AiResult = $"Error processing AI result: {ex.Message}";
            }

            return Page();
        }
    }
}

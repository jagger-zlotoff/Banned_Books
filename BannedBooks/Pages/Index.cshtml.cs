using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BannedBooks.Data;
using BannedBooks.Models;
using Microsoft.AspNetCore.Authorization;
using BannedBooks.Services;

namespace BannedBooks.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly BannedBooksContext _context;
        private readonly AiService _aiService;

        public const int PageSize = 100; // Adjust if you implement pagination later

        public IndexModel(BannedBooksContext context, AiService aiService)
        {
            _context = context;
            _aiService = aiService;
        }

        // Bind the search term from the query string.
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        // This list will hold the semantic search results.
        public List<SearchResult> SearchResults { get; set; } = new List<SearchResult>();

        // Container class for each search result.
        public class SearchResult
        {
            public Book Book { get; set; }
            public float Similarity { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // If no search term is provided, return an empty set.
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                SearchResults = new List<SearchResult>();
                return Page();
            }

            // 1. Get the query embedding from the AI service.
            float[] queryEmbedding = await _aiService.GetQueryEmbeddingAsync(SearchTerm);

            // Verify that the query embedding is valid.
            if (queryEmbedding == null || queryEmbedding.Length == 0)
            {
                Console.WriteLine("Query embedding is null or empty.");
                SearchResults = new List<SearchResult>();
                return Page();
            }

            Console.WriteLine($"Query embedding length: {queryEmbedding.Length}");
            if (queryEmbedding.Length >= 3)
            {
                Console.WriteLine($"First three query embedding values: {queryEmbedding[0]}, {queryEmbedding[1]}, {queryEmbedding[2]}");
            }

            // 2. Retrieve all books with a non-empty Embedding.
            var books = await _context.Books
                .Where(b => !string.IsNullOrEmpty(b.Embedding))
                .ToListAsync();

            var results = new List<SearchResult>();

            // 3. Compute cosine similarity for each book.
            foreach (var book in books)
            {
                try
                {
                    float[] bookEmbedding = JsonSerializer.Deserialize<float[]>(book.Embedding);
                    if (bookEmbedding == null || bookEmbedding.Length == 0)
                    {
                        Console.WriteLine($"Warning: Embedding is null or empty for book ID {book.Id}.");
                        continue;
                    }

                    // Ensure the dimensions match.
                    if (queryEmbedding.Length != bookEmbedding.Length)
                    {
                        Console.WriteLine($"Dimension mismatch for book ID {book.Id}: Query length = {queryEmbedding.Length}, Book embedding length = {bookEmbedding.Length}");
                        continue;
                    }

                    float similarity = CosineSimilarity(queryEmbedding, bookEmbedding);
                    Console.WriteLine($"Book ID {book.Id} similarity: {similarity}");
                    results.Add(new SearchResult { Book = book, Similarity = similarity });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing embedding for book ID {book.Id}: {ex.Message}");
                    continue;
                }
            }

            // 4. Order the results by descending similarity.
            SearchResults = results.OrderByDescending(r => r.Similarity).Take(PageSize).ToList();

            return Page();
        }

        /// <summary>
        /// Computes cosine similarity between two vectors.
        /// </summary>
        public float CosineSimilarity(float[] vectorA, float[] vectorB)
        {
            float dotProduct = 0;
            float normA = 0;
            float normB = 0;
            for (int i = 0; i < vectorA.Length; i++)
            {
                dotProduct += vectorA[i] * vectorB[i];
                normA += vectorA[i] * vectorA[i];
                normB += vectorB[i] * vectorB[i];
            }
            if (normA == 0 || normB == 0)
                return 0;
            return dotProduct / ((float)Math.Sqrt(normA) * (float)Math.Sqrt(normB));
        }
    }
}

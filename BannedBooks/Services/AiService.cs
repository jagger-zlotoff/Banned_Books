﻿using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BannedBooks.Services
{
    public class AiService
    {
        private readonly HttpClient _httpClient;

        public AiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetClassificationAsync(string description)
        {
            // Construct the JSON payload.
            var payload = new { text = description };
            var jsonPayload = JsonSerializer.Serialize(payload);
            using var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            // Call your Flask API at the specified URL.
            var response = await _httpClient.PostAsync("http://127.0.0.1:5000/classify", content);
            if (!response.IsSuccessStatusCode)
            {
                return $"Error: {response.StatusCode}";
            }
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<float[]> GetQueryEmbeddingAsync(string query)
        {
            var payload = new { text = query };
            var jsonPayload = JsonSerializer.Serialize(payload);
            using var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://127.0.0.1:5000/embed", content);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to retrieve query embedding");

            var resultJson = await response.Content.ReadAsStringAsync();

            // Option 1: Use options to allow case-insensitive mapping.
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<QueryEmbeddingResult>(resultJson, options);

            return result.Embedding;
        }

        public class QueryEmbeddingResult
        {
            // Optionally, annotate the property to map the JSON "embedding" property.
            [JsonPropertyName("embedding")]
            public float[] Embedding { get; set; }
        }
    }
}

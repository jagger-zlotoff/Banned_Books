Overview
In this project, I developed a web application that allows users to search and analyze a curated database of banned books. The application is built using ASP.NET Core and uses MySQL for its database. AI integration is a key component, consisting of:

Text Classification:
A fine-tuned Hugging Face Transformer model classifies books by ban reason (e.g., political, religious, cultural, offensive language) based on book text (Title, Description, etc.).

Semantic Search:
Semantic search is enabled by generating dense vector embeddings using Sentence-Transformers. User queries are converted into embeddings that are compared to precomputed embeddings stored for each book. Cosine similarity scores (expressed as percentages) rank the relevant books.

Both functionalities are exposed via a single Flask API with two endpoints (/classify and /embed), allowing seamless integration with the ASP.NET application via an AiService class.

1. Environment Setup
1.1 Python Virtual Environment
To manage dependencies for the AI components, I created a Python virtual environment:

On Windows (Command Prompt):

bash
Copy
python -m venv env
env\Scripts\activate
On Windows (PowerShell):

powershell
Copy
Set-ExecutionPolicy RemoteSigned -Scope CurrentUser
.\env\Scripts\Activate.ps1
On macOS/Linux:

bash
Copy
python3 -m venv env
source env/bin/activate
1.2 Package Installation
Within the virtual environment, I installed the following packages:

Flask:
Provides a lightweight web framework for building the AI API.

bash
Copy
pip install flask
Transformers:
Loads and fine-tunes pre-trained text classification models.

bash
Copy
pip install transformers
Sentence-Transformers:
Generates dense vector embeddings for semantic search.

bash
Copy
pip install sentence-transformers
Datasets:
Simplifies data loading and preprocessing.

bash
Copy
pip install datasets
PyMySQL:
A client library used to connect to MySQL.

bash
Copy
pip install pymysql
Cryptography:
Required by PyMySQL for secure password authentication.

bash
Copy
pip install cryptography
Each package has a specific role: Flask runs the API; Transformers and Sentence-Transformers handle the AI tasks; and PyMySQL and Cryptography support database interactions.

2. Database and Scaffolding
2.1 MySQL Database
I used MySQL as the database to store all book details. The Book model (in ASP.NET) is defined as follows:

csharp
Copy
public class Book
{
    public int Id { get; set; }
    public string UserId { get; set; }     // Associated user (if applicable)
    
    [Required]
    public string Title { get; set; }
    
    public string Author { get; set; }
    public string Genre { get; set; }
    public string Reason { get; set; }       // Fine-grained ban reason (if applicable)
    public bool IsBanned { get; set; }        // Stored as a BIT (true/false)
    
    [DataType(DataType.MultilineText)]
    public string Description { get; set; }
    
    // Embedding stored as a JSON string
    public string Embedding { get; set; }
}
After updating the model, I created a migration and updated the database schema using ASP.NET's scaffolding tools. ASP.NET scaffolding was also used to generate CRUD pages for the Book entity, allowing rapid development of the user interface.

2.2 ASP.NET Scaffolding
Scaffolding automatically generated a set of pages (Index, Details, Create, Edit, and Delete) for managing books. I then customized the Index and Details pages to integrate semantic search and AI classification features.

3. AI Integration via Flask API
3.1 Flask API Endpoints
I built a single Flask API (app.py) that serves both AI components:

/classify:
Uses a fine-tuned Transformers model (saved as ./fine_tuned_model) to classify book text. It returns predictions with labels and confidence scores.

/embed:
Uses Sentence-Transformers (all-MiniLM-L6-v2) to generate an embedding for input text. This vector is used for semantic search.

app.py:

python
Copy
from flask import Flask, request, jsonify
from transformers import pipeline
from sentence_transformers import SentenceTransformer

app = Flask(__name__)

# Classification endpoint using the fine-tuned model
classifier = pipeline("text-classification", model="./fine_tuned_model")

# Semantic search endpoint using SentenceTransformer for embeddings
embedder = SentenceTransformer('all-MiniLM-L6-v2')

@app.route('/classify', methods=['POST'])
def classify_text():
    data = request.get_json()
    if not data or 'text' not in data:
        return jsonify({"error": "No text provided"}), 400
    text = data['text']
    result = classifier(text)
    return jsonify(result)

@app.route('/embed', methods=['POST'])
def embed_text():
    data = request.get_json()
    if not data or 'text' not in data:
        return jsonify({"error": "No text provided"}), 400
    text = data['text']
    embedding = embedder.encode(text)
    return jsonify({"embedding": embedding.tolist()})

if __name__ == '__main__':
    app.run(debug=True, port=5000)
The API is hosted on port 5000 and manages both endpoints, so only one server process is required.

3.2 Model Training and Embedding Generation
Text Classification:
I used fine_tune.py to fine-tune a pre-trained model (e.g., DistilBERT) on a dataset of 500 labeled books. The fine-tuned model was saved in the ./fine_tuned_model directory and loaded by the /classify endpoint.

Embedding Generation:
A separate Python script (e.g., generate_embeddings.py or compute_embeddings.py) computed embeddings for each book’s Title and Description using Sentence-Transformers. These embeddings are stored (as JSON strings) in the MySQL database in the Embedding column.

4. Semantic Search Integration in ASP.NET
4.1 Overview
Semantic search in the ASP.NET application works by:

Converting the user’s search query into an embedding using the Flask /embed endpoint.

Retrieving each book’s precomputed embedding from the MySQL database.

Computing the cosine similarity between the query embedding and each book embedding.

Ranking the books by similarity and displaying the results.

4.2 AiService in ASP.NET
The AiService class encapsulates the calls to the Flask API:

csharp
Copy
using System.Net.Http;
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
            var payload = new { text = description };
            var jsonPayload = JsonSerializer.Serialize(payload);
            using var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
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
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<QueryEmbeddingResult>(resultJson, options);
            return result.Embedding;
        }

        public class QueryEmbeddingResult
        {
            [JsonPropertyName("embedding")]
            public float[] Embedding { get; set; }
        }
    }
}
4.3 ASP.NET Pages for Semantic Search
I updated the Index page in ASP.NET to include a search bar. When a search query is submitted, the PageModel calls the AiService to obtain the query embedding, computes cosine similarity against each book’s stored embedding, and then displays the ranked list of results.

The relevant code in Index.cshtml.cs and Index.cshtml is detailed in previous sections.

5. Testing and Evaluation
5.1 Testing the Flask API
Using Postman or curl for /embed:
Send a POST request with a JSON payload, for example:

json
Copy
{ "text": "A sample search query for revolutionary literature." }
Verify that the returned response contains an "embedding" field with an array of floats.

5.2 Testing the ASP.NET Semantic Search
Run the ASP.NET Application:
Navigate to the Books page (e.g., https://localhost:7119/Books).

Use the Search Bar:
Enter a semantic query (e.g., “revolution political change”).

Review the Results:
The page should display a table with book details and a similarity score (as a percentage).

Debugging:
I added logging in the PageModel to output the dimension and first few values of the query embedding, along with cosine similarity scores for each book. These logs help diagnose any issues during similarity computation.

5.3 Performance Metrics
For evaluation:

Classification Model:
I monitored confidence scores (provided by the classification model) and used standard metrics such as accuracy, precision, recall, and F1-score.

Semantic Search:
I use the cosine similarity (expressed as a percentage) as an indicator of the semantic relevance of a search query.
In a more formal evaluation, metrics such as Recall@K, Mean Average Precision (mAP), or nDCG (Normalized Discounted Cumulative Gain) could be computed if ground-truth relevance data were available.

6. Additional Considerations
Scalability:
For very large datasets, consider using external vector search engines (e.g., FAISS) to improve retrieval performance.

Active Learning:
I plan to further refine the classification model by incrementally annotating additional examples based on model uncertainty.

Deployment:
For production, both the Flask API and ASP.NET application should use HTTPS, and proper authentication should be implemented for API endpoints.

Source Control and Documentation:
Version control (Git) is used to track changes, and detailed commit messages document the evolution of the project’s code and data.

Summary
Virtual Environment & Dependencies:
Set up a Python virtual environment and install Flask, Transformers, Sentence-Transformers, Datasets, PyMySQL, and Cryptography.

Database & Scaffolding:
MySQL is used for the database. ASP.NET scaffolding generated CRUD pages for the Book model, which was later customized to incorporate AI features.

AI Integration via Flask API:
A single Flask API serves both /classify and /embed endpoints. The /classify endpoint uses a fine-tuned model for text classification, while /embed uses Sentence-Transformers for semantic embeddings.

Precomputed Embeddings:
A separate script computed and stored book embeddings in the MySQL database.

Semantic Search in ASP.NET:
The ASP.NET application calls the Flask API to obtain a query embedding, computes cosine similarity with stored book embeddings, and displays the results ranked by similarity.

Testing & Evaluation:
Postman and curl were used to test the Flask endpoints. Logs and standard metrics (confidence percentages, cosine similarity scores) help monitor model performance.

This documentation should provide a thorough overview of my project’s setup, technical implementations, and evaluation methods.

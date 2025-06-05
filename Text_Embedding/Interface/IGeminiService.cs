using System.Threading.Tasks;

namespace Text_Embedding.Interface
{
    public interface IGeminiService
    {
        Task<string> GenerateContentAsync(string prompt);
        Task<double[]> GenerateEmbeddingAsync(string prompt);
    }
}

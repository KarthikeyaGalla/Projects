namespace GeminiIntegration.Interface
{
    public interface IGeminiService
    {
        Task<string> GenerateContentAsync(string prompt);
    }
}

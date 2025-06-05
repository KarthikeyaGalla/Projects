namespace Speech_Bot.Models
{
    public class InferenceConfig
    {
        public int MaxTokens { get; set; } = 1024;
        public double TopP { get; set; } = 0.9;
        public double Temperature { get; set; } = 0.7;
    }
}

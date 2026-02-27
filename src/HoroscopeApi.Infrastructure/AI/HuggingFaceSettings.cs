namespace HoroscopeApi.Infrastructure.AI;

public sealed class HuggingFaceSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string ApiUrl { get; set; } = "https://router.huggingface.co";
    public string Model { get; set; } = "meta-llama/Llama-3.1-8B-Instruct";
    public double Temperature { get; set; } = 0.7;
    public int TimeoutSeconds { get; set; } = 90;
}
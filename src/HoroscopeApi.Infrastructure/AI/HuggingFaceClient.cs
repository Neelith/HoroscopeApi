using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using HoroscopeApi.Application.Services.AI;
using HoroscopeApi.Domain.Constants;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HoroscopeApi.Infrastructure.AI;

public sealed class HuggingFaceClient : IHuggingFaceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HuggingFaceClient> _logger;
    private readonly IHoroscopePromptBuilder _promptBuilder;
    private readonly HuggingFaceSettings _settings;

    public HuggingFaceClient(
        HttpClient httpClient,
        IOptions<HuggingFaceSettings> settings,
        IHoroscopePromptBuilder promptBuilder,
        ILogger<HuggingFaceClient> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _promptBuilder = promptBuilder;
        _logger = logger;
    }

    public async Task<Result<HuggingFaceHoroscopeData>> GenerateHoroscopeAsync(
        GenerateHoroscopeRequest request,
        CancellationToken cancellationToken = default)
    {
        List<ChatMessage> messages = await _promptBuilder.BuildMessageAsync(request.Date, request.SignInfo, cancellationToken);
        return await GenerateWithMessages(messages, cancellationToken);
    }

    public async Task<Result<HuggingFaceHoroscopeData>> GenerateWithMessages(
        List<ChatMessage> messages,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var apiRequest = new
            {
                messages,
                temperature = _settings.Temperature,
                model = _settings.Model,
                stream = false,
                response_format = new { type = "json_object" }
            };

            string requestUri = $"{_settings.ApiUrl}/v1/chat/completions";
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
                requestUri,
                apiRequest,
                CancellationToken.None);

            if (!response.IsSuccessStatusCode)
            {
                return MapHttpError(response.StatusCode);
            }

            ChatCompletionResponse? apiResponse = await response.Content.ReadFromJsonAsync<ChatCompletionResponse>(
                CancellationToken.None);

            if (apiResponse?.Choices == null || apiResponse.Choices.Count == 0)
            {
                return Result.Ko<HuggingFaceHoroscopeData>(AiErrors.InvalidResponse);
            }

            string generatedText = apiResponse.Choices[0].Message.Content.Trim();

            // Try to extract JSON if wrapped in markdown code blocks (safety fallback)
            if (generatedText.StartsWith("```"))
            {
                string[] lines = generatedText.Split('\n');
                generatedText = string.Join('\n', lines.Skip(1).SkipLast(1));
            }

            // Remove any "json" prefix after code block marker
            if (generatedText.StartsWith("json"))
            {
                generatedText = generatedText.Substring(4).TrimStart();
            }

            // Log the raw AI response for debugging
            _logger.LogDebug("Raw AI response: {Response}", generatedText);

            HuggingFaceHoroscopeData? horoscopeData = JsonSerializer.Deserialize<HuggingFaceHoroscopeData>(
                generatedText,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (horoscopeData == null)
            {
                _logger.LogError("Failed to deserialize AI response. Raw response: {Response}", generatedText);
                return Result.Ko<HuggingFaceHoroscopeData>(AiErrors.InvalidResponse);
            }

            if (!ValidateResponse(horoscopeData))
            {
                _logger.LogError("AI response validation failed. Deserialized data: {@HoroscopeData}", horoscopeData);
                return Result.Ko<HuggingFaceHoroscopeData>(AiErrors.InvalidResponse);
            }

            return Result.Ok(horoscopeData);
        }
        catch (OperationCanceledException)
        {
            return Result.Ko<HuggingFaceHoroscopeData>(AiErrors.Timeout);
        }
        catch (JsonException)
        {
            return Result.Ko<HuggingFaceHoroscopeData>(AiErrors.InvalidResponse);
        }
        catch
        {
            return Result.Ko<HuggingFaceHoroscopeData>(AiErrors.GenerationFailed);
        }
    }

    private static Result<HuggingFaceHoroscopeData> MapHttpError(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden =>
                Result.Ko<HuggingFaceHoroscopeData>(AiErrors.InvalidApiKey),
            HttpStatusCode.TooManyRequests =>
                Result.Ko<HuggingFaceHoroscopeData>(AiErrors.RateLimitExceeded),
            HttpStatusCode.ServiceUnavailable =>
                Result.Ko<HuggingFaceHoroscopeData>(AiErrors.ModelLoading),
            _ => Result.Ko<HuggingFaceHoroscopeData>(AiErrors.GenerationFailed)
        };
    }

    private static bool ValidateResponse(HuggingFaceHoroscopeData horoscope)
    {
        if (string.IsNullOrWhiteSpace(horoscope.Sign) ||
            string.IsNullOrWhiteSpace(horoscope.General) ||
            string.IsNullOrWhiteSpace(horoscope.Love) ||
            string.IsNullOrWhiteSpace(horoscope.Career) ||
            string.IsNullOrWhiteSpace(horoscope.Health) ||
            horoscope.Keywords == null || horoscope.Keywords.Count == 0 ||
            horoscope.LuckyColors == null || horoscope.LuckyColors.Count == 0 ||
            horoscope.MoodScore < 1 || horoscope.MoodScore > 10)
        {
            return false;
        }

        return true;
    }

    private sealed record ChatCompletionResponse(
        [property: JsonPropertyName("choices")]
        List<ChatChoice> Choices);

    private sealed record ChatChoice(
        [property: JsonPropertyName("message")]
        ChatMessage Message);
}
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Hermes.Results;
using HoroscopeApi.Application.Infrastructure.AI;
using HoroscopeApi.Domain.AI;
using Microsoft.Extensions.Options;

namespace HoroscopeApi.Infrastructure.AI;

public sealed class HuggingFaceClient : IHuggingFaceClient
{
    private readonly HttpClient _httpClient;
    private readonly HuggingFaceSettings _settings;
    private readonly IHoroscopePromptBuilder _promptBuilder;

    public HuggingFaceClient(
        HttpClient httpClient,
        IOptions<HuggingFaceSettings> settings,
        IHoroscopePromptBuilder promptBuilder)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _promptBuilder = promptBuilder;
    }

    public async Task<Result<HuggingFaceBatchResponse>> GenerateBatchHoroscopesAsync(
        GenerateBatchHoroscopesRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var messages = _promptBuilder.BuildMessages(request.Date, request.ZodiacSigns);
            
            var apiRequest = new
            {
                messages = messages,
                temperature = _settings.Temperature,
                model = _settings.Model,
                stream = false,
                response_format = new
                {
                    type = "json_object"
                }
            };

            var requestUri = $"{_settings.ApiUrl}/v1/chat/completions";
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");

            var response = await _httpClient.PostAsJsonAsync(
                requestUri,
                apiRequest,
                CancellationToken.None);

            if (!response.IsSuccessStatusCode)
            {
                return MapHttpError(response.StatusCode);
            }

            var apiResponse = await response.Content.ReadFromJsonAsync<ChatCompletionResponse>(
                cancellationToken: CancellationToken.None);

            if (apiResponse?.Choices == null || apiResponse.Choices.Count == 0)
            {
                return Result.Ko<HuggingFaceBatchResponse>(AIErrors.InvalidResponse);
            }

            var generatedText = apiResponse.Choices[0].Message.Content.Trim();
            
            // Try to extract JSON if wrapped in markdown code blocks (safety fallback)
            if (generatedText.StartsWith("```"))
            {
                var lines = generatedText.Split('\n');
                generatedText = string.Join('\n', lines.Skip(1).SkipLast(1));
            }
            
            // Remove any "json" prefix after code block marker
            if (generatedText.StartsWith("json"))
            {
                generatedText = generatedText.Substring(4).TrimStart();
            }

            var batchResponse = JsonSerializer.Deserialize<HuggingFaceBatchResponse>(
                generatedText,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (batchResponse == null || !ValidateResponse(batchResponse))
            {
                return Result.Ko<HuggingFaceBatchResponse>(AIErrors.InvalidResponse);
            }

            return Result.Ok(batchResponse);
        }
        catch (OperationCanceledException)
        {
            return Result.Ko<HuggingFaceBatchResponse>(AIErrors.Timeout);
        }
        catch (JsonException)
        {
            return Result.Ko<HuggingFaceBatchResponse>(AIErrors.InvalidResponse);
        }
        catch
        {
            return Result.Ko<HuggingFaceBatchResponse>(AIErrors.GenerationFailed);
        }
    }

    private static Result<HuggingFaceBatchResponse> MapHttpError(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden => 
                Result.Ko<HuggingFaceBatchResponse>(AIErrors.InvalidApiKey),
            HttpStatusCode.TooManyRequests => 
                Result.Ko<HuggingFaceBatchResponse>(AIErrors.RateLimitExceeded),
            HttpStatusCode.ServiceUnavailable => 
                Result.Ko<HuggingFaceBatchResponse>(AIErrors.ModelLoading),
            _ => Result.Ko<HuggingFaceBatchResponse>(AIErrors.GenerationFailed)
        };
    }

    private static bool ValidateResponse(HuggingFaceBatchResponse response)
    {
        if (response.Horoscopes == null || response.Horoscopes.Count != 12)
            return false;

        foreach (var horoscope in response.Horoscopes)
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
        }

        return true;
    }

    private sealed record ChatCompletionResponse(
        [property: JsonPropertyName("choices")] List<ChatChoice> Choices);

    private sealed record ChatChoice(
        [property: JsonPropertyName("message")] ChatMessage Message);
}

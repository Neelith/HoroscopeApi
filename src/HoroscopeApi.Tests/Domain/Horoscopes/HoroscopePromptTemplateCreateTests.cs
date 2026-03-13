namespace HoroscopeApi.Tests.Domain.Horoscopes;

public sealed class HoroscopePromptTemplateCreateTests
{
    [Fact]
    public void Create_WithValidData_ReturnsSuccess()
    {
        var result = HoroscopePromptTemplate.Create(
            HoroscopePeriod.Daily,
            "You are an astrologer.",
            "Example 1: ...",
            "Generate a horoscope for {sign}.");

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(HoroscopePeriod.Daily, result.Value!.Period);
        Assert.Equal("You are an astrologer.", result.Value.SystemPrompt);
        Assert.Equal("Generate a horoscope for {sign}.", result.Value.UserPromptTemplate);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptySystemPrompt_ReturnsFailure(string systemPrompt)
    {
        var result = HoroscopePromptTemplate.Create(
            HoroscopePeriod.Daily,
            systemPrompt,
            "examples",
            "user template");

        Assert.True(result.IsFailure);
        Assert.Equal("Horoscope.InvalidPromptTemplate", result.Errors[0].Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyUserPromptTemplate_ReturnsFailure(string userPromptTemplate)
    {
        var result = HoroscopePromptTemplate.Create(
            HoroscopePeriod.Weekly,
            "Valid system prompt",
            "examples",
            userPromptTemplate);

        Assert.True(result.IsFailure);
        Assert.Equal("Horoscope.InvalidPromptTemplate", result.Errors[0].Code);
    }

    [Fact]
    public void Create_WithEmptyFewShotExamples_ReturnsSuccess()
    {
        var result = HoroscopePromptTemplate.Create(
            HoroscopePeriod.Monthly,
            "System prompt",
            "",
            "User template");

        Assert.True(result.IsSuccess);
        Assert.Equal("", result.Value!.FewShotExamples);
    }

    [Theory]
    [InlineData(HoroscopePeriod.Daily)]
    [InlineData(HoroscopePeriod.Weekly)]
    [InlineData(HoroscopePeriod.Monthly)]
    [InlineData(HoroscopePeriod.Yearly)]
    public void Create_WithAllPeriods_ReturnsSuccess(HoroscopePeriod period)
    {
        var result = HoroscopePromptTemplate.Create(
            period,
            "System prompt",
            "Examples",
            "User template");

        Assert.True(result.IsSuccess);
        Assert.Equal(period, result.Value!.Period);
    }
}

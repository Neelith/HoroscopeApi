namespace HoroscopeApi.Tests.Domain.Prompts;

public sealed class PromptTemplateCreateTests
{
    [Fact]
    public void Create_WithValidData_ReturnsSuccess()
    {
        Result<PromptTemplate> result = PromptTemplate.Create(
            PromptType.Daily,
            "You are an astrologer.",
            "Example 1: ...",
            "Generate a horoscope for {sign}.");

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(PromptType.Daily, result.Value!.Type);
        Assert.Equal("You are an astrologer.", result.Value.SystemPrompt);
        Assert.Equal("Generate a horoscope for {sign}.", result.Value.UserPromptTemplate);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptySystemPrompt_ReturnsFailure(string systemPrompt)
    {
        Result<PromptTemplate> result = PromptTemplate.Create(
            PromptType.Daily,
            systemPrompt,
            "examples",
            "user template");

        Assert.True(result.IsFailure);
        Assert.Equal("PromptTemplate.InvalidPromptTemplate", result.Errors[0].Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyUserPromptTemplate_ReturnsFailure(string userPromptTemplate)
    {
        Result<PromptTemplate> result = PromptTemplate.Create(
            PromptType.Weekly,
            "Valid system prompt",
            "examples",
            userPromptTemplate);

        Assert.True(result.IsFailure);
        Assert.Equal("PromptTemplate.InvalidPromptTemplate", result.Errors[0].Code);
    }

    [Fact]
    public void Create_WithEmptyFewShotExamples_ReturnsSuccess()
    {
        Result<PromptTemplate> result = PromptTemplate.Create(
            PromptType.Monthly,
            "System prompt",
            "",
            "User template");

        Assert.True(result.IsSuccess);
        Assert.Equal("", result.Value!.FewShotExamples);
    }

    [Theory]
    [InlineData(PromptType.Daily)]
    [InlineData(PromptType.Weekly)]
    [InlineData(PromptType.Monthly)]
    [InlineData(PromptType.Yearly)]
    [InlineData(PromptType.Compatibility)]
    public void Create_WithAllTypes_ReturnsSuccess(PromptType type)
    {
        Result<PromptTemplate> result = PromptTemplate.Create(
            type,
            "System prompt",
            "Examples",
            "User template");

        Assert.True(result.IsSuccess);
        Assert.Equal(type, result.Value!.Type);
    }
}
using HoroscopeApi.Application.Infrastructure.AI;
using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Infrastructure.AI;

public sealed class HoroscopePromptBuilder : IHoroscopePromptBuilder
{
    public List<ChatMessage> BuildMessage(DateOnly date, ZodiacSignInfo zodiacSignInfo)
    {
        var messages = new List<ChatMessage>();

        // System message with instructions
        messages.Add(new ChatMessage(
            "system",
            "You are an expert astrologer. Generate a daily horoscope prediction in JSON format that is positive, insightful, and meaningful. " +
            "Focus on providing guidance across general outlook, love, career, and health. " +
            "Include relevant keywords and lucky colors. Rate the overall mood on a scale of 1-10.\n\n" +
            "IMPORTANT: Return ONLY a single flat JSON object. Do NOT wrap it in an array. Do NOT use a 'horoscopes' key or any other wrapper.\n\n" +
            "Required JSON structure:\n" +
            "{\n" +
            "  \"sign\": \"aries\",\n" +
            "  \"general\": \"...\",\n" +
            "  \"love\": \"...\",\n" +
            "  \"career\": \"...\",\n" +
            "  \"health\": \"...\",\n" +
            "  \"keywords\": [\"word1\", \"word2\", \"word3\"],\n" +
            "  \"luckyColors\": [\"color1\", \"color2\"],\n" +
            "  \"moodScore\": 8\n" +
            "}"
        ));

        // Few-shot Example 1: Aries
        messages.Add(new ChatMessage(
            "user",
            "Generate daily horoscope prediction for aries for 2024-01-15.\n\n" +
            "Aries: Fire element, Cardinal quality, ruled by Mars. The Ram - bold, pioneering, and energetic.\n\n" +
            "Return a single JSON object (not an array)."
        ));

        messages.Add(new ChatMessage(
            "assistant",
            "{\n" +
            "  \"sign\": \"aries\",\n" +
            "  \"general\": \"Dynamic energy surrounds you today as Mars energizes your sign. You'll find yourself brimming with confidence and ready to tackle challenges head-on. This is an excellent time to initiate new projects or revisit goals you've set aside. Your natural leadership qualities shine brightly, attracting positive attention from colleagues and friends alike. However, remember to channel this powerful energy constructively rather than impulsively. The stars encourage you to balance your pioneering spirit with thoughtful consideration. By evening, you may discover an unexpected opportunity that aligns perfectly with your ambitions. Trust your instincts but don't rush into decisions without proper evaluation. Your enthusiasm is contagious today, inspiring those around you to take action alongside you.\",\n" +
            "  \"love\": \"Romance takes center stage as Venus casts a warm glow over your relationships. Single Aries may encounter someone intriguing through social gatherings or unexpected encounters. Those in committed partnerships will find deeper emotional connections through honest communication. Express your feelings openly, as your partner is receptive to vulnerability. Plan something spontaneous to reignite the spark. Your passionate nature is particularly magnetic today, drawing admiration from potential romantic interests.\",\n" +
            "  \"career\": \"Professional opportunities abound as your innovative ideas gain recognition. Supervisors notice your dedication and may present advancement possibilities. This is an ideal moment to pitch that project you've been developing. Collaboration with colleagues proves particularly fruitful, especially if you take the lead. Your problem-solving abilities are sharp, helping you navigate workplace challenges with ease. Financial prospects look promising, potentially through raises or new income streams.\",\n" +
            "  \"health\": \"Physical vitality runs high, making this perfect for starting new fitness routines or intensifying existing ones. Your energy reserves seem limitless, but avoid overexertion which could lead to burnout. Channel your dynamism into activities you genuinely enjoy rather than forcing yourself into rigid regimens. Mental clarity is equally strong, providing excellent conditions for meditation or mindfulness practices. Stay hydrated and ensure adequate rest despite your inclination to keep moving.\",\n" +
            "  \"keywords\": [\"energetic\", \"bold\", \"confident\"],\n" +
            "  \"luckyColors\": [\"red\", \"gold\"],\n" +
            "  \"moodScore\": 9\n" +
            "}"
        ));

        // Few-shot Example 2: Leo
        messages.Add(new ChatMessage(
            "user",
            "Generate daily horoscope prediction for leo for 2024-02-20.\n\n" +
            "Leo: Fire element, Fixed quality, ruled by the Sun. The Lion - charismatic, generous, and creative.\n\n" +
            "Return a single JSON object (not an array)."
        ));

        messages.Add(new ChatMessage(
            "assistant",
            "{\n" +
            "  \"sign\": \"leo\",\n" +
            "  \"general\": \"The Sun illuminates your path with confidence and charisma today. You're in your element, radiating warmth and attracting opportunities like a magnet. Creative projects flourish under your touch, and your natural leadership inspires others to follow your vision. This is an excellent time to step into the spotlight and showcase your talents. However, remember to balance your regal nature with genuine appreciation for others' contributions. Your generous spirit shines when you lift others up alongside yourself. By evening, you might find yourself at the center of social gatherings, where your presence brings joy and excitement to everyone around you.\",\n" +
            "  \"love\": \"Romantic passion ignites as you express your feelings with theatrical flair and genuine warmth. Single Leos attract admirers through their magnetic personality and confident self-expression. Those in relationships find joy in grand gestures and heartfelt declarations. Your partner appreciates the attention and affection you shower upon them. Plan something special that celebrates your connection. Your loyalty and devotion deepen bonds, creating lasting memories through shared adventures and passionate moments.\",\n" +
            "  \"career\": \"Leadership opportunities present themselves as your creative vision gains recognition from superiors. Your ability to inspire and motivate teams sets you apart in professional settings. Bold ideas receive enthusiastic support, making this an ideal time to propose innovative solutions. Collaborative projects benefit from your organizational skills and natural authority. Financial rewards may follow successful presentations or completed projects. Your confidence and competence open doors to advancement and recognition in your field.\",\n" +
            "  \"health\": \"Vitality flows through you, making physical activities particularly enjoyable. Heart-centered exercises like dancing or activities that bring joy support your wellbeing. Your energy levels remain high throughout the day, allowing you to accomplish much. However, remember to pace yourself and avoid burning out from overcommitment. Creative expression through movement or artistic pursuits nourishes both body and spirit. Maintain a balance between activity and rest to sustain your radiant energy.\",\n" +
            "  \"keywords\": [\"charismatic\", \"generous\", \"creative\"],\n" +
            "  \"luckyColors\": [\"gold\", \"orange\"],\n" +
            "  \"moodScore\": 9\n" +
            "}"
        ));

        // Final request for the specific sign
        messages.Add(new ChatMessage(
            "user",
            $"Generate daily horoscope prediction for {zodiacSignInfo.Name.ToLowerInvariant()} for {date:yyyy-MM-dd}.\n\n" +
            $"{zodiacSignInfo.Name}: {zodiacSignInfo.Element} element, {zodiacSignInfo.Quality} quality, ruled by {zodiacSignInfo.RulingPlanet}. {zodiacSignInfo.Description}\n\n" +
            "Return a single JSON object (not an array)."
        ));

        return messages;
    }
}

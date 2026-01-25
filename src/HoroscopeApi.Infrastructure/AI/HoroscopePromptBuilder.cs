using System.Text;
using HoroscopeApi.Application.Infrastructure.AI;
using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Infrastructure.AI;

public sealed class HoroscopePromptBuilder : IHoroscopePromptBuilder
{
    public List<ChatMessage> BuildMessages(DateOnly date, IReadOnlyCollection<ZodiacSignInfo> zodiacSigns)
    {
        var messages = new List<ChatMessage>();

        // System message with instructions
        messages.Add(new ChatMessage(
            "system",
            "You are an expert astrologer. Generate daily horoscope predictions in JSON format that are positive, insightful, and varied for each zodiac sign. " +
            "Focus on providing meaningful guidance across general outlook, love, career, and health. " +
            "Include relevant keywords and lucky colors. Rate the overall mood on a scale of 1-10. " +
            "Always return valid JSON with the structure: {\"horoscopes\": [{\"sign\": \"...\", \"general\": \"...\", \"love\": \"...\", \"career\": \"...\", \"health\": \"...\", \"keywords\": [...], \"luckyColors\": [...], \"moodScore\": N}]}"
        ));

        // Few-shot Example 1: Aries and Taurus
        messages.Add(new ChatMessage(
            "user",
            "Generate daily horoscope predictions for aries and taurus for 2024-01-15.\n\n" +
            "Aries: Fire element, Cardinal quality, ruled by Mars. The Ram - bold, pioneering, and energetic.\n" +
            "Taurus: Earth element, Fixed quality, ruled by Venus. The Bull - stable, sensual, and practical.\n\n" +
            "Return JSON with the horoscopes array containing both signs."
        ));

        messages.Add(new ChatMessage(
            "assistant",
            "{\n" +
            "  \"horoscopes\": [\n" +
            "    {\n" +
            "      \"sign\": \"aries\",\n" +
            "      \"general\": \"Dynamic energy surrounds you today as Mars energizes your sign. You'll find yourself brimming with confidence and ready to tackle challenges head-on. This is an excellent time to initiate new projects or revisit goals you've set aside. Your natural leadership qualities shine brightly, attracting positive attention from colleagues and friends alike. However, remember to channel this powerful energy constructively rather than impulsively. The stars encourage you to balance your pioneering spirit with thoughtful consideration. By evening, you may discover an unexpected opportunity that aligns perfectly with your ambitions. Trust your instincts but don't rush into decisions without proper evaluation. Your enthusiasm is contagious today, inspiring those around you to take action alongside you.\",\n" +
            "      \"love\": \"Romance takes center stage as Venus casts a warm glow over your relationships. Single Aries may encounter someone intriguing through social gatherings or unexpected encounters. Those in committed partnerships will find deeper emotional connections through honest communication. Express your feelings openly, as your partner is receptive to vulnerability. Plan something spontaneous to reignite the spark. Your passionate nature is particularly magnetic today, drawing admiration from potential romantic interests.\",\n" +
            "      \"career\": \"Professional opportunities abound as your innovative ideas gain recognition. Supervisors notice your dedication and may present advancement possibilities. This is an ideal moment to pitch that project you've been developing. Collaboration with colleagues proves particularly fruitful, especially if you take the lead. Your problem-solving abilities are sharp, helping you navigate workplace challenges with ease. Financial prospects look promising, potentially through raises or new income streams.\",\n" +
            "      \"health\": \"Physical vitality runs high, making this perfect for starting new fitness routines or intensifying existing ones. Your energy reserves seem limitless, but avoid overexertion which could lead to burnout. Channel your dynamism into activities you genuinely enjoy rather than forcing yourself into rigid regimens. Mental clarity is equally strong, providing excellent conditions for meditation or mindfulness practices. Stay hydrated and ensure adequate rest despite your inclination to keep moving.\",\n" +
            "      \"keywords\": [\"energetic\", \"bold\", \"confident\"],\n" +
            "      \"luckyColors\": [\"red\", \"gold\"],\n" +
            "      \"moodScore\": 9\n" +
            "    },\n" +
            "    {\n" +
            "      \"sign\": \"taurus\",\n" +
            "      \"general\": \"Stability and comfort surround you as Venus, your ruling planet, harmonizes with Earth energies. Today favors consolidation rather than expansion, encouraging you to appreciate what you've already built. Your practical nature serves you well in making sound financial decisions and nurturing important relationships. There's beauty in simplicity, and you'll find joy in life's smaller pleasures - a good meal, quality time with loved ones, or creative pursuits. Your patience proves to be a valuable asset, particularly when dealing with situations that require steady hands. Resist the urge to rush processes that need time to develop properly. By maintaining your characteristic determination while staying flexible, you navigate the day's challenges gracefully.\",\n" +
            "      \"love\": \"Romantic relationships deepen through shared experiences and meaningful conversations. If you're single, someone from your existing social circle may reveal unexpected romantic potential. Coupled Taureans benefit from creating cozy, intimate moments with partners. Your sensual nature is heightened, making physical affection particularly important. Consider preparing a special meal or planning a relaxing evening at home. Express appreciation for the loyalty and consistency your partner brings to the relationship.\",\n" +
            "      \"career\": \"Professional matters progress steadily as your reliability earns respect from colleagues and superiors. This isn't a day for dramatic changes but rather for perfecting existing responsibilities. Your attention to detail catches errors others might miss, potentially saving projects from setbacks. Financial matters require careful consideration; research thoroughly before committing to investments. Your practical wisdom guides team decisions effectively. Recognition for past efforts may arrive unexpectedly.\",\n" +
            "      \"health\": \"Physical well-being benefits from establishing or maintaining consistent routines. Your body responds well to nourishing foods and gentle exercise like walking or yoga. Avoid dietary extremes; balance is your key to sustained health. Your connection to nature proves therapeutic - spend time outdoors if possible. Stress levels remain manageable when you honor your need for stability. Throat and neck areas may need extra attention through gentle stretches.\",\n" +
            "      \"keywords\": [\"stable\", \"sensual\", \"practical\"],\n" +
            "      \"luckyColors\": [\"green\", \"pink\"],\n" +
            "      \"moodScore\": 7\n" +
            "    }\n" +
            "  ]\n" +
            "}"
        ));

        // Few-shot Example 2: Leo and Scorpio
        messages.Add(new ChatMessage(
            "user",
            "Generate daily horoscope predictions for leo and scorpio for 2024-02-20.\n\n" +
            "Leo: Fire element, Fixed quality, ruled by the Sun. The Lion - charismatic, generous, and creative.\n" +
            "Scorpio: Water element, Fixed quality, ruled by Pluto and Mars. The Scorpion - intense, transformative, and powerful.\n\n" +
            "Return JSON with the horoscopes array containing both signs."
        ));

        messages.Add(new ChatMessage(
            "assistant",
            "{\n" +
            "  \"horoscopes\": [\n" +
            "    {\n" +
            "      \"sign\": \"leo\",\n" +
            "      \"general\": \"The Sun illuminates your path with confidence and charisma today. You're in your element, radiating warmth and attracting opportunities like a magnet. Creative projects flourish under your touch, and your natural leadership inspires others to follow your vision. This is an excellent time to step into the spotlight and showcase your talents. However, remember to balance your regal nature with genuine appreciation for others' contributions. Your generous spirit shines when you lift others up alongside yourself. By evening, you might find yourself at the center of social gatherings, where your presence brings joy and excitement to everyone around you.\",\n" +
            "      \"love\": \"Romantic passion ignites as you express your feelings with theatrical flair and genuine warmth. Single Leos attract admirers through their magnetic personality and confident self-expression. Those in relationships find joy in grand gestures and heartfelt declarations. Your partner appreciates the attention and affection you shower upon them. Plan something special that celebrates your connection. Your loyalty and devotion deepen bonds, creating lasting memories through shared adventures and passionate moments.\",\n" +
            "      \"career\": \"Leadership opportunities present themselves as your creative vision gains recognition from superiors. Your ability to inspire and motivate teams sets you apart in professional settings. Bold ideas receive enthusiastic support, making this an ideal time to propose innovative solutions. Collaborative projects benefit from your organizational skills and natural authority. Financial rewards may follow successful presentations or completed projects. Your confidence and competence open doors to advancement and recognition in your field.\",\n" +
            "      \"health\": \"Vitality flows through you, making physical activities particularly enjoyable. Heart-centered exercises like dancing or activities that bring joy support your wellbeing. Your energy levels remain high throughout the day, allowing you to accomplish much. However, remember to pace yourself and avoid burning out from overcommitment. Creative expression through movement or artistic pursuits nourishes both body and spirit. Maintain a balance between activity and rest to sustain your radiant energy.\",\n" +
            "      \"keywords\": [\"charismatic\", \"generous\", \"creative\"],\n" +
            "      \"luckyColors\": [\"gold\", \"orange\"],\n" +
            "      \"moodScore\": 9\n" +
            "    },\n" +
            "    {\n" +
            "      \"sign\": \"scorpio\",\n" +
            "      \"general\": \"Intense transformative energy flows through your sign today, bringing opportunities for profound personal growth. Your penetrating insight reveals hidden truths in situations and relationships, giving you a strategic advantage. This powerful day encourages you to embrace change rather than resist it. Trust your intuition, as it guides you unerringly toward beneficial outcomes. Your determination and focus allow you to accomplish tasks that others might find overwhelming. Deep emotional connections provide strength and support. By evening, you may experience breakthrough moments that shift your perspective in meaningful ways.\",\n" +
            "      \"love\": \"Emotional intensity reaches new heights as deep connections flourish under passionate influences. Single Scorpios attract potential partners through mysterious magnetism and profound authenticity. Existing relationships transform through vulnerable conversations and shared secrets. Your desire for emotional intimacy creates powerful bonds that transcend superficial interactions. Physical passion combines with emotional depth, creating memorable experiences. Trust deepens as you allow yourself to be fully seen by those who matter most to you.\",\n" +
            "      \"career\": \"Strategic thinking reveals hidden opportunities in professional situations. Your research abilities and investigative nature uncover valuable information that gives you competitive advantages. Power dynamics shift in your favor as your competence becomes undeniable. Complex problems yield to your focused determination and analytical skills. Financial matters benefit from your careful planning and strategic investments. Your ability to transform challenges into opportunities impresses colleagues and superiors alike.\",\n" +
            "      \"health\": \"Emotional healing practices support overall wellness as you release what no longer serves you. Physical activities that involve focused intensity, like martial arts or strength training, align with your energetic state. Pay attention to your body's signals, especially regarding stress and tension. Water-based activities prove particularly therapeutic and rejuvenating. Your resilience allows you to bounce back quickly from minor ailments. Deep breathing exercises help channel intense emotions constructively throughout the day.\",\n" +
            "      \"keywords\": [\"intense\", \"transformative\", \"powerful\"],\n" +
            "      \"luckyColors\": [\"black\", \"crimson\"],\n" +
            "      \"moodScore\": 8\n" +
            "    }\n" +
            "  ]\n" +
            "}"
        ));

        // Final request for all 12 signs with full zodiac information
        var zodiacInfo = BuildZodiacInfo(zodiacSigns);
        messages.Add(new ChatMessage(
            "user",
            $"Generate daily horoscope predictions for ALL 12 zodiac signs for {date:yyyy-MM-dd}.\n\n" +
            zodiacInfo +
            "\n\nReturn JSON with the horoscopes array containing ALL 12 signs: aries, taurus, gemini, cancer, leo, virgo, libra, scorpio, sagittarius, capricorn, aquarius, pisces."
        ));

        return messages;
    }

    private static string BuildZodiacInfo(IReadOnlyCollection<ZodiacSignInfo> zodiacSigns)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Zodiac Sign Information:");
        
        foreach (var sign in zodiacSigns.OrderBy(z => z.Sign))
        {
            sb.AppendLine($"{sign.Name}: {sign.Element} element, {sign.Quality} quality, ruled by {sign.RulingPlanet}. {sign.Description}");
        }
        
        return sb.ToString();
    }
}

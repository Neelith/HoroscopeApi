using HoroscopeApi.Application.Infrastructure.AI;
using HoroscopeApi.Domain.ZodiacSigns;

namespace HoroscopeApi.Infrastructure.AI;

public sealed class HoroscopePromptBuilder : IHoroscopePromptBuilder
{
    public List<ChatMessage> BuildMessage(DateOnly date, ZodiacSignInfo zodiacSignInfo)
    {
        List<ChatMessage> messages = new();

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

    public List<ChatMessage> BuildWeeklyMessage(DateOnly date, ZodiacSignInfo zodiacSignInfo)
    {
        List<ChatMessage> messages = new();

        // System message with instructions
        messages.Add(new ChatMessage(
            "system",
            "You are an expert astrologer. Generate a weekly horoscope prediction in JSON format that is positive, insightful, and meaningful. " +
            "Focus on providing guidance across general outlook, love, career, and health for the entire week. " +
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

        // Few-shot Example: Aries
        messages.Add(new ChatMessage(
            "user",
            "Generate weekly horoscope prediction for aries for the week starting 2024-01-15.\n\n" +
            "Aries: Fire element, Cardinal quality, ruled by Mars. The Ram - bold, pioneering, and energetic.\n\n" +
            "Return a single JSON object (not an array)."
        ));

        messages.Add(new ChatMessage(
            "assistant",
            "{\n" +
            "  \"sign\": \"aries\",\n" +
            "  \"general\": \"This week brings dynamic energy as Mars amplifies your natural drive and determination. Monday and Tuesday are perfect for launching new initiatives or tackling challenging projects that require courage and innovation. Midweek brings unexpected opportunities through networking and social connections - keep your schedule flexible to accommodate spontaneous meetings. Thursday and Friday favor collaborative efforts where your leadership naturally emerges. The weekend offers a chance to reflect on your progress while recharging your batteries. Throughout the week, your confidence attracts positive attention, but remember to balance assertiveness with diplomacy. By Sunday, you'll have made significant strides toward your goals while strengthening key relationships.\",\n" +
            "  \"love\": \"Romance blooms early in the week with passionate encounters and meaningful conversations. Single Aries should be especially social on Tuesday and Wednesday when romantic prospects are highest. Those in relationships benefit from planning a special midweek date that reignites the spark. Weekend energy supports deeper emotional bonding and honest sharing of feelings. Your magnetic charm is particularly strong, making this an excellent week to express your desires openly. Friday brings an opportunity to resolve any lingering relationship tensions through direct but compassionate communication.\",\n" +
            "  \"career\": \"Professional momentum builds throughout the week, with Monday being ideal for pitching innovative ideas to decision-makers. Collaborative projects gain traction midweek, and your problem-solving abilities shine in team settings. Thursday may bring recognition for past accomplishments or news about advancement opportunities. Financial discussions toward the end of the week could lead to improved compensation or new income streams. Trust your instincts when evaluating business proposals. The weekend is perfect for strategic planning and setting goals for the coming weeks.\",\n" +
            "  \"health\": \"Physical energy runs high, making this an excellent week to intensify your fitness routine or try new athletic activities. Monday through Wednesday are ideal for vigorous exercise, while Thursday and Friday favor gentler movement like yoga or stretching. Mental clarity peaks midweek, supporting meditation or mindfulness practices. Ensure adequate rest despite your desire to stay constantly active - your body needs recovery time. Stay hydrated and maintain balanced nutrition to sustain your high energy levels. The weekend calls for relaxation and self-care to prepare for the week ahead.\",\n" +
            "  \"keywords\": [\"dynamic\", \"courageous\", \"progressive\"],\n" +
            "  \"luckyColors\": [\"red\", \"scarlet\"],\n" +
            "  \"moodScore\": 8\n" +
            "}"
        ));

        // Final request for the specific sign
        messages.Add(new ChatMessage(
            "user",
            $"Generate weekly horoscope prediction for {zodiacSignInfo.Name.ToLowerInvariant()} for the week starting {date:yyyy-MM-dd}.\n\n" +
            $"{zodiacSignInfo.Name}: {zodiacSignInfo.Element} element, {zodiacSignInfo.Quality} quality, ruled by {zodiacSignInfo.RulingPlanet}. {zodiacSignInfo.Description}\n\n" +
            "Return a single JSON object (not an array)."
        ));

        return messages;
    }

    public List<ChatMessage> BuildMonthlyMessage(DateOnly date, ZodiacSignInfo zodiacSignInfo)
    {
        List<ChatMessage> messages = new();

        // System message with instructions
        messages.Add(new ChatMessage(
            "system",
            "You are an expert astrologer. Generate a monthly horoscope prediction in JSON format that is positive, insightful, and meaningful. " +
            "Focus on providing guidance across general outlook, love, career, and health for the entire month. " +
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

        // Few-shot Example: Aries
        messages.Add(new ChatMessage(
            "user",
            "Generate monthly horoscope prediction for aries for January 2024.\n\n" +
            "Aries: Fire element, Cardinal quality, ruled by Mars. The Ram - bold, pioneering, and energetic.\n\n" +
            "Return a single JSON object (not an array)."
        ));

        messages.Add(new ChatMessage(
            "assistant",
            "{\n" +
            "  \"sign\": \"aries\",\n" +
            "  \"general\": \"January 2024 marks a powerful month of new beginnings and bold initiatives for Aries. The first two weeks bring exceptional clarity about your goals and the courage to pursue them fearlessly. Mars energizes your ambitions throughout the month, making this ideal for launching projects you've been planning. Mid-month planetary alignments support networking and collaborative ventures that expand your influence. The third week may present unexpected challenges, but your natural problem-solving abilities turn obstacles into opportunities. As the month closes, you'll find yourself in a stronger position than when it began, with momentum building for continued success. Overall, January sets a dynamic tone for your entire year, establishing patterns of growth and achievement.\",\n" +
            "  \"love\": \"Romance flourishes this January, particularly during the first and last weeks of the month. Single Aries encounter intriguing prospects through social events or while pursuing personal passions around mid-month. Those in relationships experience renewed passion and deeper emotional connection, especially after the 15th when communication flows more freely. Plan meaningful experiences together to strengthen your bond. The month favors honest conversations about relationship goals and future plans. Your magnetic energy attracts admiration throughout January, but focus on authentic connections rather than superficial attractions. By month's end, you'll have greater clarity about what you seek in partnership, whether deepening current relationships or opening yourself to new romantic possibilities.\",\n" +
            "  \"career\": \"Professional opportunities abound in January, with the first week being particularly auspicious for pitching innovative ideas or requesting advancement. Your leadership qualities gain recognition from superiors, potentially leading to increased responsibilities or promotions around mid-month. Collaborative projects initiated now have excellent long-term potential. Financial prospects improve steadily, with possible raises or new income streams emerging in the latter half of the month. Networking events prove especially valuable around the 20th. Any career risks taken with proper planning tend to pay off handsomely. End the month by strategizing for the coming months - your January momentum can carry you far into the year.\",\n" +
            "  \"health\": \"Physical vitality runs strong throughout January, making this an excellent month to establish new fitness routines or intensify existing ones. The first two weeks are ideal for starting ambitious wellness programs. Energy levels remain high, but avoid overexertion around mid-month when your enthusiasm might exceed your limits. Mental clarity supports meditation and mindfulness practices, particularly during the third week. Ensure adequate rest and recovery time despite your drive to stay constantly active. Balanced nutrition and proper hydration are essential to maintain your high performance level. By month's end, you'll have established sustainable health habits that serve you throughout the year, feeling energized and prepared for upcoming challenges.\",\n" +
            "  \"keywords\": [\"initiative\", \"momentum\", \"vitality\"],\n" +
            "  \"luckyColors\": [\"crimson\", \"gold\"],\n" +
            "  \"moodScore\": 9\n" +
            "}"
        ));

        // Final request for the specific sign
        string monthName = date.ToString("MMMM");
        int year = date.Year;
        messages.Add(new ChatMessage(
            "user",
            $"Generate monthly horoscope prediction for {zodiacSignInfo.Name.ToLowerInvariant()} for {monthName} {year}.\n\n" +
            $"{zodiacSignInfo.Name}: {zodiacSignInfo.Element} element, {zodiacSignInfo.Quality} quality, ruled by {zodiacSignInfo.RulingPlanet}. {zodiacSignInfo.Description}\n\n" +
            "Return a single JSON object (not an array)."
        ));

        return messages;
    }

    public List<ChatMessage> BuildYearlyMessage(int year, ZodiacSignInfo zodiacSignInfo)
    {
        List<ChatMessage> messages = new();

        // System message with instructions
        messages.Add(new ChatMessage(
            "system",
            "You are an expert astrologer. Generate a yearly horoscope prediction in JSON format that is positive, insightful, and meaningful. " +
            "Focus on providing broad guidance across general outlook, love, career, and health for the entire year. " +
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
            "Generate yearly horoscope prediction for aries for 2024.\n\n" +
            "Aries: Fire element, Cardinal quality, ruled by Mars. The Ram - bold, pioneering, and energetic.\n\n" +
            "Return a single JSON object (not an array)."
        ));

        messages.Add(new ChatMessage(
            "assistant",
            "{\n" +
            "  \"sign\": \"aries\",\n" +
            "  \"general\": \"2024 brings transformative energy for Aries, marking a year of significant personal growth and new beginnings. The first quarter emphasizes career advancement and professional recognition, while summer months offer opportunities for travel and expanding your horizons. Autumn presents chances to deepen relationships and forge meaningful connections. Throughout the year, your natural leadership qualities will be called upon, and you'll find yourself in positions where your pioneering spirit can truly shine. Planetary alignments suggest that risks taken with careful consideration will pay off handsomely. The year closes with renewed confidence and clarity about your path forward, setting the stage for even greater achievements ahead.\",\n" +
            "  \"love\": \"Romance flourishes throughout 2024, with particularly auspicious periods during spring and late autumn. Single Aries may encounter a significant connection around March or November, potentially through professional networks or while pursuing personal passions. Existing relationships deepen through shared adventures and honest communication. The middle of the year tests partnerships, but those willing to grow together emerge stronger. Your passionate nature attracts admirers, but focus on quality over quantity. By year's end, you'll have a clearer vision of what you truly seek in partnership, whether that's commitment with a current partner or readiness for new romantic possibilities.\",\n" +
            "  \"career\": \"Professional success defines 2024 for Aries, with major opportunities emerging in the first and third quarters. Leadership roles or independent ventures gain momentum, and your innovative ideas receive recognition from influential figures. Financial growth accompanies career advancement, though wise budgeting in summer months proves beneficial. Collaborations formed this year could extend beyond 2024, creating lasting professional networks. Midyear may bring a pivotal decision about your career direction - trust your instincts. By December, you'll have established yourself as a force to be reckoned with in your field, with exciting prospects lined up for the coming year.\",\n" +
            "  \"health\": \"Physical vitality runs strong throughout 2024, though mindful attention to work-life balance proves essential. The year favors establishing sustainable fitness routines rather than intense short-term programs. Spring is ideal for starting new wellness practices, while autumn calls for rest and recuperation. Mental health benefits from creative outlets and outdoor activities. Watch for stress accumulation during career-intensive periods, particularly in summer. Regular movement, adequate sleep, and stress management techniques keep you operating at peak performance. The year ends with you feeling energized and prepared for future challenges, having learned to honor your body's needs alongside your ambitious drive.\",\n" +
            "  \"keywords\": [\"transformation\", \"leadership\", \"growth\"],\n" +
            "  \"luckyColors\": [\"crimson\", \"gold\"],\n" +
            "  \"moodScore\": 9\n" +
            "}"
        ));

        // Few-shot Example 2: Leo
        messages.Add(new ChatMessage(
            "user",
            "Generate yearly horoscope prediction for leo for 2025.\n\n" +
            "Leo: Fire element, Fixed quality, ruled by the Sun. The Lion - charismatic, generous, and creative.\n\n" +
            "Return a single JSON object (not an array)."
        ));

        messages.Add(new ChatMessage(
            "assistant",
            "{\n" +
            "  \"sign\": \"leo\",\n" +
            "  \"general\": \"2025 shines brightly for Leo, emphasizing creative expression and personal fulfillment. The year begins with renewed confidence and clarity about your life's direction. Spring brings opportunities to showcase your talents on larger platforms, while summer months deepen your understanding of what truly matters. Autumn presents chances for meaningful collaborations that align with your values. Your natural charisma attracts influential supporters throughout the year, opening doors you hadn't previously considered. The latter half of 2025 focuses on consolidating gains and building sustainable foundations for long-term success. By year's end, you'll have transformed aspirations into tangible achievements, standing proud of how far you've come.\",\n" +
            "  \"love\": \"Love takes center stage in 2025, with romance flourishing in unexpected ways. Single Leos may experience a profound connection during spring or early autumn, possibly with someone from a different background or culture. Existing relationships benefit from renewed passion and deeper emotional intimacy. The middle months test your ability to balance personal needs with partnership demands, but communication breakthroughs around August strengthen bonds. Your generous heart attracts genuine affection, though discernment helps you distinguish flattery from authentic admiration. Creative dates and grand gestures keep romance alive. The year concludes with you feeling loved and appreciated, whether in a committed partnership or confidently embracing single life while open to possibilities.\",\n" +
            "  \"career\": \"Professional achievements reach new heights in 2025, with recognition for your creative contributions and leadership abilities. The first quarter brings opportunities to take on more visible roles or launch passion projects. Financial rewards follow your efforts, though midyear requires strategic thinking about long-term career direction. Collaborations with equally talented individuals produce exceptional results, particularly in creative or leadership-focused fields. Your ability to inspire others opens mentorship opportunities that benefit both you and emerging talent. Autumn may present a significant career milestone or achievement that validates years of hard work. December finds you well-positioned for continued success, with exciting projects lined up for the coming year.\",\n" +
            "  \"health\": \"Vitality and wellness flourish in 2025 when you prioritize joy alongside discipline. The year favors activities that bring pleasure - dancing, creative movement, or social sports that combine fitness with fun. Heart health deserves attention, both physical and emotional. Spring is ideal for establishing wellness routines that you genuinely enjoy, ensuring long-term adherence. Summer calls for balance between activity and rest, while autumn emphasizes stress management through creative outlets. Your natural exuberance serves you well, though learning to pace yourself prevents burnout. By year's end, you've discovered wellness approaches that honor your need for both vitality and pleasure, feeling radiant inside and out.\",\n" +
            "  \"keywords\": [\"radiance\", \"achievement\", \"passion\"],\n" +
            "  \"luckyColors\": [\"gold\", \"royal purple\"],\n" +
            "  \"moodScore\": 9\n" +
            "}"
        ));

        // Final request for the specific sign
        messages.Add(new ChatMessage(
            "user",
            $"Generate yearly horoscope prediction for {zodiacSignInfo.Name.ToLowerInvariant()} for {year}.\n\n" +
            $"{zodiacSignInfo.Name}: {zodiacSignInfo.Element} element, {zodiacSignInfo.Quality} quality, ruled by {zodiacSignInfo.RulingPlanet}. {zodiacSignInfo.Description}\n\n" +
            "Return a single JSON object (not an array)."
        ));

        return messages;
    }
}
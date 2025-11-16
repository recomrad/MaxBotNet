// 📁 [TextFormat] - Формат текста в Max Messenger
// 🎯 Core function: Перечисление форматов текста (markdown, html)
// 🔗 Key dependencies: System.Text.Json.Serialization
// 💡 Usage: Используется в моделях запросов для форматирования текста

using System.Text.Json.Serialization;

namespace Max.Bot.Types.Enums;

/// <summary>
/// Represents the text format for message content.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TextFormat
{
    /// <summary>
    /// Markdown formatting.
    /// </summary>
    Markdown,

    /// <summary>
    /// HTML formatting.
    /// </summary>
    Html
}


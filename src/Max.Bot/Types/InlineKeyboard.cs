using System.Text.Json.Serialization;

namespace Max.Bot.Types;

/// <summary>
/// Represents an inline keyboard with buttons arranged in rows.
/// </summary>
public class InlineKeyboard
{
    /// <summary>
    /// Gets or sets the buttons arranged in rows.
    /// Each inner array represents a row of buttons.
    /// </summary>
    /// <value>An array of button rows, where each row is an array of buttons.</value>
    [JsonPropertyName("buttons")]
    public InlineKeyboardButton[][] Buttons
    {
        get => _buttons;
        set
        {
            if (value == null)
            {
                _buttons = Array.Empty<InlineKeyboardButton[]>();
                return;
            }

            var sanitized = new InlineKeyboardButton[value.Length][];
            for (var i = 0; i < value.Length; i++)
            {
                sanitized[i] = value[i] ?? Array.Empty<InlineKeyboardButton>();
            }

            _buttons = sanitized;
        }
    }

    private InlineKeyboardButton[][] _buttons = Array.Empty<InlineKeyboardButton[]>();

    /// <summary>
    /// Initializes a new instance of the <see cref="InlineKeyboard"/> class.
    /// </summary>
    public InlineKeyboard()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InlineKeyboard"/> class with the specified buttons.
    /// </summary>
    /// <param name="buttons">The buttons arranged in rows.</param>
    public InlineKeyboard(InlineKeyboardButton[][] buttons)
    {
        Buttons = buttons ?? throw new ArgumentNullException(nameof(buttons));
    }
}


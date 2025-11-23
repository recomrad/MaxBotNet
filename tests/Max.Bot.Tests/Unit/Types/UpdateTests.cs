using FluentAssertions;
using Max.Bot.Networking;
using Max.Bot.Types;
using Max.Bot.Types.Enums;
using Xunit;

namespace Max.Bot.Tests.Unit.Types;

public class UpdateTests
{
    [Fact]
    public void Deserialize_ShouldDeserializeUpdateWithMessage()
    {
        // Arrange
        var json = """{"update_id":1,"update_type":"message_created","message":{"id":123,"chat":{"id":456,"type":"private"},"from":{"user_id":789,"username":"testuser","is_bot":false},"text":"Hello","date":1609459200}}""";

        // Act
        var result = MaxJsonSerializer.Deserialize<Update>(json);

        // Assert
        result.Should().NotBeNull();
        result!.UpdateId.Should().Be(1);
        result.Type.Should().Be(UpdateType.Message);
        result.Message.Should().NotBeNull();
        result.Message!.Id.Should().Be(123);
        result.Message.Text.Should().Be("Hello");
    }

    [Fact]
    public void Deserialize_ShouldDeserializeUpdateWithCallbackQuery()
    {
        // Arrange
        var json = """{"update_id":2,"update_type":"message_callback","callback_query":{"id":"callback123","from":{"user_id":123,"username":"user123","is_bot":false},"data":"callbackData123"}}""";

        // Act
        var result = MaxJsonSerializer.Deserialize<Update>(json);

        // Assert
        result.Should().NotBeNull();
        result!.UpdateId.Should().Be(2);
        result.Type.Should().Be(UpdateType.CallbackQuery);
        result.CallbackQuery.Should().NotBeNull();
        result.CallbackQuery!.Id.Should().Be("callback123");
        result.CallbackQuery.From.Id.Should().Be(123);
        result.CallbackQuery.Data.Should().Be("callbackData123");
    }

    [Fact]
    public void Serialize_ShouldSerializeUpdate()
    {
        // Arrange
        var update = new Update
        {
            UpdateId = 1,
            UpdateTypeRaw = "message_created",
            Message = new Message
            {
                Id = 123,
                Text = "Hello",
                Date = new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        };

        // Act
        var json = MaxJsonSerializer.Serialize(update);

        // Assert
        json.Should().NotBeNullOrEmpty();
        json.Should().Contain("\"update_id\":1");
        json.Should().Contain("\"update_type\":\"message_created\"");
        json.Should().Contain("\"message\"");
        json.Should().Contain("\"id\":123");
    }

    [Fact]
    public void Serialize_ShouldSerializeUpdateWithCallbackQuery()
    {
        // Arrange
        var update = new Update
        {
            UpdateId = 2,
            UpdateTypeRaw = "message_callback",
            CallbackQuery = new CallbackQuery
            {
                Id = "callback123",
                From = new User { Id = 123, Username = "user123", IsBot = false },
                Data = "callbackData123"
            }
        };

        // Act
        var json = MaxJsonSerializer.Serialize(update);

        // Assert
        json.Should().NotBeNullOrEmpty();
        json.Should().Contain("\"update_id\":2");
        json.Should().Contain("\"update_type\":\"message_callback\"");
        json.Should().Contain("\"callback_query\"");
        json.Should().Contain("\"id\":\"callback123\"");
    }

    [Fact]
    public void Serialize_ShouldNotIncludeNullMessage()
    {
        // Arrange
        var update = new Update
        {
            UpdateId = 2,
            UpdateTypeRaw = "message_callback",
            Message = null,
            CallbackQuery = null
        };

        // Act
        var json = MaxJsonSerializer.Serialize(update);

        // Assert
        json.Should().NotBeNullOrEmpty();
        json.Should().Contain("\"update_id\":2");
        json.Should().Contain("\"update_type\":\"message_callback\"");
        json.Should().NotContain("\"message\"");
        json.Should().NotContain("\"callback_query\"");
    }

    [Fact]
    public void Deserialize_ShouldDeserializeWebhookUpdateWithoutMessageId()
    {
        // Arrange - формат вебхука без message.id, только body.mid
        var json = """{"message":{"recipient":{"chat_id":79313411,"chat_type":"dialog","user_id":94399782},"timestamp":1763928007254,"body":{"mid":"mid.0000000004ba3a03019ab24d62566a52","seq":115600785883425360,"text":"/start"},"sender":{"user_id":18503461,"first_name":"Александр","last_name":"Сюзев","is_bot":false,"last_activity_time":1763927992000,"name":"Александр Сюзев"}},"timestamp":1763928007254,"user_locale":"ru","update_type":"message_created"}""";

        // Act
        var result = MaxJsonSerializer.Deserialize<Update>(json);

        // Assert
        result.Should().NotBeNull();
        result!.UpdateTypeRaw.Should().Be("message_created");
        result.Type.Should().Be(UpdateType.Message);
        result.Message.Should().NotBeNull();
        result.Message!.Id.Should().BeNull(); // ID отсутствует в JSON
        result.Message.Body.Should().NotBeNull();
        result.Message.Body!.Mid.Should().Be("mid.0000000004ba3a03019ab24d62566a52");
        result.Message.Body.Text.Should().Be("/start");
        result.Message.Sender.Should().NotBeNull();
        result.Message.Sender!.Id.Should().Be(18503461);
        result.Message.Recipient.Should().NotBeNull();
        result.Message.Recipient!.ChatId.Should().Be(79313411);
    }
}


# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.5.1-alpha] - 2026-03-09

### Changed

- **Chore**: Добавлен `.pi/` в `.gitignore` для исключения файлов Pi Coding Agent из репозитория.

---

## [0.5.0-alpha] - 2026-03-09

### Added

- **`FileUploadResult`**: Новая модель для результата загрузки файла, содержащая `Token`, `FileId`, `PhotoId` и `Photos`.
- **`ContactInfo.PhoneNumber`**: Добавлено свойство для прямого доступа к номеру телефона контакта.
- **`ContactInfo.FullName`**: Добавлено свойство для полного имени контакта.
- **`ContactInfo.Name`**: Добавлено свойство-алиас для совместимости с разными форматами API.
- **`ContactAttachment.PhoneNumber`**: Convenience property для получения телефона из контакта (приоритет: `MaxInfo.PhoneNumber` → парсинг `VcfInfo`).
- **`ContactAttachment.FullName`**: Convenience property для получения имени из контакта (приоритет: `MaxInfo.FullName` → парсинг `VcfInfo`).
- **`ContactHelpers`**: Новый helper-класс с методами для работы с контактами:
  - `ContactHelpers.ParseVcf(string? vcfInfo)` — парсит vCard и извлекает телефон, имя, фамилию.
  - `ContactHelpers.GetPhoneNumber(Contact? contact)` — получает телефон из контакта.
  - `ContactHelpers.GetFullName(Contact? contact)` — получает полное имя из контакта.
- **Пример `ContactBotSample`**: Добавлен пример бота для обработки контактов.
- **`LocationAttachment`**: Поддержка вложений местоположения (широта/долгота).
- **`ImageAttachment`**: Поддержка вложений изображений с множественной отправкой фото.
- **`UpdateTypeJsonConverter`**: Кастомный JSON конвертер для enum `UpdateType`.
- **`UpdateTypeHelper`**: Helper-класс для конвертации `UpdateType` между string и enum.
- **`IMaxHttpClient.Dispose()`**: Реализация `IDisposable` для корректного освобождения ресурсов HTTP клиента.

### Fixed

- **`UploadFileDataAsync` / `UploadFileResumableAsync`**: Теперь возвращают `FileUploadResult` вместо `object`. Это позволяет легко получать токен или ID файла после загрузки без ручного парсинга JSON.
- **Контакты (ContactAttachment)**: Теперь телефон и имя контакта можно получить напрямую через `contactAttachment.PhoneNumber` и `contactAttachment.FullName` без необходимости ручного парсинга vCard.
- Исправлена проблема, при которой данные vCard (особенно номер телефона) были недоступны в удобном виде в обработчике update.
- Исправлена десериализация vCard для сложных случаев (параметры, folded lines, экранирование).
- **`BaseApi.ExecuteRequestAsync<T>()`**: Исправлена обработка ошибок при пустом response body — теперь генерируется `MaxNetworkException` с корректным сообщением.
- **`MaxHttpClient.HandleHttpResponseError()`**: Исправлена логика определения типа исключения по статус коду:
  - `HttpStatusCode.RequestTimeout` (408) теперь корректно выбрасывает `MaxNetworkException`.
  - Server errors (5xx) корректно классифицируются как `MaxNetworkException`.
- **`UpdatePoller`**: Исправлена фильтрация update types — теперь корректно работает с `UpdateTypeHelper.ToStringValues()`.
- **`MaxJsonSerializer`**: Исправлена обработка nullable типов в `ErrorResponse` и `Error` моделях.
- **`SubscriptionsApi`**: Улучшена валидация параметров для `GetUpdatesAsync` и `SetWebhookAsync`.
- **`DeleteWebhook`**: Endpoint теперь принимает `url` только как query parameter (согласно спецификации Max API).

### Changed

- **BREAKING**: `IMaxHttpClient` теперь реализует `IDisposable`. Все кастомные реализации должны добавить метод `Dispose()`.
- **BREAKING**: `MaxHttpClient.SendAsync<TResponse>()` теперь всегда вызывает `SendAsyncRaw()` и десериализует ответ централизованно.
- **BREAKING**: Обработка ошибок в `MaxHttpClient` изменена — метод `HandleHttpResponseError()` теперь sync вместо async.
- **BREAKING**: `FileUploadResult` больше не содержит свойство `PhotoId` (удалено как устаревшее).
- Обновлены тесты `AttachmentTests.cs`, `FilesApiTests.cs`, `BotApiTests.cs`, `ChatsApiTests.cs`, `SubscriptionsApiTests.cs`.
- Добавлены новые тесты `ContactHelpersTests.cs`, `FileUploadResultTests.cs`, `UpdateTypeJsonConverterTests.cs`, `UpdateTypeHelperTests.cs`.
- Увеличено покрытие тестами HTTP клиент логики (retry, logging, error handling).
- **Refactor**: Удалены дублирующиеся using директивы и оптимизирован код `MaxHttpClient.cs`.

### Removed

- **`FileUploadResult.PhotoId`**: Удалено как устаревшее свойство (используйте `FileId` или `Photos` коллекцию).

---

## [0.4.1-alpha] - 2025-03-09

### Added

- **Location Attachment**: Поддержка вложений местоположения (`LocationAttachment`) с полями `Latitude` и `Longitude`.
- **Image Attachment**: Поддержка вложений изображений (`ImageAttachment`) с возможностью отправки нескольких фото одновременно.
- **Contact Attachment**: Полная поддержка вложений контактов (`ContactAttachment`) с автоматическим парсингом vCard данных.

### Fixed

- **DeleteWebhook URL**: Endpoint `DeleteWebhook` теперь корректно принимает `url` только как query parameter (согласно спецификации Max API).

### Changed

- Обновлены тесты для новых типов вложений.

---

## [0.4.0-alpha] - 2025-03-08

### Added

- **NewMessageLink Type**: Добавлен тип `NewMessageLink` с полями `ChatId`, `MessageId`, `Text`.
- **Validation Attributes**: Добавлена валидация для `NewMessageLink` (Required, MaxLength, Range).

### Fixed

- **NewMessageLink JSON Field**: Изменено имя JSON поля с `id` на `mid` для совместимости со спецификацией Max API.
- **ChatId Validation**: Удалена валидация `Range` для `ChatId` в `NewMessageLink` для поддержки отрицательных ID групповых чатов.
- **MessageId Type**: Изменён тип `MessageId` с `long` на `string` в `ForwardMessageAsync` и `ReplyToMessageAsync` для совместимости с API.

### Changed

- **Breaking**: Тип `MessageId` изменён с `long` на `string` в методах работы с сообщениями.

---

## [0.3.8-alpha] - 2025-03-07

### Changed

- Version bump для republication на NuGet.

---

## [0.3.6-alpha] - 2025-03-06

### Fixed

- **ChatId Validation**: Удалена валидация `ChatId = 0` для поддержки direct messages.
- **UserId Validation**: Централизована валидация `UserId` через общий метод валидации.

---

## [0.3.5-alpha] - 2025-03-05

### Fixed

- **Critical**: Исправлена валидация `ChatId = 0` для direct messages (DM).

---

## [0.3.4-alpha] - 2025-03-04

### Added

- **MessageRecipient Tests**: Добавлены unit тесты для сериализации/десериализации `MessageRecipient`.

### Fixed

- **Direct Messages**: Исправлена валидация `ChatId` и `UserId` для поддержки direct messages.

---

## [0.3.3-alpha] - 2025-03-03

### Added

- **Inline Button Intents**: Добавлена поддержка интентов для inline кнопок.
- **BotCommand Model**: Введена новая модель `BotCommand` с полями `Name` и `Description`.
- **SetCommandsAsync**: Новый метод в `BotApi` для установки команд бота.

### Changed

- Обновлены тесты для новых функций inline кнопок и команд бота.

---

## [0.3.2-alpha] - 2025-03-02

### Fixed

- **EditMessageReplyMarkupAsync**: Исправлена критическая проблема — метод теперь сохраняет вложения сообщения при обновлении клавиатуры.
- **Null Safety**: Улучшена обработка null значений в методах редактирования сообщений.

### Changed

- **Refactor**: Логика редактирования сообщений переработана для сохранения вложений.

---

## [0.3.0-alpha] - 2025-03-01

### Added

- Initial alpha release for Max Messenger Bot API.
- Basic bot API methods: `GetMe`, `SendMessage`, `ForwardMessage`, etc.
- Webhook support: `SetWebhook`, `DeleteWebhook`, `GetWebhookInfo`.
- Update polling: `GetUpdates` with filtering support.
- File management: `UploadFile`, `GetFile`, `DeleteFile`.
- Chat management: `GetChat`, `UpdateChat`.
- Subscription management: `GetSubscriptions`, `CreateSubscription`, `DeleteSubscription`.

### Changed

- Migrated to .NET 9.0.
- Implemented custom JSON serializer (`MaxJsonSerializer`).
- Added comprehensive error handling with custom exceptions (`MaxApiException`, `MaxNetworkException`, etc.).

---

[Unreleased]: https://github.com/NanoAgents/MaxBotNet/compare/v0.5.1-alpha...HEAD
[0.5.1-alpha]: https://github.com/NanoAgents/MaxBotNet/compare/v0.5.0-alpha...v0.5.1-alpha
[0.5.0-alpha]: https://github.com/NanoAgents/MaxBotNet/compare/v0.4.1-alpha...v0.5.0-alpha
[0.4.1-alpha]: https://github.com/NanoAgents/MaxBotNet/compare/v0.4.0-alpha...v0.4.1-alpha
[0.4.0-alpha]: https://github.com/NanoAgents/MaxBotNet/compare/v0.3.8-alpha...v0.4.0-alpha
[0.3.8-alpha]: https://github.com/NanoAgents/MaxBotNet/compare/v0.3.6-alpha...v0.3.8-alpha
[0.3.6-alpha]: https://github.com/NanoAgents/MaxBotNet/compare/v0.3.5-alpha...v0.3.6-alpha
[0.3.5-alpha]: https://github.com/NanoAgents/MaxBotNet/compare/v0.3.4-alpha...v0.3.5-alpha
[0.3.4-alpha]: https://github.com/NanoAgents/MaxBotNet/compare/v0.3.3-alpha...v0.3.4-alpha
[0.3.3-alpha]: https://github.com/NanoAgents/MaxBotNet/compare/v0.3.2-alpha...v0.3.3-alpha
[0.3.2-alpha]: https://github.com/NanoAgents/MaxBotNet/compare/v0.3.0-alpha...v0.3.2-alpha
[0.3.0-alpha]: https://github.com/NanoAgents/MaxBotNet/releases/tag/v0.3.0-alpha

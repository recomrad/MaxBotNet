# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).


## [0.2.7-alpha] - 2025-11-24

### Changed
- Обновление версии пакета.

## [0.2.6-alpha] - 2025-11-24

### Fixed
- Исправлена критическая ошибка сериализации inline-клавиатуры: изменено JSON-поле с `inline_keyboard` на `buttons` для соответствия API Max Messenger. Теперь сервер корректно получает структуру `payload.buttons` вместо `payload.inline_keyboard`, что устраняет `NullPointerException` на стороне сервера при вызове `getButtons()`.
- Добавлена нормализация кнопок клавиатуры в `CreateInlineKeyboardAttachment` для предотвращения null-значений в массивах кнопок. Свойство `Buttons` в `InlineKeyboard` теперь гарантированно не содержит null-элементов.

## [0.2.5-alpha] - 2025-11-24

### Added
- Типизированные обертки `MessageUpdate` и `CallbackQueryUpdate` для улучшенной типобезопасности при работе с обновлениями. Структура теперь аналогична Telegram Bot API с разделением по типу обновления.
- Метод `SendMessageToUserAsync` для отправки сообщений с inline-клавиатурой пользователю по `userId` (дополняет существующий метод с `chatId`).

### Changed
- Рефакторинг структуры `Update`: добавлены свойства `MessageUpdate` и `CallbackQueryUpdate` для доступа к типизированным оберткам. Старые свойства `Message` и `CallbackQuery` сохранены для обратной совместимости.
- Улучшена структура обновлений по аналогии с Telegram Bot API, что позволяет использовать pattern matching для обработки разных типов обновлений.

## [0.2.4-alpha] - 2025-11-24

### Fixed
- Исправлена ошибка валидации при обработке вебхуков типа `message_created`. Свойство `Message.Id` теперь nullable (`long?`), что позволяет корректно десериализовать обновления, где числовой `message.id` отсутствует, а уникальный ID сообщения передаётся только в `message.body.mid` (строка).

## [0.2.3-alpha] - 2025-11-24

### Added
- Перегрузка метода `SendMessageAsync` с опциональным параметром `InlineKeyboard? keyboard` для упрощения отправки сообщений с клавиатурой. Теперь можно отправлять сообщения с inline-клавиатурой без необходимости вручную создавать `SendMessageRequest` и `AttachmentRequest`.

### Changed
- Обновлен пример `KeyboardBotSample` для демонстрации использования новой упрощенной перегрузки `SendMessageAsync` с клавиатурой.

## [Unreleased]

### Added
- GitHub Actions workflow `ci.yml` с форматированием, анализаторами, покрытием и упаковкой артефактов.
- Workflow `release.yml`, автоматически публикующий NuGet-пакеты по тегам `v*`.
- `LoopbackSampleRuntime` и рефакторинг `SampleBotsTests`, которые прогоняют все сценарии `SampleRegistry` офлайн.
- Фикстуры API для `/subscriptions` и `/updates`, используемые в интеграционных тестах.

### Changed
- README и новый `RELEASING.md`, описывающие CI/CD и инструкции по релизу.

## [0.2.0-alpha] - 2025-11-17

### Added
- Enforced XML documentation (CS1591) for all public APIs and added `DocumentationCoverageTests` to guard regressions.
- Introduced `examples/Max.Bot.Examples` console project with Echo/Command/Keyboard/File bot samples inspired by Telegram.Bot and VkNet.
- Added comprehensive smoke tests for the new samples (`SampleBotsTests`) leveraging mocked runtimes.
- Expanded README with installation steps, quick start, feature overview, and sample catalogue plus references to official docs and sibling SDKs.

### Changed
- Updated `Directory.Build.props` to treat missing XML docs as errors while exempting the test project.
- Refined solution structure to include the examples project for local experimentation.

## [0.1.0-alpha] - 2025-01-XX

### Added
- Initial project setup
- Basic infrastructure and folder structure
- Build configuration for .NET 9

[Unreleased]: https://github.com/MaxBotNet/MaxBotNet/compare/v0.2.7-alpha...HEAD
[0.2.7-alpha]: https://github.com/MaxBotNet/MaxBotNet/compare/v0.2.6-alpha...v0.2.7-alpha
[0.2.6-alpha]: https://github.com/MaxBotNet/MaxBotNet/compare/v0.2.5-alpha...v0.2.6-alpha
[0.2.5-alpha]: https://github.com/MaxBotNet/MaxBotNet/compare/v0.2.4-alpha...v0.2.5-alpha
[0.2.4-alpha]: https://github.com/MaxBotNet/MaxBotNet/compare/v0.2.0-alpha...v0.2.4-alpha
[0.2.0-alpha]: https://github.com/MaxBotNet/MaxBotNet/compare/v0.1.0-alpha...v0.2.0-alpha
[0.1.0-alpha]: https://github.com/MaxBotNet/MaxBotNet/releases/tag/v0.1.0-alpha


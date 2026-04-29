# WordGame — Итерация 9: Tutorial Overlay

## Что сделано

При первом запуске Game-сцены показывается **серия туториал-подсказок**, объясняющих базовые механики. Тап в любое место — следующий шаг.

### Шаги туториала (динамические)
1. **Welcome** — представление игры
2. **Swipe** — основная механика, "start near the empty cell"
3. **Dictionary** — правила валидности слов и роста vacant-зоны
4. **Number cells** — показывается только если на поле есть number-клетки (не всегда, генератор может не разместить)
5. **Escape mode** — показывается **только в Escape-режиме**, объясняет цель и таймер

Минимум 3 шага (Explore без number-клеток), максимум 5 (Escape с number-клетками).

### Состояние
Сохраняется в PlayerPrefs:
- `WG_TutorialDone_Explore` — пройден туториал в Explore
- `WG_TutorialDone_Escape` — пройден в Escape

Туториалы независимы: первый Escape после первого Explore покажет туториал заново (новые механики).

### Settings → Reset Tutorial
В попапе Settings (доступен из MainMenu) добавлена кнопка `RESET TUTORIAL`. По нажатию: оба ключа удаляются из PlayerPrefs, появляется зелёная подпись `Tutorial reset!` (1.5 сек). Следующая Game-сцена снова покажет туториал.

### Поведение в Escape-режиме
**Таймер не запускается** пока туториал активен. После закрытия туториала (последний tap) — таймер стартует. Это сделано через корутину в `GameController`, которая ждёт пока Tutorial GameObject не станет inactive.

Edge highlighter (зелёные glow на краю) активируется сразу — он визуальный, не мешает туториалу, наоборот помогает иллюстрировать "reach the edge".

---

## Новые/изменённые файлы

```
Assets/WordGame/
├── Scripts/
│   ├── Tutorial.cs                       ← новое
│   ├── GameController.cs                 ← обновлён (показ туториала, отложенный таймер)
│   └── SettingsPopup.cs                  ← обновлён (Reset Tutorial кнопка)
└── Editor/
    └── SetupTutorialIteration9.cs        ← новое (правит обе сцены)
```

---

## Установка

1. Распаковать архив поверх проекта.
2. Дождаться компиляции.
3. Запустить: `WordGame > Setup Tutorial (Iteration 9)`
   - Скрипт правит обе сцены: добавляет Tutorial overlay в Game.unity и Reset кнопку в SettingsPopup MainMenu.unity
4. `MainMenu.unity` → Play.

---

## Тестирование

| Сценарий | Ожидание |
|---|---|
| Первый запуск (PlayerPrefs пусты) → PLAY → Explore | Game-сцена загрузилась, появился dim-overlay с панелью "Welcome!". Внизу `Tap to continue (1/3)` |
| Тапнуть → Тапнуть → Тапнуть | Шаги переключаются. На последнем `Tap to start`. После него — fade out, можно играть |
| Снова PLAY → Explore | Туториал уже не показывается, сразу можно играть |
| PLAY → **Escape** (после прохождения Explore) | Таймер не идёт пока туториал. Появляется новый туториал с Escape-шагом в конце |
| PLAY → Escape, последний tap | Таймер начинает идти с 0:66 |
| MainMenu → SETTINGS → RESET TUTORIAL | Появляется зелёное `Tutorial reset!` |
| Закрыть Settings → PLAY → Explore | Туториал показывается снова |

### Если на поле нет number-клеток
Генератор иногда не может разместить number-клетки (если коротких слов мало). В этом случае шаг "Cells with numbers" не показывается — туториал идёт 3 шага вместо 4.

---

## Параметры в инспекторе

**`Tutorial`** на объекте `Canvas/Tutorial`:
- `Canvas Group` — для fade
- `Dim Image` — затемняющая подложка
- `Callout Panel` — основная панель
- `Callout Text` — текст шага
- `Hint Text` — `Tap to continue` / `Tap to start`
- `Grid` — ссылка на HexGrid (для проверки наличия number-клеток)
- `Preview RT` / `Timer Hud RT` — пока не используются (резерв для подсветки конкретных областей в будущем)

**`SettingsPopup`** новые поля:
- `Reset Tutorial Button`
- `Reset Tutorial Feedback`

Editor-скрипт прописывает всё автоматически.

---

## Что НЕ реализовано (опционально)

- **Cutout / spotlight на конкретные области** (например, прозрачное окно вокруг центра при шаге "Start near empty cell"). Сейчас просто dim full-screen overlay. Реализация cutout требует кастомного shader или multiple-images масок — пропустил, чтобы не разрастаться.
- **Animated arrows** указывающие на нужный элемент — тоже пропущено в пользу простоты.

Если решишь усилить — добавь `RectTransform` в `Tutorial` для "spotlight area" и кастомный mask material. Сейчас текстовые подсказки достаточны для понимания.

---

## Что НЕ сделано в итерации

- Stats / retention фичи (it.10 — финальная)
- Какие-то financial/onboarding оптимизации (вне рамок проекта)

---

## Идемпотентность

Editor-скрипт можно запускать повторно: всё ищется по имени, переиспользуется. Tutorial GameObject в Game-сцене начинает скрытым (SetActive(false)) — его покажет `GameController.SetupModeSpecific()` при загрузке, если туториал не пройден.

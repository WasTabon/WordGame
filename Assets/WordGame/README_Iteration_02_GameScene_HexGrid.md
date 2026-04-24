# WordGame — Итерация 2: Game Scene + Hex Grid

## Что сделано в этой итерации

Добавлена игровая сцена с хекс-полем. Visual только — инпута и геймплея пока нет (итерация 3).

### Новые скрипты
- **HexCoord.cs** — структура axial-координат (q, r), соседи (6 направлений), pixel-позиция для pointy-top
- **HexCell.cs** — UI-компонент ячейки: хранит букву, число, флаг vacant, обновляет визуал
- **HexGrid.cs** — генерирует поле заданного радиуса, держит словарь `HexCoord → HexCell`, даёт соседей
- **LetterDistribution.cs** — Scrabble-подобные частоты (E — 12, A/I — 9, Z/J/K/Q/X — 1, и т.д.)
- **GameController.cs** — на Start строит сетку и делает центр vacant
- **GameHUD.cs** — верхняя панель: BACK-кнопка слева, название режима справа

### Что изменилось
- **ModeSelectPopup.cs** — теперь вызывает `SceneLoader.LoadScene("Game")` вместо показа placeholder. Если `SceneLoader` не найден (сцена запущена напрямую без MainMenu), используется обычный `SceneManager.LoadScene`.
- Поле `placeholderPopup` оставлено в `ModeSelectPopup`, чтобы ссылка в `MainMenu.unity` не стала битой. `PlaceholderPopup` и сам попап-объект тоже остались — просто больше не используются.

### Сгенерированные ассеты
- **Assets/WordGame/Sprites/Hex.png** — pointy-top hex, 174×200, с антиалиасингом (3×3 supersampling). Генерируется editor-скриптом один раз. Импортирован как Sprite (2D/UI), alpha-is-transparency, без mipmap, bilinear.

---

## Установка

### Предусловия
Уже выполнено из итерации 1:
- Unity 2022.3, 2D Built-in
- TMP Essentials + DOTween Free импортированы
- Итерация 1 запущена хотя бы раз (должна быть сцена `MainMenu.unity`)

### Шаги

1. **Распаковать архив в корень проекта** (папка `Assets/WordGame/`). Скрипты и editor-скрипт добавятся, ModeSelectPopup.cs перезапишется.

2. **Дождаться компиляции.**

3. **Запустить editor-скрипт:**
   - В меню Unity: `WordGame > Setup Game Scene (Iteration 2)`
   - Скрипт создаст:
     - `Assets/WordGame/Sprites/Hex.png` (если ещё нет)
     - `Assets/WordGame/Scenes/Game.unity` (если ещё нет)
     - Сцена будет добавлена в Build Settings

4. **Открыть `MainMenu.unity`** и нажать Play.

---

## Как тестировать

| Проверка | Ожидаемое поведение |
|---|---|
| MainMenu → PLAY → ESCAPE | Fade-out, загрузка Game-сцены, fade-in, в HUD справа "ESCAPE" |
| MainMenu → PLAY → EXPLORE | То же, но в HUD "EXPLORE" |
| Game-сцена | 37 hex-ячеек по форме большого гексагона, центральная клетка пустая (прозрачная, без буквы) |
| Буквы | Каждый запуск — новый случайный расклад, гласных и частых согласных больше (E, A, I, N, O, R, T заметно чаще, чем Z, Q, J, X) |
| BACK | Fade, возврат в MainMenu |
| Прямой запуск `Game.unity` без MainMenu | Грид собирается, но fade-переход без SceneLoader — обычная мгновенная загрузка |

---

## Размер поля и настройки

В инспекторе на `GameController > HexGrid` доступно:
- **Cell Size** — радиус шестиугольника (от центра до вершины) в пикселях. По умолчанию `75` → ячейка ~130×150.
- **Grid Radius** — радиус hex-формы. `3` → 37 клеток (7 в ширину). `4` → 61 клетка.
- **Cell Sprite** — спрайт hex-формы (уже назначен).
- **Container** — RectTransform-контейнер, в который складываются ячейки.

При увеличении `gridRadius` до 4 нужно будет уменьшить `cellSize` до ~60, чтобы поле влезло в экран.

---

## Структура файлов

Новое в итерации 2:
```
Assets/WordGame/
├── Scenes/
│   └── Game.unity                 ← создаётся editor-скриптом
├── Sprites/
│   └── Hex.png                    ← генерируется editor-скриптом
├── Scripts/
│   ├── HexCoord.cs
│   ├── HexCell.cs
│   ├── HexGrid.cs
│   ├── LetterDistribution.cs
│   ├── GameController.cs
│   ├── GameHUD.cs
│   └── ModeSelectPopup.cs         ← изменён (загрузка Game-сцены)
└── Editor/
    └── SetupGameSceneIteration2.cs
```

Без изменений (из итерации 1): `GameMode.cs`, `SoundManager.cs`, `SceneLoader.cs`, `UIButtonFeedback.cs`, `PopupBase.cs`, `SettingsPopup.cs`, `HowToPlayPopup.cs`, `PlaceholderPopup.cs`, `MainMenuUI.cs`.

---

## Что НЕ сделано в этой итерации

- Swipe-инпут, выделение клеток, сбор слова (итерация 3)
- Словарь английских слов и валидация (итерация 4)
- Рост пустой области после валидного слова (итерация 4)
- Number-клетки с ограничением минимальной длины слова (итерация 5)
- Счёт (итерация 5)
- Таймер Escape-режима (итерация 7)
- Проверка на разрешимость при генерации букв — появится вместе со словарём в итерации 4

---

## Иерархия Game-сцены

```
Canvas
├── Background (dark)
├── HUD (200px высоты, вверху)
│   ├── BackButton (слева)
│   └── ModeLabel (справа)
├── GridContainer (центр)
│   └── Cell_q_r × 37  ← создаются runtime через HexGrid.Build()
└── GameController (GameObject с GameController + HexGrid компонентами)

EventSystem
```

---

## Замечания

- **SceneLoader из итерации 1** продолжает работать через `DontDestroyOnLoad` — при загрузке Game-сцены он уже в памяти и даёт fade-переход. При возврате в MainMenu — то же.
- **Центральный vacant** сейчас всегда один — в итерациях 3–4 сюда добавится логика роста пустой зоны после валидных слов.
- **Number-клеток пока нет** — `HexCell.SetMinWordLength` готов, но вызов будет в итерации 5.
- **Hex.png** генерируется один раз. Если хочешь перегенерировать (другой размер или стиль) — удали файл вручную и перезапусти `WordGame > Setup Game Scene (Iteration 2)`.

---

## Идемпотентность

Editor-скрипт можно запускать повторно:
- Проверяет существование Hex.png, не пересоздаёт если есть
- Открывает существующую Game.unity или создаёт новую
- Все GameObject'ы ищутся по имени и переиспользуются
- Ссылки на компоненты и значения полей переписываются каждый раз

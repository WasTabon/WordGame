# WordGame — Итерация 12: Levels в Escape Mode

## Что сделано

Escape стал прогрессией с уровнями. Поле растёт от уровня к уровню, время больше, цель та же — добраться до края.

### Прогрессия уровней (линейная, бесконечная)
| Level | Radius | Cells | Время (примерно) |
|---|---|---|---|
| 1 | 2 | 19 | 54 сек |
| 2 | 3 | 37 | 66 сек (как раньше) |
| 3 | 4 | 61 | 78 сек |
| 4 | 5 | 91 | 90 сек |
| 5 | 6 | 127 | 102 сек |
| 6 | 7 | 169 | 114 сек |
| 7+ | 8 | 217 | 126 сек (capped) |

Radius capped на 8 чтобы поле визуально влезало в экран. Время продолжает расти.

Формула: `radius = level + 1` (capped at 8). Время: `30 + radius * 12`. CellSize динамический: `230 / (radius + 1)` — поле остаётся примерно того же визуального размера.

### Старт / Победа / Поражение
- **Win**: попап `LEVEL N WON!` с кнопкой `NEXT LEVEL`. По нажатию: `LevelManager.IncrementLevel()` + reload Game-сцены
- **Lose** (timeout / trap): попап `TIME'S UP` или `TRAPPED` с кнопкой `RETRY LEVEL`. Уровень **не меняется**, перезагрузка тот же уровень
- **Main Menu**: можно выйти в любой момент, прогресс сохраняется

### Explore без изменений
В Explore radius и cellSize берутся из `GameController` (по умолчанию 3 / 75). Никаких уровней, никаких изменений.

### UI
- **GameHUD** в Escape показывает `ESCAPE • LV 5` вместо просто `ESCAPE`
- **ModeSelectPopup** под кнопкой Escape видно `Lv 5 • Best: 320`
- **StatsPopup** новая строка `Escape level: Lv 5 (best 7)` — показывает текущий и лучший достигнутый
- Reset stats теперь сбрасывает и `HighestEscapeLevel` (но НЕ `CurrentLevel` — это прогресс)

### Сохранение прогресса
PlayerPrefs ключи:
- `WG_EscapeLevel` — текущий уровень (по умолчанию 1)
- `WG_Stats_HighestEscapeLevel` — лучший достигнутый уровень

Reset stats удалит `HighestEscapeLevel` но не `WG_EscapeLevel`. Если хочешь начать с уровня 1 — вызови `LevelManager.ResetToLevel1()` из консоли или через `PlayerPrefs.DeleteKey("WG_EscapeLevel")`.

---

## Новые/изменённые файлы

```
Assets/WordGame/
├── Scripts/
│   ├── LevelManager.cs                ← новое
│   ├── GameController.cs              ← обновлён (применяет уровень к gridRadius+cellSize)
│   ├── GameHUD.cs                     ← обновлён (показывает LV N)
│   ├── ModeSelectPopup.cs             ← обновлён (Level + Best)
│   ├── WinPopup.cs                    ← обновлён (IncrementLevel, NEXT LEVEL)
│   ├── GameOverPopup.cs               ← обновлён (RETRY LEVEL в Escape)
│   ├── GameStats.cs                   ← обновлён (TrackHighestEscapeLevel)
│   └── StatsPopup.cs                  ← обновлён (показ EscapeLevel)
└── Editor/
    └── SetupLevelsEscapeIteration12.cs    ← новое (добавляет EscapeLevel в Stats popup)
```

---

## Установка

1. Распаковать архив поверх проекта.
2. Дождаться компиляции.
3. Запустить: `WordGame > Setup Levels Escape (Iteration 12)`
   - Добавит строку `Escape level` в StatsPopup, сдвинет TimePlayed/ExploreBest/EscapeBest ниже
4. `MainMenu.unity` → Play.

---

## Тестирование

| Сценарий | Ожидание |
|---|---|
| Первый запуск (PlayerPrefs пусты) | В Escape стартует с `LV 1`, поле маленькое (radius 2 = 19 клеток) |
| MainMenu → Play → Escape | В попапе под кнопкой Escape: `Lv 1 • Best: 0` |
| Game-сцена Escape | В HUD `ESCAPE • LV 1`, поле 19 клеток, таймер ~54 сек |
| Победить → попап ESCAPED | Текст `LEVEL 1 WON!`, кнопка `NEXT LEVEL` |
| Тап NEXT LEVEL | Reload Game, теперь `LV 2`, поле 37 клеток, таймер 66 сек |
| Победить ещё → NEXT LEVEL → ... | Прогрессия до radius=8 максимум, потом radius capped, но время продолжает расти |
| Проиграть на уровне 5 | Попап `TIME'S UP`/`TRAPPED`, кнопка `RETRY LEVEL` |
| Тап RETRY LEVEL | Reload Game, тот же `LV 5`, новое поле |
| Закрыть Editor и снова открыть | Текущий уровень сохранился |
| MainMenu → Stats | Строка `Escape level: Lv 5 (best 7)` — current 5, best 7 |
| Reset Stats | `Lv N (best 0)` — best обнулён, current нет |

---

## Параметры в инспекторе

**`GameController`** (новые поля):
- `Explore Radius` = 3 — radius для Explore-режима (не меняется по уровням)
- `Explore Cell Size` = 75 — размер клетки в Explore

Остальное (`Escape Base Seconds`, `Escape Seconds Per Radius`) как раньше.

**`LevelManager`** — статический, параметров в инспекторе нет. Если нужно поменять формулы:
- `GetRadiusForLevel(level)` — `1 + level` (capped 2-8)
- `GetCellSizeForLevel(level)` — `230 / (radius + 1)`

---

## Известные нюансы

- **Radius capped на 8** — при level 7 и выше поле не растёт по клеткам, но время продолжает расти. На 1080×1920 экране radius=8 это ~217 клеток. Можно увеличить cap до 10 если очень хочется.
- **HighestEscapeLevel записывается на WIN** (момент когда вызывается `LevelManager.IncrementLevel()`). Если игрок выиграл level 5 и не нажал `NEXT LEVEL` (вышел в меню), best остаётся 5.
- **При Edge Highlighter** в Escape — границы поля изменяются от уровня. Glow создаётся каждый раз заново через `Activate()` при загрузке Game scene.
- **Tutorial показывается заново только если он не пройден.** Если игрок уже прошёл tutorial Escape — на новых уровнях он не появится.

---

## Что НЕ сделано

- **Анимация перехода на новый уровень** — сейчас просто scene reload через SceneLoader fade. Можно добавить специальный `LEVEL UP` overlay перед началом нового уровня. Если захочется — отдельная мини-итерация.
- **Сложность number-клеток по уровням** — пока number-клетки одинаково ставятся независимо от уровня (3 штуки, min=4). На больших полях можно ставить больше или с большими цифрами. Сейчас оставил как есть.

---

## Идемпотентность

Editor-скрипт можно запускать повторно — все элементы ищутся по имени, переиспользуются, ссылки переписываются. Если StatsPopup уже имеет `EscapeLevel` row, скрипт его обновит а не дублирует.

Следующая итерация: **13. Endless Explore** (continue-механика при тупике).

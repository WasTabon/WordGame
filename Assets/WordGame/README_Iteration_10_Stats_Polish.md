# WordGame — Итерация 10: Stats + Polish (FINAL)

## Что сделано

Финальная итерация. Игра feature-complete.

### Stats system
- **`GameStats`** — статический класс, сохраняет в PlayerPrefs:
  - Игр сыграно (всего, отдельно по режимам)
  - Слов найдено всего
  - Самое длинное слово за всё время
  - Лучший счёт за одно слово
  - Escape wins / total (winrate)
  - Общее время игры
- Записывается в реальном времени: `RecordGameStarted` при старте, `RecordWord` при валидном слове, `RecordEscapeWin/Loss` + `AddTimePlayed` при game over

### StatsPopup
Доступен из главного меню по новой кнопке **STATS** (третья в нижнем ряду рядом с Settings и How to Play). Большой попап показывает:

| Поле | Пример |
|---|---|
| Games played | `42` |
| Words found | `187` |
| Longest word | `STARE (5)` |
| Best word score | `100` |
| Escape wins | `8 / 15 (53%)` |
| Time played | `12m 34s` |
| Explore best | `420` |
| Escape best | `680` |

В попапе кнопка `RESET STATS` — сброс всей статистики (с подтверждающим feedback). High scores из `HighScoreManager` сбрасываются отдельно — их трогать не будем (они отдельная сущность).

### Polish (мелкие улучшения)
- **Анимация count-up счёта** в Game Over и Win попапах. Финальный счёт начинается с 0 и плавно растёт за 0.8-0.9 сек через DOTween (`OutQuart`), потом punch на финале. Намного приятнее чем мгновенное появление.
- **Bottom row главного меню** перепланирован под 3 кнопки: Settings | How to Play | Stats. Каждая 290×130 (раньше две по 430×140).
- **Legacy stub'ы** оставлены: `PlaceholderPopup.cs` и `LetterDistribution.cs` сохранены для обратной совместимости с editor-скриптами iteration 1 и резервного fallback в HexGrid.

---

## Новые/изменённые файлы

```
Assets/WordGame/
├── Scripts/
│   ├── GameStats.cs                      ← новое (PlayerPrefs stats)
│   ├── StatsPopup.cs                     ← новое
│   ├── GameOverPopup.cs                  ← обновлён (анимация count-up)
│   ├── WinPopup.cs                       ← обновлён (анимация count-up)
│   ├── MainMenuUI.cs                     ← обновлён (Stats кнопка)
│   ├── GameController.cs                 ← обновлён (RecordGameStarted, RecordPlayedTime)
│   └── WordBuilder.cs                    ← обновлён (RecordWord, RecordEscapeWin/Loss)
└── Editor/
    └── SetupStatsPolishIteration10.cs    ← новое (правит обе сцены)
```

---

## Установка

1. Распаковать архив поверх проекта.
2. Дождаться компиляции.
3. Запустить: `WordGame > Setup Stats + Polish (Iteration 10)`
4. `MainMenu.unity` → Play.

---

## Тестирование

| Сценарий | Ожидание |
|---|---|
| MainMenu | В нижнем ряду 3 кнопки: SETTINGS, HOW TO PLAY, STATS |
| Тапнуть STATS | Открывается попап с текущей статистикой. Если играл первый раз: `Games played: 0`, `Time played: 0s` |
| Сыграть пару раз в Explore | После каждой партии в STATS: Games played растёт, Words found растёт, Longest word обновляется при длинном слове, Time played увеличивается |
| Сыграть в Escape с победой | `Escape wins: 1 / 1 (100%)` |
| Сыграть Escape с проигрышем | `Escape wins: 1 / 2 (50%)` |
| GameOver / Win popup | Финальный счёт **анимированно** растёт от 0 до настоящего значения за 0.8-0.9 сек, потом punch |
| Reset Stats в STATS popup | Все цифры сбрасываются на 0 / `-` / `0s`. Появляется зелёное `Stats reset!` (1.5 сек). High scores при этом остаются (это отдельная штука, сбрасывается через `Reset Tutorial` в Settings + Reset Stats отдельно или вручную через PlayerPrefs.DeleteAll) |

### Полный процесс «новый игрок» от старта до game over:
1. Запуск → Tutorial показывается (если не пройден)
2. Каждый шаг tutorial добавляет в Tutorial done state, но не в stats
3. После tutorial — стат-таймер начинает идти (он стартует в `Start()` GameController)
4. Сбор первого слова → `Words found: 1`, `Longest word: ...`
5. Игра доходит до тупика → `Games played: 1`, `Time played` обновляется

---

## Полная структура проекта (итог)

```
Assets/WordGame/
├── Scripts/                 (36 файлов)
│   ├── BoardGenerator.cs           — 3-фазная генерация поля
│   ├── DeadlockDetector.cs         — детект тупика
│   ├── Dictionary.cs               — загрузка словаря
│   ├── EdgeHighlighter.cs          — glow на краю в Escape
│   ├── EscapeTimer.cs              — таймер
│   ├── EscapeWinDetector.cs        — проверка достижения края
│   ├── FloatingScorePopup.cs       — всплывающий +score
│   ├── GameController.cs           — оркестратор Game-сцены
│   ├── GameHUD.cs                  — UI счёта/режима в Game
│   ├── GameMode.cs                 — текущий режим
│   ├── GameOverPopup.cs            — попап проигрыша
│   ├── GameStats.cs                — статистика
│   ├── HexCell.cs                  — клетка
│   ├── HexCoord.cs                 — axial координаты
│   ├── HexGrid.cs                  — поле
│   ├── HighScoreManager.cs         — лучший счёт
│   ├── HowToPlayPopup.cs           — попап правил
│   ├── LetterDistribution.cs       — Scrabble-fallback
│   ├── MainMenuUI.cs               — главное меню
│   ├── ModeSelectPopup.cs          — выбор режима
│   ├── NumberCellPlacer.cs         — number-клетки
│   ├── PlaceholderPopup.cs         — legacy stub
│   ├── PopupBase.cs                — базовый класс попапа
│   ├── ProceduralSounds.cs         — генерация звуков
│   ├── SceneLoader.cs              — fade переходы сцен
│   ├── ScoreManager.cs             — счёт за игру
│   ├── ScreenShaker.cs             — тряска Canvas
│   ├── SettingsPopup.cs            — настройки
│   ├── SoundManager.cs             — звук + громкости
│   ├── StatsPopup.cs               — попап статистики
│   ├── TimerHUD.cs                 — UI таймера в Escape
│   ├── Tutorial.cs                 — обучение
│   ├── UIButtonFeedback.cs         — press-feedback
│   ├── WinPopup.cs                 — попап победы
│   ├── WordBuilder.cs              — главная логика свайпа+валидации+endgame
│   ├── WordPreviewUI.cs            — preview слова сверху
│   └── WordValidator.cs            — валидация слов
├── Editor/                  (10 файлов)
│   ├── SetupMainMenuIteration1.cs
│   ├── SetupGameSceneIteration2.cs
│   ├── SetupSwipeInputIteration3.cs
│   ├── SetupDictionaryIteration4.cs
│   ├── SetupNumberCellsScoreIteration5.cs
│   ├── SetupEndgameHighScoreIteration6.cs
│   ├── SetupEscapeModeIteration7.cs
│   ├── SetupJuicePassIteration8.cs
│   ├── SetupTutorialIteration9.cs
│   └── SetupStatsPolishIteration10.cs
├── Resources/
│   └── word_list.txt        — 401 слово
├── Sprites/
│   └── Hex.png              — pointy-top hex (174×200, генерируется в it.2)
└── Scenes/
    ├── MainMenu.unity
    └── Game.unity
```

---

## PlayerPrefs ключи

| Ключ | Назначение |
|---|---|
| `WG_MusicVolume` | Громкость музыки |
| `WG_SfxVolume` | Громкость SFX |
| `WG_HighScore_Explore` | Лучший счёт Explore |
| `WG_HighScore_Escape` | Лучший счёт Escape (с time bonus) |
| `WG_TutorialDone_Explore` | Туториал пройден в Explore |
| `WG_TutorialDone_Escape` | Туториал пройден в Escape |
| `WG_Stats_GamesTotal` | Игр всего |
| `WG_Stats_GamesExplore` | Игр в Explore |
| `WG_Stats_GamesEscape` | Игр в Escape |
| `WG_Stats_EscapeWins` | Побед в Escape |
| `WG_Stats_EscapeLosses` | Поражений в Escape |
| `WG_Stats_WordsTotal` | Слов всего |
| `WG_Stats_LongestWord` | Самое длинное слово |
| `WG_Stats_BestWordScore` | Лучший счёт за слово |
| `WG_Stats_TimePlayed` | Общее время игры (сек) |

Чтобы полностью обнулить сохранения — `Edit > Clear All PlayerPrefs` или `PlayerPrefs.DeleteAll()` в коде.

---

## Возможные дальнейшие улучшения (не входит в проект, на твоё усмотрение)

- **Замена процедурных звуков** на качественные клипы — описано в README iteration 8
- **Замена Hex-спрайта** на стилизованный — `Assets/WordGame/Sprites/Hex.png`, переразметка через TextureImporter
- **Расширение словаря** — просто добавь строки в `Resources/word_list.txt`. Современные мобильные word-games используют 60-100k слов
- **Daily challenge** — фиксированный seed для генерации, чтобы все игроки получали одно поле в день
- **Online leaderboards** — через любой backend (Firebase, PlayFab, etc.)
- **Achievements** — систематизация stats в коллекцию badges
- **Push notifications** — типа "your daily challenge is ready"

Все они опциональны для базовой версии. Игра как есть — полностью играбельная, со стартом, циклом и завершением.

---

## Идемпотентность всех editor-скриптов

Все 10 editor-скриптов идемпотентны: можно запускать в любом порядке и сколько угодно раз. Каждый ищет свои объекты по имени, не дублирует, переписывает ссылки актуальными. Ребёнки с неправильными типами (без RectTransform) автоматически удаляются и пересоздаются.

---

## Финальный статус: WordGame v1.0 готов

10/10 итераций. Игра играется от MainMenu до GameOver и обратно, имеет два режима, туториал, статистику, лучший счёт и достаточно много juice-feedback'а. Размер кода ~3500 строк C#.

Спасибо за совместную работу!

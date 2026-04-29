# WordGame — Итерация 4: Словарь + валидация + рост vacant-зоны

## Что сделано

Появилась реальная игровая петля: ввёл слово → проверилось по словарю → буквы превратились в пустые клетки → поле «растёт» в пустоту, открывая новые стартовые точки.

### Новые скрипты
- **Dictionary.cs** — статический класс. Загружает `Resources/word_list.txt` лениво при первом обращении, держит `HashSet<string>` (UPPERCASE). `Contains(word)` — O(1).
- **WordValidator.cs** — компонент. `Validate(word, requiredMinLength) → ValidationResult` (`Valid` / `TooShort` / `NotInDictionary` / `AlreadyUsed`). Хранит `usedWords` — повторно одно слово не засчитывается за игру.

### Новый ассет
- **Resources/word_list.txt** — 100 английских слов длиной 3–7 букв. Подобраны под Scrabble-распределение (много гласных, T/N/R/S). Распределение по длине:
  - 40 слов по 3 буквы (CAT, DOG, RUN, RAY, NUT, ICE...)
  - 40 слов по 4 буквы (RAIN, TEAR, STAR, SAND, IRON...)
  - 10 слов по 5 букв (STARE, TRAIN, RIVER, EARTH, RAISE...)

### Что изменилось
- **WordPreviewUI.cs** — добавлены `FlashSuccess(message)` и `FlashError(message)`. На submit показывает результат с пульсом и fade через DOTween (0.7 сек по умолчанию). Цвета: оранжевый — обычный preview, зелёный — успех, красный — ошибка.
- **WordBuilder.cs** — на release вызывает `validator.Validate(...)`. При `Valid` — буквы становятся vacant через `grid.MakeCellsVacant(...)`, preview показывает `✓ WORD` зелёным, слово маркируется как использованное. При invalid — preview показывает причину красным, поле остаётся как было.
- **HexGrid.cs** — добавлен метод `MakeCellsVacant(IEnumerable<HexCoord>)`.

---

## Установка

### Предусловия
Уже выполнено: итерации 1, 2, 3 настроены и работают.

### Шаги

1. **Распаковать архив** поверх проекта. Перезапишутся `WordBuilder.cs`, `WordPreviewUI.cs`, `HexGrid.cs`. Добавятся `Dictionary.cs`, `WordValidator.cs`, editor-скрипт.

2. **Дождаться компиляции.**

3. **Запустить editor-скрипт:**
   `WordGame > Setup Dictionary (Iteration 4)`
   
   Скрипт создаст `Assets/WordGame/Resources/word_list.txt` (если ещё нет), откроет Game.unity, добавит компонент `WordValidator` на `GameController` и пропишет ссылку в `WordBuilder`.

4. **Открыть `MainMenu.unity`** → Play.

---

## Как тестировать

### Базовый сценарий
1. PLAY → выбрать режим → Game-сцена
2. Найти настоящее слово, начинающееся рядом с центром:
   - **CAT** — найти C, A, T расположенные цепочкой соседей через клетку, граничащую с центром
   - Или **RUN**, **SUN**, **EAR**, **RAY**, **TEA**, **NET**, **TIE** и т.п.
3. Свайп → release → preview зелёным `✓ CAT`, использованные клетки превращаются в прозрачные (vacant). Поле выросло.
4. Теперь стартовать слово можно с **любой клетки, граничащей с новыми пустыми**, не только вокруг центра

### Невалидные случаи
| Действие | Ожидаемое |
|---|---|
| Собрать `QXZ` или другую бессмыслицу | Красный `NOT A WORD`, поле не меняется |
| Собрать `CAT` дважды за игру | Второй раз красный `ALREADY USED` |
| Собрать одну букву и release | Тихая отмена (минимум 2 буквы) |
| Стартовать в углу поля (нет vacant-соседа) | Клетка не подсвечивается |
| После роста зоны попробовать стартовать в новых местах | Работает — клетки рядом с новой пустой зоной теперь стартовые |

### Проверка консоли
- При первом submit любого слова в консоль выпадает `Dictionary loaded: 100 words.`
- Каждый submit логируется: `[WordBuilder] Accepted: CAT` или `[WordBuilder] Rejected (NotInDictionary): QXZ`

---

## Состав словаря (100 слов)

```
3-letter (40): CAT, DOG, RUN, SUN, RAY, SEA, TEA, EAR, OAR, ARM,
               ART, EAT, ATE, ICE, OIL, ORE, ONE, TWO, TEN, TOE,
               TOP, TIE, TIP, RAT, ROT, ROW, RAW, RED, RIB, RID,
               NET, NOT, NOR, NEW, OWL, NUT, OUR, OUT, OWN, ODD

4-letter (40): RAIN, ROAR, ROAD, RACE, RIDE, RIPE, RICE, RING, ROSE, RUST,
               TEAR, TIDE, TIRE, TONE, TORN, TOUR, TOWN, TRIP, TREE, TRUE,
               STAR, SOAR, SORT, SORE, SAND, SEAT, SEED, SEEN, SIDE, SITE,
               EARN, EAST, EDIT, ENDS, ICED, IRON, IDEA, INTO, AREA, ANTS

5-letter (10): STARE, STORE, STORM, STAIN, STEEP, STILT, STIRS, STONE, STOOD, STRIP
5-letter (10): TRAIN, TRADE, TRIES, TRUST, EARNS, EARTH, ENTER, RAISE, ROAST, RIVER
```

Это рабочая база. В будущих итерациях легко расширяется — просто добавить строки в `Resources/word_list.txt` (по одному слову на строку).

---

## Структура файлов

Новое в итерации 4:
```
Assets/WordGame/
├── Resources/
│   └── word_list.txt              ← создаётся editor-скриптом
└── Scripts/
    ├── Dictionary.cs              ← новое
    ├── WordValidator.cs           ← новое
    ├── WordBuilder.cs             ← изменён (валидация на release)
    ├── WordPreviewUI.cs           ← изменён (Flash методы)
    └── HexGrid.cs                 ← изменён (MakeCellsVacant)
```

---

## Параметры в инспекторе

**`GameController > WordValidator`:**
- **Min Word Length** = 2 — глобальный минимум. Локальный минимум на number-клетках (it.5) переопределяет это значение в большую сторону.

**`GameController > WordBuilder`:**
- Поле `Validator` — ссылка на WordValidator (выставляется автоматически editor-скриптом).

**`WordPreview > WordPreviewUI`:**
- **Normal Color** — оранжевый (текущее слово)
- **Success Color** — зелёный (✓)
- **Error Color** — красный (✗)
- **Flash Duration** = 0.7 сек

---

## Что НЕ сделано

- Number-клетки с минимальной длиной (it.5)
- Счёт за слова (it.5)
- Проверка на тупик (нет валидных слов → game over) (it.6)
- Гарантия решаемости при генерации поля (it.6, потребует evaluator на словаре)
- Анимации превращения букв в пустые (это в juice pass, it.8)

---

## Известные ограничения

- **Маленький словарь.** 100 слов — мало для уверенной игры. Часто будут попадаться валидные комбинации, но не из словаря (например, `CAR`, `CARS`, `CAB` — нет в списке). Если хочется поиграть «по-настоящему», можно вручную добавить нужные слова в `word_list.txt`.
- **Регенерация букв при отказе** не делается. То есть если нет валидных слов, можно зайти в тупик. Будем решать в it.6.
- **Слова с буквами Q/Z/X/J** в словаре отсутствуют — Scrabble-распределение делает их редкими, и слов под них не хватило бы.

---

## Идемпотентность

- `word_list.txt` создаётся только если файла нет. Если хочешь обновить — удали файл и перезапусти setup, или редактируй вручную.
- WordValidator находится по компоненту, не дублируется при повторном запуске setup.
- При повторном запуске setup ссылка в WordBuilder перезаписывается актуальным валидатором.

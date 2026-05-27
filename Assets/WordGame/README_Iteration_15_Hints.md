# WordGame — Итерация 15: Hints + IAP

## Что сделано

В игре появились **подсказки**. 5 стартовых, можно зарабатывать в игре или купить пачку через IAP.

### Hint механика
- **Стартовые 5 hints** в PlayerPrefs (`WG_Hints`), даются один раз при первом запуске
- **Кнопка `💡 N`** в правом нижнем углу HUD — показывает текущее количество
- **Тап на кнопку с count > 0:**
  - HintFinder находит первое валидное слово на доске (DFS, depth ≤ 8)
  - Подсветка: клетки загораются **по очереди** зелёным glow (от первой буквы к последней)
  - После полного появления — путь держится 3 секунды, потом fade out
  - Списывается 1 hint
- **Тап с count == 0:** открывается попап `OUT OF HINTS` с кнопкой `BUY 10 HINTS`
- **Тап во время свайпа слова** — игнорируется (чтобы не сбить выделение)

### Earning
- За **каждое валидное слово ≥ 6 букв** в любом режиме (Escape или Explore) — `+1 hint`
- В Endless Explore validator сбрасывается между stages → можно повторно собирать те же слова (но игрок реально набирает 6+, это не free farm)

### Кнопка hint в HUD
- Справа внизу HUD
- Лампочка зелёная (`💡`) + цифра справа
- Pulse-анимация при изменении count

---

## IAP — настройка вручную

Я НЕ создал `IAPButton` компонент автоматически — он зависит от Unity IAP package. README объясняет шаги.

### Шаг 1 — Unity Services + IAP package

1. `Window > Package Manager` → `In-App Purchasing` → версия **5.2.1** (у тебя уже стоит)
2. `Window > General > Services` → Connect project to Unity Cloud → Enable **In-App Purchasing**
3. В Services-панели IAP может попросить:
   - Google Play Public Key (если Android)
   - Возрастной рейтинг
   - Согласие — Test Mode для начала

### Шаг 2 — IAP Catalog

1. `Services > In-App Purchasing > IAP Catalog`
2. Добавить продукт:
   - **ID**: `com.tipspack.ten`
   - **Type**: **Consumable**
3. Сохранить, продукт появится в Catalog'е

### Шаг 3 — Привязка IAPButton к кнопке `BUY 10 HINTS`

Открой `Game.unity` → `Canvas/Popups/OutOfHintsPopup/Panel/BuyButton`.

1. **Add Component → IAPButton** (или `Codeless IAP / IAP Button`, имя зависит от версии)
2. В IAPButton инспекторе:
   - **Product ID**: вставить `com.tipspack.ten` (выпадающий список из Catalog'а)
   - **Button Type**: Purchase (default)
3. Скролл вниз — события:
   - **On Purchase Complete (Product)** → нажать `+` →
     - Object: перетащить сам **`OutOfHintsPopup`** GameObject (на нём уже сидит `HintIAPBridge`)
     - Function: **`HintIAPBridge → OnPurchaseComplete()`**
   - **On Purchase Failed (Product, PurchaseFailureDescription)** → нажать `+` →
     - Object: **`OutOfHintsPopup`**
     - Function: **`HintIAPBridge → OnPurchaseFailed()`**
4. Опционально, если есть событие fetch / product info loaded:
   - **`HintIAPBridge → OnProductFetched()`**

> UnityEvent инспектор передаст аргументы (Product, PurchaseFailureDescription) — это OK, мост игнорирует их и просто вызывает соответствующий метод `HintManager`.

### Шаг 4 — Build + тест

1. Build for Android/iOS (test mode)
2. Запустить, потратить все hints, открыть popup, нажать BUY 10 HINTS
3. В Test Mode IAP покажет fake purchase окно
4. После complete → `[HintManager] IAP success: granted 10 hints. Total: 15`

---

## Новые/изменённые файлы

```
Assets/WordGame/
├── Scripts/
│   ├── HintManager.cs              ← новое (PlayerPrefs, event)
│   ├── HintIAPBridge.cs            ← новое (мост для IAPButton UnityEvents)
│   ├── HintFinder.cs               ← новое (DFS поиск valid слова)
│   ├── HintHighlighter.cs          ← новое (последовательная подсветка)
│   ├── OutOfHintsPopup.cs          ← новое (попап без hints)
│   ├── GameHUD.cs                  ← обновлён (hint button + handler)
│   └── WordBuilder.cs              ← обновлён (HintManager.CheckAndRewardForWord для 6+ слов, IsActivelyBuilding getter)
└── Editor/
    └── SetupHintsIteration15.cs    ← новое
```

---

## Установка

1. Распаковать архив поверх проекта.
2. Дождаться компиляции (без IAP package скрипты компилируются нормально — `HintIAPBridge` не использует IAP API).
3. Запустить: `WordGame > Setup Hints (Iteration 15)`
4. (Опционально) Сделать шаги IAP-настройки выше.
5. `MainMenu.unity` → Play.

> Можно играть и без IAP-настройки — кнопка BUY 10 HINTS будет неактивной placeholder'ом, но hints можно зарабатывать в игре (free path работает всегда).

---

## Тестирование

| Сценарий | Ожидание |
|---|---|
| Первый запуск игры | В HUD `💡 5` |
| Тап на hint | На доске путь к валидному слову загорается зелёным по буквам, держится 3 сек, fade out. Count `💡 4` |
| Использовать все 5 | После 5-го: count `💡 0` |
| Тап с 0 hints | Открывается popup `OUT OF HINTS` с кнопкой `BUY 10 HINTS` |
| Собрать слово 6 букв (`STREAM`, `PLAYER`) | `💡 1` сразу → +1 visible pulse-анимация |
| Собрать 5 букв (`STARE`) | Hints не изменились |
| Собрать слово во время hint highlight | Работает; highlight продолжается своими anims |
| Тап hint во время свайпа слова | Игнорируется |
| Pan mode + тап hint | Работает (hint независим от pan) |
| BACK → MainMenu → новая партия | Hints сохранились (PlayerPrefs) |
| IAP buy → confirm | Count `💡 +10` после успешного fake-purchase в test mode |
| IAP cancel | Без изменений, в Console: `IAP: purchase failed or cancelled` |

---

## Параметры

**`HintHighlighter`** в инспекторе:
- `Glow Color` — зелёный `#4FCC81`
- `Per Cell Delay` = 0.18 (пауза между подсветкой соседних клеток)
- `Fade In Duration` = 0.22
- `Hold Duration` = 3.0 (показ после полного отображения)
- `Fade Out Duration` = 0.45
- `Glow Max Alpha` = 0.7
- `Glow Padding` = 12 (насколько overlay больше hex'а — чтобы glow «торчал» вокруг)

**`HintManager`** константы (в коде, не в инспекторе):
- `STARTER_HINTS = 5`
- `MIN_WORD_LENGTH_FOR_HINT = 6`
- `IAP_PACK_AMOUNT = 10`

**`HintFinder`** константы:
- `MAX_DEPTH = 8` (макс длина слова для поиска)
- `MIN_LENGTH = 3` (мин длина hint-слова, если number-клеток нет)

---

## Известные нюансы

- **HintFinder использует placement pool** (401 слово, маленький словарь), не 466k. Это для скорости — DFS по 466k слов медленный. Подсказка всегда показывает «базовое» слово, не редкое
- **Если игрок собирает слово которое hint указал**, hint засчитывается как обычное word; если 6+ букв — `+1 hint` обратно. То есть hint'ы можно «прокручивать», но это нормальный flow
- **Hint highlight не блокирует input** — игрок может во время highlight тапать клетки и собирать слово. Это by-design, чтобы не мешать темпу
- **HintHighlighter glow находится между hex bg и letter** (`SetAsFirstSibling`) — буква остаётся читаемой
- **GameStats не считает hints как отдельную статистику** — можно добавить если захочешь "Total hints used / earned"

---

## Что НЕ сделано

- **Анимация number-клеток**: hint не показывает дополнительной info про number-клетки в пути. Если path содержит number-клетку, она просто подсветится зелёным как все остальные
- **Sound для hint** — пока нет отдельного звука. Можно добавить в `SoundManager` метод `PlayHint` и вызывать в `HintHighlighter.Highlight`
- **Tutorial для hints** — текущий tutorial не объясняет hint-кнопку. Можно расширить tutorial later, но это отдельная итерация
- **Restore Purchases** — кнопка отдельная для iOS требуется по гайдлайнам Apple. Добавь в Settings popup → `IAPListener` с продуктом
- **Free-to-play balance**: возможно 6 букв — слишком частое, игрок будет накапливать. Если хочешь раритет — увеличь до 7 в `HintManager.MIN_WORD_LENGTH_FOR_HINT`

---

## Идемпотентность

Editor-скрипт можно запускать повторно — все элементы ищутся по имени, переиспользуются, ссылки переписываются. **IAPButton, прикреплённый вручную к `BuyButton`, скриптом НЕ затрагивается** — он остаётся как ты настроил.

---

## Это финальная итерация плана.

Все 15 итераций roadmap'а завершены:
1. Foundation + MainMenu ✓
2. GameScene + HexGrid ✓
3. Swipe + WordBuilding ✓
4. Dictionary + Validation + VacantGrowth + Generator ✓
5. NumberCells + Score ✓
6. ExploreMode ✓
7. EscapeMode + Timer ✓
8. JuicePass ✓
9. Tutorial ✓
10. Stats + Polish ✓
11. BigDictionary ✓
12. LevelsEscape ✓
13. PanControls ✓
14. EndlessExplore ✓
15. **Hints + IAP** ✓

После завершения IAP-настройки — игра готова к Build for iOS.

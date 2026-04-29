# WordGame — Итерация 8: Juice Pass

## Что сделано

Игра стала ощутимо «живее»: добавлены анимации и звуки на все ключевые события. Звуки **сгенерированы кодом** (синусы и noise через `AudioClip.Create`) — без файлов, работают сразу из коробки.

### Анимации
- **Letter pop при выделении.** Когда буква в клетке становится оранжевой — она делает короткий punch (scale +30%).
- **Vacant transition.** Когда клетка превращается в пустую — лёгкий "взрыв": увеличение в 1.2× → возврат с OutBack ease, буква fade-in заново. Сопровождается звуковым "поп".
- **Floating score popup.** При засчитанном слове над центром выделения всплывает зелёный текст `+24` или `+50 ×2` с time-bonus. Масштабируется с OutBack, поднимается на 220px и fade out.
- **Screen shake.** При невалидном слове Canvas трясётся (DOShakeAnchorPos, strength 25, duration 0.25s).
- **Edge highlight в Escape.** Все 18 клеток на краю поля окружены зелёным glow с пульсацией alpha (0.2 ↔ 0.55, yoyo). Глоу — отдельный Image позади клетки.

### Звуки (процедурные)
| Событие | Звук |
|---|---|
| Кнопка | короткий 800Hz click |
| Старт выделения слова | sweep 440→640Hz |
| Каждая добавленная клетка | повышающийся тон (chromatic от 440Hz × 1.06ⁿ) |
| Валидное слово | мажорное аккордовое восхождение (C-E-G) |
| Невалидное слово | низкий saw, спадающий по частоте |
| Vacant pop | спадающий sine + noise |
| Tick на последних 10 сек Escape | короткий 1200Hz |
| Win | C-E-G-C мажорная фанфара |
| Lose | минорное спадающее G-Eb-Bb |
| Popup open / close | sine sweep (вверх / вниз) |

Все звуки генерируются один раз в `SoundManager.Awake()` через статический класс `ProceduralSounds`. Громкость управляется существующими настройками в Settings → SFX.

### Edge highlight в Escape
- Активируется только в Escape-режиме через `EdgeHighlighter.Activate()`
- Зелёные glow-image позади всех 18 клеток края
- Пульсируют alpha 0.2 ↔ 0.55 с периодом 1.4 сек (yoyo)
- Сразу видно, куда нужно «прорваться»

---

## Новые/изменённые файлы

```
Assets/WordGame/
├── Scripts/
│   ├── ProceduralSounds.cs           ← новое (генератор звуковых клипов)
│   ├── FloatingScorePopup.cs         ← новое (всплывающий +score)
│   ├── ScreenShaker.cs               ← новое (трясёт Canvas через DOTween)
│   ├── EdgeHighlighter.cs            ← новое (зелёная подсветка края)
│   ├── SoundManager.cs               ← переписан (auto-generate clips, новые методы)
│   ├── HexCell.cs                    ← обновлён (анимации pop и vacant)
│   ├── TimerHUD.cs                   ← обновлён (tick sound на каждой секунде ≤10)
│   ├── WordBuilder.cs                ← обновлён (звуки, shake, floating popup)
│   └── GameController.cs             ← обновлён (активация EdgeHighlighter)
└── Editor/
    └── SetupJuicePassIteration8.cs   ← новое
```

---

## Установка

1. Распаковать архив поверх проекта.
2. Дождаться компиляции.
3. Запустить: `WordGame > Setup Juice Pass (Iteration 8)`
4. `MainMenu.unity` → Play.

**Важно:** SoundManager переписан. Если у тебя в `MainMenu.unity` уже был привязан `SoundManager` (через iteration 1) с какими-то AudioClip-полями (`buttonClickClip`, `popupOpenClip`, `popupCloseClip`) — они исчезли, теперь клипы генерируются сами. Сериализованные ссылки на `musicSource` и `sfxSource` остались, ничего повторно настраивать не нужно.

---

## Тестирование

| Сценарий | Ожидание |
|---|---|
| MainMenu, нажать кнопку | Click sound + press-feedback |
| MainMenu, открыть Settings | Popup-open sound |
| Game-сцена, тапнуть на стартовую клетку | Sweep up sound + клетка + буква pop |
| Свайп по соседям | Каждая клетка добавляет повышающийся тон. Буква pop в каждой |
| Release с валидным словом | Мажорная фанфара + зелёный `+24` всплывает + клетки делают "взрыв" с pop sound |
| Release с невалидным словом | Низкий buzz + Canvas трясётся |
| Слово через number-клетку с бонусом | `+72 ×2` всплывает, цифры в number-клетке остаются (на других клетках исчезают вместе с буквами) |
| Escape: запуск | Все 18 клеток края окружены зелёным glow, пульсируют |
| Escape: последние 10 секунд | Каждую секунду — короткий tick. Цифра таймера красная и пульсирует |
| Escape: достичь края | Win sound (восходящая фанфара) + зелёный popup `ESCAPED!` |
| Тупик в любом режиме | Lose sound (минорный спад) + соответствующий popup |
| Escape: timeout | Lose sound + `TIME'S UP` popup |

### Где можно убедиться что звук работает
В Settings ползунок **SFX** — на 0 звуки выключаются, на 100% играют. Если ничего не слышно — проверь:
1. Громкость системы и Unity-плеера
2. Настройки SFX в игре (Settings)
3. AudioSource на SoundManager-объекте в MainMenu сцене (должно быть `Mute = false`, `Output = None` (default Master))

---

## Параметры в инспекторе

**`ScreenShaker`** на Canvas:
- `Target` — RectTransform для тряски (выставлен на сам Canvas)

**`EdgeHighlighter`** на GameController:
- `Highlight Color` — цвет glow (зелёный 50% alpha)
- `Pulse Alpha Min/Max` — границы пульса (0.2 / 0.55)
- `Pulse Duration` — период пульса (1.4 сек)
- `Scale Multiplier` — насколько glow больше клетки (1.18×)

**`WordBuilder`** новое поле:
- `Floating Scores Parent` — контейнер для всплывающих `+score` (выставлен на FloatingScores-объект)

---

## Что НЕ сделано

- **Музыка фоном** — пропущено по запросу. Слот `musicSource` в SoundManager есть, можешь сам подкинуть AudioClip и поставить на воспроизведение
- **Particles при vacant** — нынешний "взрыв" через scale достаточно хорошо передаёт ощущение. Реальные partikles я бы делал через Unity Particle System, что выходит за рамки UI-only архитектуры
- **Tutorial overlay** при первом запуске — итерация 9
- **Stats / retention** — итерация 10

---

## Замечания по звукам

Процедурные звуки **не самые приятные** на слух — это базовые синусы с envelope. Если решишь заменить на качественные клипы:
1. Добавь поля в `SoundManager` (например, `public AudioClip customClickClip`)
2. В методе `Play[Что-то]` сначала проверь custom clip, иначе fallback на процедурный
3. Привяжи в инспекторе

Например, добавишь публичное `customSuccessClip`, и в `PlaySuccess()` сделаешь `Play(customSuccessClip ?? successClip)` — и сразу будет красивее.

---

## Идемпотентность

Editor-скрипт можно запускать повторно: `ScreenShaker` и `EdgeHighlighter` — компоненты на существующих GameObject, `FloatingScores` — найдётся по имени. Все ссылки переписываются.

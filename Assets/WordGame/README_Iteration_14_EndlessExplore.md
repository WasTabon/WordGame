# WordGame — Итерация 14: Endless Explore

## Что сделано

Explore-режим теперь **бесконечный**. При тупике (нет валидных слов) поле перегенерируется автоматически, счёт сохраняется и копится. Игра продолжается пока игрок сам не выйдет через BACK.

### Continue-механика
- Player собирает слова до тупика → `DeadlockDetector` возвращает false
- В **Explore**: показывается полупрозрачный попап `STAGE 2 / Board cleared! New stage starting...` на 1.5 секунды
- Параллельно: поле перегенерируется через `BoardGenerator.Generate`, validator's used-words сбрасывается, number-клетки переразмещаются
- Score **не сбрасывается** — продолжает копиться. Игрок может накопить очень большой score
- В **Escape**: всё как раньше — попап `TRAPPED` или `TIME'S UP`, конец партии

### Stage tracker
- Отслеживается в `GameController.CurrentExploreStage` (не сохраняется в PlayerPrefs — только в сессии)
- HUD показывает `EXPLORE • STAGE 3` (на stage 1 — просто `EXPLORE`)
- Toast в центре показывает `STAGE N` при переходе

### Выход → сохранение score
- При BACK в Explore: `HighScoreManager.TrySetHighScore` вызывается с текущим score → если он больше предыдущего best, перезаписывается
- Также `RecordPlayedTime()` для статистики
- В Escape всё как раньше (high score сохраняется только при win/loss popup'е)

### Validator reset
При переходе на новый stage `WordValidator.ResetUsedWords()` сбрасывает список использованных. Поэтому слово, которое ты собрал на stage 1, можно снова собрать на stage 2.

---

## Новые/изменённые файлы

```
Assets/WordGame/
├── Scripts/
│   ├── StageToast.cs                  ← новое (центральный STAGE N тост)
│   ├── GameController.cs              ← переписан (ContinueExplore, BuildBoard, SaveExploreProgressOnExit)
│   ├── WordBuilder.cs                 ← обновлён (в Explore deadlock → continue + ClearAndUnlock)
│   └── GameHUD.cs                     ← обновлён (показ STAGE N, save on back)
└── Editor/
    └── SetupEndlessExploreIteration14.cs   ← новое
```

---

## Установка

1. Распаковать архив поверх проекта.
2. Дождаться компиляции.
3. Запустить: `WordGame > Setup Endless Explore (Iteration 14)`
4. `MainMenu.unity` → Play.

---

## Тестирование

| Сценарий | Ожидание |
|---|---|
| MainMenu → PLAY → **Explore** | Игра как обычно, в HUD `EXPLORE` |
| Собирать слова до тупика | Появляется зелёный попап `STAGE 2 / Board cleared!`, держится 1.5 сек |
| Параллельно | Поле перегенерируется, есть свежие буквы и number-клетки, score **не сбросился** |
| После toast | HUD показывает `EXPLORE • STAGE 2`, можно играть дальше |
| Собрать слово которое уже использовалось на stage 1 | Засчитывается (used words сбросились) |
| Собрать несколько stages подряд | HUD `EXPLORE • STAGE 3`, `STAGE 4`, score растёт без ограничений |
| BACK | Возврат в MainMenu, в попапе выбора режима: `Best: <твой score>` обновился если был лучшим |
| MainMenu → PLAY → **Escape** | Всё как раньше: тупик = TRAPPED popup, конец партии |

---

## Параметры

**`StageToast`** в инспекторе:
- `Scale Up Duration` = 0.35
- `Hold Duration` = 1.0
- `Fade Out Duration` = 0.45

Суммарная длительность ~1.95 сек. Уменьши `Hold Duration` если хочешь быстрее.

---

## Известные нюансы

- **Number-клетки могут случайно встать на новое расположение** — это норма, поле полностью свежее
- **Floating score popups и edge highlighter** — Edge highlighter в Explore выключен (он только для Escape), floating scores работают как раньше
- **Pan position сбрасывается** при переходе на новый stage — поле центрируется
- **GameStats.GamesTotal** инкрементируется **один раз** при старте партии. Continue stages не считаются как отдельные игры
- **High score** записывается **во время игры** (при BACK), не в конце. Если приложение крашнется — score теряется. Но в нормальном UX игрок всегда выходит через Back

---

## Что НЕ сделано

- **Time per stage tracking** — сколько игрок провёл на каждой stage, не считается. Просто общее время партии в `TimePlayed`
- **Stage milestones / achievements** — типа "Reach Stage 10". Можно добавить в Stats отдельно если захочешь
- **Сложность от stage** — пока number-клетки и параметры одинаковые. Можно добавлять больше number-клеток на высоких stages для challenge

Следующая итерация (15): **Hints** — внутриигровая валюта, кнопка hint, подсветка валидного слова.

---

## Идемпотентность

Editor-скрипт можно запускать повторно — `StageToast` ищется по имени в Canvas. Все ссылки переписываются.

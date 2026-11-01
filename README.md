# Coin Catcher

A simple 2D Unity game: move the orange platform to catch falling coins and dodge the bombs.

## How to play

| Key | Action |
| --- | --- |
| ← / → or A / D | Move the platform |
| Space | Restart after Game Over |

- 🟡 **Coin**: +1 score
- 💣 **Bomb**: lose a heart (you have 3)
- Lose all 3 hearts and it's **Game Over**

## Run it yourself

1. Install **Unity 6000.5.9f1** via [Unity Hub](https://unity.com/download). Other Unity 6 versions will probably work too.
2. Clone the repo:
   ```bash
   git clone https://github.com/GT-Snix/Coin-Catcher.git
   ```
3. In Unity Hub, click **Add → Add project from disk** and pick the `Coin-Catcher` folder.
4. Open `Assets/Scenes/SampleScene` and press **Play** ▶️

## Using the code

All the game logic is in `Assets/Scripts/`:

| Script | What it does |
| --- | --- |
| `Catcher.cs` | Moves the platform left and right |
| `CoinSpawner.cs` | Drops coins and bombs at random positions |
| `FallingObject.cs` | Shared falling and catch logic |
| `Coin.cs` / `Bomb.cs` | What happens when each one is caught |
| `GameManager.cs` | Score, hearts, Game Over screen and restart |

The spawner and game manager set themselves up when you press Play, so you can copy the `Scripts` folder into any 2D project, give your platform the `Catcher` script, and it just works.

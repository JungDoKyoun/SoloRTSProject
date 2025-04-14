using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameMod
{
    Single,
    Multiplayer
}

public static class GameModManager
{
    private static GameMod _currentMode = GameMod.Single;
    public static GameMod CurrentMode { get { return _currentMode; } }

    public static bool IsMultiplayer => _currentMode == GameMod.Multiplayer;

    public static void SetMod(GameMod mod)
    {
        _currentMode = mod;
    }
}

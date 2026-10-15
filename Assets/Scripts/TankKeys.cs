using UnityEngine;
using UnityEngine.InputSystem;

// Lectura del teclado para los dos jugadores (nuevo Input System).
// Jugador 1: WASD + Espacio.  Jugador 2: Flechas + Enter.
public static class TankKeys
{
    public static float GetMove(int player)
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return 0f;
        if (player == 1)
            return (kb.wKey.isPressed ? 1f : 0f) - (kb.sKey.isPressed ? 1f : 0f);
        return (kb.upArrowKey.isPressed ? 1f : 0f) - (kb.downArrowKey.isPressed ? 1f : 0f);
    }

    public static float GetTurn(int player)
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return 0f;
        if (player == 1)
            return (kb.dKey.isPressed ? 1f : 0f) - (kb.aKey.isPressed ? 1f : 0f);
        return (kb.rightArrowKey.isPressed ? 1f : 0f) - (kb.leftArrowKey.isPressed ? 1f : 0f);
    }

    public static bool FireDown(int player)
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return false;
        return player == 1 ? kb.spaceKey.wasPressedThisFrame
                           : (kb.enterKey.wasPressedThisFrame || kb.numpadEnterKey.wasPressedThisFrame);
    }

    public static bool FireHeld(int player)
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return false;
        return player == 1 ? kb.spaceKey.isPressed
                           : (kb.enterKey.isPressed || kb.numpadEnterKey.isPressed);
    }

    public static bool FireUp(int player)
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return false;
        return player == 1 ? kb.spaceKey.wasReleasedThisFrame
                           : (kb.enterKey.wasReleasedThisFrame || kb.numpadEnterKey.wasReleasedThisFrame);
    }
}
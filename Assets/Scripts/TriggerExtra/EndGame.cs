using UnityEngine;
using UnityEngine.InputSystem;

public class EndGame : MonoBehaviour
{
    /// <summary>Disables keyboard input through Unity's Input System.</summary>
    public void DisableKeyboardInput()
    {
        if (Keyboard.current != null)
        {
            InputSystem.DisableDevice(Keyboard.current);
        }
    }

    /// <summary>Disables mouse input through Unity's Input System.</summary>
    public void DisableMouseInput()
    {
        if (Mouse.current != null)
        {
            InputSystem.DisableDevice(Mouse.current);
        }
    }

    /// <summary>Quits the built game. In the Editor, stops Play mode.</summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

using UnityEngine;
using UnityEngine.UI;

public class GamePauseButton : MonoBehaviour
{
    public Button pauseButton;

    private void Start()
    {
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(TogglePause);
        }
    }

    private void TogglePause()
    {
        if (GameTimeController.IsPaused)
        {
            GameTimeController.Unpause();
        }
        else
        {
            GameTimeController.Pause();
        }
    }
}
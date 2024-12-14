using UnityEngine;
using UnityEngine.UI;

public class CardProgressUI : MonoBehaviour
{
    public Slider progressBar;
    private CardController cardController => GetComponentInParent<CardController>();

    void Update()
    {
        CalculateSliderValues();
        // Check if CardController and progressBar are valid
        if (cardController != null && progressBar != null)
        {
            // Update the progress bar with the current task progress
            progressBar.value = cardController.taskProgress;
        }
    }

    private void CalculateSliderValues()
    {
        if (progressBar != null && cardController != null)
        {
            progressBar.maxValue = cardController.taskDuration;  // Set the max value to match the task duration
            progressBar.value = 0f;  // Initialize the progress to 0
        } else
        {
            Debug.LogError("Progress Bar or CardController not assigned!");
        }
    }
}

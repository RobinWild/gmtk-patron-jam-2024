using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;  // For accessing UI components like Slider

public class CardProgressUI : MonoBehaviour
{
    public Slider progressBar;  // Reference to the Slider component
    private CardController cardController;  // Reference to the CardController component

    void Start()
    {
        // Get reference to CardController
        cardController = GetComponentInParent<CardController>();  // Assuming the UI is a child of the same parent as CardController
        
        // Initialize the progress bar
        if (progressBar != null && cardController != null)
        {
            progressBar.maxValue = cardController.taskDuration;  // Set the max value to match the task duration
            progressBar.value = 0f;  // Initialize the progress to 0
        }
        else
        {
            Debug.LogError("Progress Bar or CardController not assigned!");
        }
    }

    void Update()
    {
        // Check if CardController and progressBar are valid
        if (cardController != null && progressBar != null)
        {
            // Update the progress bar with the current task progress
            progressBar.value = cardController.taskProgress;
        }
    }
}

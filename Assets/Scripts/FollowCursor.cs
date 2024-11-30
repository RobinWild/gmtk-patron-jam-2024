using UnityEngine;

public class FollowCursor : MonoBehaviour
{
    private float screenWidth;
    private float screenHeight;

    void Start()
    {
        GetScreenSize();

        Cursor.visible = false;
    }

    void Update()
    {
        // Convert mouse position from screen space to world space
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.WorldToScreenPoint(transform.position).z; // Retain the current z position

        // Convert screen space to world space
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        // Update the GameObject's position, retaining the original z position
        transform.position = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);


        // Check if the cursor is outside the game view bounds
        if (IsCursorOutsideBounds())
        {
            // Show the hardware cursor when it leaves the bounds
            Cursor.visible = true;
        }
        else
        {
            // Hide the hardware cursor when inside the game view
            Cursor.visible = false;
        }
    }

    bool IsCursorOutsideBounds()
    {
        // Get the current position of the cursor
        Vector3 cursorPosition = Input.mousePosition;

        // Check if the cursor is outside the screen bounds (considering screen width and height)
        return cursorPosition.x < 0 || cursorPosition.x > screenWidth || cursorPosition.y < 0 || cursorPosition.y > screenHeight;
    }

    void OnApplicationFocus(bool hasFocus)
    {
        GetScreenSize();
        // If the application gains focus, ensure the cursor is hidden and confined
        if (hasFocus)
        {
            Cursor.visible = false;
        }
    }

    void GetScreenSize()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
    }

}

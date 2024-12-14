using System.Diagnostics;


public static class GameTimeController
{
    private static float gameTime = 0f;
    private static float previousGameTime = 0f;
    private static bool isPaused = false;

    public static float GameTime => gameTime;

    public static bool IsPaused => isPaused;

    public static float DeltaTime
    {
        get
        {
            if (isPaused)
                return 0f;
            else
                return gameTime - previousGameTime;
        }
    }

    public static void Update(float deltaTime)
    {
        if (!isPaused)
        {
            previousGameTime = gameTime;
            gameTime += deltaTime;
        }
    }

    public static void Pause()
    {
        isPaused = true;
    }

    public static void Unpause()
    {
        isPaused = false;
    }

    public static void Reset()
    {
        gameTime = 0f;
        previousGameTime = 0f;
    }
}

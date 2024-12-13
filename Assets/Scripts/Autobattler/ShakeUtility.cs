using UnityEngine;
using DG.Tweening;

public static class ShakeUtility
{
    /// <summary>
    /// Shakes a Transform and returns it to its original position after the shake.
    /// </summary>
    /// <param name="target">The Transform to shake.</param>
    /// <param name="duration">The duration of the shake.</param>
    /// <param name="strength">The intensity of the shake.</param>
    /// <param name="vibrato">The number of vibrations during the shake.</param>
    /// <param name="randomness">The randomness of the shake movement.</param>
    /// <param name="returnDuration">The time it takes to return to the original position.</param>
    public static void ShakeAndReturn(Transform target, float duration, float strength = 1f, int vibrato = 10, float randomness = 90f, float returnDuration = 0.3f)
    {
        if (target == null)
        {
            Debug.LogError("Target Transform is null! Cannot shake.");
            return;
        }

        // Store the initial position
        Vector3 initialPosition = target.localPosition;

        // Create a Sequence
        Sequence shakeSequence = DOTween.Sequence();

        // Add the shake
        shakeSequence.Append(target.DOShakePosition(duration, strength, vibrato, randomness, fadeOut: true));

        // Return to the initial position
        shakeSequence.Append(target.DOLocalMove(initialPosition, returnDuration).SetEase(Ease.OutQuad));
    }


    /// <summary>
    /// Scales a Transform out and then back in to its original scale.
    /// </summary>
    /// <param name="target">The Transform to scale.</param>
    /// <param name="scaleFactor">The factor by which to scale the object out. E.g., 1.5f to scale 1.5 times bigger.</param>
    /// <param name="durationOut">The duration for the scaling out effect.</param>
    /// <param name="delay">The delay before the scaling starts.</param>
    /// <param name="durationIn">The duration for the scaling in effect.</param>
    public static void ScaleOutAndBack(Transform target, float scaleFactor = 1.5f, float durationOut = 0.5f, float delay = 0f, float durationIn = 0.5f)
    {
        if (target == null)
        {
            Debug.LogError("Target Transform is null! Cannot scale.");
            return;
        }

        // Store the initial scale
        Vector3 initialScale = target.localScale;

        // Create a Sequence for scaling
        Sequence scaleSequence = DOTween.Sequence();

        // Scale out
        scaleSequence.Append(target.DOScale(initialScale * scaleFactor, durationOut).SetEase(Ease.OutCubic));

        // Apply delay before starting the scaling action
        scaleSequence.AppendInterval(delay);

        // Scale back in to original scale
        scaleSequence.Append(target.DOScale(initialScale, durationIn).SetEase(Ease.InOutQuad));
    }
}

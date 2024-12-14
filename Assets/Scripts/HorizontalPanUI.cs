using UnityEngine;
using DG.Tweening;

public class HorizontalPanUI : MonoBehaviour
{
    public float panSpeed = 200f;   // Speed of panning with the mouse wheel
    public float panDuration = 0.3f; // Duration of the panning animation

    private RectTransform _rectTransform; // Cached RectTransform of this GameObject
    private Vector3 _targetPosition;     // The target position for panning

    void Start()
    {
        // Cache the RectTransform of the current GameObject
        _rectTransform = GetComponent<RectTransform>();
        if (_rectTransform == null)
        {
            Debug.LogError("This script must be attached to a GameObject with a RectTransform.");
            return;
        }

        // Initialize the target position to the current position
        _targetPosition = _rectTransform.anchoredPosition;
    }

    void Update()
    {
        float scrollDelta = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scrollDelta) > 0.01f)
        {
            PanHorizontally(scrollDelta);
        }
    }

    void PanHorizontally(float scrollDelta)
    {
        // Calculate the pan distance based on the scroll delta and speed
        float panDistance = scrollDelta * panSpeed;

        // Update the target position
        _targetPosition.x += panDistance;

        // Smoothly move the RectTransform to the new position using DOTween
        _rectTransform.DOAnchorPos(_targetPosition, panDuration).SetEase(Ease.OutQuad);
    }
}

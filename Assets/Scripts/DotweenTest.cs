using UnityEngine;
using DG.Tweening;

public class DotweenTest : MonoBehaviour
{
    public float duration = 1;
    public float speed;
    private Vector3 myOffset;

    void Start()
    {
        myOffset = transform.position;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0)) DoMove();
    }

    void DoMove()
    {
        transform.DOKill();

        Vector3 targetPosition = GetMouseWorldPosition();
        targetPosition.z = transform.position.z;
        transform.DOMove(targetPosition + myOffset, duration * (1/speed)).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            TurnManager.Instance.NotifyActionCompleted(gameObject);
        });

        TurnManager.Instance.NotifyActionStarted(gameObject);


    }

    public Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }
}

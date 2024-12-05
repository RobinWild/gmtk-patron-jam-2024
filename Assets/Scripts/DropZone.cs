    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using DG.Tweening;

    public class DropZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
    {
        [SerializeField] private Vector3 dropOffset;
        public Vector3 DropPosition => transform.localPosition + dropOffset;
        public Color unselectedColour;
        public Color selectedColour;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (DragAndDrop.Active != null)
            {
                DragAndDrop.Active.SetDropZone(this);
                GetComponent<Image>().DOColor(selectedColour, 0.5f);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (DragAndDrop.Active != null)
            {
                DragAndDrop.Active.SetDropZone(null);
                GetComponent<Image>().DOColor(unselectedColour, 0.5f);
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            GetComponent<Image>().DOColor(unselectedColour, 0.1f);
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(transform.position + dropOffset, Vector3.one * 10);
        }
    }
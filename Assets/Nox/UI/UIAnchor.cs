using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class AnchorHPToTop : MonoBehaviour
{
    public float distanceFromTop = 10f; // This is the distance from the top of the screen. Adjust as needed.

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        Vector2 anchoredPosition = rectTransform.anchoredPosition;
        anchoredPosition.y = -distanceFromTop; // Directly set the distance from the top edge
        rectTransform.anchoredPosition = anchoredPosition;
    }
}

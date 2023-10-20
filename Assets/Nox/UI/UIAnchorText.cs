using UnityEngine;
using TMPro;

[RequireComponent(typeof(RectTransform))]
public class AnchorTextMeshProToTop : MonoBehaviour
{
    public float distanceFromTop = 10f;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        Vector2 anchoredPosition = rectTransform.anchoredPosition;
        anchoredPosition.y = -distanceFromTop;  // We only need to adjust based on the desired distance from top
        rectTransform.anchoredPosition = anchoredPosition;
    }
}

using UnityEngine;
using UnityEngine.UI;

public class CursorFollow : MonoBehaviour
{
    private RectTransform rectTransform;
    private RectTransform canvasRectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    private void Update()
    {
        Vector2 anchoredPosition;

        if (IsUsingController())
        {
            anchoredPosition = rectTransform.anchoredPosition + new Vector2(Input.GetAxis("HorizontalRightStick"), Input.GetAxis("VerticalRightStick")) * 10f;
        }
        else
        {
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, Input.mousePosition, null, out localPoint))
            {
                anchoredPosition = localPoint;
            }
            else
            {
                return; // Don't update position if the conversion failed
            }
        }

        rectTransform.anchoredPosition = anchoredPosition;
    }

    private bool IsUsingController()
    {
        return Mathf.Abs(Input.GetAxis("HorizontalRightStick")) > 0.2f || Mathf.Abs(Input.GetAxis("VerticalRightStick")) > 0.2f;
    }
}

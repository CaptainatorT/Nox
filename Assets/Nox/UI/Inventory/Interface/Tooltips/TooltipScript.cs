using UnityEngine;
using TMPro;

public class TooltipScript : MonoBehaviour
{
    // Singleton instance
    public static TooltipScript Instance { get; private set; }

    public GameObject tooltipPanel;
    public TMP_Text TooltipText; // or TextMeshProUGUI if using TextMeshPro

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else 
        {
            Destroy(gameObject);
            return;
        }
    }

    public void ShowTooltip(string content)
    {
        tooltipPanel.SetActive(true);
        TooltipText.text = content;

        RectTransform tooltipRect = tooltipPanel.GetComponent<RectTransform>();
        tooltipRect.pivot = new Vector2(0, 1); // top-left pivot

        Vector3 offset = new Vector3(1, -1, 0); // adjust the offset values as needed
        tooltipPanel.transform.position = Input.mousePosition + offset;
    }



    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }
}

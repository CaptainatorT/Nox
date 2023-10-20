using UnityEngine;
using System.Collections;

public class DashAfterImage : MonoBehaviour
{
    public float afterImageDuration = 0.5f;
    public float fadeSpeed = 2.0f;

    private SpriteRenderer playerSprite;
    private SpriteRenderer ghostSprite;

    void Start()
    {
        playerSprite = GetComponent<SpriteRenderer>();
    }

    public void CreateAfterImage()
    {
        GameObject ghost = new GameObject("AfterImage");
        ghost.transform.position = transform.position;
        ghost.transform.rotation = transform.rotation;
        ghost.transform.localScale = transform.localScale;

        ghostSprite = ghost.AddComponent<SpriteRenderer>();
        ghostSprite.sprite = playerSprite.sprite;
        ghostSprite.color = new Color(1, 1, 1, 0.5f);  // Assuming 50% transparency

        StartCoroutine(FadeOutAfterImage(ghost));
    }

    private IEnumerator FadeOutAfterImage(GameObject ghost)
    {
        float alpha = ghostSprite.color.a;
        while (alpha > 0)
        {
            alpha -= fadeSpeed * Time.deltaTime;
            ghostSprite.color = new Color(1, 1, 1, alpha);
            yield return null;
        }
        Destroy(ghost);
    }
}

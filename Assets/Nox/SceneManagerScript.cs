using UnityEngine;

public class SceneManagerScript : MonoBehaviour
{
    public GameObject waveMaker;
    public GameObject gridMaker;
    public GameObject postProcessing;

    void Start()
    {
        EnsureObjectActive(waveMaker);
        EnsureObjectActive(gridMaker);
        EnsureObjectActive(postProcessing);
    }

    void EnsureObjectActive(GameObject obj)
    {
        if (obj != null && !obj.activeSelf)
        {
            obj.SetActive(true);
        }
    }
}

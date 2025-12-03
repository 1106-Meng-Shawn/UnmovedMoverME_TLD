using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AlphaBlocker : MonoBehaviour
{
    [Range(0f, 1f)]
    public float threshold = 0.1f;

    void Awake()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = threshold;
    }

}


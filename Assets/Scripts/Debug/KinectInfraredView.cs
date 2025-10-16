using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KinectInfraredView : MonoBehaviour
{
    public InfraredSourceManager colorManager;
    public RawImage rawImage;

    void Awake()
    {
        rawImage.color = Color.white;
    }

    void Update()
    {
        if (!colorManager || !rawImage) return;
        rawImage.texture = colorManager.GetInfraredTexture();
    }
}

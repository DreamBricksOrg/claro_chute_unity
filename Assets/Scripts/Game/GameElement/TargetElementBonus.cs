using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class TargetElementBonus : MonoBehaviour, IElementState
{
    public Transform stageTransform;
    public float animTime = 0.75f;
    private GameObject connectedObject;
    public Renderer stageRenderer;
    private float actionTime = 3.0f;
    public StorageEntityData entityData;
    private bool isPlaying = false;

    void Awake()
    {
        stageTransform.transform.localRotation = Quaternion.Euler(-90, 0, 0);
    }

    void Start()
    {
        ClearElement();
    }

    public void OnBegin()
    {
        Invoke(nameof(OnPrepare), entityData.interval);
        isPlaying = true;
    }

    void GenerateElement()
    {
        ClearElement();
        if (entityData)
        {
            actionTime = entityData.actionTime;
            stageRenderer.material.color = entityData.color;
            stageRenderer.enabled = true;
            connectedObject = Instantiate(entityData.prefab, stageTransform);
            var comp = connectedObject.GetComponent<Entity>();
            comp.parentTransform = transform;
            comp.score = entityData.score;
        }
    }

    public void OnPrepare()
    {
        if (!isPlaying) return;
        GenerateElement();
        connectedObject.GetComponent<IElementState>()?.OnPrepare();
        stageTransform.DOKill();
        stageTransform.DOLocalRotate(new Vector3(0, 0, 0), animTime)
            .SetEase(Ease.OutBounce)
            .OnComplete(() =>
                {
                    Invoke(nameof(OnReset), actionTime);
                }
            );
    }

    public void OnPlay()
    {
        connectedObject.GetComponent<IElementState>()?.OnPlay();
    }

    public void OnReset()
    {
        if (!isPlaying) return;
        CancelInvoke();
        connectedObject.GetComponent<IElementState>()?.OnReset();
        stageRenderer.enabled = false;
        stageTransform.DOKill();
        stageTransform.DOLocalRotate(new Vector3(-90, 0, 0), animTime)
            .SetEase(Ease.InFlash)
            .OnComplete(() =>
                {
                    Invoke(nameof(OnPrepare), entityData.interval);
                }
            );
    }

    void ClearElement()
    {
        CancelInvoke();
        stageTransform.DOKill();
        if (connectedObject != null)
        {
            Destroy(connectedObject);
            connectedObject = null;
        }

        for (int i = 0; i < stageTransform.childCount; i++)
        {
            var child = stageTransform.GetChild(i);
            if (child != null)
            {
                Destroy(child.gameObject);
            }
        }
    }
    
    public void OnCancel()
    {
        isPlaying = false;
        ClearElement();
    }
}

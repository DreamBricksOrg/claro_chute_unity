using System;
using UnityEngine;

[CreateAssetMenu(fileName = "StorageEntityData", menuName = "ScriptableObjects/StorageEntityData", order = 1)]
public class StorageEntityData : ScriptableObject
{
    public EntityType type;
    public GameObject prefab;
    public int score = 0;
    public float interval = 2.0f;
    public float actionTime = 3.0f;
    public int chanceWeight = 1;
    public Color color = Color.white;
}


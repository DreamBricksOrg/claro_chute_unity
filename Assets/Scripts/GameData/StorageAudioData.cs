using System;
using UnityEngine;

[CreateAssetMenu(fileName = "StorageAudioData", menuName = "ScriptableObjects/StorageAudioData", order = 1)]
public class StorageAudioData : ScriptableObject
{
    public AudioTypes type;
    public AudioClip clip;
}


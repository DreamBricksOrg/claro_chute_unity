using System;
using UnityEngine;

[CreateAssetMenu(fileName = "RoundData", menuName = "ScriptableObjects/RoundData", order = 1)]
public class RoundData : ScriptableObject
{
    public float gameTime = 60f;
    public int score = 0;
}


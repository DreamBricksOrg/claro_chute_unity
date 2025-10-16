using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StorageController : MonoBehaviour
{
   public static StorageController Instance { get; private set; }
   private void Awake()
   {
      Instance = this;
   }

   [SerializeField] private List<StorageAudioData> audioList = new();
   [SerializeField] private List<StorageEntityData> entityList = new();

   public StorageAudioData GetAudio(AudioTypes type) => audioList.FirstOrDefault(a => a.type == type);
   public StorageEntityData GetEntity(EntityType type) => entityList.FirstOrDefault(e => e.type == type);


   public StorageEntityData GetRandomEntity()
   {
      if (entityList.Count == 0) return null;
      int totalWeight = entityList.Sum(e => e.chanceWeight);
      int randomValue = UnityEngine.Random.Range(0, totalWeight);
      int cumulativeWeight = 0;
      foreach (var entity in entityList)
      {
         cumulativeWeight += entity.chanceWeight;
         if (randomValue < cumulativeWeight)
         {
            return entity;
         }
      }
      return null;
   }


}

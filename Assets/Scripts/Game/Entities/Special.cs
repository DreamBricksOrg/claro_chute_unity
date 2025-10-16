using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Special : Entity
{

    override public void OnPlay()
    {
        base.OnPlay();
        fxParticle?.transform.SetParent(null, true);
        EventManager.Game.SetBallStyle(1);
    }
  
}

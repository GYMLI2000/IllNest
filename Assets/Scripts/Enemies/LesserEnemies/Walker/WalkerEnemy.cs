using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class WalkerEnemy : Enemy
{
    protected override void SetPoolKeys()
    {
        poolKey = "WalkerEnemy";
    }

    protected override void InitializeStats()
    {
        killParticleColor = new Color(227f/255f, 130f/255f, 34f/255f, 0.2f);
        chaseRange =  -1;
        damage = 1;
        speed =5;
        health = 4;
        idleState = new IdleState(this);
        chaseState = new ChaseState(this);
        knockback = 2f;

    }
}

using UnityEngine;
using UnityEngine.EventSystems;

public class WalkingEnemy : Enemy
{


    protected override void InitializeStats()
    {
        killParticleColor = new Color(102f / 255f, 43f / 255f, 0f, 0.2f);
        chaseRange =  -1; // porad chase state
        damage = 1;
        speed = 3;
        health = 10;
        poolKey = "WalkingEnemy";
    }
}

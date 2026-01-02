using UnityEngine;

public class OCDHitpoint : Enemy
{
    [SerializeField]
    private OCDEnemy parent;
    public int sequenceIndex;

    public Color hitpointColor;

    public SpriteRenderer sprite;


    public override void TakeHit(Player player, int damage, float knockback)
    {
        parent.HitHitpoint(sequenceIndex);
    }

    protected override void InitializeStats() { }

    protected override void Update() { }

}

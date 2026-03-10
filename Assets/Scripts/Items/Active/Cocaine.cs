using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Cocaine", menuName = "Items/Active/Cocaine")]
public class Cocaine : ActiveItem
{
    public float activateTime;
    public float duration;
    private Player player;

    public Cocaine()
    {
        chargeRequired = 6;
        duration = 5;
        activateTime = 0;
        currentCharge = 0;
        itemName = "Cocaine";
    }

    private void OnEnable()
    {
        chargeRequired = 6;
        duration = 5;
        activateTime = 0;
        currentCharge = 0;
        itemName = "Cocaine";
    }

    public override void OnAdd() { }

    public override void OnRemove() { }

    public override void UpdateItem(Player player)
    {
        if (isActive && activateTime + duration < Time.time)
        {
            this.player = player;
            RemoveEffect();
        }
    }

    protected override void Activate(Player player)
    {
        if (isActive) return;
        player.playerLight.color = new Color(1f, .5f, .5f);
        player.firerateMult += 2;
        player.movementSpeed += 6;
        player.projSpeed += 6;
        activateTime = Time.time;
        isActive = true;
    }

    protected override void RemoveEffect()
    {
        player.playerLight.color = Color.white;
        player.firerateMult -= 2;
        player.movementSpeed -= 6;
        player.projSpeed -= 6;
        isActive = false;
    }
}

using UnityEngine;

public class DamageLiquid : MonoBehaviour
{
    [SerializeField]
    private float damageInterval = 1.0f;
    private float lastDamageTime;


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && Time.time >= lastDamageTime + damageInterval)
        {
            Player player = collision.gameObject.GetComponentInParent<Player>();
            if (player.TakeDamage(1))
            {
                lastDamageTime = Time.time;
            }
        }
    }
}

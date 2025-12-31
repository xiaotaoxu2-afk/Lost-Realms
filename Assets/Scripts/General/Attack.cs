using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("攻击")] public int damage;

    public float attackRange;
    public int attackRate;

    private void OnTriggerStay2D(Collider2D other)
    {
        Property prop = other.GetComponent<Property>();
        if (prop != null)
        {
            prop.Takedamage(this);
        }
    }

    public void OnAttack(GameObject other)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            other = player;
        }

        if (other != null)
        {
            Property prop = other.GetComponent<Property>();
            if (prop != null)
            {
                prop.Takedamage(this);
            }
        }
    }
}

using UnityEngine;

public class TeleportControl : MonoBehaviour
{
    public GameObject teleport;
    public GameObject boss;
    
    private Property bossProperty;
    private float initialHealth;
    private bool isTeleportActivated = false;

    private void Start()
    {
        // 缓存组件引用以提高性能
        bossProperty = boss.GetComponent<Property>();
        if (bossProperty != null)
        {
            initialHealth = bossProperty.currentHealth;
        }
    }

    private void Update()
    {
        // 检查 Boss 是否死亡，且传送门尚未激活
        if (!isTeleportActivated && bossProperty != null && bossProperty.currentHealth <= 0)
        {
            teleport.SetActive(true);
            isTeleportActivated = true; // 避免重复激活
        }
    }
}

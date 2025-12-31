using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    public Vector2 Checkground;
    public Vector2 Rightground;
    public Vector2 Leftground;
    public float 检测半径;
    public LayerMask groundLayer;
    public bool isGround;
    public bool isRightground;
    public bool isLeftground;
    public bool isManual;
    public bool onWall;
    public bool isPlayer;
    private CapsuleCollider2D coll;
    private PlayerControl playercontrol;
    private Rigidbody2D rb;


    private void Awake()
    {
        coll = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        playercontrol = GetComponent<PlayerControl>();

        if (isManual)
        {
            Rightground = new Vector2((coll.bounds.size.x + coll.offset.x) / 2, coll.bounds.size.x / 2);
            Leftground = new Vector2(-Rightground.x, Rightground.y);
        }
    }

    private void Update()
    {
        Check();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + Checkground * transform.localScale, 检测半径);
        Gizmos.DrawWireSphere((Vector2)transform.position + Rightground, 检测半径);
        Gizmos.DrawWireSphere((Vector2)transform.position + Leftground, 检测半径);
    }

    public void Check()
    {
        if (onWall)
            isGround = Physics2D.OverlapCircle(
                (Vector2)transform.position + new Vector2(Checkground.x * transform.localScale.x, -0.185f), 检测半径,
                groundLayer);
        else
            isGround = Physics2D.OverlapCircle((Vector2)transform.position + Checkground * transform.localScale, 检测半径,
                groundLayer);


        isRightground = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(Rightground.x, Rightground.y),
            检测半径, groundLayer);
        isLeftground = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(Leftground.x, Leftground.y),
            检测半径, groundLayer);

        if (isPlayer)
            onWall = ((isRightground && playercontrol.inputDirection.x > 0) ||
                      (isLeftground && playercontrol.inputDirection.x < 0)) && rb.velocity.y < 0f;
    }
}
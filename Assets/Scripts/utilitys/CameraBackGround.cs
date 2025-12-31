using System.Collections;

using UnityEngine;

public class CameraBackGround : MonoBehaviour
{
    public Camera cam;
    public Transform subject;


    private Vector2 startPos;
    private float startZ;

    private Vector2 travel => (Vector2)cam.transform.position - startPos;


    private void Start()
    {
        startPos = transform.position;
        startZ = transform.position.z;
        if (cam == null)
        {
            cam = Camera.main; // 自动引用标签为 "MainCamera" 的相机
        }

        StartCoroutine(DelayedFindPlayer(5));
    }

    public void Update()
    {
        Vector2 newPos = startPos + travel * 0.8f;
        transform.position = new Vector3(newPos.x, newPos.y, startZ);
    }


    private IEnumerator DelayedFindPlayer(int maxAttempts)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                subject = player.transform;
                Debug.Log("attacker 自动赋值成功: " + player.name);
                yield break;
            }

            yield return null; // 等待一帧
        }

        Debug.LogError("未找到 Tag 为 'Player' 的对象，请手动赋值 attacker 或检查玩家 Tag。");
    }
}

using UnityEngine;

/// <summary>
/// 通过 Rigidbody 跟随 URDF link 的骨骼
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class FollowLinkRigidbody : MonoBehaviour
{
    [Header("机器人 URDF Link")]
    public Transform targetLink;

    [Header("是否跟随旋转")]
    public bool followRotation = true;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // 通过 MovePosition/MoveRotation 控制，不受力
    }

    void FixedUpdate()
    {
        if (targetLink == null) return;

        // 1️⃣ 跟随位置
        Vector3 targetPos = targetLink.position;
        rb.MovePosition(targetPos);

        // 2️⃣ 跟随旋转
        if (followRotation)
        {
            Quaternion targetRot = targetLink.rotation;
            rb.MoveRotation(targetRot);
        }
    }
}

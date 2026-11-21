using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float followSpeed = 10f;

    private bool warnedMissingTarget = false;

    private void LateUpdate()
    {
        if (target == null)
        {
            if (!warnedMissingTarget)
            {
                Debug.LogWarning($"[CameraFollow] No target assigned on {gameObject.name}. Camera will hold position until one is set.", this);
                warnedMissingTarget = true;
            }
            return;
        }

        Vector3 targetPosition = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            followSpeed * Time.deltaTime
        );
    }

    /// <summary>
    /// Assign or change the follow target at runtime (e.g. after spawning the player).
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        warnedMissingTarget = false;
    }
}

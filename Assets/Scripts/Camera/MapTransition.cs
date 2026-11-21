using UnityEngine;
using Unity.Cinemachine;

public class MapTransition : MonoBehaviour
{
    [SerializeField] private BoxCollider2D mapBoundary;

    [SerializeField] private CinemachineCamera virtualCamera;
    private CinemachineConfiner2D confiner;
    [SerializeField] private float nudgeAmount = 0.2f;

    private void Awake()
    {
        if (virtualCamera != null)
        {
            confiner = virtualCamera.GetComponent<CinemachineConfiner2D>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && confiner != null && mapBoundary != null)
        {
            confiner.BoundingShape2D = mapBoundary;
            UpdatePlayerPosition(collision);
        }
    }

    private void UpdatePlayerPosition(Collider2D collision)
    {
        // 1. بنجيب سكريبت الحركة عشان نعرف آخر اتجاه
        PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
        
        if (playerMovement == null) return;

        // 2. بنجيب الـ Rigidbody عشان نعمله Nudge آمن
        Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
        
        if (playerRb != null)
        {
            // 3. بنحسب الاتجاه ونزود عليه الدفعة الصغيرة
            Vector2 nudge = playerMovement.LastDirection * nudgeAmount;
            
            // 4. بنحركه في الـ FixedUpdate بتاع يونيتي عشان مفيش حاجات تتعارض
            playerRb.MovePosition(playerRb.position + nudge);
        }
    }
}
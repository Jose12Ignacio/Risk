using UnityEngine;

public class ClickTest : MonoBehaviour
{
    void Update()
    {
        if (UnityEngine.Input.GetMouseButtonDown(0))
        {
            // Convierte la posición del mouse al espacio de mundo
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);

            // Realiza el Raycast 2D exactamente en esa posición
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log($"🎯 Raycast golpeó: {hit.collider.gameObject.name}");
            }
            else
            {
                Debug.Log("❌ No golpeó nada.");
            }
        }
    }
}

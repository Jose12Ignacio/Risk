using UnityEngine;
[ExecuteAlways]
public class FitSpriteToCamera : MonoBehaviour {
  public SpriteRenderer sr;
  void Reset(){ sr = GetComponent<SpriteRenderer>(); }
  void LateUpdate(){
    if (!sr || !Camera.main) return;
    float h = 2f * Camera.main.orthographicSize;
    float w = h * Camera.main.aspect;
    Vector2 s = sr.sprite.bounds.size;              // tamaño del sprite en unidades
    transform.localScale = new Vector3(w/s.x, h/s.y, 1f);
    transform.position = new Vector3(Camera.main.transform.position.x,
                                     Camera.main.transform.position.y, 0f);
  }
}

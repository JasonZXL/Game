using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class AttackHitBox : MonoBehaviour
{
    void Start() => gameObject.SetActive(false);

    public void Activate() => gameObject.SetActive(true);
      
    
    
    public void Deactivate() => gameObject.SetActive(false);

    void OnTriggerEnter2D(Collider2D other) 
    {
        // 当碰撞体碰到任何物体时输出日志
        Debug.Log($"攻击命中: {other.name}", other.gameObject);
    }
}


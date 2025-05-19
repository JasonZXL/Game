using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float pressedYOffset = -0.2f; // 下沉距离
    [SerializeField] private float moveSpeed = 2f;        // 移动速度
    
    [Header("Target Object")]
    [SerializeField] private GameObject targetObject;     // 控制的门/机关
    
    private Vector3 originalPosition;
    private Vector3 targetPosition;
    private int objectsOnPlate = 0;

    void Start()
    {
        originalPosition = transform.position;
        targetPosition = originalPosition;
    }

    void Update()
    {
        // 平滑移动
        transform.position = Vector3.Lerp(
            transform.position, 
            targetPosition, 
            moveSpeed * Time.deltaTime
        );
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            objectsOnPlate++;
            UpdatePlateState();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            objectsOnPlate--;
            UpdatePlateState();
        }
    }

    void UpdatePlateState()
    {
        bool isPressed = objectsOnPlate > 0;
        
        // 设置目标位置
        targetPosition = isPressed ? 
            originalPosition + Vector3.down * pressedYOffset : 
            originalPosition;
            
        // 控制关联对象
        if (targetObject != null)
            targetObject.SetActive(!isPressed);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // ��������
using System;

public class Input_Control : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float interactionDistance = 1.5f;

    [Header("食材预制体")]
    [SerializeField] private KitchenStuffSO tomatoSO;
    [SerializeField] private KitchenStuffSO cheeseSO;
    [SerializeField] private KitchenStuffSO lettuceSO;
    // ... 可以继续添加更多食材

    // 事件声明
    public event Action OnDetectedReset;

    public event Action<LayerMask, GameObject> OnDetected; // 碰撞检测事件
    
    // 私有变量
    private Vector3 movement;
    private bool canInteract = false;
    private PlayerInputActions playerInputActions; 
    private LayerMask detectedLayer;
    private GameObject detectedObject; // 用于存储检测到的对象

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player_1.Enable();
        
        // 绑定输入事件
        playerInputActions.Player_1.Move.performed += OnMove;
        playerInputActions.Player_1.Move.canceled += OnMove;
        playerInputActions.Player_1.Jump.performed += OnJump;
        playerInputActions.Player_1.Interact.performed += OnInteract;
      
    }
    
    private void OnDestroy()
    {
        // 解绑输入事件
        playerInputActions.Player_1.Move.performed -= OnMove;
        playerInputActions.Player_1.Move.canceled -= OnMove;
        playerInputActions.Player_1.Jump.performed -= OnJump;
        playerInputActions.Player_1.Interact.performed -= OnInteract;
        playerInputActions.Player_1.Disable();
    }


    private void Update()
    {
        // 处理移动和旋转
        HandleMovement();
        HandleRotation();
        
        // 检测可交互物体
        CheckInteractable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 inputVector = context.ReadValue<Vector2>();
        movement = new Vector3(inputVector.x, 0, inputVector.y);
    }
    
    // 处理拾取物品按键 (E键)
    private void OnInteract(InputAction.CallbackContext context)
    {
        if (canInteract && detectedObject != null)
        {
            Debug.Log("尝试与物体交互");
            // 调用player的交互方法
            player.Interact(detectedObject);
            
        }
    }

    // 检测可交互物体
    private void CheckInteractable()
    {
        RaycastHit hit;
        bool detector_horizontal = Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out hit, interactionDistance);
        if (detector_horizontal)
        {
            detectedObject = hit.collider.gameObject;   
            detectedLayer = hit.collider.gameObject.layer;
            canInteract = true;
            OnDetected?.Invoke(detectedLayer, detectedObject);
            //Debug.Log("检测到了可交互物体");
        }
        else
        {
            canInteract = false;
            detectedObject = null;
            OnDetectedReset?.Invoke();
            //Debug.Log("未检测到可交互物体");
        }
    }
    
    private void OnJump(InputAction.CallbackContext context)
    {
        player.Jump(jumpForce);
    }
    
    private void HandleMovement()
    {
        player.Move(movement * moveSpeed);
    }
    
    private void HandleRotation()
    {
        if (movement.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            player.Rotate(targetRotation, rotationSpeed);
        }
    }
    
    // 获取当前检测到的对象
    public GameObject GetDetectedObject()
    {
        return detectedObject;
    }
    
    // 检查是否可以交互
    public bool CanInteract()
    {
        return canInteract;
    }
}


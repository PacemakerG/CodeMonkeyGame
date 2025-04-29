using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour, IKitchenItemHolder, IKitchenItemReceiver
{
    private bool isGrounded = true; // 是否在地面上
    private Rigidbody rb;
    private Animator animator;
    //控制玩家动画
    private bool ismoving = false;
    private bool isjumping = false; 

    public event Action<GameObject> GetContainerOpenClose; // container事件声明 
    public event Action GetCuttingprocess; // cuttingprocess事件声明

    // 物品持有相关变量
    private KitchenStuffSO heldItem = null;
    private GameObject heldItemGameObject = null;
    [SerializeField] private Transform handPoint; // 玩家手的位置点
    
    public bool GetIsMoving()
    {
        return ismoving;
    }

    public bool GetIsJumping()
    {
        return isjumping;
    }
 
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    public void Move(Vector3 movement)
    {
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
        ismoving = movement.magnitude > 0.1f;
    }

    public void Rotate(Quaternion targetRotation, float rotationSpeed)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void Jump(float jumpForce)
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    // 改进后的交互方法，直接使用接口
    public void Interact(GameObject interactedObject)
    {
        Debug.Log($"尝试与 {interactedObject.name} 交互");

        // 检查是否是厨房物品持有者
        IKitchenItemHolder itemHolder = interactedObject.GetComponent<IKitchenItemHolder>();
        if (itemHolder != null)
        {
            Debug.Log($"找到持有者接口: {itemHolder.GetType().Name}");
            InteractWithItemHolder(itemHolder);
            return;
        }
        Debug.Log("无法与此对象交互 - 未找到 IKitchenItemHolder 接口");
    }

    // 与物品持有者交互，判断是那种类型
  private void InteractWithItemHolder(IKitchenItemHolder otherItemHolder)
    { 
    #region 交互对象为Processor
    IKitchenItemProcessor processor = otherItemHolder as IKitchenItemProcessor;
    if (processor != null)
    {
        // 玩家有物品，加工器没物品 - 可以放置
        if (HasItem() && !processor.HasItem())
        {
            Debug.Log($"与加工器交互: {processor.GetType().Name}");
            GiveItemTo(processor);
            return;
        }
        // 玩家有物品，加工器有物品 - 无法放置
        else if (HasItem() && processor.HasItem()) 
        {   
            Debug.Log("加工器上已有物体，无法放置新物品");
            return;
        }
        // 玩家没物品，加工器有物品 - 触发切割事件
        else if (!HasItem() && processor.HasItem())
        {
            // 检查物品是否已经被加工
            Recipe matchingRecipe = processor.GetMatchingRecipe_inputItem(); 
            // 如果没有匹配的配方，说明物品已被加工或无法加工
            if (matchingRecipe == null)
            {
                // 物品已加工过或无法加工，直接取走物品
                Debug.Log("物品已加工完成或无法加工，直接拿取");
                TakeItemFrom(processor);
            }
            else
            {
                // 物品未加工，触发加工事件
                Debug.Log("物品可加工，触发加工事件");
                GetCuttingprocess?.Invoke();
            }
            return;
        }
        else
        {
            // 都没有物品的情况
            Debug.Log("没有物品可以交互");
            return;
        }
    }
    #endregion

    #region 交互对象为Container
    IKitchenItemContainer container = otherItemHolder as IKitchenItemContainer;
    if (container != null)
    {
        // 只有在符合条件时才执行容器交互
        if (!HasItem() && container.HasItem())
        {
            GameObject itemObject = container.GetKitchenItemGameObject();
            GameObject containerObject = itemObject.transform.parent.parent.gameObject;
            // 传递容器对象而不是物品对象
            GetContainerOpenClose?.Invoke(containerObject);
            // 从容器拿取物品
            TakeItemFrom(container);
            return;
        }
        else if (HasItem() && container.HasItem())
        {
            Debug.Log("容器中有物品且玩家手中也有物品，无法交互");
            return;
        }
        else if (HasItem() && !container.HasItem())
        {
            Debug.Log("容器是空的，不能从中取物品");
            return;
        }
        else
        {
            Debug.Log("容器中没有物品，玩家手中也没有物品");
            return;
        }
    }
    #endregion
    
    #region   交互对象为Receiver
        IKitchenItemReceiver receiver = otherItemHolder as IKitchenItemReceiver;
        if (receiver != null)
        {
            // 双方都有物品 - 交换物品
            if (HasItem() && receiver.HasItem())
            {
                ExchangeItems(receiver);
                return;
            }
            // 玩家有物品，对方没有 - 给予物品
            else if (HasItem() && !receiver.HasItem())
            {
                GiveItemTo(receiver);
                return;
            }
            // 玩家没有物品，对方有 - 拿取物品
            else if (!HasItem() && receiver.HasItem())
            {
                TakeItemFrom(receiver);
                return;
            }
            else
            {
                Debug.Log("未满足任何交互条件");
                return;
            }
        }
    #endregion

    // 如果所有接口检查都未成功，再打印最终的错误信息
    Debug.Log("无法与此对象交互 - 未找到合适的接口类型");
}

#region Player三大功能的具体实现

    private void ExchangeItems(IKitchenItemReceiver receiver)
    {
        // 需要 SetKitchenItem 方法
        KitchenStuffSO playerItem = heldItem;
        GameObject playerItemObject = heldItemGameObject;
        
        KitchenStuffSO otherItem = receiver.GetKitchenItem();
        GameObject otherItemObject = receiver.GetKitchenItemGameObject();
        
        ClearKitchenItem();
        receiver.ClearKitchenItem();
        
        SetKitchenItem(otherItem, otherItemObject);
        receiver.SetKitchenItem(playerItem, playerItemObject);
        
        Debug.Log("与对方交换了物品");
    }

    private void GiveItemTo(IKitchenItemReceiver receiver)
    {
        // 需要 SetKitchenItem 方法
        receiver.SetKitchenItem(heldItem, heldItemGameObject);
        ClearKitchenItem();
        Debug.Log("物品给予成功");
    }

    private void TakeItemFrom(IKitchenItemHolder holder)
    {
        // 只需要基础的 Get 和 Clear 方法
        KitchenStuffSO itemSO = holder.GetKitchenItem();
        GameObject itemObject = holder.GetKitchenItemGameObject();
        
        SetKitchenItem(itemSO, itemObject);
        holder.ClearKitchenItem();
        
        Debug.Log($"物品拿取成功: {itemSO?.name}");
    }
#endregion

#region physics engine
    //离开碰撞
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            isjumping = true;
        }
    }

    //发生碰撞
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isjumping = false;
        }
    }
#endregion

#region IKitchenItemReceiver 接口实现
    public bool HasItem()
    {
        return heldItem != null;
    }
    
    public KitchenStuffSO GetKitchenItem()
    {
        return heldItem;
    }
    
    public GameObject GetKitchenItemGameObject()
    {
        return heldItemGameObject;
    }
    
    public void SetKitchenItem(KitchenStuffSO kitchenItem, GameObject kitchenItemGameObject)
    {
        this.heldItem = kitchenItem;
        this.heldItemGameObject = kitchenItemGameObject;
         // 获取手部位置
        Transform handTransform = GetKitchenItemFollowTransform();
        
        // 在手部位置创建物品
        this.heldItemGameObject = Instantiate(
            kitchenItem.prefab,  // 使用 SO 中预制体
            handTransform.position,  // 位置
            Quaternion.identity,  // 旋转
            handTransform  // 父对象
        );
          
        // 调整位置和旋转
        // this.heldItemGameObject.transform.localPosition = Vector3.zero;
        // this.heldItemGameObject.transform.localRotation = Quaternion.identity;
        
        Debug.Log($"物品 {kitchenItem.name} 已创建在手部位置");

    }
    
    public void ClearKitchenItem()
    {
        heldItem = null;
        if (heldItemGameObject != null)
        {
            heldItemGameObject.transform.parent = null;
            Destroy(heldItemGameObject);
        
            //Debug.Log("物品已被清除并销毁");
        }
        heldItemGameObject = null;
    }
    
    public Transform GetKitchenItemFollowTransform()
    {
        return handPoint;
    }
#endregion
}

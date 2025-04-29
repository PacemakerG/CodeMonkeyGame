using UnityEngine;

public class KitchenStuff_Control : MonoBehaviour
{
    [SerializeField] private KitchenStuffSO defaultKitchenStuff;  // 默认要生成的物品类型
    [SerializeField] private bool SpawnControl;  // 生成控制器
    
    private GameObject currentObject;
    private KitchenStuffSO itemSO;
    private bool hasSpawnedObject = false;

    private void Start()
    {
        // 如果有默认物品需要生成，直接生成
        if (defaultKitchenStuff != null && SpawnControl)
        {
            SpawnKitchenItem(defaultKitchenStuff);
        }
      
    }

    // 生成厨房物品的方法
    public void SpawnKitchenItem(KitchenStuffSO itemData)
    {
        // 先清除现有物品
        ClearSpawnedItem();
        
        if (itemData != null)
        {
            // 保存数据引用
            this.itemSO = itemData;
            
            // 生成新物体
            currentObject = Instantiate(
                itemData.prefab, 
                transform.position, 
                Quaternion.identity
            );
            
            // 设置父子关系和位置
            currentObject.transform.parent = transform;
            currentObject.transform.localPosition = Vector3.zero;
            currentObject.transform.localRotation = Quaternion.identity;
            
            hasSpawnedObject = true;
            //Debug.Log($"生成了物品: {itemData.itemName}");
        }
    }

    // 获取当前对象的方法
    public GameObject GetCurrentObject()
    {
        return currentObject;
    }

    public KitchenStuffSO GetCurrentItemData()
    {
        return itemSO;
    }

    // 检查是否已生成物体
    public bool HasSpawnedObject()
    {
        return hasSpawnedObject;
    }

    private void OnDestroy()
    {
        // 清理生成的物体
        ClearSpawnedItem();
    }

    // 清理物品的方法
    public void ClearSpawnedItem()
    {
        if (currentObject != null)
        {
            Destroy(currentObject);
            currentObject = null;
        }
        
        itemSO = null;
        hasSpawnedObject = false;
    }
}
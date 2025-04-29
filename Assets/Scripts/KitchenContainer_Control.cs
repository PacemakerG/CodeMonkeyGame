using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenContainer_Control : MonoBehaviour, IKitchenItemContainer
{
    [SerializeField] private KitchenStuff_Control kitchenStuffControl; // 物品控制器
    
    
#region IKitchenItemReceiver 接口实现
    public bool HasItem()
    {
        return kitchenStuffControl.HasSpawnedObject();
    }
    
    public KitchenStuffSO GetKitchenItem()
    {
       return kitchenStuffControl.GetCurrentItemData();
    }
    
    public GameObject GetKitchenItemGameObject()
    {
        return kitchenStuffControl.GetCurrentObject();
    }
    
    public void ClearKitchenItem()
    {
       
    }
    
#endregion
}

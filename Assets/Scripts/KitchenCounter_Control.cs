using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenCounter_Control : MonoBehaviour, IKitchenItemReceiver
{
    [SerializeField] private Input_Control inputControl;
    [SerializeField] private Material defaultMaterial;  // element0
    [SerializeField] private Material highlightMaterial;  // element1
    [SerializeField] private KitchenStuff_Control kitchenStuffControl; // 物品控制器
    
    private MeshRenderer meshRenderer;
    private Material[] mixMaterials;
    private Material[] defaultMaterials;

    void Start()
    {
        InitializeMaterials();
        
        // 订阅输入控制器的事件
        if (inputControl != null)
        {
            inputControl.OnDetected += HandleDetected;
            inputControl.OnDetectedReset += HandleDetectedReset;
        }
    }

    void OnDestroy()
    {
        // 取消订阅事件
        if (inputControl != null)
        {
            inputControl.OnDetected -= HandleDetected;
            inputControl.OnDetectedReset -= HandleDetectedReset;
        }
    }
    
    // 封装材质初始化逻辑
    private void InitializeMaterials()
    {
        // 获取渲染器组件
        meshRenderer = GetComponent<MeshRenderer>();
        
        // 设置默认材质
        meshRenderer.material = defaultMaterial;
        
        // 初始化材质数组
        mixMaterials = new Material[2];
        mixMaterials[0] = defaultMaterial;
        mixMaterials[1] = highlightMaterial;
        
        defaultMaterials = new Material[1];
        defaultMaterials[0] = defaultMaterial;
        
        Debug.Log("材质已初始化");
    }
    
    // 切换到默认材质
    public void SetDefaultMaterial()
    {
        meshRenderer.material = defaultMaterial;
    }
    
    // 切换到混合材质
    public void SetHighlightMaterial()
    {
        meshRenderer.materials = mixMaterials;
    }
    
    //处理碰撞到更换材质
    private void HandleDetected(LayerMask layer, GameObject interactedObject) 
    {
        if(layer == 7 && interactedObject == this.gameObject)
        {
            SetHighlightMaterial();
        }
    }
    
    //处理没有碰撞切换默认材质
    private void HandleDetectedReset() 
    {
        meshRenderer.materials = defaultMaterials;
    }
    
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
    
    public void SetKitchenItem(KitchenStuffSO kitchenItem, GameObject kitchenItemGameObject)
    {
        kitchenStuffControl.SpawnKitchenItem(kitchenItem);
    }
    
    public void ClearKitchenItem()
    {
        kitchenStuffControl.ClearSpawnedItem();
    }
    
    public Transform GetKitchenItemFollowTransform()
    {
       return kitchenStuffControl.GetCurrentObject().transform;
    }
#endregion
}

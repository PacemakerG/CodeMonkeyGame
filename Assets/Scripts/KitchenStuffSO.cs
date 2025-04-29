using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 确保在Unity编辑器中可以创建这种类型的资源
[CreateAssetMenu(fileName = "New Kitchen Item", menuName = "Kitchen/Kitchen Item")]
public class KitchenStuffSO : ScriptableObject
{
    [Header("基本信息")]
    public string itemName;                 // 食材名称
    public Sprite icon;                     // 图标
    public GameObject prefab;               // 3D模型预制体
}

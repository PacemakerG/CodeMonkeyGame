using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 这个类用于定义菜的配方
[System.Serializable]
public class Recipe
{
    public KitchenStuffSO input;  // 输入物品
    public KitchenStuffSO output; // 输出物品
}

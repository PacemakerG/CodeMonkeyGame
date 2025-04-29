using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenCutting_Control : MonoBehaviour, IKitchenItemProcessor
{
    [SerializeField] private KitchenStuff_Control kitchenStuffControl;
    [SerializeField] private Recipe[] recipes;
    [SerializeField] private Player player;

    void start()
    {
        player.GetCuttingprocess += Cutting;
    }
    private void Cutting()
    {
        Recipe cuttingresult =  GetMatchingRecipe();
        KitchenStuffSO cuttingItem = cuttingresult.output;
        kitchenStuffControl.SpawnKitchenItem(cuttingItem);
    }

    #region IKitchenItemProcessor 接口实现
    // 当前接口中唯一定义的方法
    public Recipe GetMatchingRecipe()
    {
        KitchenStuffSO currentItem = GetKitchenItem();

        foreach (var recipe in recipes)
        {
            if (recipe.input == currentItem)
                return recipe;
        }

        return null;
    }
    #endregion

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
        kitchenStuffControl.ClearSpawnedItem();
    }
    public void SetKitchenItem(KitchenStuffSO kitchenItem, GameObject kitchenItemGameObject)
    {
        kitchenStuffControl.SpawnKitchenItem(kitchenItem);
    }

    public Transform GetKitchenItemFollowTransform()
    {
        return transform;
    }
    #endregion
}
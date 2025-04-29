using UnityEngine;
// 物品持有能力接口
public interface IKitchenItemHolder
{
    bool HasItem();
    KitchenStuffSO GetKitchenItem();
    GameObject GetKitchenItemGameObject();
    void ClearKitchenItem();
}

// 可接收物品接口
public interface IKitchenItemReceiver : IKitchenItemHolder
{
    void SetKitchenItem(KitchenStuffSO kitchenItem, GameObject kitchenItemGameObject);
    Transform GetKitchenItemFollowTransform();
}
public interface IKitchenItemContainer : IKitchenItemHolder
{
    
}

// 物品处理能力接口
public interface IKitchenItemProcessor : IKitchenItemReceiver
{
    // 找到合适的配方
    Recipe GetMatchingRecipe_inputItem();
}

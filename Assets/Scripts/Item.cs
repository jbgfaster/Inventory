using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KosanInventory
{
public class Item : MonoBehaviour
{
    public ItemData itemData;
    public int quantity = 1;

    public void PickUpItem()
    {
        if(InventoryController.Get().InsertConcreteItem(itemData,quantity,true,true))
        {
            Destroy(gameObject);
        }
    }

    public void OnClick()
    {
        PickUpItem();
    }

    public void SetQuantity(int _quantity = 1)
    {
        quantity = _quantity;
    }
}
}
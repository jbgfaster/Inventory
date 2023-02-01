using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KosanInventory
{
public class Chest : MonoBehaviour
{
    [HideInInspector] public bool isPublicStorage=true;
    [SerializeField] private List<ItemStruct> items=new List<ItemStruct>();
    
    public List<ItemStruct> Items
    {
        get{
            return items;
        }
        set{
            items = value;
        }
    }

    public void OpenChest()
    {
        InventoryController.Get().Exchange(items,this);       
    }

    public void CloseChest(List<ItemStruct> _items)
    {
        Items=_items;
    }

    public void OnClick()
    {
        if(isPublicStorage)
        {
            OpenChest();
        }        
    }    
}

[System.Serializable]
public struct ItemStruct
{
    public ItemData itemData;
    public int quantity;
    public ItemStruct(ItemData _itemData,int _quantity )
    {
        this.itemData = _itemData;
        this.quantity = _quantity;
    }
}
}
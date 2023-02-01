using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KosanInventory
{
[CreateAssetMenu][System.Serializable]
public class ItemData : ScriptableObject
{
    public int width = 1;//holding space in inventory
    public int height = 1;
    public int maxInStack = 1;
    public Sprite itemIcon;
    public GameObject itemPrefab;
    public ItemUseActionData[] itemAction;
    public string itemNote;
    public TypeItem itemType;
    public ItemData itemTransform;
}
public enum TypeItem
{
    None,
    Fuel,
    Fire,
    RawFood
}
}
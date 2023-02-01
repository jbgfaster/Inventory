using UnityEngine;
using UnityEngine.UI;

namespace KosanInventory
{
public class InventoryItem : MonoBehaviour
{
    public ItemData itemData;

    public int onGridPositionX;
    public int onGridPositonY;

    public int quantity =1;
    [SerializeField] private Text quantityText;

    internal void Set(ItemData itemData)
    {
        this.itemData = itemData;

        GetComponent<Image>().sprite = itemData.itemIcon;

        Vector2 size = new Vector2();
        size.x = itemData.width*ItemGrid.tileSizeWidth;
        size.y = itemData.height*ItemGrid.tileSizeHeight;
        GetComponent<RectTransform>().sizeDelta = size;
        SetQuantity();
    }

    internal void SetQuantity()
    {
        quantityText.text=quantity.ToString();
        quantityText.gameObject.SetActive(quantity>1);
    }
}
}
using UnityEngine;
using UnityEngine.UI;

namespace KosanInventory
{
public class InventoryNote : MonoBehaviour
{
    [SerializeField] private RectTransform note; 
    private Text noteText;

    public void Show(bool b)
    {
        note.gameObject.SetActive(b);
        noteText=note.GetComponentInChildren<Text>();
    }

    public void SetPosition(ItemGrid targetGrid, InventoryItem targetItem)
    {
        Vector2 pos = targetItem.transform.position;
        note.position = pos+new Vector2(targetItem.itemData.width*ItemGrid.tileSizeWidth/2+1,targetItem.itemData.height*ItemGrid.tileSizeHeight/2+1);
    }

    public void SetNoteText(string _noteText="")
    {
        noteText.text=_noteText;
    }
}
}
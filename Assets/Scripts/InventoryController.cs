using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace KosanInventory
{
public class InventoryController : MonoBehaviour
{
   [HideInInspector]
   private ItemGrid selectedItemGrid;
   public ItemGrid SelectedItemGrid 
   {
        get => selectedItemGrid;
        set {
            selectedItemGrid = value;
            if(value!=null)
            {
                selectedItem?.transform.SetParent(value.transform);
            }            
            inventoryHighlight.SetParent(value);
        }
   }

   [SerializeField]private InventoryItem selectedItem;
   private InventoryItem overlapItem;
   private RectTransform rectTransform;

   [SerializeField]private List<ItemData> items;
   [SerializeField]private GameObject itemPrefab;
   [SerializeField]private Transform canvasTransform;
   [SerializeField] private Button closeButton; 

   private InventoryHighlight inventoryHighlight;
   private InventoryNote inventoryNote;

   private bool isInventoryHide = false;

   public UnityEvent CloseBaseInventoryEvent;

   private static InventoryController instance;

   void Awake()
   {
        instance = this;
        inventoryHighlight = GetComponent<InventoryHighlight>();   
        inventoryNote = GetComponent<InventoryNote>();   
        foreach(ItemData i in items)
        {
            InsertConcreteItem(i);
        }    
        closeButton.onClick.AddListener(CloseInventory);    
   }

    void Start()
    {        
        CloseInventory();                
    }

   void Update()
    {
        if(isInventoryHide)
        {
            return;
        }
        ItemIconDrag(); 
        HandleNote();       

        if (selectedItemGrid == null)
        {
            if (Input.GetMouseButtonDown(0)&&selectedItem!=null)
            {                
                DropItem();
            }
            inventoryHighlight.Show(false);
            return;
        }

        HandleHighlight();        

        if (Input.GetMouseButtonDown(0))
        {
            LeftMouseButtonPress();
        }

        if (Input.GetMouseButtonDown(1))
        {
            RightMouseButtonPress();
        }
    }

    private void HandleNote()
    {        
        if(itemToHighlight!=null&&selectedItemGrid!=null&&selectedItem==null)
        {
             inventoryNote.Show(true); 
             inventoryNote.SetPosition(selectedItemGrid,itemToHighlight);
             string _note=itemToHighlight.itemData.name +"\n Item note: \n";
             foreach(ItemUseActionData i in itemToHighlight.itemData.itemAction)
             {
                if(i!=null)
                {
                    _note+=i.GetItemNote()+"\n";
                }
             }
             inventoryNote.SetNoteText(_note);
        }
        else
        {
            inventoryNote.Show(false); 
        }
    }

    public bool InsertConcreteItem(ItemData itemData,int quantity=0, bool isMainGrid=true, bool fromFloor = false)
    {
        if(isMainGrid)
        {
            selectedItemGrid=mainItemGrid;
        }
        else
        {
            selectedItemGrid=exchangeItemGrid;
        }        
        CreateItem(itemData);
        InventoryItem itemToInsert = selectedItem;
        selectedItem = null;
        InsertItem(itemToInsert);
        if(quantity==0)
        {
            quantity=itemData.maxInStack;
        }
        itemToInsert.quantity=quantity;
        itemToInsert.SetQuantity();
        if(fromFloor)
        {
            if(GetComponent<Chest>()!=null)
                GetComponent<Chest>().Items= CloseChest(selectedItemGrid);
            selectedItemGrid=null;
        }
        return true;
    }

    public bool InsertConcreteItem(ItemData itemData, ItemGrid _grid,int quantity=0)
    {
        selectedItemGrid=_grid;       
        CreateItem(itemData);
        InventoryItem itemToInsert = selectedItem;
        selectedItem = null;
        InsertItem(itemToInsert);
        if(quantity==0)
        {
            quantity=itemData.maxInStack;
        }
        itemToInsert.quantity=quantity;
        itemToInsert.SetQuantity();
        selectedItemGrid=null;
        return true;
    }

    private void InsertRandomItem()
    {
        if(selectedItemGrid == null)
        {
            return;
        }

        CreateRandomItem();
        InventoryItem itemToInsert = selectedItem;
        selectedItem = null;
        InsertItem(itemToInsert);
    }

    private void InsertItem(InventoryItem itemToInsert)
    {
        Vector2Int? posOnGrid = selectedItemGrid.FindSpaceForObject(itemToInsert);

        if(posOnGrid == null)
        {
            Destroy(itemToInsert.gameObject);            
            return;
        }

        selectedItemGrid.PlaceItem(itemToInsert,posOnGrid.Value.x,posOnGrid.Value.y);
    }

    Vector2Int oldPosition;
    InventoryItem itemToHighlight;
    private void HandleHighlight()
    {
        Vector2Int positionOnGrid = GetTileGridPosition();
        if(oldPosition == positionOnGrid)
        {
            return;
        }
        oldPosition = positionOnGrid;
        if(selectedItem==null)
        {
            itemToHighlight = selectedItemGrid.GetItem(positionOnGrid.x,positionOnGrid.y);
            if(itemToHighlight!=null)
            {
                inventoryHighlight.Show(true);
                inventoryHighlight.SetSize(itemToHighlight);
                inventoryHighlight.SetPosition(selectedItemGrid,itemToHighlight);
            }
            else
            {
                inventoryHighlight.Show(false);
            }            
        }
        else
        {
            inventoryHighlight.Show(selectedItemGrid.BoundryCheck(
                positionOnGrid.x,
                positionOnGrid.y,
                selectedItem.itemData.width,
                selectedItem.itemData.height)
                );

            inventoryHighlight.SetSize(selectedItem);
            inventoryHighlight.SetPosition(selectedItemGrid,selectedItem, positionOnGrid.x,positionOnGrid.y);
        }
    }

    private void CreateRandomItem()
    {
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        selectedItem = inventoryItem;

        rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasTransform);
        rectTransform.SetAsLastSibling();

        int selectedItemID = UnityEngine.Random.Range(0,items.Count);
        inventoryItem.Set(items[selectedItemID]);
    }

    private void CreateItem(ItemData itemData)
    {
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        selectedItem = inventoryItem;

        rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasTransform);
        rectTransform.SetAsLastSibling();

        inventoryItem.Set(itemData);
    }

    private void LeftMouseButtonPress()
    {
        Vector2Int tileGridPosition = GetTileGridPosition();

        if (selectedItem == null)
        {
            PickUpItem(tileGridPosition);
        }
        else
        {
            PlaceItem(tileGridPosition);
        }
    }

    private void RightMouseButtonPress()
    {
        if(selectedItem!=null)
        {
            return;
        }
        Vector2Int tileGridPosition = GetTileGridPosition();
        selectedItem = selectedItemGrid.GetItemToUse(tileGridPosition.x, tileGridPosition.y);
        if (selectedItem != null)
        {
            UseItem(selectedItem);            
        }
        selectedItem=null;
        if(overlapItem!=null)
        {
            selectedItem = overlapItem;
            overlapItem = null;
            rectTransform = selectedItem.GetComponent<RectTransform>();
            rectTransform.SetAsLastSibling();
        }
    }

    private void UseItem(InventoryItem item)
    {
        if(item.itemData.itemAction==null)
        {
            print("action not set to item: "+item.name);
            return;
        }
        int remainingQuantity=item.quantity;
        foreach(ItemUseActionData i in item.itemData.itemAction)
        {
            if(i!=null)
            {
                remainingQuantity=i.ItemUseAction(item.quantity);
            }            
        }
        if(remainingQuantity<=0)
        {
            Destroy(item.gameObject);
        }
        else
        {
            item.quantity=remainingQuantity;
            item.SetQuantity();
        }
    }

    public int GetQuantity(ItemData item)
    {
        int result =0;
        foreach(InventoryItem i in mainItemGrid.GetComponentsInChildren<InventoryItem>())
        {
            if(i.itemData==item)
            {
                result+=i.quantity;
            }
        }
        return result;
    }

    public int GetQuantityInternal(ItemData item)
    {
        int result =0;
        foreach(ItemStruct i in GetComponent<Chest>().Items)
        {
            if(i.itemData==item)
            {
                result+=i.quantity;
            }
        }
        return result;
    }

    public bool GetItems(ItemData item,int quantity)
    {
        List<InventoryItem> itemsToDestroy = new List<InventoryItem>();
        if(GetQuantity(item)<quantity)
        {
            return false;
        }
        foreach(InventoryItem i in mainItemGrid.GetComponentsInChildren<InventoryItem>())
        {
            if(i.itemData==item)
            {
                i.quantity-=quantity;
                quantity=0;
                if(i.quantity<=0)
                {
                    quantity+=-i.quantity;  
                    i.quantity=0;   
                    itemsToDestroy.Add(i);               
                }
                else
                {
                    
                }
            }
            i.SetQuantity();
            if(quantity<=0)
            {
                break;
            }
            
        }
        for(int i=itemsToDestroy.Count;i>0;i--)
        {
            Destroy(itemsToDestroy[i-1].gameObject);
        }

        return true;
    }

    private Vector2Int GetTileGridPosition()
    {
        Vector2 position = Input.mousePosition;

        if (selectedItem != null)
        {
            position.x -= (selectedItem.itemData.width - 1) * ItemGrid.tileSizeWidth / 2;
            position.y += (selectedItem.itemData.height - 1) * ItemGrid.tileSizeHeight / 2;
        }
        return selectedItemGrid.GetTileGridPosition(position);
    }

    public void DropItem()
    {
        Instantiate(selectedItem.itemData.itemPrefab,CursorToWorldPosition(),selectedItem.itemData.itemPrefab.transform.rotation).GetComponent<Item>()?.SetQuantity(selectedItem.quantity);
        Destroy(selectedItem.gameObject);
        selectedItem = null;        
    }

    public void DropItem(ItemData itemData,Vector3 position)
    {
      GameObject item =  Instantiate(itemData.itemPrefab,position,itemData.itemPrefab.transform.rotation);        
    }

    private Vector3 CursorToWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit hitInfo,maxDistance:300f))
        {
            var target = hitInfo.point;
            target.y = target.y +1;
            return target;
        }
        return Vector3.zero;
    }

    private void PlaceItem(Vector2Int tileGridPosition)
    {
        bool complete= selectedItemGrid.PlaceItem(selectedItem, tileGridPosition.x, tileGridPosition.y,ref overlapItem);
        if(complete)
        {
            selectedItem = null;
            if(overlapItem!=null)
            {
                selectedItem = overlapItem;
                overlapItem = null;
                rectTransform = selectedItem.GetComponent<RectTransform>();
                rectTransform.SetAsLastSibling();
            }
        }        
    }

    private void PickUpItem(Vector2Int tileGridPosition)
    {
        selectedItem = selectedItemGrid.PickUpItem(tileGridPosition.x, tileGridPosition.y);
        if (selectedItem != null)
        {
            rectTransform = selectedItem.GetComponent<RectTransform>();
        }
    }

    private void ItemIconDrag()
    {
        if (selectedItem != null)
        {
            rectTransform.position = Input.mousePosition;
        }
    }

    public static InventoryController Get()
    {
        return instance;
    }

    [SerializeField] private ItemGrid mainItemGrid;
    [SerializeField] private ItemGrid exchangeItemGrid;
    private Chest currentChest;

    public bool Exchange(List<ItemStruct> items,Chest chest)
    {        
        if(chest==currentChest)
        {
            return false;
        }
        selectedItemGrid = null;
        selectedItem = null;
        isInventoryHide=false;
        GetComponent<UIPanel>()?.Show();
        CloseExchange();
        exchangeItemGrid.gameObject.SetActive(true);
        currentChest=chest;
        exchangeItemGrid.CleanGrid();
        foreach(ItemStruct i in items)
        {
            InsertConcreteItem(i.itemData,i.quantity,false);
        }
        selectedItemGrid=null;
        return true;
    }

     public void SwitchInventory()
    {
        if(isInventoryHide)
        {
            OpenInventory();
        }
        else
        {            
            CloseInventory();
        }
    }

    public void OpenInventory()
    { 
        GetComponent<UIPanel>()?.Show();
        isInventoryHide=false;
        selectedItemGrid=null;
        if(GetComponent<Chest>()!=null)
        {
            OpenChest(mainItemGrid,GetComponent<Chest>().Items);
        }
    }

    public void CloseInventory()
    {        
        GetComponent<UIPanel>()?.Hide();
        isInventoryHide=true;
        CloseExchange();
        exchangeItemGrid.gameObject.SetActive(false);
        if(GetComponent<Chest>()!=null)
        {
            GetComponent<Chest>().Items=CloseChest(mainItemGrid);
        }
        CloseBaseInventoryEvent.Invoke();
    }

    public List<ItemStruct> CloseChest(ItemGrid itemGrid)
    {
        List<ItemStruct> items=new List<ItemStruct>();
        foreach(InventoryItem i in itemGrid.GetComponentsInChildren<InventoryItem>())
        {
            items.Add(new ItemStruct(i.itemData,i.quantity));
        }
        //itemGrid.CleanGrid();
        return items;
    }

    public void OpenChest(ItemGrid itemGrid,List<ItemStruct> items)
    {
        itemGrid.CleanGrid();

        foreach(ItemStruct i in items)
        {
            InsertConcreteItem(i.itemData,itemGrid,i.quantity);
        }
    }

    public void CloseExchange()
    {
        if(currentChest==null)
        {
            return;
        }
        List<ItemStruct> items=new List<ItemStruct>();
        foreach(InventoryItem i in exchangeItemGrid.GetComponentsInChildren<InventoryItem>())
        {
            ItemStruct itemStruct = new ItemStruct();
            itemStruct.itemData = i.itemData;
            itemStruct.quantity = i.quantity;
            items.Add(itemStruct);
        }
        currentChest.CloseChest(items);
        exchangeItemGrid.CleanGrid();
        currentChest=null;
        exchangeItemGrid.gameObject.SetActive(false);
    }
}
}
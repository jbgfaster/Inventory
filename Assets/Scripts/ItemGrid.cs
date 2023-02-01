using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KosanInventory
{
public class ItemGrid : MonoBehaviour
{
    public const float tileSizeWidth = 32;
    public const float tileSizeHeight = 32;

    private InventoryItem[,] inventoryItemSlot;

    private RectTransform rectTransform;

    [SerializeField] private int gridSizeWidth = 20;
    [SerializeField] private int gridSizeHeight = 10; 
    [SerializeField] private TypeItem accessibleItemType;

    public UnityEngine.Events.UnityEvent UpdateItemsEvent;

    void Awake()
    {
        rectTransform=GetComponent<RectTransform>();
        Init(gridSizeWidth,gridSizeHeight);
    }   

    public InventoryItem PickUpItem(int x, int y)
    {
        InventoryItem toReturn = inventoryItemSlot[x, y];

        if (toReturn == null)
        {
            return null;
        }

        CleanGridReference(toReturn);
        UpdateItemsEvent.Invoke();
        return toReturn;
    }

    public InventoryItem GetItemToUse(int x, int y)
    {
        InventoryItem toReturn = inventoryItemSlot[x, y];

        if (toReturn == null)
        {
            return null;
        }

        return toReturn;
    }

    private void CleanGridReference(InventoryItem item)
    {
        for (int ix = 0; ix < item.itemData.width; ix++)
        {
            for (int iy = 0; iy < item.itemData.height; iy++)
            {
                inventoryItemSlot[item.onGridPositionX + ix, item.onGridPositonY + iy] = null;
            }
        }
    }

    private void Init(int width, int height)
    {
        foreach(InventoryItem i in GetComponentsInChildren<InventoryItem>())
        {
            Destroy(i.gameObject);
        }
        inventoryItemSlot = new InventoryItem[width,height];
        Vector2 size = new Vector2(width*tileSizeWidth, height*tileSizeHeight);
        rectTransform.sizeDelta = size;
    }  

    internal Vector2Int? FindSpaceForObject(InventoryItem itemToInsert)
    {
        int height = gridSizeHeight - itemToInsert.itemData.height+1;
        int width = gridSizeWidth - itemToInsert.itemData.width+1;
        for(int y = 0;y<height;y++)
        {
            for(int x = 0; x< width;x++)
            {
                if(CheckAvailableSpace(x,y,itemToInsert.itemData.width,itemToInsert.itemData.height)==true)
                {
                    return new Vector2Int(x,y);
                }
            }
        }

        return null;
    }

    private bool StackItems(InventoryItem inventoryItem,InventoryItem overlapItem)
    {
        if(inventoryItem.itemData.name!=overlapItem.itemData.name)
        {
            return false;
        }

        if(overlapItem.itemData.maxInStack>overlapItem.quantity)
        {
            inventoryItem.quantity+=overlapItem.quantity;
            overlapItem.quantity=inventoryItem.quantity-inventoryItem.itemData.maxInStack;
            if(inventoryItem.quantity>inventoryItem.itemData.maxInStack)
            {
                inventoryItem.quantity=inventoryItem.itemData.maxInStack;
            }
            overlapItem.SetQuantity();
            inventoryItem.SetQuantity();

            if(overlapItem.quantity<=0)
            {
                Destroy(overlapItem.gameObject);
            }
        }
        return true;
    }

    public bool PlaceItem(InventoryItem inventoryItem, int posX, int posY, ref InventoryItem overlapItem)
    {
        if(!ItemTypeCheck(inventoryItem))
        {
            return false;
        }

        if (!BoundryCheck(posX, posY, inventoryItem.itemData.width, inventoryItem.itemData.height))
        {
            return false;
        }

        if (OverlapCheck(posX, posY, inventoryItem.itemData.width, inventoryItem.itemData.height, ref overlapItem) == false)
        {
            overlapItem = null;
            return false;
        }

        if (overlapItem != null)
        {
            StackItems(inventoryItem,overlapItem); 
            CleanGridReference(overlapItem);
        }

        PlaceItem(inventoryItem, posX, posY);
        return true;
    }

    public void PlaceItem(InventoryItem inventoryItem, int posX, int posY)
    {
        RectTransform rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(this.rectTransform);

        for (int x = 0; x < inventoryItem.itemData.width; x++)
        {
            for (int y = 0; y < inventoryItem.itemData.height; y++)
            {
                inventoryItemSlot[posX + x, posY + y] = inventoryItem;
            }
        }

        inventoryItem.onGridPositionX = posX;
        inventoryItem.onGridPositonY = posY;
        Vector2 position = CalculatePositionOnGrid(inventoryItem, posX, posY);

        rectTransform.localPosition = position;
        UpdateItemsEvent.Invoke();
    }    

    public Vector2 CalculatePositionOnGrid(InventoryItem inventoryItem, int posX, int posY)
    {
        Vector2 position = new Vector2();
        position.x = posX * tileSizeWidth + tileSizeWidth * inventoryItem.itemData.width / 2;
        position.y = -(posY * tileSizeHeight + tileSizeHeight * inventoryItem.itemData.height / 2);
        return position;
    }

    private bool OverlapCheck(int posX, int posY, int width, int height, ref InventoryItem overlapItem)
    {
        for(int x = 0; x<width;x++)
        {
            for(int y = 0;y<height;y++)
            {
                if(inventoryItemSlot[posX+x,posY+y]!=null)
                {
                    overlapItem =  inventoryItemSlot[posX+x,posY+y];
                }
                else
                {
                    if(overlapItem!= inventoryItemSlot[posX + x,posY + y])
                    {
                    return false;
                    }
                }
            }
        }
        return true;
    }

    private bool CheckAvailableSpace(int posX, int posY, int width, int height)
    {
        for(int x = 0; x<width;x++)
        {
            for(int y = 0;y<height;y++)
            {
                if(inventoryItemSlot[posX+x,posY+y]!=null)
                {       
                    return false;                   
                }
            }
        }
        return true;
    }

    bool PositionCheck(int posX,int posY)
    {
        if(posX<0||posY<0)
        {
            return false;
        }

        if(posX >= gridSizeWidth||posY>=gridSizeHeight)
        {
            return false;
        }

        return true;
    }

    public bool BoundryCheck(int posX, int posY, int width, int height)
    {
        if(PositionCheck(posX,posY)==false)
        {
            return false;
        }

        posX+=width-1;
        posY+=height-1;

        if(PositionCheck(posX,posY)==false)
        {
            return false;
        }

        return true;
    }

    private bool ItemTypeCheck(InventoryItem inventoryItem)
    {
        if(accessibleItemType==TypeItem.None)
        {
            return true;
        }
        if(accessibleItemType==inventoryItem.itemData.itemType)
        {
            return true;
        }
        return false;        
    }

    internal InventoryItem GetItem(int x, int y)
    {
        return inventoryItemSlot[x,y];
    }

    internal void CleanGrid()
    {
        Init(gridSizeWidth,gridSizeHeight);
    }

    private Vector2 positionOnTheGrid = new Vector2();
    private Vector2Int tileGridPosition = new Vector2Int();

    public Vector2Int GetTileGridPosition(Vector2 mousePosition)
    {
        mousePosition = this.WorldToGridRelative(rectTransform.position, mousePosition, GetComponentInParent<Canvas>().scaleFactor);

        tileGridPosition.x = (int)(mousePosition.x);
        tileGridPosition.y = Mathf.Abs((int)(mousePosition.y)+1);

        return tileGridPosition;
    }

    private Vector2 WorldToGridRelative(Vector2 origin, Vector2 point, float ScaleFactor = 1)
    {
        positionOnTheGrid.x=point.x - origin.x;
        positionOnTheGrid.y=-(origin.y - point.y);

        positionOnTheGrid.x = Mathf.Floor(positionOnTheGrid.x / (tileSizeWidth  * ScaleFactor));
        positionOnTheGrid.y = Mathf.Floor(positionOnTheGrid.y / (tileSizeHeight * ScaleFactor));
        
        return positionOnTheGrid;
    }   
}
}
using UnityEngine;

public class ItemUseActionData : ScriptableObject
{
    public virtual int ItemUseAction(int _quantity)
    {
        return 0;//return how many items remaining after use
    }

    public virtual string GetItemNote()
    {
        return "";
    }
}

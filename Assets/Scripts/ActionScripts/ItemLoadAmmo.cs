using UnityEngine;

[CreateAssetMenu(fileName = "ReloadAction", menuName ="ItemActions/ReloadAction",order = 0)][System.Serializable]
public class ItemLoadAmmo : ItemUseActionData
{
    public override int ItemUseAction(int _quantity)
    {
        Debug.Log("some code for load ammo in weapon");
        return _quantity-1;
    }
}

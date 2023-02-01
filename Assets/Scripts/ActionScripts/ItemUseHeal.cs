using UnityEngine;

[CreateAssetMenu(fileName = "HealAction", menuName ="ItemActions/HealAction",order = 0)][System.Serializable]
public class ItemUseHeal : ItemUseActionData
{
    [SerializeField] private int healPoints=10;
    public override int ItemUseAction(int _quantity)
    {
        Debug.Log("some code for add"+healPoints+" points to health parameter");
        return _quantity-1;
    }

    public override string GetItemNote()
    {
        return "Restore "+healPoints+" hitpoints.";
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "EatAction", menuName ="ItemActions/EatAction",order = 0)][System.Serializable]
public class ItemUseFood : ItemUseActionData
{
    [SerializeField] private int foodPoints=10;
    public override int ItemUseAction(int _quantity)
    {
        Debug.Log("some code for add"+foodPoints+" points to food parameter");
        return _quantity-1;
    }

    public override string GetItemNote()
    {
        return "Restore "+foodPoints+" starving.";
    }
}

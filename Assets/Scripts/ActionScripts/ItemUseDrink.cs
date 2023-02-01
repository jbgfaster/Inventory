using UnityEngine;

[CreateAssetMenu(fileName = "DrinkAction", menuName ="ItemActions/DrinkAction",order = 0)][System.Serializable]
public class ItemUseDrink : ItemUseActionData
{
    [SerializeField] private int drinkPoints=10;
    public override int ItemUseAction(int _quantity)
    {
        Debug.Log("some code for add"+drinkPoints+" points to drink parameter");
        return 0;
    }

    public override string GetItemNote()
    {
        return "Restore "+drinkPoints+" thirst.";
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "PoisonAction", menuName ="ItemActions/PoisonAction",order = 0)][System.Serializable]
public class ItemUsePoison : ItemUseActionData
{
    [SerializeField] private float fullDamage=10f;
    [SerializeField] private float timeToDamage=10f;


    public override int ItemUseAction(int _quantity)
    {
        Debug.Log("Make damage "+fullDamage/timeToDamage+" per second"+ timeToDamage +"seconds ");
        return 0;
    }

    public override string GetItemNote()
    {
        return "You can be poison after drink.";
    }
}

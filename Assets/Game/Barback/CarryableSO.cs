using UnityEngine;


public interface ICarryable
{
    public Sprite GetSprite();
}

[CreateAssetMenu(fileName = "CarryableSO", menuName = "Barback/CarryableSO", order = 1)]
public class CarryableSO : ScriptableObject, ICarryable
{
    [SerializeField]
    private Sprite _sprite;

    public Sprite GetSprite()
    {
        return _sprite;
    }
}
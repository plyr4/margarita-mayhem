using UnityEngine;

public class TequilaStock : TileMono
{
    public CarryableSO _carryable;

    public override CarryableSO GetCarryable()
    {
        return _carryable;
    }

    public override bool CanInteractFrom(Vector2Int moveDirection)
    {
        // stock is at the top of screen so the player is technically inside it when you interact with it "from below"
        if (moveDirection.y > 0)
        {
            return true;
        }

        return false;
    }
}
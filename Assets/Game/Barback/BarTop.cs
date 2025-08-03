using UnityEngine;

public class BarTop : TileMono
{
    private void Reset()
    {
        TileMono[] tileMonos = GetComponentsInChildren<TileMono>();
        foreach (TileMono tileMono in tileMonos)
        {
            if (tileMono != this)
            {
                _grid64Mono = tileMono._grid64Mono;
                _gridPosition = tileMono._gridPosition;
                DestroyImmediate(tileMono);
                break;
            }
        }
    }

    public override bool CanMoveTo(Vector2Int moveDirection)
    {
        if (moveDirection.y < 0)
        {
            return false;
        }

        return true;
    }

    public override bool CanMoveFrom(Vector2Int moveDirection)
    {
        if (moveDirection.y > 0)
        {
            return false;
        }

        return true;
    }
}
using UnityEngine;

public interface ITile
{
    public void DestroyTile(Grid64Mono grid);
    public bool CanMoveTo(Vector2Int moveDirection);
    public bool CanMoveFrom(Vector2Int moveDirection);
    public bool CanInteractTo(Vector2Int moveDirection);
    public bool CanInteractFrom(Vector2Int moveDirection);
    public CarryableSO GetCarryable();
    public CarryableSO GetReceivable();
    public bool OnInteract(PlayerMono player, Vector2Int moveDirection);
}

public class Grid64Mono : MonoBehaviour
{
    public float _worldSize = 5f;
    public float _screenSize = 640;
    public int _pixelSize = 8;
    public int _columns = 8;
    public int _rows = 8;

    private ITile[,] _tiles;

    public void Start()
    {
        ResetTiles();
    }

    public ITile GetTile(Vector2Int gridPosition)
    {
        if (gridPosition.x < 0 || gridPosition.x >= _columns ||
            gridPosition.y < 0 || gridPosition.y >= _rows)
        {
            return null;
        }

        return _tiles[gridPosition.x, gridPosition.y];
    }

    private void ResetTiles()
    {
        if (_tiles != null)
        {
            for (int i = 0; i < _tiles.GetLength(0); i++)
            {
                for (int j = 0; j < _tiles.GetLength(1); j++)
                {
                    if (_tiles[i, j] != null)
                    {
                        _tiles[i, j].DestroyTile(this);
                    }
                }
            }
        }

        _tiles = new ITile[_columns, _rows];

        TileMono[] tileMonos = FindObjectsOfType<TileMono>();

        for (int i = 0; i < _columns; i++)
        {
            for (int j = 0; j < _rows; j++)
            {
                foreach (TileMono tileMono in tileMonos)
                {
                    if (tileMono._gridPosition == new Vector2Int(i, j))
                    {
                        if (_tiles[i, j] != null)
                        {
                            Debug.LogError($"tile at position {i}, {j} already exists. Please ensure unique grid positions for each tile.");
                            continue;
                        }
                        _tiles[i, j] = tileMono;
                        break;
                    }
                }
            }
        }
    }

    public Vector3 GridPositionToWorldPosition(Vector2Int gridPosition)
    {
        return new Vector3(
            -_worldSize + gridPosition.x * 2 * (_worldSize / _pixelSize),
            -_worldSize + gridPosition.y * 2 * (_worldSize / _pixelSize),
            0f);
    }

    public Vector3 GridPositionToScreenPosition(Vector2Int gridPosition)
    {
        return new Vector3(
            -_screenSize + gridPosition.x * 2 * (_screenSize / _pixelSize),
            -_screenSize + gridPosition.y * 2 * (_screenSize / _pixelSize),
            0f);
    }

    public int GridPositionToSortingOrder(Vector2Int gridPosition)
    {
        return _rows - gridPosition.y;
        ;
    }
}
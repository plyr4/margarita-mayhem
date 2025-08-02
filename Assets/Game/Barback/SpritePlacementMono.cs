using UnityEngine;

[ExecuteInEditMode]
public class SpritePlacementMono : MonoBehaviour
{
    public TileMono _tileMono;
    private SpriteRenderer _sprite;
    public int _sortingOrderOffset = 0;

    private void Update()
    {
        if (_sprite == null) _sprite = GetComponentInChildren<SpriteRenderer>();
        if (_tileMono == null) _tileMono = GetComponentInChildren<TileMono>();
        if (Application.isPlaying)
        {
            return;
        }

        transform.localPosition = _tileMono._grid64Mono.GridPositionToWorldPosition(_tileMono._gridPosition);
        _sprite.sortingOrder = _tileMono._grid64Mono.GridPositionToSortingOrder(_tileMono._gridPosition) + _sortingOrderOffset;
    }
}
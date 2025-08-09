using TMPro;
using UnityEngine;

[ExecuteInEditMode]
public class TextPlacementMono : MonoBehaviour
{
    public TileMono _tileMono;
    private TextMeshPro _textMeshPro;

    private void Update()
    {
        if (_textMeshPro == null) _textMeshPro = GetComponentInChildren<TextMeshPro>();
        if (_tileMono == null) _tileMono = GetComponentInChildren<TileMono>();
        if (Application.isPlaying)
        {
            return;
        }

        transform.localPosition = _tileMono._grid64Mono.GridPositionToWorldPosition(_tileMono._gridPosition);
    }
}
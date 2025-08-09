using TMPro;
using UnityEngine;

[ExecuteInEditMode]
public class SetTMPOrderInLayerOnStart : MonoBehaviour
{
    public int _orderInLayer = 0;
    public string _sortingLayerName = "Default";
    TMP_SubMesh _submesh;

    private void Update()
    {
        if (_submesh == null) _submesh = GetComponentInChildren<TMP_SubMesh>();
        if (_submesh == null) return;
        _submesh.renderer.sortingLayerName = _sortingLayerName;
        _submesh.renderer.sortingOrder = _orderInLayer;
    }
}
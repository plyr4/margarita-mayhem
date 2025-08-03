using System.Collections.Generic;
using UnityEngine;

public class BarStock : MonoBehaviour
{
    public IceBar _ice;
    public TequilaBar _tequila;

    public bool IsStocked()
    {
        return _ice.IsStocked() && _tequila.IsStocked();
    }

    public List<CarryableSO> GetUnstockedCarryables()
    {
        List<CarryableSO> unstockedCarryables = new List<CarryableSO>();
        if (!_ice.IsStocked())
        {
            unstockedCarryables.Add(_ice._emptyCarryable);
        }

        if (!_tequila.IsStocked())
        {
            unstockedCarryables.Add(_tequila._emptyCarryable);
        }

        return unstockedCarryables;
    }
}
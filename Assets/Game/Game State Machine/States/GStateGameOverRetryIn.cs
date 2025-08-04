using UnityEngine;

public class GStateGameOverRetryIn : GStateBase
{
    public GStateGameOverRetryIn(StateMachineMono context, StateFactory factory) : base(context, factory)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        if (_context == null) return;
        
        _done = true;
    }

    public override void OnExit()
    {
        base.OnExit();

        if (_context == null) return;
    }
}
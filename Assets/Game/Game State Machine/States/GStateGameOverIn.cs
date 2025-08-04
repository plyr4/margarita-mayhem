using UnityEngine;

public class GStateGameOverIn : GStateBase
{
    public GStateGameOverIn(StateMachineMono context, StateFactory factory) : base(context, factory)
    {
    }

    public IGameEventOpts _opts;

    public override void OnEnter()
    {
        base.OnEnter();

        if (_context == null) return;
    }

    public override void OnExit()
    {
        base.OnExit();

        if (_context == null) return;
        
        ((GStateMachineGame)_context).HandleGameOverResets();
    }
}
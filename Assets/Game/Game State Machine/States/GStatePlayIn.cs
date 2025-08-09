using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GStatePlayIn : GStateBase
{
    public GStatePlayIn(StateMachineMono context, StateFactory factory) : base(context, factory)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        if (_context == null) return;

        ScreenTransition.Instance.Open(2f);

        ((GStateMachineGame)_context).HandlePauseResets();
        ((GStateMachineGame)_context).HandleStartResets();
        ((GStateMachineGame)_context).HandleGameOverResets();
    }

    public override void OnExit()
    {
        base.OnExit();

        if (_context == null) return;
    }
}
using UnityEngine;

public class GStateTutorial : GStateBase
{
    public GStateTutorial(StateMachineMono context, StateFactory factory) : base(context, factory)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        if (_context == null) return;

        Time.timeScale = 0f;
    }

    public override void OnExit()
    {
        base.OnExit();

        if (_context == null) return;

        Time.timeScale = 1f;
        _done = true;
    }
}
public class GStateGameOver : GStateBase
{
    public GStateGameOver(StateMachineMono context, StateFactory factory) : base(context, factory)
    {
    }

    public IGameEventOpts _opts;

    public override void OnEnter()
    {
        base.OnEnter();

        if (_context == null) return;
    }
}
using UnityEngine;

public class Play : MonoBehaviour
{
    [SerializeField]
    private bool _active;
    [SerializeField]
    private GameEvent _playPause;

    public void HandleGameStateChange(IGameEventOpts opts)
    {
        GameStateChangeOpts opts_ = (GameStateChangeOpts)opts;

        switch (opts_._newState)
        {
            case GStateInit _:
                _active = false;
                break;
            case GStatePlay _:
                _active = true;
                break;
        }
    }

    public void Update()
    {
        if (!_active)
        {
            return;
        }

        if (GameInput.Instance._menuBackPressed)
        {
            HandleOnClickEscape();
        }
    }

    public void HandleOnClickEscape()
    {
        _playPause.Invoke();
    }
}
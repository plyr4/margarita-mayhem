using UnityEngine;

public class GUIGameOverMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _viewParent;

    public void HandleGameStateChange(IGameEventOpts opts)
    {
        GameStateChangeOpts opts_ = (GameStateChangeOpts)opts;

        switch (opts_._newState)
        {
            case GStateInit _:
                _viewParent.SetActive(false);
                break;
            // todo: add GStateGameOver
            // case GStatePause _:
            //     _viewParent.SetActive(true);
            //     _viewParent.GetComponentInChildren<UnityEngine.UI.Button>()?.Select();
            //     break;
            case GStatePlay _:
                _viewParent.SetActive(false);
                break;
            case GStatePauseRetry _:
                _viewParent.SetActive(false);
                break;
        }
    }
}
using UnityEngine;

public class GUIStartMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _viewParent;
    [SerializeField]
    private GameObject _tutorialViewParent;
    public GameEvent _startPlayEvent;
    public bool _tutorialEnabled;
    public float _cooldownTime = 0.5f;
    private bool _cooldown;

    private void Update()
    {
        if (GameInput.Instance._menuContinuePressed || GameInput.Instance._menuSubmitPressed)
        {
            if (_tutorialEnabled)
            {
                TutorialHandleOnClick();
            }
            else
            {
                StartMenuHandleOnClick();
            }
        }
    }

    public void HandleGameStateChange(IGameEventOpts opts)
    {
        GameStateChangeOpts opts_ = (GameStateChangeOpts)opts;

        switch (opts_._newState)
        {
            case GStateInit _:
                _tutorialViewParent.SetActive(false);
                break;
            case GStateStartIn _:
                _viewParent.SetActive(true);
                break;
            case GStateStart _:
                _viewParent.SetActive(true);
                _viewParent.GetComponentInChildren<UnityEngine.UI.Button>().Select();
                break;
            case GStatePlayLoad _:
                _viewParent.SetActive(false);
                _tutorialViewParent.SetActive(false);
                break;
        }
    }

    public void StartMenuHandleOnClick()
    {
        if (_cooldown)
            return;

        _cooldown = true;
        Invoke(nameof(resetCooldown), _cooldownTime);

        _tutorialViewParent.SetActive(true);
        _tutorialEnabled = true;
    }

    private void resetCooldown()
    {
        _cooldown = false;
    }

    public void TutorialHandleOnClick()
    {
        if (!_tutorialEnabled)
            return;
        if (_cooldown)
            return;

        _cooldown = true;
        Invoke(nameof(resetCooldown), _cooldownTime);

        _startPlayEvent.Invoke();
    }
}
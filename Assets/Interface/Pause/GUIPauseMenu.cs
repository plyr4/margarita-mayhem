using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GUIPauseMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _viewParent;
    public GameObject _buttonsViewParent;
    public GameEvent _pausePlayEvent;
    public GameEvent _pauseRetryEvent;
    public float _cooldownTime = 0.5f;
    private bool _cooldown;
    public float _pausePlayDelay = 0.2f;
    public SpriteRenderer _image;
    public List<Sprite> _sprites;
    public float _spritesDelay = 0.5f;
    public float _spritesDuration = 1f;
    private Sequence _spriteSequence;
    public ButtonSprite _muteButtonSprite;
    public ButtonSprite _mutedButtonSprite;
    public ButtonSprite _unmutedButtonSprite;

    public void Start()
    {
        UpdateMutedSprite(false);
    }

    public void ContinueHandleOnClick()
    {
        if (_cooldown)
            return;

        _cooldown = true;
        DOVirtual.DelayedCall(_cooldownTime, () => _cooldown = false).SetUpdate(true);

        DOVirtual.DelayedCall(_pausePlayDelay, () => _pausePlayEvent.Invoke()).SetUpdate(true);

        SoundManager.Instance.PlayButtonConfirm();
    }

    public void RetryHandleOnClick()
    {
        if (_cooldown)
            return;

        _cooldown = true;
        DOVirtual.DelayedCall(_cooldownTime, () => _cooldown = false).SetUpdate(true);

        _pauseRetryEvent.Invoke();

        SoundManager.Instance.PlayButtonConfirm();
    }

    public void MuteHandleOnClick()
    {
        bool muted = SoundManager.Instance.ToggleMute();

        UpdateMutedSprite(muted);

        SoundManager.Instance.PlayButtonConfirm();
    }

    void UpdateMutedSprite(bool muted)
    {
        ButtonSprite muteStatusButton = !muted ? _mutedButtonSprite : _unmutedButtonSprite;
        _muteButtonSprite._normalSprite = muteStatusButton._normalSprite;
        _muteButtonSprite._highlightedSprite = muteStatusButton._highlightedSprite;
        _muteButtonSprite._selectedSprite = muteStatusButton._selectedSprite;
        _muteButtonSprite._pressedSprite = muteStatusButton._pressedSprite;
        _muteButtonSprite._disabledSprite = muteStatusButton._disabledSprite;

        _muteButtonSprite.UpdateSprite();
    }

    public void HandleGameStateChange(IGameEventOpts opts)
    {
        GameStateChangeOpts opts_ = (GameStateChangeOpts)opts;

        switch (opts_._newState)
        {
            case GStateInit _:
                _viewParent.SetActive(false);
                _buttonsViewParent.SetActive(false);
                if (_spriteSequence != null) _spriteSequence.Kill();
                break;
            case GStatePause _:
                OpenPauseMenu();
                SoundManager.Instance.PlayButtonConfirm();
                break;
            case GStatePlay _:
                // ClosePauseMenu();
                _viewParent.SetActive(false);
                _buttonsViewParent.SetActive(false);
                if (_spriteSequence != null) _spriteSequence.Kill();
                break;
            case GStatePauseRetry _:
                _viewParent.SetActive(false);
                _buttonsViewParent.SetActive(false);
                if (_spriteSequence != null) _spriteSequence.Kill();
                break;
        }
    }

    private void OpenPauseMenu()
    {
        if (_spriteSequence != null) _spriteSequence.Kill();

        _viewParent.SetActive(true);
        if (_image == null)
            _image = _viewParent.GetComponentInChildren<SpriteRenderer>();

        _image.sprite = _sprites[0];

        if (_sprites != null && _sprites.Count > 0 && _image != null)
        {
            DOVirtual.DelayedCall(_spritesDelay, () =>
            {
                _spriteSequence = DOTween.Sequence();
                for (int i = 0; i < _sprites.Count; i++)
                {
                    int index = i;
                    _spriteSequence.AppendCallback(() => _image.sprite = _sprites[index]);
                    _spriteSequence.AppendInterval(_spritesDuration);
                }

                _spriteSequence.OnComplete(() =>
                {
                    _buttonsViewParent.SetActive(true);
                    _buttonsViewParent.GetComponentInChildren<UnityEngine.UI.Button>().Select();
                });
                _spriteSequence.SetUpdate(true);
                _spriteSequence.Play();
            });
        }
    }
}
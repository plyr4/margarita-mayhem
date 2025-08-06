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

    public void ContinueHandleOnClick()
    {
        if (_cooldown)
            return;

        _cooldown = true;
        Invoke(nameof(resetCooldown), _cooldownTime);

        DOVirtual.DelayedCall(_pausePlayDelay, () => _pausePlayEvent.Invoke()).SetUpdate(true);
    }

    public void RetryHandleOnClick()
    {
        if (_cooldown)
            return;

        _cooldown = true;
        Invoke(nameof(resetCooldown), _cooldownTime);

        _pauseRetryEvent.Invoke();
    }

    private void resetCooldown()
    {
        _cooldown = false;
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
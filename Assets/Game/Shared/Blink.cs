using System.Collections;
using UnityEngine;

public class Blink : MonoBehaviour
{
    private Coroutine _blinkCoroutine;
    public SpriteRenderer _spriteRenderer;
    public float _duration = 3f;

    public void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        gameObject.SetActive(false);
    }

    public void StartBlinking()
    {
        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
        }

        _blinkCoroutine = StartCoroutine(blink(_duration));
    }

    private IEnumerator blink(float duration, int blinkCount)
    {
        for (int i = 0; i < blinkCount; i++)
        {
            _spriteRenderer.enabled = false;
            yield return new WaitForSeconds(duration / (blinkCount * 2));
            _spriteRenderer.enabled = true;
            yield return new WaitForSeconds(duration / (blinkCount * 2));
        }
    }

    private IEnumerator blink(float duration)
    {
        while (true)
        {
            _spriteRenderer.enabled = false;
            yield return new WaitForSeconds(duration / 4);
            _spriteRenderer.enabled = true;
            yield return new WaitForSeconds(duration / 2);
        }
    }
}
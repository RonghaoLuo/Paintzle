using System.Collections;
using TMPro;
using UnityEngine;

public class PopupItem : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private TMP_Text label;
    [SerializeField] private CanvasGroup canvasGroup; // optional but recommended for fade

    private Coroutine routine;

    public void Show(string text, float duration, float fadeOutSeconds, System.Action<PopupItem> onDone)
    {
        if (routine != null) StopCoroutine(routine);

        label.text = text;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        gameObject.SetActive(true);
        routine = StartCoroutine(Run(duration, fadeOutSeconds, onDone));
        AudioManager.Instance.PlaySfx("Popup", Camera.main.transform.position);
    }

    private IEnumerator Run(float duration, float fadeOutSeconds, System.Action<PopupItem> onDone)
    {
        // Stay visible
        yield return new WaitForSeconds(duration);

        // Fade out (optional)
        if (canvasGroup != null && fadeOutSeconds > 0f)
        {
            float t = 0f;
            float start = canvasGroup.alpha;

            while (t < fadeOutSeconds)
            {
                t += Time.unscaledDeltaTime; // unscaled for UI toasts
                float a = Mathf.Lerp(start, 0f, t / fadeOutSeconds);
                canvasGroup.alpha = a;
                yield return null;
            }

            canvasGroup.alpha = 0f;
        }

        // Done
        routine = null;
        onDone?.Invoke(this);
    }
}

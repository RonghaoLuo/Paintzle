using System.Collections.Generic;
using UnityEngine;

public class PopupQueue : MonoBehaviour
{
    [System.Serializable]
    private struct PopupRequest
    {
        public string text;
        public float duration;
        public float fadeOut;
        public PopupRequest(string t, float d, float f) { text = t; duration = d; fadeOut = f; }
    }

    [Header("UI Layout")]
    [SerializeField] private Transform container;    // a VerticalLayoutGroup is perfect here
    [SerializeField] private PopupItem popupPrefab;

    [Header("Behavior")]
    [SerializeField, Min(1)] private int maxVisible = 3;
    [SerializeField, Min(0.1f)] private float defaultDuration = 2.5f;
    [SerializeField, Min(0f)] private float defaultFadeOut = 0.15f;

    private readonly Queue<PopupRequest> queue = new Queue<PopupRequest>();
    private readonly List<PopupItem> visible = new List<PopupItem>();
    private readonly Stack<PopupItem> pool = new Stack<PopupItem>();

    /// <summary>
    /// Enqueue a popup. If there is room, it shows immediately; otherwise it waits.
    /// </summary>
    public void Enqueue(string text, float? duration = null, float? fadeOut = null)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        float d = duration ?? defaultDuration;
        float f = fadeOut ?? defaultFadeOut;

        queue.Enqueue(new PopupRequest(text, d, f));
        TryDequeueAndShow();
    }

    /// <summary>
    /// Clears queued popups (does not hide ones already visible).
    /// </summary>
    public void ClearQueue()
    {
        queue.Clear();
    }

    /// <summary>
    /// Hides everything and clears queue (useful for scene changes).
    /// </summary>
    public void ClearAll()
    {
        queue.Clear();

        for (int i = visible.Count - 1; i >= 0; i--)
        {
            ReturnToPool(visible[i]);
        }
        visible.Clear();
    }

    private void TryDequeueAndShow()
    {
        // Fill available slots
        while (visible.Count < maxVisible && queue.Count > 0)
        {
            var req = queue.Dequeue();

            PopupItem item = GetFromPool();
            visible.Add(item);

            item.Show(req.text, req.duration, req.fadeOut, OnPopupDone);
        }
    }

    private void OnPopupDone(PopupItem item)
    {
        // Remove from visible
        visible.Remove(item);
        ReturnToPool(item);

        // Now a slot is free, show next queued message(s)
        TryDequeueAndShow();
    }

    private PopupItem GetFromPool()
    {
        PopupItem item;
        if (pool.Count > 0)
        {
            item = pool.Pop();
        }
        else
        {
            item = Instantiate(popupPrefab, container);
        }

        // Ensure it's under the container (in case you move it around)
        if (item.transform.parent != container)
            item.transform.SetParent(container, false);

        return item;
    }

    private void ReturnToPool(PopupItem item)
    {
        item.gameObject.SetActive(false);
        pool.Push(item);
    }
}

using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Temporarily swaps materials on static objects to a neutral material (for light baking),
/// then restores original materials.
/// </summary>
public class BakeNeutralMaterialSwitcher : MonoBehaviour
{
    [Header("Neutral Material")]
    [Tooltip("Material to apply to all static renderers temporarily.")]
    public Material neutralMaterial;

    [Header("Filter (optional)")]
    [Tooltip("If enabled, only renderers on these layers are affected.")]
    public bool filterByLayer = false;

    [Tooltip("Layers to affect when Filter By Layer is enabled.")]
    public LayerMask layers;

    [Tooltip("If enabled, only objects with this tag are affected (leave empty to ignore).")]
    public bool filterByTag = false;

    [Tooltip("Tag to affect when Filter By Tag is enabled.")]
    public string requiredTag = "Untagged";

    [Header("Behavior")]
    [Tooltip("Include renderers that are static via GameObject.isStatic (recommended).")]
    public bool onlyStaticObjects = true;

    [Tooltip("Include inactive objects as well.")]
    public bool includeInactive = true;

    // Cache: Renderer -> original sharedMaterials array
    private readonly Dictionary<Renderer, Material[]> _original = new Dictionary<Renderer, Material[]>();
    private bool _isApplied;

    public void ApplyNeutral()
    {
        if (_isApplied) return;

        if (neutralMaterial == null)
        {
            Debug.LogError("[BakeNeutralMaterialSwitcher] Neutral material is not assigned.", this);
            return;
        }

        _original.Clear();

        // Find all renderers in scene(s)
        var renderers = includeInactive
            ? Resources.FindObjectsOfTypeAll<Renderer>()
            : Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None);

        int affected = 0;

        foreach (var r in renderers)
        {
            if (r == null) continue;

#if UNITY_EDITOR
            // Skip assets/prefab assets not in a scene
            if (!r.gameObject.scene.IsValid()) continue;
#endif

            var go = r.gameObject;

            if (onlyStaticObjects && !go.isStatic) continue;

            if (filterByLayer && ((layers.value & (1 << go.layer)) == 0)) continue;

            if (filterByTag)
            {
                if (string.IsNullOrEmpty(requiredTag))
                {
                    // If they enabled tag filter but left blank, treat as ignore.
                }
                else if (!go.CompareTag(requiredTag))
                {
                    continue;
                }
            }

            var shared = r.sharedMaterials;
            if (shared == null || shared.Length == 0) continue;

            // Cache original
            _original[r] = shared;

            // Create replacement array preserving submesh count
            var replacement = new Material[shared.Length];
            for (int i = 0; i < replacement.Length; i++)
                replacement[i] = neutralMaterial;

            r.sharedMaterials = replacement;
            affected++;
        }

        _isApplied = true;
        Debug.Log($"[BakeNeutralMaterialSwitcher] Applied neutral material to {affected} renderers.", this);
    }

    public void RestoreOriginal()
    {
        if (!_isApplied) return;

        int restored = 0;

        foreach (var kvp in _original)
        {
            var r = kvp.Key;
            if (r == null) continue;

#if UNITY_EDITOR
            if (!r.gameObject.scene.IsValid()) continue;
#endif

            r.sharedMaterials = kvp.Value;
            restored++;
        }

        _original.Clear();
        _isApplied = false;

        Debug.Log($"[BakeNeutralMaterialSwitcher] Restored original materials on {restored} renderers.", this);
    }

    private void OnDisable()
    {
        // Safety: if you disable the component while neutral is applied, restore.
        RestoreOriginal();
    }
}

#if UNITY_EDITOR
/// <summary>
/// Adds buttons in the inspector for quick Apply/Restore.
/// </summary>
[CustomEditor(typeof(BakeNeutralMaterialSwitcher))]
public class BakeNeutralMaterialSwitcherEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var s = (BakeNeutralMaterialSwitcher)target;

        EditorGUILayout.Space();
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Apply Neutral"))
                s.ApplyNeutral();

            if (GUILayout.Button("Restore Original"))
                s.RestoreOriginal();
        }
    }
}
#endif

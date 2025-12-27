using System;
using UnityEngine;

public class Paintable : MonoBehaviour
{
    private enum PaintCoverageMode { None, Slight, Partial, Most, Full }

    [Header("Renderer")]
    [SerializeField] private Renderer myRenderer;

    [Header("Coverage Progression")]
    [Tooltip("How many paint actions have been applied to this Paintable.")]
    [SerializeField, Min(0)] private int paintCount = 0;

    [Tooltip("Coverage values for each step (index matches PaintCoverageMode).")]
    [SerializeField]
    private float[] coverageSteps = new float[]
    {
        0.00f, // None
        0.1f, // Slight
        0.2f, // Partial
        0.3f, // Most
        1.00f  // Full
    };

    [Header("State")]
    [SerializeField] private bool isPainted = false;
    [SerializeField] private Color paintColour = Color.gray;
    [SerializeField] private bool enableSetColour = true;

    private Color oldColour;
    private MaterialPropertyBlock mpb;

    public Action OnColourChange;

    public Color PaintColour => paintColour;
    public bool IsPainted => isPainted;

    public int PaintCount => paintCount;

    private static readonly int PaintEnabledID = Shader.PropertyToID("_PaintEnabled");
    private static readonly int PaintColorID = Shader.PropertyToID("_PaintColor");
    private static readonly int PaintCoverageID = Shader.PropertyToID("_PaintCoverage");

    private void Awake()
    {
        ApplyMPB();
    }

    public void Paint(Color newColour)
    {
        if (!enableSetColour) return;

        // Increment paint layers first (so coverage increases even if color stays the same)
        IncrementPaintCount();

        isPainted = paintCount > 0;

        oldColour = paintColour;
        paintColour = newColour;

        // Always apply because coverage might have changed even if colour didn't
        ApplyPaintRuntime();

        if (paintColour != oldColour)
        {
            OnColourChange?.Invoke();
        }
    }

    public void ErasePaint()
    {
        if (!enableSetColour) return;

        if (mpb == null) mpb = new MaterialPropertyBlock();

        isPainted = false;
        paintCount = 0; // reset layers
        oldColour = paintColour;
        paintColour = Color.gray;

        myRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat(PaintEnabledID, 0f);
        mpb.SetColor(PaintColorID, Color.gray);
        mpb.SetFloat(PaintCoverageID, GetCoverageFromCount()); // will be 0
        myRenderer.SetPropertyBlock(mpb);

        if (paintColour != oldColour)
        {
            OnColourChange?.Invoke();
        }
    }

    private void IncrementPaintCount()
    {
        int maxCount = GetMaxPaintCount();
        paintCount = Mathf.Clamp(paintCount + 1, 0, maxCount);
    }

    private int GetMaxPaintCount()
    {
        // If you keep 5 steps, max "paintCount" should be 4 (index 4 => Full)
        return Mathf.Max(0, coverageSteps.Length - 1);
    }

    private float GetCoverageFromCount()
    {
        if (coverageSteps == null || coverageSteps.Length == 0) return 0f;

        int idx = Mathf.Clamp(paintCount, 0, coverageSteps.Length - 1);
        return Mathf.Clamp01(coverageSteps[idx]);
    }

    private PaintCoverageMode GetModeFromCount()
    {
        int idx = Mathf.Clamp(paintCount, 0, Enum.GetValues(typeof(PaintCoverageMode)).Length - 1);
        return (PaintCoverageMode)idx;
    }

    private void ApplyPaintRuntime()
    {
        ApplyPaintWithMPB();
    }

    private void ApplyMPB()
    {
        if (myRenderer == null) return;
        if (mpb == null) mpb = new MaterialPropertyBlock();

        float coverage = GetCoverageFromCount();

        myRenderer.GetPropertyBlock(mpb);

        mpb.SetFloat(PaintCoverageID, coverage);
        mpb.SetColor(PaintColorID, paintColour);
        mpb.SetFloat(PaintEnabledID, isPainted ? 1f : 0f);

        myRenderer.SetPropertyBlock(mpb);
    }

    private void ApplyPaintWithMPB()
    {
        if (myRenderer == null) return;
        if (mpb == null) mpb = new MaterialPropertyBlock();

        float coverage = GetCoverageFromCount();

        myRenderer.GetPropertyBlock(mpb);

        mpb.SetFloat(PaintCoverageID, coverage);
        mpb.SetColor(PaintColorID, paintColour);
        mpb.SetFloat(PaintEnabledID, isPainted ? 1f : 0f);

        myRenderer.SetPropertyBlock(mpb);
    }

    private void OnValidate()
    {
        // Keep paintCount within valid range in Inspector
        paintCount = Mathf.Clamp(paintCount, 0, GetMaxPaintCount());

        // If paintCount is 0, consider it not painted
        isPainted = paintCount > 0;

        ApplyMPB();
    }

    public void EnableSetColour() => enableSetColour = true;
    public void DisableSetColour() => enableSetColour = false;

    // Optional helper if you want to read the current mode (debug/UI)
    public string CurrentCoverageMode => GetModeFromCount().ToString();
}

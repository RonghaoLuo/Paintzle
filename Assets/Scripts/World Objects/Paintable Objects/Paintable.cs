using System;
using UnityEngine;

public class Paintable : MonoBehaviour
{
    private enum PaintCoverageMode { Partial, Full }

    [Header("Renderer")]
    [SerializeField] private Renderer myRenderer;

    [Header("Paint Settings")]
    [SerializeField] private PaintCoverageMode paintCoverageMode = PaintCoverageMode.Partial;
    [SerializeField, Range(0, 1)] private float fullPaintCoverage = 1f;
    [SerializeField, Range(0, 1)] private float partialPaintCoverage = 0.3f;

    [Header("State")]
    [SerializeField] private bool isPainted = false;
    [SerializeField] private Color paintColour = Color.gray5;
    [SerializeField] private bool enableSetColour = true;

    private Color oldColour;
    private MaterialPropertyBlock mpb;

    public Action OnColourChange;

    public Color PaintColour => paintColour;
    public bool IsPainted => isPainted;

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
        isPainted = true;
        oldColour = paintColour;
        paintColour = newColour;

        if (paintColour != oldColour)
        {
            ApplyPaintRuntime();
            OnColourChange?.Invoke();
        }
    }

    public void ErasePaint()
    {
        if (!enableSetColour) return;
        if (mpb == null)
            mpb = new MaterialPropertyBlock();

        isPainted = false;
        oldColour = paintColour;
        paintColour = Color.gray5;

        myRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat(PaintEnabledID, 0f);
        mpb.SetColor(PaintColorID, Color.gray5);
        myRenderer.SetPropertyBlock(mpb);

        if (paintColour != oldColour)
        {
            OnColourChange?.Invoke();
        }
    }

    private void ApplyPaintRuntime()
    {
        ApplyPaintWithMPB();
    }

    private void ApplyMPB()
    {
        if (myRenderer == null) return;
        if (mpb == null)
        {
            mpb = new MaterialPropertyBlock();
        }

        float coverage;

        switch (paintCoverageMode)
        {
            case PaintCoverageMode.Partial:
                coverage = partialPaintCoverage;
                break;
            default:
                coverage = fullPaintCoverage;
                break;
        }

        myRenderer.GetPropertyBlock(mpb);

        mpb.SetFloat(PaintCoverageID, coverage);
        mpb.SetColor(PaintColorID, paintColour);
        mpb.SetFloat(PaintEnabledID, isPainted ? 1f : 0f);

        myRenderer.SetPropertyBlock(mpb);
    }

    private void ApplyPaintWithMPB()
    {
        if (myRenderer == null) return;
        if (mpb == null)
        {
            mpb = new MaterialPropertyBlock();
        }

        float coverage;

        switch (paintCoverageMode)
        {
            case PaintCoverageMode.Partial:
                coverage = partialPaintCoverage;
                break;
            default:
                coverage = fullPaintCoverage;
                break;
        }

        myRenderer.GetPropertyBlock(mpb);

        mpb.SetFloat(PaintCoverageID, coverage);
        mpb.SetColor(PaintColorID, paintColour);
        mpb.SetFloat(PaintEnabledID, 1f);

        myRenderer.SetPropertyBlock(mpb);
    }

    private void OnValidate()
    {
        ApplyMPB();
    }

    public void EnableSetColour() => enableSetColour = true;
    public void DisableSetColour() => enableSetColour = false;
}

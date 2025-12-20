#ifndef PAINTABLE_OVERLAY_INCLUDED
#define PAINTABLE_OVERLAY_INCLUDED

float3 ApplyPaintOverlayColor(
    float3 baseColor,
    float3 paintColor,
    float noise,
    float paintCoverage,
    float paintEnabled
)
{
    float n = saturate(noise);

    // CHANGE: Lower 'softness' makes the edge sharper. 
    // 0.05 = soft/fuzzy edge
    // 0.01 = sharp/defined edge
    // 0.001 = jagged/pixelated edge
    float softness = 0.01;
    
    // Logic: As coverage increases, it overcomes the noise value.
    float mask = smoothstep(n - softness, n + softness, paintCoverage);

    // Apply the global toggle
    mask *= step(0.5, paintEnabled);

    return lerp(baseColor, paintColor, mask);
}

#endif
#ifndef COLOR_BLINDNESS_INCLUDED
#define COLOR_BLINDNESS_INCLUDED

void ColorBlindness_float(
    float3 Color,
    float Mode,
    out float3 Out
)
{
    Out = Color;

    if (Mode < 0.5)
    {
        Out = Color;
    }
    else if (Mode < 1.5)
    {
        Out.r = 0.567 * Color.r + 0.433 * Color.g;
        Out.g = 0.558 * Color.r + 0.442 * Color.g;
        Out.b = 0.242 * Color.g + 0.758 * Color.b;
    }
    else if (Mode < 2.5)
    {
        Out.r = 0.625 * Color.r + 0.375 * Color.g;
        Out.g = 0.700 * Color.r + 0.300 * Color.g;
        Out.b = 0.300 * Color.g + 0.700 * Color.b;
    }
    else
    {
        Out.r = 0.950 * Color.r + 0.050 * Color.g;
        Out.g = 0.433 * Color.g + 0.567 * Color.b;
        Out.b = 0.475 * Color.g + 0.525 * Color.b;
    }
}

#endif
Shader "CustomRenderTexture/TimeSeriesShader"
{
    Properties
    {
        _Color1 ("Color 1", Color) = (0.4,0.01,0.01,1)
        _Color2 ("Color 2", Color) = (0.01,0.4,0.01,1)
        _Color3 ("Color 3", Color) = (0.01,0.01,0.4,1)
        _Color4 ("Color 4", Color) = (0.4,0.4,0.01,1)
        _Color5 ("Color 5", Color) = (0.4,0.01,0.4,1)
        _Color6 ("Color 6", Color) = (0.01,0.4,0.4,1)
        _Value1 ("Value 1", Float) = 0
        _Value2 ("Value 2", Float) = 0
        _Value3 ("Value 3", Float) = 0
        _Value4 ("Value 4", Float) = 0
        _Value5 ("Value 5", Float) = 0
        _Value6 ("Value 6", Float) = 0
        _MinValue ("Min Value", Float) = 0
        _MaxValue ("Max Value", Float) = 10
        _HorizontalPadding ("Horizontal Padding", Float) = 0.01
    }

    SubShader
    {
        Lighting Off
        Blend One Zero

        Pass
        {
            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag
            #pragma target 3.0

            float4 _Color1, _Color2, _Color3, _Color4, _Color5, _Color6;
            float _Value1, _Value2, _Value3, _Value4, _Value5, _Value6;
            float _MinValue, _MaxValue, _HorizontalPadding;

            float4 getBrightColor(float4 color, int wraps) {
                return min(color * pow(2, wraps), float4(1,1,1,1));
            }

            float4 frag(v2f_customrendertexture IN) : COLOR
            {
                float2 uv = IN.localTexcoord.xy;
                float4 prevColor = tex2D(_SelfTexture2D, float2(uv.x + 1.0 / _CustomRenderTextureWidth, uv.y));
                
                // Determine which plot we're in
                int plotIndex = floor(uv.y * 6);
                float plotY = frac(uv.y * 6);
                
                // Calculate the value for this pixel
                float value = 0;
                float4 color = float4(0,0,0,1);
                
                if (plotIndex == 0) { value = _Value1; color = _Color1; }
                else if (plotIndex == 1) { value = _Value2; color = _Color2; }
                else if (plotIndex == 2) { value = _Value3; color = _Color3; }
                else if (plotIndex == 3) { value = _Value4; color = _Color4; }
                else if (plotIndex == 4) { value = _Value5; color = _Color5; }
                else if (plotIndex == 5) { value = _Value6; color = _Color6; }
                
                // Normalize the value
                value = (value - _MinValue) / (_MaxValue - _MinValue);
                
                // Apply horizontal padding
                if (plotY < _HorizontalPadding || plotY > (1 - _HorizontalPadding))
                    return float4(0,0,0,1);
                
                // Draw the new value on the right edge as a "horizon plot"
                if (uv.x > (1.0 - 1.0 / _CustomRenderTextureWidth))
                {
                    int wraps = floor(value);
                    float wrappedValue = frac(value);
                    
                    if (wraps == 0) {
                        // Normal case: value is within range
                        return (plotY < value) ? color : float4(0,0,0,1);
                    } else {
                        // Horizon plotting case: value is above range
                        if (plotY < wrappedValue) {
                            // Draw the wrapped bar
                            return getBrightColor(color, wraps);
                        } else if (wraps > 1 && plotY < 1) {
                            // Draw the background for multiple wraps
                            return getBrightColor(color, wraps - 1);
                        } else {
                            // Draw the original color as background
                            return color;
                        }
                    }
                }
                
                // Shift the previous values to the left
                return prevColor;
            }
            ENDCG
        }
    }
}
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

CBUFFER_START(UnityPerMaterial)
TEXTURE2D(_ColorMap); SAMPLER(sampler_ColorMap);
float4 _ColorMap_ST;
float4 _ColorTint;
float _CutOff;
float4 _WindDirection;
float _WindSpeed;
float _NoiseScale;
float _NoiseForce;
float3 _LightDirection;
CBUFFER_END

void AlphaCut(float4 colorSample)
{
    clip(colorSample.a - _CutOff);
}

float2 gradientNoise_dir(float2 p)
{
    p = p % 289;
    float x = (34 * p.x + 1) * p.x % 289 + p.y;
    x = (34 * x + 1) * x % 289;
    x = frac(x / 41) * 2 - 1;
    return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
}

float gradientNoise(float2 p)
{
    float2 ip = floor(p);
    float2 fp = frac(p);
    float d00 = dot(gradientNoise_dir(ip), fp);
    float d01 = dot(gradientNoise_dir(ip + float2(0, 1)), fp - float2(0, 1));
    float d10 = dot(gradientNoise_dir(ip + float2(1, 0)), fp - float2(1, 0));
    float d11 = dot(gradientNoise_dir(ip + float2(1, 1)), fp - float2(1, 1));
    fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
    return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x);
}

float Unity_GradientNoise_float(float2 UV)
{
    return gradientNoise(UV * _NoiseScale) + 0.5;
}

float3 ApplyWindOffset(float3 positionOS)
{
    float2 uv = float2(positionOS.x + _Time.y, positionOS.z + _Time.y);
    float noiseOffset = Unity_GradientNoise_float(uv) * _NoiseForce;
    return positionOS + _WindDirection.rgb * cos(_Time.y * _WindSpeed) + _WindDirection.rgb * noiseOffset;
}
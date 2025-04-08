#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "MyShaderGeneral.hlsl"

struct Attributes
{
    float3 positionOS : POSITION;
    float3 normalOS : NORMAL;
    float2 uv : TEXCOORD0;
};

struct Interpolators
{
    float4 PositionCS : SV_Position;
    float2 uv : TEXCOORD0;
};

float4 GetShadowCasterPositionCS(float3 positionWS, float3 normalWS)
{
    float3 lightDirectionWS = _LightDirection;
    float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));
    #if UNITY_REVERSED_Z
        positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
    #else
        positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
    #endif
    return positionCS;
};


Interpolators Vertex(Attributes input)
{
    Interpolators output;
    input.positionOS = ApplyWindOffset(input.positionOS);
    VertexPositionInputs posInputs = GetVertexPositionInputs(input.positionOS);
    VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS);
    output.PositionCS = GetShadowCasterPositionCS(posInputs.positionWS,normalInputs.normalWS);
    output.uv = TRANSFORM_TEX(input.uv, _ColorMap);
    return output;
}

float4 Fragment(Interpolators input) : SV_Target
{
    float2 uv = input.uv;
    float4 colorSample = SAMPLE_TEXTURE2D(_ColorMap, sampler_ColorMap, uv);
    AlphaCut(colorSample);
    return 0;
}
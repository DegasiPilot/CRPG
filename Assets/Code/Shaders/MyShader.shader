Shader "MyShaders/Shader1"
{
    Properties{
        [MainTexture] _ColorMap("MainTexture", 2D) = "white" {}
        [HDR][MainColor] _ColorTint("Color", Color) = (1,1,1,1)
        _CutOff("AlphaCutOut", Range(0,1)) = 0.5
        _WindDirection("WindDirection", Vector) = (0,0,0,0)
        _WindSpeed("WindSpeed", Float) = 0
        _NoiseScale("NoiseScale", Float) = 1
        _NoiseForce("NoiseForce", Float) = 0.01
    }

    SubShader{
        Tags{"RendererPipeline" = "UniversalPipeline"}
        Cull Off

        Pass{
            Name "ForwardLit"
            Tags{"LightMode" = "UniversalForward" "RendererType" = "TransparentCutout" "Queue" = "AlphaTest"}

            HLSLPROGRAM
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma vertex Vertex
            #pragma fragment Fragment
            #include "MyShaderPass.hlsl"
            ENDHLSL
        }
        Pass{
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ColorMask 0

            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "MyShaderShadowPass.hlsl"
            ENDHLSL
        }
    }
}
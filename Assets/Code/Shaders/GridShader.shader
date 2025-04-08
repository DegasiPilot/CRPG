Shader "Custom/GridShader"
{
    Properties
    {
        _GridSize ("Grid Size", Float) = 1.0
        _LineWidth ("Line Width", Float) = 0.1
        _GridColor ("Grid Color", Color) = (1,1,1,1)
        _BackgroundColor ("Background Color", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" // Указываем, что шейдер прозрачный
            "Queue"="Transparent"     // Рендерим после непрозрачных объектов
        }
        LOD 200

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha // Включаем смешивание для прозрачности
            ZWrite Off                      // Отключаем запись в буфер глубины

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float _GridSize;
            float _LineWidth;
            fixed4 _GridColor;
            fixed4 _BackgroundColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * _GridSize; // Масштабируем UV по размеру сетки
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Вычисляем дробную часть для определения линий
                float2 grid = frac(i.uv);
                float gridLine = min(min(grid.x, grid.y), 1-max(grid.x, grid.y));
                // Определяем, где рисовать линии
                float2 border = step(_LineWidth, gridLine);
                float alpha = (border.x * border.y);

                // Смешиваем цвет сетки и фона
                return lerp(_GridColor, _BackgroundColor, alpha);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}

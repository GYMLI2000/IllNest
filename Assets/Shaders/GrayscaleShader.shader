Shader "Custom/LitGrayscaleSprite"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Grayscale ("Grayscale Amount", Range(0,1)) = 1
        _LightIntensity ("Light Intensity", Range(0,2)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Grayscale;
            float _LightIntensity;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR; // <- tady pøichází SpriteRenderer.color
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color; // pøedání barvy do fragment shaderu
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // grayscale
                float gray = dot(col.rgb, float3(0.299,0.587,0.114));
                col.rgb = lerp(col.rgb, gray.xxx, _Grayscale);

                // lighting
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float NdotL = max(0.0, dot(float3(0,0,1), lightDir));
                col.rgb *= (_LightIntensity * NdotL + 0.5); // ambient

                // aplikace SpriteRenderer.color
                col *= i.color;

                return col;
            }
            ENDCG
        }
    }
}

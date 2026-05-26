Shader "Custom/AlzheimerFogWorld"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {} // Added to sample the sprite's image
        _PlayerPos ("Player Position", Vector) = (0,0,0,0)
        _Radius ("Radius", Float) = 2
        _Softness ("Softness", Float) = 0.1
        _Darkness ("Darkness", Float) = 0.8
        _NoiseStrength ("Noise Strength", Float) = 0.05
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        Cull Off ZWrite Off ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex; // Required to read the sprite graphic
            float4 _PlayerPos;
            float _Radius;
            float _Softness;
            float _Darkness;
            float _NoiseStrength;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0; // Added UVs to map the texture
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float2 uv : TEXCOORD1; // Added UVs for the fragment pass
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv = v.uv; 
                return o;
            }

            float rand(float2 co)
            {
                return frac(sin(dot(co, float2(12.9898,78.233))) * 43758.5453);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Grab the actual colors of the sprite the material is attached to
                fixed4 texColor = tex2D(_MainTex, i.uv);

                float dist = distance(i.worldPos.xy, _PlayerPos.xy);

                float mask = smoothstep(_Radius, _Radius + _Softness, dist);

                float noise = (rand(i.worldPos.xy * 123.456) - 0.5) * _NoiseStrength;
                float dark = saturate(mask * _Darkness + noise);

                fixed3 fogColor = fixed3(.2f, .2f, .2f); 
                
                // Blend between the sprite's actual color and the fog color (instead of black and fog)
                fixed3 finalColor = lerp(texColor.rgb, fogColor, dark);

                // Multiply by the sprite's alpha so we don't draw fog outside the sprite's borders,
                // and multiply by 'dark' to maintain the transparent cutout hole at the player position.
                return fixed4(finalColor, texColor.a * dark); 
            }
            ENDCG
        }
    }
}
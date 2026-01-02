Shader "Custom/AlzheimerFogWorld"
{
    Properties
    {
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

            float4 _PlayerPos;
            float _Radius;
            float _Softness;
            float _Darkness;
            float _NoiseStrength;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            float rand(float2 co)
            {
                return frac(sin(dot(co, float2(12.9898,78.233))) * 43758.5453);
            }

            fixed4 frag(v2f i) : SV_Target
{
    float dist = distance(i.worldPos.xy, _PlayerPos.xy);


    float mask = smoothstep(_Radius, _Radius + _Softness, dist);

    float noise = (rand(i.worldPos.xy * 123.456) - 0.5) * _NoiseStrength;
    float dark = saturate(mask * _Darkness + noise);


    fixed3 fogColor = fixed3(.2f, .2f, .2f); 
    fixed3 finalColor = lerp(fixed3(0,0,0), fogColor, dark);

    return fixed4(finalColor, dark); 
}

            ENDCG
        }
    }
}

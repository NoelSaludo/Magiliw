Shader "UI/SimpleBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur", Range(0, 5)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float _BlurSize;

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * 0.4;
                col += tex2D(_MainTex, i.uv + float2(_BlurSize * 0.01, 0)) * 0.3;
                col += tex2D(_MainTex, i.uv - float2(_BlurSize * 0.01, 0)) * 0.3;
                return col;
            }
            ENDCG
        }
    }
}
Shader "UI/GrabBlur"
{
    Properties
    {
        _BlurSize ("Blur Amount", Range(0, 10)) = 2
        _Opacity ("Opacity", Range(0, 1)) = 1
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" }

        // Capture the screen behind the UI
        GrabPass { "_GrabTexture" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 grabPos : TEXCOORD0;
            };

            sampler2D _GrabTexture;
            float _BlurSize;
            float _Opacity;

            v2f vert(appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Apply blur by sampling multiple points
                float2 uv = i.grabPos.xy / i.grabPos.w;
                fixed4 blur = fixed4(0, 0, 0, 0);
                float blurScale = _BlurSize * 0.01;

                // Simple 5-tap blur (adjust for performance/quality)
                blur += tex2D(_GrabTexture, uv + float2(-2.0 * blurScale, 0)) * 0.1;
                blur += tex2D(_GrabTexture, uv + float2(-1.0 * blurScale, 0)) * 0.2;
                blur += tex2D(_GrabTexture, uv) * 0.4;
                blur += tex2D(_GrabTexture, uv + float2(1.0 * blurScale, 0)) * 0.2;
                blur += tex2D(_GrabTexture, uv + float2(2.0 * blurScale, 0)) * 0.1;

                blur.a = _Opacity; // Control transparency
                return blur;
            }
            ENDCG
        }
    }
}
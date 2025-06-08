Shader "UI/BlurMask"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _BlurSize ("Blur Size", Range(0, 10)) = 1
        
        // Rect Mask 2D critical properties
        [HideInInspector] _ClipRect ("Clip Rect", Vector) = (-32767,-32767,32767,32767)
        [HideInInspector] _UseClipRect ("Use Clip Rect", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float _BlurSize;
            float4 _ClipRect;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.worldPosition = IN.vertex;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                #ifdef UNITY_UI_CLIP_RECT
                clip(UnityGet2DClipping(IN.worldPosition.xy, _ClipRect) - 0.001);
                #endif

                float4 sum = float4(0,0,0,0);
                float2 texcoord = IN.texcoord;
                
                // 9-tap Gaussian blur
                sum += tex2D(_MainTex, float2(texcoord.x - 4.0*_BlurSize, texcoord.y)) * 0.05;
                sum += tex2D(_MainTex, float2(texcoord.x - 3.0*_BlurSize, texcoord.y)) * 0.09;
                sum += tex2D(_MainTex, float2(texcoord.x - 2.0*_BlurSize, texcoord.y)) * 0.12;
                sum += tex2D(_MainTex, float2(texcoord.x - _BlurSize, texcoord.y)) * 0.15;
                sum += tex2D(_MainTex, float2(texcoord.x, texcoord.y)) * 0.18;
                sum += tex2D(_MainTex, float2(texcoord.x + _BlurSize, texcoord.y)) * 0.15;
                sum += tex2D(_MainTex, float2(texcoord.x + 2.0*_BlurSize, texcoord.y)) * 0.12;
                sum += tex2D(_MainTex, float2(texcoord.x + 3.0*_BlurSize, texcoord.y)) * 0.09;
                sum += tex2D(_MainTex, float2(texcoord.x + 4.0*_BlurSize, texcoord.y)) * 0.05;
                
                return sum * IN.color;
            }
            ENDCG
        }
    }
}
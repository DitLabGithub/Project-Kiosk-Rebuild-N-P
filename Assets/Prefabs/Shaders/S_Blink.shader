Shader "Custom/UIBlink"
{
    Properties
    {
        _MainTex ("Sprite", 2D) = "white" {}
        _BlinkSpeed ("Blink Speed", Float) = 2
        _MinAlpha ("Min Alpha", Range(0,1)) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

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

            float _BlinkSpeed;
            float _MinAlpha;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
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

                o.vertex =
                    UnityObjectToClipPos(v.vertex);

                o.uv = v.uv;

                o.color = v.color;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col =
                    tex2D(_MainTex, i.uv);

                float blink =
                    abs(
                        sin(
                            _Time.y *
                            _BlinkSpeed
                        )
                    );

                blink =
                    lerp(
                        _MinAlpha,
                        1,
                        blink
                    );

                col.a *= blink;

                return col * i.color;
            }

            ENDCG
        }
    }
}
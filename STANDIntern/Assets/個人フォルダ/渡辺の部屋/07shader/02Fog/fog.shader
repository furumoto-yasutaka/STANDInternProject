Shader "Custom/fog"
{
    Properties
    {
        //[NoScaleOffset]
        _MainTex("Texture", 2D) = "white" {}

        [Space(15)]
        [Header(Color)]
        [Space(5)]
        _Tint("Tint",Color) = (1,1,1,1)
        _FogColor("FogColor",Color) = (0.2588236, 0.6790779, 0.8784314, 0.4901961)
    }

        SubShader
    {
        //ブレンドモード
        Blend SrcAlpha OneMinusSrcAlpha
        //レンダラーモード
        Tags { "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }

        Pass
        {
            CGPROGRAM

            #pragma vertex   vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv     : TEXCOORD;
            };

            sampler2D _MainTex;
            float4    _MainTex_ST;

            fixed4 _Tint;
            fixed4 _FogColor;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                //テクスチャの設定
                fixed4 color = tex2D(_MainTex,i.uv);
            //color = mul(_Tint, color);
            color.rgb = lerp(_FogColor.rgb, color.rgb, _FogColor.a);

            return color;
            }

            ENDCG
        }
    }
}
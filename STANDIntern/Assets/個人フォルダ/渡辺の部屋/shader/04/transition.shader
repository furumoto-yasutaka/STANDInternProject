Shader "Custom/transition"
{
	Properties
	{
		_MainTex("MainTexture", 2D) = "white" {}
		_Pattern("Pattern",2D) = "white"{}
		_Threshold("Threshold",Range(0.0,1.0)) = 0.5
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 100

			Pass
			{
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

				sampler2D _MainTex;
				sampler2D _Pattern;
				float _Threshold;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}

				float getgray(fixed4 c)
						{
							return c.r * 0.2126 + c.g * 0.7152 + c.b * 0.0722;
						}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.uv);
					float g = getgray(col);

					fixed4 gray = fixed4(g, g, g, col.a);

					fixed4 pattern = tex2D(_Pattern, i.uv);
					float pg = getgray(pattern);
					pg = clamp(pg, 0.01, 0.99);

					float a = step(pg, _Threshold);
					return lerp(gray, col, a);
				}
				ENDCG
			}
		}
}
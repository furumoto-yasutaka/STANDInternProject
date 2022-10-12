Shader "Custom/water"
{
    Properties
    {
        [HDR] _Color("Color",Color) = (1,1,1,1)
        _Tiling("Tiling",Vector) = (1,1,0,0)
        _Offset("Offset",Vector) = (0,0,0,0)
        _CellDensity("CellDensity",Float) = 0
        _CellShuffleSpeed("CellShuffleSpeed",Float) = 0
        _WavePower("WavePower",Range(0,1)) = 0.5
        _WhiteStrongs("WhiteStrongs",Float) = 10
    }

        CGINCLUDE
#include "UnityCG.cginc"

#include "Common.cginc"

    float2 _Tiling;
    float2 _Offset;
    float  _CellDensity;
    float  _CellShaffleSpeed;
    float  _WavePower;
    float4 _Color;
    float  _WhiteStrongs;

    float4 zeroColor = float4(0, 0, 0, 0);

    


    
        //タイリングUV
        float2 TileUV(float2 UV, float2 Tile, float2 Offset)
        {
            return UV * Tile + Offset;
        }
        //ノイズの動く方向
        float2 voronoi_noise_randomvector(float2 UV, float offset)
        {
            float2x2 rotate = float2x2(15.27, 47.63, 99.41, 89.98);
            UV = frac(sin(mul(UV, rotate)) * 46839.32);
            return float2(sin(UV.y * +offset) * 0.5 + 0.5, cos(UV.x * offset) * 0.5 + 0.5);
        }
        //ボロノイシェーダー
        float Voronoi(float2 UV, float AngleOffset, float CellDensity)
        {
            float2 ist = floor(UV * CellDensity);
            float2 fst = frac(UV * CellDensity);

            float d;
            float t = 8.0;
            float3 res = float3(8.0, 0.0, 0.0);
            float angle = mul(_Time.y, _CellShaffleSpeed);
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    float2 lattice = float2(x, y);//隣接しているマス
                    float2 offset = voronoi_noise_randomvector(lattice + ist, _Time.y);

                    float d = distance(lattice + offset, fst);

                    if (d < res.x)
                    {
                        res = float3(d, offset.x, offset.y);
                    }
                }
            }
            return res.x;
        }
        //
        float UpWater(float2 UV)
        {
            float upwater = UV.y;
            upwater = pow(abs(-1 * upwater + 1), _WhiteStrongs + sin(_Time.y));
            return upwater;
        }
    
    float4 frag(v2f_img i) : SV_Target
    {
        float2 worldPosUV = TileUV(i.uv,_Tiling,_Offset);
        float angle = mul(_Time.y, _CellShaffleSpeed);

        //UV
        float up = UpWater(i.uv);

        //ボロノイの作成
        float caustic = Voronoi(worldPosUV, angle, _CellDensity);
        caustic = pow(abs(caustic),2.64);
        caustic = clamp(caustic,0, 1);
        caustic = mul(caustic, _WavePower);
        caustic = up + caustic;
        float4 color = lerp(zeroColor, _Color, caustic);
        return float4(color.rgb,caustic);
    }
    ENDCG

    SubShader
    {
        Tags{
           "Queue" = "AlphaTest"
           "RenderType" = "TransparentCutout"
        }
            Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            ENDCG
        }
    }
}
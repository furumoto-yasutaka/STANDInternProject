Shader "Custom/reflectwater"
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

    //グラデーションノイズの向き
    float2 unity_gradientNoise_dir(float2 p)
    {
        p = p % 289;
        float x = (34 * p.x + 1) * p.x % 289 + p.y;
        x = (34 * x + 1) * x % 289;
        x = frac(x / 41) * 2 - 1;
        return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
    }
    //グラデーションノイズ
    float unity_gradientNoise(float2 p)
    {
        float2 ip = floor(p);
        float2 fp = frac(p);
        float d00 = dot(unity_gradientNoise_dir(ip), fp);
        float d01 = dot(unity_gradientNoise_dir(ip + float2(0, 1)), fp - float2(0, 1));
        float d10 = dot(unity_gradientNoise_dir(ip + float2(1, 0)), fp - float2(1, 0));
        float d11 = dot(unity_gradientNoise_dir(ip + float2(1, 1)), fp - float2(1, 1));
        fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
        return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x);
    }
    //ノイズの出力
    float Unity_GradientNoise_float(float2 UV, float Scale)
    {
        return  unity_gradientNoise(UV * Scale) + 0.5;
    }
    

    float4 frag(v2f_img i) : SV_Target
    {

        return (1,1,1,1);
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
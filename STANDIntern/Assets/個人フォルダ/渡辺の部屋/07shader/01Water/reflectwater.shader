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

        _RippleScale("RippleScale",Float) = 10
        _ReflectionStrength("ReflectStrength",Float) = 1
        _CausticScale("CausticScale",Vector) = (1,1,1,1)
        _CellSize("CellSize",Float)=0.03
        _CausticBrightNess("CausticBrightNess",Float)=1
        _WaveStrength("WaveStrength",Range(0,1))=0.5
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
    float  _RippleScale;
    float  _ReflectionStrength;
    float4 _CausticScale;
    float _CellSize;
    float _CausticBrightNess;
    float _WaveStrength;
    float4 zeroColor = float4(0, 0, 0, 0);
    

    //タイリングUV
    float2 TileUV(float2 UV, float2 Tile, float2 Offset)
    {
        return UV * Tile + Offset;
    }
    //Remap
    float Remap(float4 In, float2 InMinMax, float2 OutMinMax)
    {
        return OutMinMax.x + (In - InMinMax.x) * (OutMinMax.x) / (InMinMax.y - InMinMax.x);
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
    
    float UvAutScrollSpeed;
    float2 UVDown;
    float2 UVUp;
    float GradientNoiseUP;
    float GradientNoiseDown;
    float MixGradientNoise;

    float TopGradient;
    float RemapReflection;
    float2 VornoiUV;
    float Vornoi;

    float VornoiSpeed;

    //コースティック
    float CausticShuffleSpeed;

    float2 MixGradientVornoi;

    float4 frag(v2f_img i) : SV_Target
    {
        //波のノイズ生成
        UvAutScrollSpeed=mul(_Time.y,0.05);
        UVDown = TileUV(i.uv, float2(0.18, 1.0), float2(0, UvAutScrollSpeed));
        UVUp = TileUV(i.uv, float2(0.18, 1.0), float2(0, 1 - UvAutScrollSpeed));
        GradientNoiseUP = Unity_GradientNoise_float(UVUp, _RippleScale);
        GradientNoiseDown = Unity_GradientNoise_float(UVDown, _RippleScale);
        MixGradientNoise = mul(GradientNoiseDown, GradientNoiseUP);

        //ノイズの強さの割合UVの位置によってを決定
        TopGradient = pow(abs(i.uv.y),13.1);
        RemapReflection = Remap(_ReflectionStrength, float2(0, 1), float2(0, 0.02));
        MixGradientNoise = mul(MixGradientNoise, RemapReflection);
        MixGradientNoise = lerp(MixGradientNoise, 0, TopGradient);

        //ボロノイ
        CausticShuffleSpeed = mul(_Time.y, 0.58);
        VornoiUV = TileUV(i.pos, _CausticScale,float2(0,0));
        Vornoi = Voronoi(VornoiUV, CausticShuffleSpeed, _CellSize);
        Vornoi = clamp(pow(abs(mul(Vornoi, _CausticBrightNess)),8),0,1);
        VornoiSpeed = mul(Vornoi, _WaveStrength);

        MixGradientVornoi = float2(MixGradientNoise, VornoiSpeed) + float2(0,1);


        return (i.uv.x,i.uv.y,0,1);
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
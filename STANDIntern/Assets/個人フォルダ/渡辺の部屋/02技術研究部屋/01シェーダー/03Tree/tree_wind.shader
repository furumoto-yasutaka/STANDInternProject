Shader "Custom/tree_wind"
{
    Properties
    {
        _WindSpeed("WindSpeed",Float)=1
        _WindDirection("WindDirection",Vector)=(1,1,0,0)
    }

     CGINCLUDE
     #include "UnityCG.cginc"

    struct v2f
    {
        float4 pos :POSITION;
    };

    float2 Calicurate_WorldUVScrollDirection()
    {

    }


    //タイリングUV
    float2 TileUV(float2 UV, float2 Tile, float2 Offset)
    {
        return UV * Tile + Offset;
    }


    ////ノイズの動く方向
    //float2 voronoi_noise_randomvector(float2 UV, float offset)
    //{
    //    float2x2 rotate = float2x2(15.27, 47.63, 99.41, 89.98);
    //    UV = frac(sin(mul(UV, rotate)) * 46839.32);
    //    return float2(sin(UV.y * +offset) * 0.5 + 0.5, cos(UV.x * offset) * 0.5 + 0.5);
    //}
    ////ボロノイシェーダー
    //float Voronoi(float2 UV, float AngleOffset, float CellDensity)
    //{
    //    float2 ist = floor(UV * CellDensity);
    //    float2 fst = frac(UV * CellDensity);
    //    float d;
    //    float t = 8.0;
    //    float3 res = float3(8.0, 0.0, 0.0);
    //    float angle = mul(_Time.y, _CellShaffleSpeed);
    //    for (int y = -1; y <= 1; y++)
    //    {
    //        for (int x = -1; x <= 1; x++)
    //        {
    //            float2 lattice = float2(x, y);//隣接しているマス
    //            float2 offset = voronoi_noise_randomvector(lattice + ist, _Time.y);
    //            float d = distance(lattice + offset, fst);
    //            if (d < res.x)
    //            {
    //                res = float3(d, offset.x, offset.y);
    //            }
    //        }
    //    }
    //    return res.x;
    //}
    //

    //変数
    float2 uv;

    //プロパティ
    float _WindSpeed;
    float4 _WindDirection;

    v2f vertex(appdata_base v)
    {
        //頂点を受け取る
        float3 pos = v.vertex;
        float2 xy = pos.xy;

        //風のスピードを計算
        float windspeed = mul(_Time.y, _WindSpeed);
        //風の向き
        float2 winddirection = _WindDirection.xy;
        //風の向きをかけ合わせる
        float2 windvec = mul(windspeed, winddirection);

        //風の向きとベクトルを得たUV
        uv = TileUV(xy, float2(1, 1), windvec);


    }

    float4 frag(v2f_img i) : SV_Target
    {
        return (1,1,1,1);
    }
        ENDCG

        SubShader
    {
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
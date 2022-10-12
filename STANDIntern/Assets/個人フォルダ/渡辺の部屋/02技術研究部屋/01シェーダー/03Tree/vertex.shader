Shader "Custom/vertex"
{
    Properties
    {
        _Maintex("Main Texture",2D) = "white"{}
        _WindSpeed("WindSpeed",Float) = 1
        _WindDirection("WindDirection",Vector) = (1,1,0,0)
        _WindScale("WindScale",Float) = 1
        _WindStrength("WindStrength",Float) = 0.01
        _WindInfluence("WindInfluence",Float) = 0.01
    }

    SubShader
    {
            Blend SrcAlpha OneMinusSrcAlpha
        Tags { "RenderType" = "Opaque" }

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
                float4 vertex : SV_POSITION;
                float2 uv:TEXCOORD0;
            };
            //�^�C�����OUV
            float2 TileUV(float2 UV, float2 Tile, float2 Offset)
            {
                return UV * Tile + Offset;
            }
            //�O���f�[�V�����m�C�Y�̌���
            float2 unity_gradientNoise_dir(float2 p)
            {
                p = p % 289;
                float x = (34 * p.x + 1) * p.x % 289 + p.y;
                x = (34 * x + 1) * x % 289;
                x = frac(x / 41) * 2 - 1;
                return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
            }
            //�O���f�[�V�����m�C�Y
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
            //�m�C�Y�̏o��
            float Unity_GradientNoise_float(float2 UV, float Scale)
            {
                return  unity_gradientNoise(UV * Scale) + 0.5;
            }

            //�ϐ�
            float2 uv;
            float2 noisedirection;
            float3 transform;
            float3 noisepos;

            //�v���p�e�B
            sampler _Maintex;
            float _WindSpeed;
            float4 _WindDirection;
            float _WindScale;
            float _WindStrength;
            float _WindInfluence;

            v2f vert(appdata v)
            {
                v2f o;
                //���_���󂯎��
                float3 pos = v.vertex;
                float2 xy = pos.xy;
                float2 uv = v.uv;
                o.uv = v.uv;

                //���̃X�s�[�h���v�Z
                float windspeed = mul(_Time.y, _WindSpeed);
                //���̌���
                float2 winddirection = _WindDirection.xy;
                //���̌������������킹��
                float2 windvec = mul(windspeed, winddirection);

                //���̌����ƃx�N�g���𓾂�UV
                uv = TileUV(xy, float2(1, 1), windvec);
                //�O���f�[�V�����m�C�Y��UV�������
                float noise = Unity_GradientNoise_float(uv, _WindScale);

                noise = noise - 0.5;
                //�m�C�Y�ɕ��̋������������킹��
                noise = mul(noise, _WindStrength);
                //�m�C�Y�ɕ��̌������������킹��
                noisedirection = mul(noise, _WindDirection);
                
                //�}�X�N�v�Z
                float mask = abs(uv.y);
                mask = clamp(pow(mask, _WindInfluence),0,1);
                
                //�}�X�N�ƃm�C�Y���������킹��
                noisedirection = mul(mask, noisedirection);
                
                noisepos = float3(noisedirection.x, noisedirection.y, 0);
                
                transform = noisepos + pos;

                v.vertex.xy=transform;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                 fixed4 color = tex2D(_Maintex,i.uv);
                 return color;
            }
            ENDCG
        }
    }
}
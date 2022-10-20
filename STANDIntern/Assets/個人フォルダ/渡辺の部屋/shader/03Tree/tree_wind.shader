Shader "Custom/tree_wind"
{
    Properties
    {
        [Header(Texture)]
        [Space(5)]
        [NoScaleOffset]_Maintex("Main Texture",2D) = "white"{}
        [Space(15)]

        [Header(Wind)]
        [Space(5)]
        [PowerSlider(2)]_WindSpeed("WindSpeed",Range(0,10)) = 1
        _WindDirection("WindDirection",Vector) = (1,1,0)
        [PowerSlider(2)]_WindScale("WindScale",Range(0,0.2)) = 1
        [PowerSlider(2)]_WindStrength("WindStrength",Range(0,1.0)) = 0.01
        [PowerSlider(2)]_WindInfluence("WindInfluence",Range(0,10)) = 0.01

        [Space(15)]
        [Header(Color)]
        [Space(5)]
        _Tint("Tint",Color)=(1,1,1,1)
        _FogColor("FogColor",Color)= (0.2588236, 0.6790779, 0.8784314, 0.4901961)
    }

    SubShader
    {
        //�u�����h���[�h
        Blend SrcAlpha OneMinusSrcAlpha
        //�����_���[���[�h
        Tags { "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }

        Pass
        {
            CGPROGRAM
           #pragma vertex vert
           #pragma fragment frag

           #include "UnityCG.cginc"

            //���_�V�F�[�_�[
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            //�t���O�����g�V�F�[�_�[
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
            float2 noisedirection;
            float2 st;

            //�v���p�e�B
            sampler _Maintex;
            float _WindSpeed;
            float4 _WindDirection;
            float _WindScale;
            float _WindStrength;
            float _WindInfluence;
            fixed4 _Tint;
            fixed4 _FogColor;

            v2f vert(appdata v)
            {
                v2f o;
                //UV�̃f�[�^���킽��
                o.uv = v.uv;
                //���_���󂯎��
                float3 pos = v.vertex;
                
                //���̏���
                //���̃X�s�[�h���v�Z
                float windspeed = mul(_Time.y, _WindSpeed);
                //���̌���
                float2 winddirection = _WindDirection.xy;
                //���̌������������킹��
                float2 windvec = mul(windspeed, winddirection);

                //���̌����ƃx�N�g���𓾂�UV
                st= TileUV(pos.xy, float2(1, 1), windvec);

                //�m�C�Y
                //�O���f�[�V�����m�C�Y��UV�������
                float noise = Unity_GradientNoise_float(st, _WindScale);
                noise = noise - 0.5;
                //�m�C�Y�ɕ��̋������������킹��
                noise = mul(noise, _WindStrength);
                //�m�C�Y�ɕ��̌������������킹��
                noisedirection = mul(noise, _WindDirection);

                //�}�X�N�v�Z
                float mask = abs(st.y);//y��������Ήe�����󂯂₷��
                mask = clamp(pow(mask, _WindInfluence),0,1);

                //�}�X�N�ƃm�C�Y���������킹��
                noisedirection = mul(mask, noisedirection);

                //���_�f�[�^(x,y)�ɒ��_���ƃm�C�Y�𑫂������̂�n��
                v.vertex.xy = float3(noisedirection.x, noisedirection.y, 0) + pos;
                //�X�N���[�����W�ɑΉ����o��
                o.vertex = UnityObjectToClipPos(v.vertex);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float alpha;
                fixed4 combinecolor;
                //�e�N�X�`���̐ݒ�
                fixed4 color = tex2D(_Maintex,i.uv);
                //color = mul(_Tint, color);
                color.rgb = lerp( _FogColor.rgb, color.rgb, _FogColor.a);
                
                return color;
            }
            ENDCG
        }
    }
}
Shader "Custom/FourSidedTexture" {
    Properties {
        _TopTex ("Top Texture", 2D) = "white" {}
        _BottomTex ("Bottom Texture", 2D) = "white" {}
        _FrontTex ("Front Texture", 2D) = "white" {}
        _BackTex ("Back Texture", 2D) = "white" {}
        _LeftTex ("Left Texture", 2D) = "white" {}
        _RightTex ("Right Texture", 2D) = "white" {}
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _TopTex;
            sampler2D _BottomTex;
            sampler2D _FrontTex;
            sampler2D _BackTex;
            sampler2D _LeftTex;
            sampler2D _RightTex;

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 normal : NORMAL;
            };

            v2f vert (appdata_t v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normal = v.normal;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                if (i.normal.y > 0.5) {
                    return tex2D(_TopTex, i.uv); // 윗면 텍스처
                } else if (i.normal.y < -0.5) {
                    return tex2D(_BottomTex, i.uv); // 아랫면 텍스처
                } else if (i.normal.z > 0.5) {
                    return tex2D(_FrontTex, i.uv); // 앞면 텍스처
                } else if (i.normal.z < -0.5) {
                    return tex2D(_BackTex, i.uv); // 뒷면 텍스처
                } else if (i.normal.x > 0.5) {
                    return tex2D(_RightTex, i.uv); // 오른쪽 텍스처
                } else if (i.normal.x < -0.5) {
                    return tex2D(_LeftTex, i.uv); // 왼쪽 텍스처
                }
                return fixed4(1, 1, 1, 1); // 기본 텍스처 (없을 경우)
            }
            ENDCG
        }
    }
}

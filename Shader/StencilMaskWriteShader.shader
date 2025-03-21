Shader "Custom/StencilMaskWrite"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {} // Thêm dòng này
    }
        SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Overlay" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Stencil
        {
            Ref 1
            Comp Always
            Pass Replace
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0; // Thêm UV
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex; // Thêm texture
            fixed4 _Color;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv; // Truyền UV
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv); // Dùng texture để tránh lỗi
            }
            ENDCG
        }
    }
}

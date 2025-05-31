
Shader "Custom/ThreeWayImageSplitter"
{
    Properties
    {
        _MainTex ("MainTex (For UI Compatibility)", 2D) = "white" {}
        _ImageA ("Image A", 2D) = "white" {}
        _ImageB ("Image B", 2D) = "white" {}
        _ImageC ("Image C", 2D) = "white" {}
        _Angle ("Rotation Angle", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _ImageA;
            sampler2D _ImageB;
            sampler2D _ImageC;
            float _Angle;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float rotateLine(float2 uv, float2 pivot, float angleRad)
            {
                float2 p = uv - pivot;
                float s = sin(angleRad);
                float c = cos(angleRad);
                float2 rotated = float2(p.x * c + p.y * s, -p.x * s + p.y * c);
                return rotated.x;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // Double the angle value
                float angleRad = radians(_Angle * 2.0);

                float d1 = rotateLine(uv, float2(1.0 / 3.0, 0.5), angleRad);
                float d2 = rotateLine(uv, float2(2.0 / 3.0, 0.5), angleRad);

                if (d1 < 0)
                    return tex2D(_ImageA, uv);
                else if (d2 < 0)
                    return tex2D(_ImageB, uv);
                else
                    return tex2D(_ImageC, uv);
            }
            ENDCG
        }
    }
}

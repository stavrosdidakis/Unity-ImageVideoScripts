Shader "Custom/PolarTunnelURP"
{
    Properties
    {
        _MainTex ("Video Texture", 2D) = "white" {}
        _TunnelSize ("Tunnel Size", Float) = 0.25
        _TunnelSpeed ("Tunnel Speed", Float) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            Name "Unlit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _TunnelSize;
            float _TunnelSpeed;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 screenUV : TEXCOORD0;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.screenUV = IN.uv * 2.0 - 1.0; // [-1, 1] space
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float2 p = IN.screenUV;

                // Tunnel math
                float a = atan2(p.y, p.x); // Angle
                float r = sqrt(dot(p, p)); // Radius

                // Map polar to UV
                float tunnelU = a / 3.141592; // angle to -1..1
                float tunnelV = (_Time.y * _TunnelSpeed) + (_TunnelSize / max(r, 0.0001)); // scrolling + radial

                // Combine UVs
                float2 tunnelUV = float2(tunnelU, tunnelV);

                // --- MIRROR WRAP UVs ---
                tunnelUV = abs(frac(tunnelUV)); // this is the magic line

                return tex2D(_MainTex, tunnelUV);
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}

Shader "Custom/VortexTunnel"
{
    Properties
    {
        _MainTex ("Video Texture", 2D) = "white" {}
        _ScrollSpeed ("Scroll Speed", Float) = 0.5
        _CurveFrequency ("Curve Frequency", Float) = 5
        _CurveAmplitude ("Curve Amplitude", Float) = 0.1
        _CurveSpeed ("Curve Speed", Float) = 1
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

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _ScrollSpeed;
            float _CurveFrequency;
            float _CurveAmplitude;
            float _CurveSpeed;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;

                // Time value
                float t = _Time.y;

                // Scroll UV.y over time
                uv.y += t * _ScrollSpeed;

                // Apply vortex distortion to UV.x
                uv.x += sin(uv.y * _CurveFrequency + t * _CurveSpeed) * _CurveAmplitude;

                // Sample texture
                half4 col = tex2D(_MainTex, uv);
                return col;
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}

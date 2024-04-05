// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/NormalTextureShader"
{
    Properties
    {
        _NormalMap("NormalMap", 2D) = "white" {}
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _NormalMap;
            float4 _NormalMap_ST;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float3 tangent : TANGENT;
                float2 uv: TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;

                float3 T : TEXCOORD3;
                float3 B : TEXCOORD4;
                float3 N : TEXCOORD5;
            };


            v2f vert(appdata input)
            {
                v2f output;
                output.vertex = UnityObjectToClipPos(input.vertex);
                output.uv = input.uv;

                float3 worldNormal = mul((float3x3)unity_ObjectToWorld, input.normal);
                float3 worldTangent = mul((float3x3)unity_ObjectToWorld, input.tangent);

                float3 binormal = cross(input.normal, input.tangent.xyz); // *input.tangent.w;
                float3 worldBinormal = mul((float3x3)unity_ObjectToWorld, binormal);

                // and, set them
                output.N = normalize(worldNormal);
                output.T = normalize(worldTangent);
                output.B = normalize(worldBinormal);
                return output;
            }

            fixed4 frag(v2f input) : SV_Target
            {
                float3 normal = tex2D(_NormalMap, input.uv).zyx;
                return float4(normal, 1);
            }
            ENDCG
        }
    }
}

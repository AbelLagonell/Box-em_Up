Shader "Unlit/WorldSpaceTiling"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlendSharpness ("Blend Sharpness", Range(1, 8)) = 2 // Controls how sharp the transition is
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100

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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _BlendSharpness;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Get the absolute value of the normal
                float3 blendWeights = abs(i.worldNormal);
                // Raise to power to make the blend sharper
                blendWeights = pow(blendWeights, _BlendSharpness);
                // Normalize so the sum equals 1
                blendWeights /= (blendWeights.x + blendWeights.y + blendWeights.z);

                // Sample texture from three different directions
                // YZ plane (for surfaces facing X)
                float2 uvYZ = i.worldPos.yz * _MainTex_ST.xy + _MainTex_ST.zw;
                fixed4 colYZ = tex2D(_MainTex, uvYZ);

                // XZ plane (for surfaces facing Y)
                float2 uvXZ = i.worldPos.xz * _MainTex_ST.xy + _MainTex_ST.zw;
                fixed4 colXZ = tex2D(_MainTex, uvXZ);

                // XY plane (for surfaces facing Z)
                float2 uvXY = i.worldPos.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                fixed4 colXY = tex2D(_MainTex, uvXY);

                // Blend the three samples together using the blend weights
                fixed4 finalColor = colYZ * blendWeights.x +
                    colXZ * blendWeights.y +
                    colXY * blendWeights.z;

                return finalColor;
            }
            ENDCG
        }
    }
}
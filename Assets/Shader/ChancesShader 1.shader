// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "Unlit/ChancesShader1"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_TintColor ("Color", Color) = (1,0,0,1)
		_FresnelColor ("Fresnel color", Color) = (1,0,0,1)
		_FresnelIntensity ("Fresnel intensity", Range(0,10)) = 1
	}
	SubShader
	{
		Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" "IgnoreProjector"="True"}
		LOD 100

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			//#pragma multi_compile_fog
			#pragma multi_compile __ LIGHTMAP_ON
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				#if defined(LIGHTMAP_ON)
				float2 uv2: TEXCOORD1;
				#endif
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				#if defined(LIGHTMAP_ON)
				float2 uv2 : TEXCOORD1;
				#endif
				//UNITY_FOG_COORDS(2)
				float4 vertex : SV_POSITION;
				float3 normal : TEXCOORD2;
				float4 worldPos : TEXCOORD3;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			half4 _TintColor;
			half4 _FresnelColor;
			half _FresnelIntensity;			
			
			v2f vert (appdata v)
			{
				v2f o;

				o.normal = UnityObjectToWorldNormal(v.normal);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);

				#if defined(LIGHTMAP_ON)
				o.uv2 = v.uv2.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				//UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) * _TintColor;

				half3 viewDirection = normalize(_WorldSpaceCameraPos - i.worldPos);
				half3 normal = normalize(i.normal);

				half fresnel = 1 - dot(normal, viewDirection);
				col.rgb = col.rgb + fresnel * _FresnelIntensity * _FresnelColor;

				#if defined(LIGHTMAP_ON)
				col.rgb *= (DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv2)));
				#endif

				// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}

// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "Unlit/ChancesShaderBump"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_TintColor ("Color", Color) = (1,0,0,1)
		_FresnelColor ("Fresnel color", Color) = (1,0,0,1)
		_FresnelIntensity ("Fresnel intensity", Range(0,10)) = 1

		[Header(Bumpmap Options)]		
		_BumpMap ("Bumpmap (RGB)", 2D) = "bump" {}
	}
	SubShader
	{
		Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members normal)
//#pragma exclude_renderers d3d11
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			#pragma multi_compile __ LIGHTMAP_ON
			
			#include "UnityCG.cginc"
			#include "UnityStandardUtils.cginc"
			

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				#if defined(LIGHTMAP_ON)
				float2 uv2: TEXCOORD1;
				#endif
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				#if defined(LIGHTMAP_ON)
				float2 uv2 : TEXCOORD1;
				#endif
				UNITY_FOG_COORDS(2)
				float4 vertex : SV_POSITION;
				float4 worldPos : TEXCOORD5;
				float2	bumpUV : TEXCOORD6;
				float3x3 TtoW : TEXCOORD7;
			};

			sampler2D _MainTex; float4 _MainTex_ST;
			sampler2D _BumpMap; float4 _BumpMap_ST;
			half4 _TintColor;
			half4 _FresnelColor;
			half _FresnelIntensity;			
			
			v2f vert (appdata v) 
			{
				v2f o;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);

				#if defined(LIGHTMAP_ON)
				o.uv2 = v.uv2.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				o.bumpUV = TRANSFORM_TEX(v.uv, _BumpMap);
				
				half3 wNormal = UnityObjectToWorldNormal(v.normal);
                half3 wTangent = UnityObjectToWorldDir(v.tangent.xyz);
                half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
                half3 wBitangent = cross(wNormal, wTangent) * tangentSign;
                o.TtoW[0] = half3(wTangent.x, wBitangent.x, wNormal.x);
                o.TtoW[1] = half3(wTangent.y, wBitangent.y, wNormal.y);
                o.TtoW[2] = half3(wTangent.z, wBitangent.z, wNormal.z);

				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) * _TintColor;

				half3 viewDirection = normalize(_WorldSpaceCameraPos - i.worldPos);
				//half3 normal = normalize(i.normal);

				float3 normal = UnpackNormal(tex2D(_BumpMap, i.bumpUV));
				half3 worldNormal;
                worldNormal.x = dot(i.TtoW[0], normal);
                worldNormal.y = dot(i.TtoW[1], normal);
                worldNormal.z = dot(i.TtoW[2], normal);

				half dotNV = max(0,dot(worldNormal, viewDirection));
				half fresnel = 1 - dotNV;
				col.rgb = lerp(_FresnelIntensity * _FresnelColor, col.rgb, dotNV);

				#if defined(LIGHTMAP_ON)
				col.rgb *= (DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv2)));
				#endif

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}

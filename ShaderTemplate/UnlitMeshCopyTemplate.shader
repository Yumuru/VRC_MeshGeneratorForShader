Shader "Custom/UnlitMeshCopy/NewShader" {
	Properties {
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass {
			CGPROGRAM
			#pragma target 5.0
			#pragma vertex VS
			#pragma geometry GS
			#pragma fragment FS
			
			#include "UnityCG.cginc"

      struct GS_OUT {
        float4 vertex : SV_Position;
        float4 texcoord : TEXCOORD0;
      };

			appdata_full VS(appdata_full v) { return v; }

			// メッシュの数がここで指定した数倍になる(1~31)
			#define instanceN 1

			[instance(instanceN)]
			[maxvertexcount(3)]
			void GS(triangle appdata_full i[3], inout TriangleStream<GS_OUT> stream, uint gsid : SV_GSInstanceID) {
				for (int j = 0; j < 3; j++) {
					appdata_full v = i[j];
					GS_OUT o = (GS_OUT) 0;

					// idの取得
					uint id = gsid + instanceN * v.texcoord.z;
					// positionの指定
					float3 pos = v.vertex.xyz;
					pos.z += id;

					o.texcoord = v.texcoord;
					o.vertex = UnityObjectToClipPos(float4(pos, 1));
					stream.Append(o);
				}
				stream.RestartStrip();
			}

			float4 FS(GS_OUT input) : SV_Target {
				float2 uv = input.texcoord.xy;
				return float4(uv, 0., 1);
			}
			ENDCG
		}
	}
}

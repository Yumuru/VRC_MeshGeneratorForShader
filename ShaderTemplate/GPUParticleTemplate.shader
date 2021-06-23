Shader "Custom/GPUParticle/NewShader" {
	Properties {
    _ParticleSize ("Particle Size", Float) = 1.0
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
    Cull Off
		Blend SrcAlpha One
		ZWrite Off
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

      float _ParticleSize;

			// メッシュの数がここで指定した数倍になる(1~31)
			#define instanceN 1

			// mul(UNITY_MATRIX_P, mul(UNITY_MATRIX_MV, float4(p,1)) + float4(v,0,0))
			[instance(instanceN)]
			[maxvertexcount(4)]
			void GS(point appdata_full input[1], inout TriangleStream<GS_OUT> stream, uint gsid : SV_GSInstanceID) {
        appdata_full v = input[0];
        for (uint i = 0; i < 4; i++) {
          GS_OUT o = (GS_OUT) 0;

					// idの取得
          uint id = gsid + instanceN * v.texcoord.z;
					// positionの指定
          float3 pos = 0;
          pos.z += id;

          o.texcoord = float4(i % 2, i / 2, 0, 0);
          float2 vert = (float2(i % 2, i / 2) - .5) * _ParticleSize;
          o.vertex = mul(UNITY_MATRIX_P, float4(UnityObjectToViewPos(float4(pos, 1)), 1.) + float4(vert, 0, 0));
          stream.Append(o);
        }
        stream.RestartStrip();
			}

			float4 FS(GS_OUT input) : SV_Target {
				float2 uv = input.texcoord.xy;
				float2 p = -1. + uv * 2.;
				float t = 0.1 / length(p);
				t = saturate( pow(t, 3.9) );
				t = t < .0001 ? 0 : t;
				float3 col = t;
				return float4(col, 1);
			}
			ENDCG
		}
	}
}

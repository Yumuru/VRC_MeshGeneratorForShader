Shader "Custom/SurfaceMeshCopy/NewShader" {
  Properties {
    _Color ("Color", Color) = (1,1,1,1)
    _Glossiness ("Smoothness", Range(0,1)) = 0.5
    _Metallic ("Metallic", Range(0,1)) = 0.0
    _StartId ("Start ID", Int) = 0
  }
  SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 200

    CGPROGRAM
    // Physically based Standard lighting model, and enable shadows on all light types
    #pragma surface SS Standard fullforwardshadows addshadow
    #pragma vertex VS

    // Use shader model 3.0 target, to get nicer looking lighting
    #pragma target 3.0


    struct Input {
      float4 texcoord;
      float4 color;
    };

    half _Glossiness;
    half _Metallic;
    fixed4 _Color;
    uint _StartId;

    #define PI acos(-1.)
    #define TAU atan(1.)*8.

    // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
    // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
    // #pragma instancing_options assumeuniformscaling
    UNITY_INSTANCING_BUFFER_START(Props)
        // put more per-instance properties here
    UNITY_INSTANCING_BUFFER_END(Props)

    void VS (inout appdata_full v, out Input sinput) {
      // idの取得
      uint id = v.texcoord.z + _StartId;
      // positionの指定
      float3 pos = v.vertex.xyz;
      pos.z += id;

      v.vertex.xyz = pos;
      sinput.texcoord = v.texcoord;
      sinput.color = _Color;
    }

    void SS (Input input, inout SurfaceOutputStandard o) {
        // Albedo comes from a texture tinted by color
        float4 c = input.color;
        o.Albedo = c;
        // Metallic and smoothness come from slider variables
        o.Emission = 0.0 * c;
        o.Metallic = _Metallic;
        o.Smoothness = _Glossiness;
        o.Alpha = c.a;
    }
    ENDCG
  }
  FallBack "Diffuse"
}

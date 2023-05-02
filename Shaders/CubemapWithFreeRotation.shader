Shader "Skybox/CubemapWirhFreeRotation" {
Properties {
    _Tint ("Tint Color", Color) = (.5, .5, .5, .5)
    [Gamma] _Exposure ("Exposure", Range(0, 8)) = 1.0
    _RotationX ("RotationX", Range(-360, 360)) = 0
    _RotationY ("RotationY", Range(-360, 360)) = 0
    _RotationZ ("RotationZ", Range(-360, 360)) = 0
    [NoScaleOffset] _Tex ("Cubemap   (HDR)", Cube) = "grey" {}
}

SubShader {
    Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
    Cull Off ZWrite Off

    Pass {

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 2.0

        #include "UnityCG.cginc"

        samplerCUBE _Tex;
        half4 _Tex_HDR;
        half4 _Tint;
        half _Exposure;
        float _RotationX;
        float _RotationY;
        float _RotationZ;
        

        float3 RotateAround (float3 vertex, float3 angles) // this function made by ChatGPT
        {
            float3 rotated = vertex;
    
            float sina, cosa;
            sincos(angles.x * UNITY_PI / 180.0, sina, cosa);
            float2x2 mX = float2x2(cosa, -sina, sina, cosa);
            rotated = float3(mul(mX, rotated.zy), rotated.x).zyx;
    
            sincos(angles.y * UNITY_PI / 180.0, sina, cosa);
            float2x2 mY = float2x2(cosa, -sina, sina, cosa);
            rotated = float3(mul(mY, rotated.xz), rotated.y).xzy;
    
            sincos(angles.z * UNITY_PI / 180.0, sina, cosa);
            float2x2 mZ = float2x2(cosa, -sina, sina, cosa);
            rotated = float3(mul(mZ, rotated.xy), rotated.z).xyz;
    
            return rotated;
        }

        struct appdata_t {
            float4 vertex : POSITION;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct v2f {
            float4 vertex : SV_POSITION;
            float3 texcoord : TEXCOORD0;
            UNITY_VERTEX_OUTPUT_STEREO
        };

        v2f vert (appdata_t v)
        {
            v2f o;
            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
            float3 angles = float3(_RotationX, _RotationY, _RotationZ);
            float3 rotated = RotateAround(v.vertex, angles);

            o.vertex = UnityObjectToClipPos(rotated);
            o.texcoord = v.vertex.xyz;
            return o;
        }

        fixed4 frag (v2f i) : SV_Target
        {
            half4 tex = texCUBE (_Tex, i.texcoord);
            half3 c = DecodeHDR (tex, _Tex_HDR);
            c = c * _Tint.rgb * unity_ColorSpaceDouble.rgb;
            c *= _Exposure;
            return half4(c, 1);
        }
        ENDCG
    }
}


Fallback Off

}
Shader "Custom/Water"
{
    Properties
    {
        _waterTex("waterTex"       , 2D) = "black" {}
        _waterNM ("waterNormalMap" , 2D) = "black" {}
        
        _alpha      ("alpha"      , Range(0,1)) = 1
        _metallic   ("metallic"   , Range(0,1)) = 0
        _smoothness ("smoothness" , Range(0,1)) = 0
        
        _waveSpeed  ("waveSpeed" , Float) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "ForceNoShadowCasting"="True" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard alpha
        #pragma target 3.0

        sampler2D _waterNM;
        sampler2D _waterTex;

        half _alpha;
        half _metallic;
        half _smoothness;
        half _waveSpeed;

        struct Input
        {
            float2 uv_waterTex;
        };
        

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float2 uv = IN.uv_waterTex;
            uv += float2(_SinTime.z * sin(uv.x), _CosTime.z * cos(uv.y)) * _waveSpeed;
            
            o.Albedo = tex2D(_waterTex, uv).rgb;
            o.Alpha = _alpha;
            o.Normal = UnpackNormal(tex2D(_waterNM, uv));
            o.Metallic = _metallic;
            o.Smoothness = _smoothness;
        }
        ENDCG
    }
    FallBack "Diffuse"
}

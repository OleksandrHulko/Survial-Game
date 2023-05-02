Shader "Custom/Terrain"
{
    Properties
    {
        _waterLevel ("maxWaterLevel" , Float) = 20.0
        _sandLevel  ("maxSandLevel"  , Float) = 30.0
        _grassLevel ("maxGrassLevel" , Float) = 50.0
        _rockLevel  ("maxRockLevel"  , Float) = 80.0
        
        _mixValue ("mixValue", Range(0.1,10)) = 1
        
        _waterTex ("waterTex" , 2D) = "black" {}
        _waterMetallic   ("Metallic"  , Range(0,1)) = 0
        _waterSmoothness ("Smoothness", Range(0,1)) = 0
        
        _sandTex  ("sandTex"  , 2D) = "black" {}
        _sandMetallic   ("Metallic"  , Range(0,1)) = 0
        _sandSmoothness ("Smoothness", Range(0,1)) = 0
        
        _grassTex ("grassTex" , 2D) = "black" {}
        _grassMetallic   ("Metallic"  , Range(0,1)) = 0
        _grassSmoothness ("Smoothness", Range(0,1)) = 0
        
        _rockTex  ("rockTex"  , 2D) = "black" {}
        _rockMetallic   ("Metallic"  , Range(0,1)) = 0
        _rockSmoothness ("Smoothness", Range(0,1)) = 0
        
        _snowTex  ("snowTex"  , 2D) = "black" {}
        _snowMetallic   ("Metallic"  , Range(0,1)) = 0
        _snowSmoothness ("Smoothness", Range(0,1)) = 0
        
        _waterNM ("waterNormalMap" , 2D) = "black"
        _sandNM  ("sandNormalMap"  , 2D) = "black"
        _grassNM ("grassNormalMap" , 2D) = "black"
        _rockNM  ("rockNormalMap"  , 2D) = "black"
        _snowNM  ("snowNormalMap"  , 2D) = "black"
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.5

        half _waterLevel;
        half _sandLevel;
        half _grassLevel;
        half _rockLevel;

        half _mixValue;

        half _waterMetallic;
        half _sandMetallic;
        half _grassMetallic;
        half _rockMetallic;
        half _snowMetallic;
        
        half _waterSmoothness;
        half _sandSmoothness;
        half _grassSmoothness;
        half _rockSmoothness;
        half _snowSmoothness;
        
        sampler2D _waterTex;
        sampler2D _sandTex;
        sampler2D _grassTex;
        sampler2D _rockTex;
        sampler2D _snowTex;

        sampler2D _waterNM;
        sampler2D _sandNM;
        sampler2D _grassNM;
        sampler2D _rockNM;
        sampler2D _snowNM;

        static const half _minLevel = -3000.0f;
        static const half _maxLevel = 3000.0f;
        
        struct Input
        {
            float2 uv_waterTex;
            float2 uv_sandTex;
            float2 uv_grassTex;
            float2 uv_rockTex;
            float2 uv_snowTex;
            
            half3 worldPos;
        };

        int range(half x, half min, half max)
        {
            return step(min , x) * step(x , max);
        }

        half multiplier(half x, half min, half max)
        {
            half minX = min - x;
            half maxX = x - max;
            
            return
              ( _mixValue - minX ) / ( 2.0f * _mixValue ) * range( minX , -_mixValue , _mixValue )
            + ( _mixValue - maxX ) / ( 2.0f * _mixValue ) * range( maxX , -_mixValue , _mixValue )
            + range( x , min , max )                      * range( x , min + _mixValue , max - _mixValue );
        }
        
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            half height = IN.worldPos.y;

            half waterMultiplier = multiplier(height, _minLevel,_waterLevel);
            half sandMultiplier  = multiplier(height,_waterLevel,_sandLevel);
            half grassMultiplier = multiplier(height,_sandLevel,_grassLevel);
            half rockMultiplier  = multiplier(height,_grassLevel,_rockLevel);
            half snowMultiplier  = multiplier(height,_rockLevel, _maxLevel);
            
            half3 waterColor = tex2D(_waterTex , IN.uv_waterTex).rgb * waterMultiplier;
            half3 sandColor  = tex2D(_sandTex  , IN.uv_sandTex).rgb  * sandMultiplier;
            half3 grassColor = tex2D(_grassTex , IN.uv_grassTex).rgb * grassMultiplier;
            half3 rockColor  = tex2D(_rockTex  , IN.uv_rockTex).rgb  * rockMultiplier;
            half3 snowColor  = tex2D(_snowTex  , IN.uv_snowTex).rgb  * snowMultiplier;

            half3 waterNormal = UnpackNormal(tex2D(_waterNM , IN.uv_waterTex)) * waterMultiplier;
            half3 sandNormal  = UnpackNormal(tex2D(_sandNM  , IN.uv_sandTex))  * sandMultiplier;
            half3 grassNormal = UnpackNormal(tex2D(_grassNM , IN.uv_grassTex)) * grassMultiplier;
            half3 rockNormal  = UnpackNormal(tex2D(_rockNM  , IN.uv_rockTex))  * rockMultiplier;
            half3 snowNormal  = UnpackNormal(tex2D(_snowNM  , IN.uv_snowTex))  * snowMultiplier;

            half waterMetallic = _waterMetallic * waterMultiplier;
            half sandMetallic  = _sandMetallic  * sandMultiplier;
            half grassMetallic = _grassMetallic * grassMultiplier;
            half rockMetallic  = _rockMetallic  * rockMultiplier;
            half snowMetallic  = _snowMetallic  * snowMultiplier;

            half waterSmoothness = _waterSmoothness * waterMultiplier;
            half sandSmoothness  = _sandSmoothness  * sandMultiplier;
            half grassSmoothness = _grassSmoothness * grassMultiplier;
            half rockSmoothness  = _rockSmoothness  * rockMultiplier;
            half snowSmoothness  = _snowSmoothness  * snowMultiplier;

            o.Albedo = waterColor + sandColor + grassColor + rockColor + snowColor;
            o.Normal = waterNormal + sandNormal + grassNormal + rockNormal + snowNormal;
            o.Metallic = waterMetallic + sandMetallic + grassMetallic + rockMetallic + snowMetallic;
            o.Smoothness = waterSmoothness + sandSmoothness + grassSmoothness + rockSmoothness + snowSmoothness;
        }
        
        ENDCG
    }
    FallBack "Diffuse"
}

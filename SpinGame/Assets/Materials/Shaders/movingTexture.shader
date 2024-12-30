Shader "Custom/MovingTexture"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _EmissionColor ("Emission Color", Color) = (1, 0.5, 0, 1)
        _Speed ("Flow Speed", Range(0.1, 5.0)) = 1.0
        _Distortion ("Distortion Strength", Range(0.0, 1.0)) = 0.2
        _Offset ("UV Offset", Float) = 0.0
        _Smoothness ("Smoothness", Range(0.0, 1.0)) = 0.8
        _Metallic ("Metallic", Range(0.0, 1.0)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _NormalMap;
        fixed4 _EmissionColor;
        float _Speed;
        float _Distortion;
        float _Offset;
        float _Smoothness;
        float _Metallic;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NormalMap;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float time = _Offset;

            float2 uvDistorted = IN.uv_MainTex + float2(time, time * 0.5);
            float2 uvNormal = IN.uv_NormalMap + float2(time * 0.5, time);

            uvDistorted += (_Distortion * tex2D(_NormalMap, uvNormal).rg);

            fixed4 c = tex2D(_MainTex, uvDistorted);
            o.Albedo = c.rgb;

            // Add normal mapping
            o.Normal = UnpackNormal(tex2D(_NormalMap, uvNormal));

            // Simulate emissive lava glow
            o.Emission = _EmissionColor.rgb * c.r;

            // Use user-defined Metallic and Smoothness
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
        }
        ENDCG
    }

    FallBack "Diffuse"
}
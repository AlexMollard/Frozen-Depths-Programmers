Shader "Custom/IceShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TexTwo("Texture2", 2D) = "white" {}
        _TexThree("Texture3", 2D) = "white" {}
        _TexScale("Texture Scale", Float) = 1
        _BlueMulti("Blue Multiplier", Float) = 2
        _Glossiness("Smoothness", 2D) = "white" {}
    }
        SubShader
    {
        Tags { "RenderType" = "transparent" }
       LOD 300

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        struct Input {
            float3 worldPos;
            float3 worldNormal;
        };

        sampler2D _MainTex;
        sampler2D _TexTwo;
        sampler2D _TexThree;
        float _TexScale;
        float _BlueMulti;

        sampler2D _Glossiness;

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float3 scaledWorldPos = IN.worldPos / _TexScale;
            float3 pWeight = abs(IN.worldNormal);
            pWeight /= pWeight.x + pWeight.y + pWeight.z;

            float3 xP = tex2D(_TexTwo, scaledWorldPos.yz) * pWeight.x;
            float3 yP = tex2D(_MainTex, scaledWorldPos.xz) * pWeight.y;
            float3 zP = tex2D(_TexThree, scaledWorldPos.xy) * pWeight.z;

            float3 xG = tex2D(_Glossiness, scaledWorldPos.yz) * pWeight.x;
            float3 yG = tex2D(_Glossiness, scaledWorldPos.xz) * pWeight.y;
            float3 zG = tex2D(_Glossiness, scaledWorldPos.xy) * pWeight.z;

            float3 offsetmultiplier = float3(0,0, 1);

            o.Albedo = ((xP + yP + zP) / 2) + offsetmultiplier * _BlueMulti;
            o.Smoothness = xG + yG + zG;

        }

        ENDCG
    }
        Fallback "Diffuse"
}

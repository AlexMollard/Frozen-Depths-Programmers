/*
    File name: Fresnel.shader
    Author: Michael Sweetman
    Summary: A Shader that applies emission and/or alpha separately to the edge of the mesh to give a sense of depth
    Creation Date: 01/09/2020
    Last Modified: 07/09/2020
*/

Shader "Custom/Fresnel"
{
    Properties
    {
        [Header(Textures)]
        // albedo texture
        _Albedo("Albedo", 2D) = "white" {}
        // normal texture
        _Normal("Normal", 2D) = "bump" {}
        // emission texture
        _Emission("Emission", 2D) = "black" {}
        // metallic, smoothness, occlusion and alpha texture where each texture type is stored in a different channel (r,g,b,a)
        _MetSmoOccAlp("Metallic(r), Smoothness(g), Occulusion(b), Alpha(a)", 2D) = "white" {}

        [Header(Fresnel Settings)]
        // determines whether the fresnel value will be applied to the alpha 
        [Toggle] _ApplyToAlpha("Apply Fresnel to Alpha?", Float) = 0
        // determines whether the fresnel value will be applied to the emission 
        [Toggle] _ApplyToEmission("Apply Fresnel to Emissive?", Float) = 0
        // the emission colour outputted by the Fresnel shader
        _EmissionFresnelColor("Fresnel Colour", Color) = (1,1,1,1)
        // determines the intensity of the Fresnel effect for the Alpha
        _AlphaFresnelExponent("Alpha Fresnel Exponent", Float) = 1
        // determines the intensity of the Fresnel effect for the Emission
        _EmissionFresnelExponent("Emission Fresnel Exponent", Float) = 1
    }
        SubShader
    {
        Tags
        {
            // Set RenderType to Transparent
            "RenderType" = "Transparent"
            // Set Queue to Transparent
            "Queue" = "Transparent"
        }

        // Set the Level of Detail to 200
        LOD 200

        CGPROGRAM

        // Define this shader as a surface shader
        // Call the surf function
        // Use a Physically based Standard lighting model, enable shadows on all light types and support transparency
        #pragma surface surf Standard fullforwardshadows alpha

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        // define the texture properties as Sampler2D variables
        sampler2D _Albedo;
        sampler2D _Normal;
        sampler2D _Emission;
        sampler2D _MetSmoOccAlp;

        // define the fresnel alpha toggle as a boolean
        bool _ApplyToAlpha;
        // define the fresnel emission toggle as a boolean
        bool _ApplyToEmission;
        // define the Fresnel Colour as a float3
        float3 _EmissionFresnelColor;
        // define the Alpha Fresnel Exponent as a float
        float _AlphaFresnelExponent;
        // define the Emission Fresnel Exponent as a float
        float _EmissionFresnelExponent;

        struct Input
        {
            float2 uv_Albedo;
            float2 uv_Normal;
            float2 uv_Emission;
            float2 uv_MetSmoOccAlp;
            float3 worldNormal;
            float3 viewDir;
            INTERNAL_DATA
        };

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // get the dot prouct of the world normal and the view direction
            float fresnel = dot(o.Normal, IN.viewDir);
            
            // set the alpha Fresnel value to be the clamped Fresnel value
            float alphaFresnel = saturate(fresnel);
            // apply the alpha Fresnel exponent as power to the alpha Fresnel
            alphaFresnel = pow(fresnel, _AlphaFresnelExponent);

            // set the emission Fresnel value to be the inversed and clamped Fresnel value
            float emissionFresnel = saturate(1 - fresnel);
            // apply the emission Fresnel Exponent as power to emission Fresnel
            emissionFresnel = pow(emissionFresnel, _EmissionFresnelExponent);
            // apply the emission Fresnel value to the emission Fresnel colour
            float3 emissionFresnelColor = emissionFresnel * _EmissionFresnelColor;

            // get the texture information and apply it to the corresponding section of the output
            o.Albedo = tex2D(_Albedo, IN.uv_Albedo);
            o.Normal = tex2D(_Normal, IN.uv_Normal).rgb * 2 - 1;
            o.Emission = tex2D(_Emission, IN.uv_Emission);

            // get the metallic, smoothness, occlusion and alpha information using the original uv
            half4 metSmoOccAlp = tex2D(_MetSmoOccAlp, IN.uv_MetSmoOccAlp);
            // set the metallic using the red channel of the texture
            o.Metallic = metSmoOccAlp.r;
            // set the smoothness using the green channel of the texture
            o.Smoothness = metSmoOccAlp.g;
            // set the occlusion using the blue channel of the texture
            o.Occlusion = metSmoOccAlp.b;
            // set the alpha using the alpha channel of the texture
            o.Alpha = metSmoOccAlp.a;

            // if the fresnel is to be applied to the alpha
            if (_ApplyToAlpha)
            {
                // subtract the alpha fresnel value from the alpha
                o.Alpha -= alphaFresnel;
            }

            // if the fresnel is to be applied to the alpha
            if (_ApplyToEmission)
            {
                // add the emission fresnel colour to the emission
                o.Emission += fixed4(emissionFresnelColor, 1);
            }

        }
        ENDCG
    }
    FallBack "Diffuse"
}

sampler inputTexture;

float time;
float2 resolution;

float shaderStrength;
float2 distortionCenter;
float distortionDistance;

float2 NormalizeCoordinates(float2 UVCoord)
{
    return UVCoord / resolution;
}

float4 MainPS(float2 UV : TEXCOORD0) : COLOR0
{
    //amplitude * sin((2pi/period) + phaseShift) + vertialShift
    float2 normalizedDistortionCenter = NormalizeCoordinates(distortionCenter);
    float2 displacement = 0.0f;
    if (distance(UV, normalizedDistortionCenter) <= distortionDistance)
    {
        displacement = normalize(UV - normalizedDistortionCenter) * shaderStrength;
        //UV.x += sin(UV.y) / 100.0f;
        //UV.y = UV.y * -1.0f;
        //UV.x += 0.25f * tan(((2 * 3.14f) / UV.y) + (time * 2));
    }
    return tex2D(inputTexture, UV - displacement);
}

technique Techninque1
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 MainPS();
        AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
    }
};
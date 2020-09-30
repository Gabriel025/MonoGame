float4x4 World;
float4x4 View;
float4x4 Projection;

float AmbientIntensity = 0.4f;
float4 AmbientColor = float4(1, 1, 1, 1);

float3 DiffuseDirection = float3(1, 0, 0);
float4 DiffuseColor = float4(1, 1, 1, 1);
float DiffuseIntensity = 1;

texture Texture1;
texture Texture2;
texture Texture3;
texture Texture4;
sampler s1 = sampler_state{texture = <Texture1>; MipFilter = Point; MinFilter = Linear; MagFilter = Linear;
                           AddressU = wrap; AddressV = wrap;};
sampler s2 = sampler_state{texture = <Texture2>; MipFilter = Point; MinFilter = Linear; MagFilter = Linear;
                           AddressU = wrap; AddressV = wrap;};
sampler s3 = sampler_state{texture = <Texture3>; MipFilter = Point; MinFilter = Linear; MagFilter = Linear;
                           AddressU = wrap; AddressV = wrap;};
sampler s4 = sampler_state{texture = <Texture4>; MipFilter = Point; MinFilter = Linear; MagFilter = Linear;
                           AddressU = wrap; AddressV = wrap;};

struct VSOut
{
    float4 Position : POSITION;
	float3 Normal : TEXCOORD0;
	float2 TextureCoordinate : TEXCOORD1;
	float4 TextureWeight : TEXCOORD2;
	float Depth : TEXCOORD3;
};

VSOut VSMultitextured(float4 Position : POSITION0, float3 Normal : NORMAL, float2 TextureCoordinate : TEXCOORD0,
float4 TextureWeight : TEXCOORD1)
{
    VSOut output;
    output.Position = mul(Position, mul(World, mul(View, Projection)));
	output.Normal = Normal;
	output.TextureCoordinate = TextureCoordinate;
	output.TextureWeight = TextureWeight;
	output.Depth = saturate(output.Position.z / output.Position.w);
    return output;
}

float4 PSMultitextured(VSOut input) : COLOR0
{
	float4 near, far, light;
	light = saturate(DiffuseIntensity * DiffuseColor * saturate(dot(input.Normal, -DiffuseDirection))
					 + AmbientIntensity * AmbientColor);

	near = tex2D(s1, input.TextureCoordinate) * input.TextureWeight.x;
	near += tex2D(s2, input.TextureCoordinate) * input.TextureWeight.y;
	near += tex2D(s3, input.TextureCoordinate) * input.TextureWeight.z;
	near += tex2D(s4, input.TextureCoordinate) * input.TextureWeight.w;
	near.rgb *= light;

	far =  float4(240.0f/255, 240.0f/255, 180.0f/255, 1) * input.TextureWeight.x;
	far += float4( 85.0f/255,  85.0f/255,  45.0f/255, 1) * input.TextureWeight.y;
	far += float4(130.0f/255, 130.0f/255, 130.0f/255, 1) * input.TextureWeight.z;
	far += float4(240.0f/255, 240.0f/255, 240.0f/255, 1) * input.TextureWeight.w;
	far.rgb *= light;
    return lerp(near, far, clamp((input.Depth - 0.991f) / 0.007f, 0, 0.8f));
}

technique Multitextured
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 VSMultitextured();
        PixelShader = compile ps_4_0 PSMultitextured();
    }
}
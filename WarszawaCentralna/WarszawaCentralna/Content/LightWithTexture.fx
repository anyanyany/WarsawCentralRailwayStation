#define POINT_LIGHT_NUMBER 3
#define SPOT_LIGHT_NUMBER 2

float3 CameraPosition;

float4x4 World;
float4x4 View;
float4x4 Projection;

float3 Ka;
float3 Kd[POINT_LIGHT_NUMBER + SPOT_LIGHT_NUMBER];
float3 Ks[POINT_LIGHT_NUMBER + SPOT_LIGHT_NUMBER];

float3 LightPosition[POINT_LIGHT_NUMBER + SPOT_LIGHT_NUMBER];
float Ia;
float Id[POINT_LIGHT_NUMBER + SPOT_LIGHT_NUMBER];
float Is[POINT_LIGHT_NUMBER + SPOT_LIGHT_NUMBER];
float Shininess;

float Attenuation[POINT_LIGHT_NUMBER + SPOT_LIGHT_NUMBER];
float Falloff[POINT_LIGHT_NUMBER + SPOT_LIGHT_NUMBER];

float3 LightDirection[SPOT_LIGHT_NUMBER];
float InnerConeAngle[SPOT_LIGHT_NUMBER];
float OuterConeAngle[SPOT_LIGHT_NUMBER];


Texture2D  BasicTexture;
Texture2D  AdditionalTexture;

bool TextureEnabled = true;
bool SecondTextureEnabled = false;

bool filterMagLinear;

sampler TextureSamplerMagLinear = sampler_state {
	MagFilter = Linear;
	//MipLODBias = 10;
	//AddressU = Wrap;
	//AddressV = Wrap;
};

sampler TextureSamplerMagNone = sampler_state {
	MagFilter = None;
	//MipLODBias = 10;
	//AddressU = Wrap;
	//AddressV = Wrap;
};

float     FogEnabled;
float     FogStart = 50;
float     FogEnd = 150;
float3    FogColor = float3(0.5, 0.5, 0.5);

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float2 UV : TEXCOORD0;
};
struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float3 Normal : TEXCOORD0;
	float4 WorldPosition : TEXCOORD1;
	float2 UV : TEXCOORD2;
};

float ComputeFogFactor(float d)
{
	//d is the distance to the geometry sampling from the camera
	//this simply returns a value that interpolates from 0 to 1 
	//with 0 starting at FogStart and 1 at FogEnd 
	return clamp((d - FogStart) / (FogEnd - FogStart), 0, 1) * FogEnabled;
}

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.WorldPosition = worldPosition;
	output.Normal = mul(input.Normal, (float3x3)World);
	output.UV = input.UV;
	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 phonglLight = 0;
	Ia = 0;
	for (int i = 0; i < POINT_LIGHT_NUMBER + SPOT_LIGHT_NUMBER; i++)
	{
		Ia += Is[i] + Id[i];
	}
	Ia = clamp(Ia, 0, 1);
	float3 Ambient = Ka * Ia;
	if (TextureEnabled)
	{
		if (filterMagLinear)
		{
			Ambient *= BasicTexture.Sample(TextureSamplerMagLinear, input.UV).rgb;
			if (SecondTextureEnabled)
			{
				float4 secondTextureColor = AdditionalTexture.Sample(TextureSamplerMagLinear, input.UV);
				Ambient += (secondTextureColor.rgb*secondTextureColor.a);
			}			
		}
		else
		{
			Ambient *= BasicTexture.Sample(TextureSamplerMagNone, input.UV).rgb;
			if (SecondTextureEnabled)
			{
				float4 secondTextureColor = AdditionalTexture.Sample(TextureSamplerMagNone, input.UV);
				Ambient += (secondTextureColor.rgb*secondTextureColor.a);
			}
		}
			
	}
	phonglLight += Ambient;
	for (int i = 0; i < POINT_LIGHT_NUMBER + SPOT_LIGHT_NUMBER; i++)
	{
		float3 L = normalize(LightPosition[i] - input.WorldPosition.xyz);
		float3 N = normalize(input.Normal);
		float3 V = normalize(CameraPosition - input.WorldPosition.xyz);
		float3 R = -reflect(L, N);

		float diffuseFactor = saturate(dot(N, L));
		float3 Diffuse = diffuseFactor * Kd[i] * Id[i];

		float specularFactor = pow(saturate(dot(R, V)), Shininess);
		float3 Specular = specularFactor * Ks[i] * Is[i];


		//https://www.packtpub.com/books/content/advanced-lighting-3d-graphics-xna-game-studio-40
		//point light
		float dist = distance(LightPosition[i], input.WorldPosition.xyz);
		float att = 1 - pow(clamp(dist / Attenuation[i], 0, 1), Falloff[i]);

		//spot light
		float spotFactor = 1;
		if (i >= POINT_LIGHT_NUMBER)
		{
			float angle = acos(dot(L, normalize(-LightDirection[i - POINT_LIGHT_NUMBER])));
			spotFactor = smoothstep(OuterConeAngle[i - POINT_LIGHT_NUMBER], InnerConeAngle[i - POINT_LIGHT_NUMBER], angle);
		}
		phonglLight += (Diffuse + Specular)*att*spotFactor;
	}
	float FogFactor = ComputeFogFactor(length(CameraPosition - input.WorldPosition.xyz));

	float3 light = saturate(phonglLight);

	light = lerp(light, FogColor, FogFactor);

	return float4(light, 1);
}

technique Light
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile ps_4_0 PixelShaderFunction();
	}
}
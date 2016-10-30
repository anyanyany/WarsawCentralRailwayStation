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
float Ia = 0.9;
float Id[POINT_LIGHT_NUMBER + SPOT_LIGHT_NUMBER];
float Is[POINT_LIGHT_NUMBER + SPOT_LIGHT_NUMBER];
float Shininess;

float Attenuation[POINT_LIGHT_NUMBER + SPOT_LIGHT_NUMBER];
float Falloff[POINT_LIGHT_NUMBER + SPOT_LIGHT_NUMBER];

float3 LightDirection[SPOT_LIGHT_NUMBER];
float InnerConeAngle[SPOT_LIGHT_NUMBER];
float OuterConeAngle[SPOT_LIGHT_NUMBER];




struct VertexShaderInput
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
};
struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float3 Normal : TEXCOORD0;
	float4 WorldPosition : TEXCOORD1;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.WorldPosition = worldPosition;
	output.Normal = mul(input.Normal, (float3x3)World);
	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 phonglLight = 0;
	float3 Ambient = Ka * Ia;
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

	float3 light = saturate(phonglLight);
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
texture2D ColorMap;

struct VS_IN
{
	float2 pos : POSITION;
	float2 uv : TEXCOORD0;
};

struct PS_IN
{
	float4 pos : POSITION;
	float2 uv : TEXCOORD0;
}; 

sampler2D ColorMapSampler = sampler_state
{
	Texture = <ColorMap>;
	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
	AddressW = Clamp;
};

float4x4 worldViewProj;
float4 pointColor;
float depth;
float bufferWidth;
float bufferHeight;

PS_IN VS(VS_IN input)
{
	PS_IN output = (PS_IN)0;
	output.pos = float4(input.pos.x, input.pos.y, depth, 1);
	output.pos = mul(output.pos, worldViewProj);
	output.uv = input.uv;
	return output;
}

float4 PS(PS_IN input) : COLOR0
{
	return tex2D(ColorMapSampler, input.uv);
}


technique Main {
	pass P0 {
		VertexShader = compile vs_3_0 VS();
		PixelShader = compile ps_3_0 PS();
	}
}
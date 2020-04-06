struct VS_IN
{
	float4 pos : POSITION;
};

struct PS_IN
{
	float4 pos : POSITION;
	float4 col : COLOR0;
};

float4x4 worldViewProj;
float4 pointColor;

PS_IN VS(VS_IN input)
{
	PS_IN output = (PS_IN)0;

	output.pos = mul(input.pos, worldViewProj);
	output.col = pointColor;

	return output;
}

float4 PS(PS_IN input) : COLOR
{
	return input.col;
}


technique Main {
	pass P0 {
		VertexShader = compile vs_2_0 VS();
		PixelShader = compile ps_2_0 PS();
	}
}
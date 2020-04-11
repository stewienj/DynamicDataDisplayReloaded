struct VS_IN
{
	float2 pos : POSITION0;
	float2 pos2 : POSITION1;
};

struct PS_IN
{
	float4 pos : POSITION;
	float4 col : COLOR0;
};

float4x4 worldViewProj;
float4 pointColor;
float depth;


PS_IN VS(VS_IN input)
{
	PS_IN output = (PS_IN)0;

	output.pos.x = input.pos.x + input.pos2.x;
	output.pos.y = input.pos.y + input.pos2.y;
	output.pos.z = depth;
	output.pos.w = 1;

	output.pos = mul(output.pos, worldViewProj);
	output.col = pointColor;
	return output;
}

float4 PS(PS_IN input) : COLOR
{
	return input.col;
}


technique Main {
	pass P0 {
		VertexShader = compile vs_3_0 VS();
		PixelShader = compile ps_3_0 PS();
	}
}
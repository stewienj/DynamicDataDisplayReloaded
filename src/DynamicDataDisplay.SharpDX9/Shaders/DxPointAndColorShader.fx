struct VS_IN
{
	float2 pos : POSITION;
	float4 col : COLOR0;
};

struct PS_IN
{
	float4 pos : POSITION;
	float4 col : COLOR0;
};

float4x4 worldViewProj;
float4 pointColor; // ignored for this shader
float depth;

PS_IN VS(VS_IN input)
{
	PS_IN output = (PS_IN)0;
    // Expand x,y to x,y,z,w. When w is 1 then translations are applied, if w was zero then only scale is applied.
	output.pos = float4(input.pos.x, input.pos.y, depth, 1);
    // Multiply by the world matrix
	output.pos = mul(output.pos, worldViewProj);
	// copy the color
	output.col = input.col;
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
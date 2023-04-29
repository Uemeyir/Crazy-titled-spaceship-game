#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float multi = 2.1f;
	float4 col = tex2D(SpriteTextureSampler,input.TextureCoordinates) * input.Color;
    col.r *= multi;
    col.g *= multi;
    col.b *= multi;
	if (col.r >= 1.0f) {
		col.r = 1.0f;
	} 
	if (col.g >= 1.0f) {
		col.g = 1.0f;
	} 
	if (col.b >= 1.0f) {
		col.b = 1.0f;
	} 
	return col;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
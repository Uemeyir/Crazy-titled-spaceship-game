#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
int screenWidth;
int screenHeight;
int objectRange[99];

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
    float4 col = tex2D(SpriteTextureSampler, input.TextureCoordinates) * input.Color;
    float x = input.TextureCoordinates.x * screenWidth;
    float y = input.TextureCoordinates.y * screenHeight;
	for (int i = 0; i < 33; i++)
    {
        if ((x - objectRange[i * 3]) * (x - objectRange[i * 3]) + (y - objectRange[i * 3 + 1]) * (y - objectRange[i * 3 + 1]) <= objectRange[i * 3 + 2])
        {
            col.rgba = 0;
            return col;
        }
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
using UnityEngine;
using System.Collections;



public class TerrainGenerator
{
	
	////////////// Private Member Propery ///////////////////////////////////


	private TerrainData MyTerrainData;
	private int HeightMapWidth;
	private int HeightMapHeight;
	private int Width;
	private int Length;
	private int Height;
	private float TextureCount;
	private float [,] HeightData;
	private byte [,] SymbolData;
	private float RatioMapWidth;
	private float RatioMapHeight;
	private GameObject MyTerrain;
	private SymbolTag MySymbolTag;






	////////////// Public Member Propery ///////////////////////////////////
	
	

	
	
	
	////////////// Private Member Method ///////////////////////////////////







	////////////// Public Member Method ///////////////////////////////////







	public TerrainGenerator()
	{
		RatioMapWidth = 0.0f;

		RatioMapHeight = 0.0f;

		MySymbolTag = new SymbolTag();
	}

	public SymbolTag GetSymbolTag()
	{
		return MySymbolTag;
	}

	public void SetTerrainInfo(int MapWidth , int MapHeight , int MaxAltitude)
	{
		Width  = MapWidth;
		
		Length = MapHeight;
		
		Height = MaxAltitude;

		MyTerrainData = new TerrainData();

		MyTerrainData.heightmapResolution = 256;
		
		MyTerrainData.size = new Vector3(Width,Height,Length);

		//MyTerrain = Terrain.CreateTerrainGameObject(MyTerrainData);

		Terrain.CreateTerrainGameObject(MyTerrainData);
			
		HeightMapWidth = MyTerrainData.heightmapWidth;
		
		HeightMapHeight = MyTerrainData.heightmapHeight;

		HeightData = MyTerrainData.GetHeights(0,0,HeightMapWidth,HeightMapHeight);

		MySymbolTag.SetDataSize(HeightMapWidth , HeightMapHeight);

		Debug.Log("HeightMapWidth = "+HeightMapWidth+" HeightMapHeight = "+HeightMapHeight);

		Debug.Log("Terrain Width : "+Width);
		Debug.Log("Terrain Length : "+Length);
		Debug.Log("Terrain Height : "+Height);
		Debug.Log("HeightMap Resolution : "+MyTerrainData.heightmapResolution);
		Debug.Log("Detail Resolution : "+MyTerrainData.detailResolution);
	}

	public void GenrateSampleTerrain()
	{
		float xSin;
		float zSin;

		for (int y=0 ; y<HeightMapHeight;y++)
		{
			for (int x=0; x<HeightMapWidth;x++)
			{
				HeightData[x,y] = 0;

				xSin  = Mathf.Cos(x);
				
				zSin  = -Mathf.Sin(y);

				HeightData[y,x] = (xSin - zSin) /100.0f;
			}
		}
	
		CreateTerrain(HeightData , HeightMapWidth , HeightMapHeight);
	}

	public void CreateTerrain(float [,] ElevationData , float DataWidth , float DataHeight)
	{
		float IndexX;
		float IndexY;

		RatioMapWidth = (DataWidth / (HeightMapWidth));

		RatioMapHeight = (DataHeight / (HeightMapHeight));

		for (int y=0 ; y<(HeightMapHeight-1);y++)
		{
			IndexY = (y * RatioMapHeight);
			
			for (int x=0; x<(HeightMapWidth-1);x++)
			{
				IndexX = (x * RatioMapWidth);

				if (x > 1 && y > 1)
				{
					HeightData[y,x] = ElevationData[(int)IndexY, (int)IndexX];

					MySymbolTag.SetData(y,x,HeightData[y,x]);
				}
			}
		}

		MyTerrainData.SetHeights(0,0,HeightData);
	}

	/*
	public void CreateTerrain2(float [,] ElevationData , float DataWidth , float DataHeight)
	{
		float IndexX;
		float IndexY;
		float tempU = 0;
		float tempD = 0;
		float tempL = 0;
		float tempR = 0;
		float tempUL = 0;
		float tempUR = 0;
		float tempDL = 0;
		float tempDR = 0;
		float avg;
		float Height;
		
		RatioMapWidth = (DataWidth / (HeightMapWidth));
		
		RatioMapHeight = (DataHeight / (HeightMapHeight));
		
		for (int y=0 ; y<(HeightMapHeight-1);y++)
		{
			IndexY = (y * RatioMapHeight);
			
			for (int x=0; x<(HeightMapWidth-1);x++)
			{
				IndexX = (x * RatioMapWidth);

				if(Mathf.Floor(x*RatioMapWidth)>0 && Mathf.Floor(x*RatioMapWidth)<DataWidth-1 && Mathf.Floor(y*RatioMapHeight)>0 && Mathf.Floor(y*RatioMapHeight)<DataHeight-1)
				{
					tempL = (ElevationData[(int)Mathf.Floor(y*RatioMapHeight),(int)Mathf.Floor(x*RatioMapWidth)-1] - ElevationData[(int)Mathf.Floor(y*RatioMapHeight),(int)Mathf.Floor(x*RatioMapWidth)])*RatioMapWidth;
					tempR = (ElevationData[(int)Mathf.Floor(y*RatioMapHeight),(int)Mathf.Floor(x*RatioMapWidth)] - ElevationData[(int)Mathf.Floor(y*RatioMapHeight),(int)Mathf.Floor(x*RatioMapWidth)+1])*RatioMapWidth;
					tempU = (ElevationData[(int)Mathf.Floor(y*RatioMapHeight)-1,(int)Mathf.Floor(x*RatioMapWidth)] - ElevationData[(int)Mathf.Floor(y*RatioMapHeight),(int)Mathf.Floor(x*RatioMapWidth)])*RatioMapHeight;
					tempD = (ElevationData[(int)Mathf.Floor(y*RatioMapHeight),(int)Mathf.Floor(x*RatioMapWidth)] - ElevationData[(int)Mathf.Floor(y*RatioMapHeight)+1,(int)Mathf.Floor(x*RatioMapWidth)])*RatioMapHeight;
				}

				avg = (ElevationData[(int)(y*RatioMapHeight),(int)(x*RatioMapWidth)]) + (int)(tempL+tempR+tempU+tempD);

				HeightData[x,y] = avg;
			}
		}
		
		MyTerrainData.SetHeights(0,0,HeightData);
	}*/



	public void CreateTexture(params string []ResourceName)
	{
		Texture2D TempTexture;
		SplatPrototype[] ProtoType;

		TextureCount = ResourceName.Length;
	
		ProtoType = new SplatPrototype[ResourceName.Length];

		for (int i=0;i<ResourceName.Length;i++)
		{
			ProtoType[i] = new SplatPrototype();

			TempTexture = Resources.Load(ResourceName[i]) as Texture2D;

			ProtoType[i].texture = TempTexture;

			ProtoType[i].tileSize = new Vector2(TempTexture.width,TempTexture.height);
		}

		MyTerrainData.splatPrototypes = ProtoType;

	}

	public void CreateTexture(int LevelCount)
	{
		Color TextureColor;
		SplatPrototype []ProtoType;
		int TextureWidth;
		int TextureHeight;
		float ColorValue;
		float Ratio;
		Texture2D []Textures;

		ColorValue = 0.0f;
		
		if (LevelCount < 1)
		{
			LevelCount = 1;
		}

		TextureCount = LevelCount;
		
		TextureWidth = 64;
		TextureHeight = 64;

		Textures = new Texture2D[LevelCount];
		
		ProtoType = new SplatPrototype[LevelCount];

		Ratio = (TextureWidth * LevelCount);

		Ratio = (Ratio / 256.0f);

		for (int i=0; i<LevelCount; i++) 
		{
			Textures[i] = new Texture2D (TextureWidth, TextureHeight);
			
			ProtoType[i] = new SplatPrototype();
			
			TextureColor = new Color(Random.Range(0.1f,1.0f),Random.Range(0.1f,1.0f),Random.Range(0.1f,1.0f));
			
			for (int x=0;x<TextureWidth;x++)
			{
				ColorValue += Ratio;

				//TextureColor = new Color(0,(ColorValue/(TextureWidth * LevelCount)),0);

				for (int y= 0;y<TextureHeight;y++)
				{
					//TextureColor = new Color(0,1,0,(i+1)/10.0f);
					
					//Textures[i].SetPixel(x,y,new Color(0,1,0,(i+1)/10.0f));

					Textures[i].SetPixel(x,y,TextureColor);
				}
			}
			
			Textures[i].Apply();
			
			ProtoType[i].texture = Textures[i];
			ProtoType[i].tileSize = new Vector2(TextureWidth,TextureHeight);
			ProtoType[i].tileOffset = Vector2.zero;
		}
		
		MyTerrainData.splatPrototypes = ProtoType;
	}

	public void AssignTexture()
	{
		int Index;
		int OffSetX;
		int OffSetY;
		float[, ,] SplatmapData;
		float RatioX;
		float RatioY;

		SplatmapData = new float[MyTerrainData.alphamapWidth, MyTerrainData.alphamapHeight, MyTerrainData.alphamapLayers];

		RatioX = (HeightMapWidth / (MyTerrainData.alphamapWidth * 1.0f));
		RatioY = (HeightMapHeight / (MyTerrainData.alphamapHeight * 1.0f));

		Debug.Log("-------------    Start Mapping Texture  -----------------");
		Debug.Log("Texture Info : Width = "+MyTerrainData.alphamapWidth+ " Height = "+MyTerrainData.alphamapHeight+" Layers = "+MyTerrainData.alphamapLayers);
		Debug.Log("RatioX = "+RatioX+" RatioY = "+RatioY);
		Debug.Log("TextureCount = "+TextureCount+" alphamapLayers = "+MyTerrainData.alphamapLayers);

		for (int y = 0; y < MyTerrainData.alphamapHeight; y++)
		{
			for (int x = 0; x < MyTerrainData.alphamapWidth; x++)
			{
				OffSetX = (int) (x * RatioX);
				OffSetY = (int) (y * RatioY);

				Index = (int)(HeightData[OffSetY,OffSetX] * (TextureCount));

				if (y >200 && y < 210 && x > 200 & x < 210)
				{
					Debug.Log("Index = "+Index+" HeightData[OffSetY,OffSetX] = "+HeightData[OffSetY,OffSetX]);
				}

				if (Index > 9)
				{
					Debug.Log("if (Index > 9) = "+Index+" HeightData[OffSetY,OffSetX] = "+HeightData[OffSetY,OffSetX]);

					Index = 9;
				}

				for(int i = 0; i<MyTerrainData.alphamapLayers; i++)
				{
					//SplatmapData[y, x, i] = 0.2f;

					SplatmapData[y, x, 0] = 0.2f;

					SplatmapData[y, x, i] =  Mathf.Clamp(1.0f - HeightData[OffSetY,OffSetX]  , 0.0f,0.3f);

					SplatmapData[y, x, Index] = 0.55f;

					//SplatmapData[y, x, Index] = 1.0f;
				}
			}
		}

		// Finally assign the new splatmap to the terrainData:
		MyTerrainData.SetAlphamaps(0, 0, SplatmapData);
		
		Terrain.activeTerrain.terrainData = MyTerrainData;
		
		Terrain.activeTerrain.Flush();
		
		MyTerrainData.RefreshPrototypes();
	}
}

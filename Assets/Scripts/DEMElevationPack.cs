using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class DEMElevationPack
{




	////////////// Private Member Propery ///////////////////////////////////

	private double MaxElevation;
	private double MinElevation;
	private double Interval;
	private int MinX;
	private int MinY;
	private int MaxX;
	private int MaxY;
	private int MaxElevationXPos;
	private int MaxElevationYPos;
	private int MinElevationXPos;
	private int MinElevationYPos;
	private float [,] HeightData;
	private double MaxData;
	private double MinData;




	////////////// Public Member Propery ///////////////////////////////////


	public int DataWidth { get; set; }
	public int DataHeight { get; set; }












	////////////// Private Member Method ///////////////////////////////////

















	////////////// Public Member Method ///////////////////////////////////



	public DEMElevationPack()
	{

		MaxElevation = double.MinValue;
		MinElevation = double.MaxValue;

		MaxData = double.MinValue;
		MinData = double.MaxValue;

	}

	public bool ReadFile(string FileName)
	{
		bool Success;
		double Data;
		BinaryReader Reader;

		Data = 0;

		Success = true;

		try
		{
		    Reader = new BinaryReader(File.Open(FileName , FileMode.Open));

			Interval = Reader.ReadSingle();

			MinX = Reader.ReadInt32();
			MinY = Reader.ReadInt32();
			MaxX = Reader.ReadInt32();
			MaxY = Reader.ReadInt32();

			DataWidth  = (int) ((MaxX - MinX)/Interval)+1;
			DataHeight = (int) ((MaxY - MinY)/Interval)+1;

			Debug.Log("MagTexture = "+Interval);
			Debug.Log("MinX = "+MinX);
			Debug.Log("MinY = "+MinY);
			Debug.Log("MaxX = "+MaxX);
			Debug.Log("MaxY = "+MaxY);
			Debug.Log("DataWidth = "+DataWidth);
			Debug.Log("DataHeight = "+DataHeight);

			HeightData = new float[DataHeight , DataWidth];

			for (int y=0;y<DataHeight;y++)
			{
				for (int x=0;x<DataWidth;x++)
				{
					Data = Reader.ReadSingle();

					HeightData[y, x] = (float)Data;

					if (Data > MaxElevation)
					{
						MaxElevation = Data;

						MaxElevationXPos = x;
						MaxElevationYPos = y;
					}

					if (Data < MinElevation)
					{
						MinElevation = Data;

						MinElevationXPos = x;
						MinElevationYPos = y;
					}
				}
			}

			MinElevation -= (MinElevation * 0.01);
		}
		catch (Exception ex)
		{
			Success = false;
			
			Debug.Log(ex.Message);
		}

		Debug.Log("Max Elevation : "+MaxElevation+", "+MaxElevationXPos+","+MaxElevationYPos);
		Debug.Log("Min Elevation : "+MinElevation+", "+MinElevationXPos+","+MinElevationYPos);


		return Success;
	}
	
	public float[,] GetElevationBy2DFloat()
	{
		float[,] Data;
		double Diff; 

		Data = new float[DataHeight , DataWidth];

		Diff = (MaxElevation - MinElevation);
		
		try
		{
			for (int y = 0; y < DataHeight; y++)
			{
				for (int x = 0; x < DataWidth; x++)
				{
					Data[y, x] = (float)((HeightData[y,x] - MinElevation) / Diff); // normalization 0.0 ~ 1.0

					if (Data[y, x] > MaxData)
					{
						MaxData = Data[y, x];
					}
					
					if (Data[y, x] < MinData)
					{
						MinData = Data[y, x];
					}
				}
			}
		}
		catch(Exception ex)
		{
			Debug.Log(ex.Message);
		}

		Debug.Log("GetElevationBy2DFloat , MaxData = "+MaxData+" MinData = "+MinData);
		
		return Data;
	}

	public float[,] GetElevationBy2DFloat2()
	{
		float[,] Data;
		
		Data = new float[DataHeight , DataWidth];
		
		try
		{
			for (int y = 0; y < DataHeight; y++)
			{
				for (int x = 0; x < DataWidth; x++)
				{
					Data[y, x] = HeightData[y,x];
				}
			}
		}
		catch(Exception ex)
		{
			Debug.Log(ex.Message);
		}
		
		return Data;
	}

}        




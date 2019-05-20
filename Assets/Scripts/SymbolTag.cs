
////////////// Private Member Propery ///////////////////////////////////



////////////// Public Member Propery ///////////////////////////////////



////////////// Private Member Method ///////////////////////////////////



////////////// Public Member Method ///////////////////////////////////
/// 
/// 




using System;
using System.Collections;

public class SymbolTag
{

	////////////// Private Member Propery ///////////////////////////////////

	private byte LowPass;
	private byte HighPass;
	private byte[,] SymbolTagData;

		

	////////////// Public Member Propery ///////////////////////////////////


		
	////////////// Private Member Method ///////////////////////////////////


	
	////////////// Public Member Method ///////////////////////////////////

	public SymbolTag()
	{
		LowPass = 15; //00001111

		HighPass = 240; //11110000
	}

	public void SetDataSize(int Width , int Height)
	{
		SymbolTagData = new byte[Width,Height];
	}

	public void SetData(int DataPosY , int DataPosX , float DataHeight)
	{
		byte ObjectType;
		byte Size;

		Size = 1;

		ObjectType = (byte) Math.Round(DataHeight * 10.0);

		SymbolTagData[DataPosY , DataPosX] = (byte)((ObjectType << 4) | Size);
	}

	public int GetType(int DataPosY , int DataPosX)
	{
		int Data;

		Data = 0;

		Data = (SymbolTagData[DataPosY , DataPosX] & HighPass) >> 4;

		return Data;
	}

	public int GetSize(int DataPosY , int DataPosX)
	{
		int Data;
		
		Data = 0;
		
		Data = SymbolTagData[DataPosY , DataPosX] & LowPass;
		
		return Data;
	}
}


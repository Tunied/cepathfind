using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEPathFindBasic
{
	public enum TILE_SERACH_TYPE
	{
		FOUR_DIRECTION,
		EIGHT_DIRECTION,
		EIGHT_DIRECTION_FIX_CORNER
	}


	/// <summary>
	/// 取得某个节点的相关属性
	/// </summary>
	/// <param name="_tileX">Tile x.</param> 当前Tile的X
	/// <param name="_tileY">Tile y.</param> 当前Tile的Y
	/// <param name="_star">Star.</param> 搜索的起点
	/// <param name="_end">End.</param> 搜索的终点
	/// <param name="_isWalkable">Is walkable.</param> 当前Tile是否可以行走
	/// <param name="_score">Score. </param>  Tile的整体评分 (Tile本身分数+Tile距终点的评分(霍夫曼评分))
	public virtual void GetTileProperty (int _tileX, int _tileY, 
	                                     CEPathFindNode _star, CEPathFindNode _end, 
	                                     out bool _isWalkable, out int _score)
	{
		_isWalkable = true;
		_score = 1;
	}

	/// <summary>
	/// 用于FixCorner时候的判断
	/// </summary>
	public virtual bool isTileWalkable (int _tileX, int _tileY)
	{
		return true;
	}


	public virtual TILE_SERACH_TYPE GetTileSerachType ()
	{
		return TILE_SERACH_TYPE.EIGHT_DIRECTION_FIX_CORNER;
	}

}

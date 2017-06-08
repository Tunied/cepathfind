using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class CEPathFindNode
{
	public CEPathFindNode parent;

	public int x;
	public int y;
	//当前Tile的评分 = Tile本身分数+Tile距终点的评分(霍夫曼评分)
	public int score;
	public bool isWalkable;

	public void Reset ()
	{
		x = y = -1;
		parent = null;
		score = 0;
		isWalkable = false;
	}



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public sealed class CEPathFindAgent
{
	private static Stack nodePool = new Stack ();

	/// <summary>
	/// 每次Tick调用时候,搜索Node节点的个数
	/// </summary>
	private const int EACH_TICK_SEARCH_NODE_NUM = 1;

	private CEPathFindBasic mHolder;

	private List<CEPathFindNode> mOpenList;
	private List<CEPathFindNode> mCloseList;

	private CEPathFindNode mStarNode;
	private CEPathFindNode mEndNode;

	private CEPathFindNode mCurrentNode;

	public CEPathFindAgent ()
	{
		mOpenList = new List<CEPathFindNode> ();
		mCloseList = new List<CEPathFindNode> ();
	}

	public void Reset (CEPathFindBasic _holder, int _startTileX, int _startTileY, int _endTileX, int _endTileY)
	{
		mHolder = _holder;
		mOpenList.ForEach (node => nodePool.Push (node));
		mCloseList.ForEach (node => nodePool.Push (node));
		mOpenList.Clear ();
		mCloseList.Clear ();

		mStarNode = GetNewNode ();
		mStarNode.x = _startTileX;
		mStarNode.y = _startTileY;
		mStarNode.isWalkable = mHolder.isTileWalkable (mStarNode.x, mStarNode.y);

		mEndNode = GetNewNode ();
		mEndNode.x = _endTileX;
		mEndNode.y = _endTileY;
		mEndNode.isWalkable = mHolder.isTileWalkable (mEndNode.x, mEndNode.y);

		mCurrentNode = mStarNode;
		mCurrentNode.isWalkable = mHolder.isTileWalkable (mCurrentNode.x, mCurrentNode.y);
	}

	#region A*

	public  void TickSearch (out bool _isFinish, out CEPathFindResult _reuslt)
	{
		//起始节点和结束节点本身就无法走
		if (!mStarNode.isWalkable || !mEndNode.isWalkable) {
			_isFinish = true;
			_reuslt = new CEPathFindResult ();
			_reuslt.isHavePath = false;
			return;
		}


		for (var i = 0; i < EACH_TICK_SEARCH_NODE_NUM; i++) {
			if (mCurrentNode.x == mEndNode.x && mCurrentNode.y == mEndNode.y) {
				_isFinish = true;
				_reuslt = GetPathFindResult (mCurrentNode);
				return;
			}

			mCloseList.Add (mCurrentNode);

			CheckCurrentSearchAroundTile ();

			if (mOpenList.Count == 0) {
				//没有Open节点了,全部搜索过,但未找到路径
				_isFinish = true;
				_reuslt = new CEPathFindResult ();
				_reuslt.isHavePath = false;
				return;
			}

			mOpenList.Sort (SortListByScore);
			mCurrentNode = mOpenList [0];
			mOpenList.RemoveAt (0);
		}
		_isFinish = false;
		_reuslt = null;
		return;
	}

	private void CheckCurrentSearchAroundTile ()
	{
		//上下左右
		DoCheckTile (mCurrentNode.x + 1, mCurrentNode.y);
		DoCheckTile (mCurrentNode.x - 1, mCurrentNode.y);
		DoCheckTile (mCurrentNode.x, mCurrentNode.y + 1);
		DoCheckTile (mCurrentNode.x, mCurrentNode.y - 1);

		if (mHolder.GetTileSerachType () == CEPathFindBasic.TILE_SERACH_TYPE.EIGHT_DIRECTION) {
			//右上
			DoCheckTile (mCurrentNode.x + 1, mCurrentNode.y + 1);
			//右下
			DoCheckTile (mCurrentNode.x + 1, mCurrentNode.y - 1);
			//左下
			DoCheckTile (mCurrentNode.x - 1, mCurrentNode.y - 1);
			//左上
			DoCheckTile (mCurrentNode.x - 1, mCurrentNode.y + 1);
		} else if (mHolder.GetTileSerachType () == CEPathFindBasic.TILE_SERACH_TYPE.EIGHT_DIRECTION_FIX_CORNER) {
			var upTileWalkable = mHolder.isTileWalkable (mCurrentNode.x, mCurrentNode.y + 1);
			var downTileWalkable = mHolder.isTileWalkable (mCurrentNode.x, mCurrentNode.y - 1);
			var rightTileWalkable = mHolder.isTileWalkable (mCurrentNode.x + 1, mCurrentNode.y);
			var leftTileWalkable = mHolder.isTileWalkable (mCurrentNode.x - 1, mCurrentNode.y);

			if (upTileWalkable && rightTileWalkable) {
				//右上
				DoCheckTile (mCurrentNode.x + 1, mCurrentNode.y + 1);
			}

			if (downTileWalkable && rightTileWalkable) {
				//右下
				DoCheckTile (mCurrentNode.x + 1, mCurrentNode.y - 1);
			}

			if (downTileWalkable && leftTileWalkable) {
				//左下
				DoCheckTile (mCurrentNode.x - 1, mCurrentNode.y - 1);
			}

			if (upTileWalkable && leftTileWalkable) {
				//左上
				DoCheckTile (mCurrentNode.x - 1, mCurrentNode.y + 1);
			}
		}

	}

	private void DoCheckTile (int _tileX, int _tileY)
	{
		//如果当前节点已经在Open和Close列表中则忽略
		if (IsTileInNode (_tileX, _tileY, mOpenList) || IsTileInNode (_tileX, _tileY, mCloseList)) {
			return;
		}

		var node = GetNewNode ();
		SetNodeProperty (node, _tileX, _tileY);
		node.parent = mCurrentNode;
		if (node.isWalkable) {
			mOpenList.Add (node);
		} else {
			mCloseList.Add (node);
		}
	}

	private void SetNodeProperty (CEPathFindNode _node, int _tileX, int _tileY)
	{
		_node.x = _tileX;
		_node.y = _tileY;
		mHolder.GetTileProperty (_tileX, _tileY, mStarNode, mEndNode, out _node.isWalkable, out _node.score);
	}

	private bool IsTileInNode (int _tileX, int _tileY, List<CEPathFindNode> _list)
	{
		return _list.Exists (node => node.x == _tileX && node.y == _tileY);
	}

	private int SortListByScore (CEPathFindNode _a, CEPathFindNode _b)
	{
		if (_a.score == _b.score) {
			return 0;
		} else {
			return _a.score > _b.score ? 1 : -1;
		}
	}

	#endregion


	#region 取得新的Node

	private CEPathFindNode GetNewNode ()
	{
		CEPathFindNode node = null;
		if (nodePool.Count > 0) {
			node = (CEPathFindNode)nodePool.Pop ();
			node.Reset ();
		} else {
			node = new CEPathFindNode ();
		}
		return node;
	}

	#endregion

	#region 处理Result

	public CEPathFindResult GetPathFindResult (CEPathFindNode _endNode)
	{
		var result = new CEPathFindResult ();


		int maxNum = 1;
		var node = _endNode;
		while (node.parent != null) {
			node = node.parent;
			maxNum++;
		}

		result.isHavePath = true;
		result.paths = new CEIntVector2[maxNum];

		node = _endNode;
		while (node != null) {
			result.paths [maxNum - 1] = new CEIntVector2 (node.x, node.y);
			node = node.parent;
			maxNum--;
		}

		return result;
	}

	#endregion

}

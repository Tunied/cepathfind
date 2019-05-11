using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;

namespace Eran.CopyEngine.Extension.PathFind.Sub
{
    public sealed class CEPathFindAgent
    {
        private static readonly Stack mNodePool = new Stack();

        /// <summary>
        /// 每次Tick调用时候,搜索Node节点的个数
        /// </summary>
        private const int EACH_TICK_SEARCH_NODE_NUM = 50;

        private CEPathFindMapAgent mMapAgent;

//        private readonly List<CEPathFindNode> mOpenList;
        private readonly CEPriorityQueue<CEPathFindNode> mOpenList;
        private readonly List<CEPathFindNode> mCloseList;

        private CEPathFindNode mStarNode;
        private CEPathFindNode mEndNode;

        private CEPathFindNode mCurrentNode;

        public CEPathFindAgent()
        {
            mOpenList = new CEPriorityQueue<CEPathFindNode>(new CEPathFindNode());
            mCloseList = new List<CEPathFindNode>();
        }

        public void Reset(CEPathFindMapAgent _mapAgent, int _startTileX, int _startTileY, int _endTileX, int _endTileY)
        {
            mMapAgent = _mapAgent;

            RecycleNodes();

            mStarNode = GetNewNode();
            mStarNode.x = _startTileX;
            mStarNode.y = _startTileY;
            mStarNode.isWalkable = mMapAgent.IsTileWalkable(mStarNode.x, mStarNode.y);

            mEndNode = GetNewNode();
            mEndNode.x = _endTileX;
            mEndNode.y = _endTileY;
            mEndNode.isWalkable = mMapAgent.IsTileWalkable(mEndNode.x, mEndNode.y);

            mCurrentNode = mStarNode;
            mCurrentNode.isWalkable = mMapAgent.IsTileWalkable(mCurrentNode.x, mCurrentNode.y);
        }


        public void TickSearch(out bool _isFinish, out CEPathFindResult _result)
        {
            //起始节点和结束节点本身就无法走
            if (!mStarNode.isWalkable || !mEndNode.isWalkable)
            {
                _isFinish = true;
                _result = new CEPathFindResult {isHavePath = false};
                RecycleNodes();
                return;
            }


            for (var i = 0; i < EACH_TICK_SEARCH_NODE_NUM; i++)
            {
                if (mCurrentNode.x == mEndNode.x && mCurrentNode.y == mEndNode.y)
                {
                    _isFinish = true;
                    _result = GetPathFindResult(mCurrentNode);
                    RecycleNodes();
                    return;
                }

                mCloseList.Add(mCurrentNode);

                CheckCurrentSearchAroundTile();

                if (mOpenList.Count == 0)
                {
                    //没有Open节点了,全部搜索过,但未找到路径
                    _isFinish = true;
                    _result = new CEPathFindResult {isHavePath = false};
                    RecycleNodes();
                    return;
                }

//                mOpenList.Sort(SortListByScore);
//                mCurrentNode = mOpenList[0];
//                mOpenList.RemoveAt(0);
                mCurrentNode = mOpenList.Remove();
            }

            _isFinish = false;
            _result = null;
            return;
        }

        public void DebugOutput()
        {
            Debug.Log($"Open node length :{mOpenList.Count} Close node length :{mCloseList.Count}");
        }

        private void CheckCurrentSearchAroundTile()
        {
            //上下左右
            DoCheckTile(mCurrentNode.x + 1, mCurrentNode.y);
            DoCheckTile(mCurrentNode.x - 1, mCurrentNode.y);
            DoCheckTile(mCurrentNode.x, mCurrentNode.y + 1);
            DoCheckTile(mCurrentNode.x, mCurrentNode.y - 1);

            if (mMapAgent.GetTileSearchType() == CEPathFindMapAgent.TILE_SEARCH_TYPE.EIGHT_DIRECTION)
            {
                //右上
                DoCheckTile(mCurrentNode.x + 1, mCurrentNode.y + 1);
                //右下
                DoCheckTile(mCurrentNode.x + 1, mCurrentNode.y - 1);
                //左下
                DoCheckTile(mCurrentNode.x - 1, mCurrentNode.y - 1);
                //左上
                DoCheckTile(mCurrentNode.x - 1, mCurrentNode.y + 1);
            }
            else if (mMapAgent.GetTileSearchType() == CEPathFindMapAgent.TILE_SEARCH_TYPE.EIGHT_DIRECTION_FIX_CORNER)
            {
                var upTileWalkable = mMapAgent.IsTileWalkable(mCurrentNode.x, mCurrentNode.y + 1);
                var downTileWalkable = mMapAgent.IsTileWalkable(mCurrentNode.x, mCurrentNode.y - 1);
                var rightTileWalkable = mMapAgent.IsTileWalkable(mCurrentNode.x + 1, mCurrentNode.y);
                var leftTileWalkable = mMapAgent.IsTileWalkable(mCurrentNode.x - 1, mCurrentNode.y);

                if (upTileWalkable && rightTileWalkable)
                {
                    //右上
                    DoCheckTile(mCurrentNode.x + 1, mCurrentNode.y + 1);
                }

                if (downTileWalkable && rightTileWalkable)
                {
                    //右下
                    DoCheckTile(mCurrentNode.x + 1, mCurrentNode.y - 1);
                }

                if (downTileWalkable && leftTileWalkable)
                {
                    //左下
                    DoCheckTile(mCurrentNode.x - 1, mCurrentNode.y - 1);
                }

                if (upTileWalkable && leftTileWalkable)
                {
                    //左上
                    DoCheckTile(mCurrentNode.x - 1, mCurrentNode.y + 1);
                }
            }
        }

        private void DoCheckTile(int _tileX, int _tileY)
        {
            //如果当前节点已经在Open和Close列表中则忽略
            if (IsTileInNode(_tileX, _tileY, mOpenList) || IsTileInNode(_tileX, _tileY, mCloseList))
            {
                return;
            }

            var node = GetNewNode();
            SetNodeProperty(node, _tileX, _tileY);
            node.parent = mCurrentNode;
            if (node.isWalkable)
            {
                mOpenList.Add(node);
            }
            else
            {
                mCloseList.Add(node);
            }
        }

        private void SetNodeProperty(CEPathFindNode _node, int _tileX, int _tileY)
        {
            _node.x = _tileX;
            _node.y = _tileY;
            mMapAgent.GetTileProperty(_tileX, _tileY, mStarNode, mEndNode, out _node.isWalkable, out _node.score);
        }

        private static bool IsTileInNode(int _tileX, int _tileY, ICollection<CEPathFindNode> _list)
        {
            foreach (var node in _list)
            {
                if (node.x == _tileX && node.y == _tileY)
                {
                    return true;
                }
            }

            return false;
//            return _list.Contains(node => node.x == _tileX && node.y == _tileY);
        }

        /// <summary>
        /// 节点评分,分数越小越优先遍历
        /// </summary>
        /// <param name="_a"></param>
        /// <param name="_b"></param>
        /// <returns></returns>
        private static int SortListByScore(CEPathFindNode _a, CEPathFindNode _b)
        {
            if (_a.score == _b.score)
            {
                return 0;
            }

            return _a.score > _b.score ? 1 : -1;
        }


        private static CEPathFindNode GetNewNode()
        {
            CEPathFindNode node;
            if (mNodePool.Count > 0)
            {
                node = (CEPathFindNode) mNodePool.Pop();
                node.Reset();
            }
            else
            {
                node = new CEPathFindNode();
            }

            return node;
        }


        private static CEPathFindResult GetPathFindResult(CEPathFindNode _endNode)
        {
            var result = new CEPathFindResult();
            var maxNum = 1;
            var node = _endNode;
            while (node.parent != null)
            {
                node = node.parent;
                maxNum++;
            }

            result.isHavePath = true;
            result.paths = new Vector2Int[maxNum];

            node = _endNode;
            while (node != null)
            {
                result.paths[maxNum - 1] = new Vector2Int(node.x, node.y);
                node = node.parent;
                maxNum--;
            }

            return result;
        }


        private void RecycleNodes()
        {
            foreach (var openNode in mOpenList)
            {
                mNodePool.Push(openNode);
            }

//            mOpenList.ForEach(node => mNodePool.Push(node));
            mCloseList.ForEach(node => mNodePool.Push(node));
            mOpenList.Clear();
            mCloseList.Clear();
        }
    }
}
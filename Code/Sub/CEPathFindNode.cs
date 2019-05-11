using System.Collections.Generic;

namespace Eran.CopyEngine.Extension.PathFind.Sub
{
    public sealed class CEPathFindNode : IComparer<CEPathFindNode>
    {
        public CEPathFindNode parent;

        public int x;

        public int y;

        //当前Tile的评分 = Tile本身分数+Tile距终点的评分(霍夫曼评分) 分数越小越优先选择
        public int score;
        public bool isWalkable;

        public void Reset()
        {
            x = y = -1;
            parent = null;
            score = 0;
            isWalkable = false;
        }


        public int Compare(CEPathFindNode x, CEPathFindNode y)
        {
            return x.score > y.score ? 1 : -1;
        }
    }
}
using UnityEngine;

namespace Eran.CopyEngine.Extension.PathFind.Sub
{
    public class CEPathFindResult
    {
        public bool isHavePath;
        public Vector2Int[] paths;

        public override string ToString()
        {
            var str = "IsHavePath: " + isHavePath + "\n";

            if (paths != null)
            {
                for (var i = 0; i < paths.Length; i++)
                {
                    str += "Step " + i + ":   ( " + paths[i].x + " , " + paths[i].y + " )\n";
                }
            }

            return str;
        }
    }
}
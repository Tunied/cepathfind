using System;
using System.Collections.Generic;
using Eran.CopyEngine.Extension.PathFind.Sub;
using UnityEngine;

namespace Eran.CopyEngine.Extension.PathFind
{
    public class CEPathFind : MonoBehaviour
    {
        private const int MAX_SEARCH_TIME = 100;
        private static CEPathFindAgent mShareAgent;
        private static CEPathFind mInstance;
        private static GameObject mHoldGo;

        /// <summary>
        /// 一次性返回搜索结果,用于小图搜索
        /// </summary>
        public static CEPathFindResult FindPath(int _starTileX, int _starTileY, int _endTileX, int _endTileY, CEPathFindMapAgent _findEngine)
        {
            if (mShareAgent == null)
            {
                mShareAgent = new CEPathFindAgent();
            }

            mShareAgent.Reset(_findEngine, _starTileX, _starTileY, _endTileX, _endTileY);
            CEPathFindResult result = null;
            var isFinish = false;
            var searchTime = 0;
            while (!isFinish)
            {
                mShareAgent.TickSearch(out isFinish, out result);
                searchTime++;
                if (searchTime >= MAX_SEARCH_TIME && !isFinish)
                {
                    isFinish = true;
                    result = new CEPathFindResult {isHavePath = false};
                    Debug.LogError("Reach CEPathFind max loop");
                    mShareAgent.DebugOutput();
                }
            }

            return result;
        }

        /// <summary>
        /// 异步搜索,需要等待回调
        /// </summary>
        public static void FindPathAsync(int _starTileX, int _starTileY,
            int _endTileX, int _endTileY,
            CEPathFindMapAgent _findEngine,
            Action<CEPathFindResult> _finishCallback)
        {
            if (mInstance == null)
            {
                mHoldGo = new GameObject("CEPathFind");
                DontDestroyOnLoad(mHoldGo);
                mInstance = mHoldGo.AddComponent<CEPathFind>();
            }

            var agentProxy = new CEPathFindAgentProxy {agent = new CEPathFindAgent()};
            agentProxy.agent.Reset(_findEngine, _starTileX, _starTileY, _endTileX, _endTileY);
            agentProxy.callback = _finishCallback;

            mInstance.AddAgentProxy(agentProxy);
        }


        private readonly List<CEPathFindAgentProxy> mAllAgentProxyList = new List<CEPathFindAgentProxy>();

        // Update is called once per frame
        private void Update()
        {
            if (mAllAgentProxyList.Count <= 0) return;

            mAllAgentProxyList.ForEach(proxy =>
            {
                bool isFinish;
                CEPathFindResult result;
                proxy.agent.TickSearch(out isFinish, out result);
                proxy.searchTime++;

                if (!isFinish && proxy.searchTime >= MAX_SEARCH_TIME)
                {
                    isFinish = true;
                    result = new CEPathFindResult {isHavePath = false};
                    Debug.LogError("Reach CEPathFind max loop");
                    mShareAgent.DebugOutput();
                }

                if (!isFinish) return;
                proxy.callback(result);
                proxy.isFinish = true;
            });

            mAllAgentProxyList.RemoveAll(proxy => proxy.isFinish);
        }

        private void AddAgentProxy(CEPathFindAgentProxy _proxy)
        {
            mAllAgentProxyList.Add(_proxy);
        }

        private class CEPathFindAgentProxy
        {
            public bool isFinish;
            public CEPathFindAgent agent;
            public Action<CEPathFindResult> callback;
            public int searchTime;
        }
    }
}
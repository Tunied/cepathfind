using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CEPathFind : MonoBehaviour
{
	private static  CEPathFindAgent mShareAgent;
	private static  CEPathFind mInstance;

	public static CEPathFindResult FindPath (int _starTileX, int _starTileY, int _endTileX, int _endTileY, CEPathFindBasic _findEngine)
	{
		if (mShareAgent == null) {
			mShareAgent = new CEPathFindAgent ();
		}
		mShareAgent.Reset (_findEngine, _starTileX, _starTileY, _endTileX, _endTileY);
		CEPathFindResult result = null;
		bool isFinish = false;
		while (!isFinish) {
			mShareAgent.TickSearch (out isFinish, out result);
		}
		return result;
	}

	public static void FindPath (int _starTileX, int _starTileY,
	                             int _endTileX, int _endTileY, 
	                             CEPathFindBasic _findEngine,
	                             Action<CEPathFindResult> _finishCallback)
	{
		if (mInstance == null) {
			Debug.LogError ("you should attack CEPathFind to an gameobject and call FindPath after it have been init");
		} else {
			var agentProxy = new CEPathFindAgentProxy ();
			agentProxy.agent = new CEPathFindAgent ();
			agentProxy.agent.Reset (_findEngine, _starTileX, _starTileY, _endTileX, _endTileY);
			agentProxy.callback += _finishCallback;

			mInstance.AddAgentProxy (agentProxy);
		}
	}


	private List<CEPathFindAgentProxy> mAllAgentProxyList;

	void Awake ()
	{
		mAllAgentProxyList = new List<CEPathFindAgentProxy> ();
		mInstance = this;
	}

	// Update is called once per frame
	void Update ()
	{
		if (mAllAgentProxyList.Count > 0) {
			
			mAllAgentProxyList.ForEach (proxy => {
				bool isFinish;
				CEPathFindResult result = null;
				proxy.agent.TickSearch (out isFinish, out result);
				if (isFinish) {
					proxy.callback (result);
					proxy.isFinish = true;
				}
			});

			mAllAgentProxyList.RemoveAll (proxy => proxy.isFinish == true);
		}
	}

	private void AddAgentProxy (CEPathFindAgentProxy _proxy)
	{
		mAllAgentProxyList.Add (_proxy);		
	}

	private class CEPathFindAgentProxy
	{
		public bool isFinish;
		public CEPathFindAgent agent;
		public Action<CEPathFindResult> callback;
	}

}

# CEPathFind
a path find for tilebase game in unity

![img1](md/img4.jpg)


## How to use 

there is two way to use it.

-  Immediate return

```c#
CEPathFindResult result = CEPathFind.FindPath (starTileX, starTileY, endTileX, endTileY, findEngine);
Debug.Log(result.toString());
```

in this way,the function will return the result immediately.

---

-  Async call back

```

CEPathFind.FindPath (starTileX, starTileY, endTileX, endTileY, findEngine,ShowPath);

private void ShowPath (CEPathFindResult _result)
{
	Debug.Log(result.toString());
}

```

in this way ,you need provide an callback(`Action<(CEPathFindResult >`),when the path find finish ,it will call back.

***Attaction***: if you use this way, you should attach CEPathFind.cs to an gameObject.

also you can change the each tick search node num in `CEPathFindAgent.cs`

```
private const int EACH_TICK_SEARCH_NODE_NUM = 50;
```




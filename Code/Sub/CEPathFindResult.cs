using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEPathFindResult
{
	public bool isHavePath;
	public CEIntVector2[] paths;

	public override string ToString ()
	{
		var str = "IsHavePath: " + isHavePath + "\n";

		if (paths != null) {
			for (var i = 0; i < paths.Length; i++) {
				str += "Step " + i + ":   ( " + paths [i].x + " , " + paths [i].y + " )\n";
			}
		}
		return str;
	}




}

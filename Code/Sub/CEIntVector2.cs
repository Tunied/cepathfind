
public struct CEIntVector2
{
	public int x;
	public int y;

	//因为结构体无法更改Default值,且无法和null判等
	//当使用List存储且使用Find方法查找出的结果一定是一个CEIntVector2,
	//但此时无法判断当前是否真的找到,因为此时x,y的值是0,0
	public bool isInit;

	public CEIntVector2 (int _x, int _y)
	{
		x = _x;
		y = _y;
		isInit = true;
	}

	public static bool operator == (CEIntVector2 lhs, CEIntVector2 rhs)
	{
		return lhs.x == rhs.x && lhs.y == rhs.y;
	}

	public static bool operator != (CEIntVector2 lhs, CEIntVector2 rhs)
	{
		return !(lhs == rhs);
	}

	public override bool Equals (object other)
	{
		if (!(other is CEIntVector2)) {
			return false;
		}
		return this == (CEIntVector2)other;
	}

	public bool Equals (CEIntVector2 other)
	{
		return this == other;
	}

	public override int GetHashCode ()
	{
		return (x.GetHashCode () << 6) ^ y.GetHashCode ();
	}
}

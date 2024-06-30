internal static class RectangularArrays
{
	internal static HexagonGridPathFinder.Node[][] ReturnRectangularNodeArray(int Size1, int Size2)
	{
		HexagonGridPathFinder.Node[][] array = new HexagonGridPathFinder.Node[Size1][];
		for (int i = 0; i < Size1; i++)
		{
			array[i] = new HexagonGridPathFinder.Node[Size2];
		}
		return array;
	}
}

// In-game Debug Console https://assetstore.unity.com/packages/tools/gui/in-game-debug-console-68068?aid=1101lGoY&cid=1101l7tzhnwF&utm_source=aff カスタムしてます

namespace HRYooba.UI
{
	public class DebugLogIndexList
	{
		private int[] indices;
		private int size;

		public int Count { get { return size; } }
		public int this[int index] { get { return indices[index]; } }

		public DebugLogIndexList()
		{
			indices = new int[64];
			size = 0;
		}

		public void Add( int index )
		{
			if( size == indices.Length )
			{
				int[] indicesNew = new int[size * 2];
				System.Array.Copy( indices, 0, indicesNew, 0, size );
				indices = indicesNew;
			}

			indices[size++] = index;
		}

		public void Clear()
		{
			size = 0;
		}
	}
}
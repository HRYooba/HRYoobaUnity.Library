// In-game Debug Console https://assetstore.unity.com/packages/tools/gui/in-game-debug-console-68068?aid=1101lGoY&cid=1101l7tzhnwF&utm_source=aff カスタムしてます
/*
The MIT License (MIT)

Copyright (c) 2016 Süleyman Yasir KULA

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

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
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

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Listens to scroll events on the scroll rect that debug items are stored
// and decides whether snap to bottom should be true or not
// 
// Procedure: if, after a user input (drag or scroll), scrollbar is at the bottom, then 
// snap to bottom shall be true, otherwise it shall be false
namespace HRYooba.UI
{
	public class DebugsOnScrollListener : MonoBehaviour, IScrollHandler, IBeginDragHandler, IEndDragHandler
	{
		public ScrollRect debugsScrollRect;
		public DebugLogConsoleWindow debugLogWindow;

		public void OnScroll( PointerEventData data )
		{
			if( IsScrollbarAtBottom() )
				debugLogWindow.SetSnapToBottom( true );
			else
				debugLogWindow.SetSnapToBottom( false );
		}

		public void OnBeginDrag( PointerEventData data )
		{
			debugLogWindow.SetSnapToBottom( false );
		}

		public void OnEndDrag( PointerEventData data )
		{
			if( IsScrollbarAtBottom() )
				debugLogWindow.SetSnapToBottom( true );
			else
				debugLogWindow.SetSnapToBottom( false );
		}

		public void OnScrollbarDragStart( BaseEventData data )
		{
			debugLogWindow.SetSnapToBottom( false );
		}

		public void OnScrollbarDragEnd( BaseEventData data )
		{
			if( IsScrollbarAtBottom() )
				debugLogWindow.SetSnapToBottom( true );
			else
				debugLogWindow.SetSnapToBottom( false );
		}

		private bool IsScrollbarAtBottom()
		{
			float scrollbarYPos = debugsScrollRect.verticalNormalizedPosition;
			if( scrollbarYPos <= 1E-6f )
				return true;

			return false;
		}
	}
}
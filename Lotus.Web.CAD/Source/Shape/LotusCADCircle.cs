//=====================================================================================================================
using System;
using System.Collections.Generic;
using Microsoft.JSInterop;
//=====================================================================================================================
namespace Lotus.Web
{
	namespace CAD
	{
        public class Circle : IShape
        {
            public IJSObjectReference JSShape { get; set; }

            private readonly IJSObjectReference KonvaWrapper;

            public Circle(IJSObjectReference js_circle, IJSObjectReference konva_wrapper)
            {
                JSShape = js_circle;
                KonvaWrapper = konva_wrapper;
            }
        }
    }
}
//=====================================================================================================================
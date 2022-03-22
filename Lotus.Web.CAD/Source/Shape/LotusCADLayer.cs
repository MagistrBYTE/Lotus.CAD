//=====================================================================================================================
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.JSInterop;
//=====================================================================================================================
namespace Lotus.Web
{
	namespace CAD
	{
        public class Layer
        {
            internal readonly IJSObjectReference jSLayer;
            private readonly IJSObjectReference konvaWrapper;

            internal Layer(IJSObjectReference jSLayer, IJSObjectReference konvaWrapper)
            {
                this.jSLayer = jSLayer;
                this.konvaWrapper = konvaWrapper;
            }

            public async Task AddAsync(IShape shape)
            {
                await konvaWrapper.InvokeVoidAsync("add", jSLayer, shape.JSShape);
            }

            public async Task Draw()
            {
                await konvaWrapper.InvokeVoidAsync("draw", jSLayer);
            }
        }
    }
}
//=====================================================================================================================
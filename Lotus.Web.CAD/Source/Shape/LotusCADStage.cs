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
		public class Stage
		{
			private readonly IJSObjectReference jSStage;
			private readonly IJSObjectReference konvaWrapper;

			internal Stage(IJSObjectReference jSStage, IJSObjectReference konvaWrapper)
			{
				this.jSStage = jSStage;
				this.konvaWrapper = konvaWrapper;
			}

			public async Task AddAsync(Layer Layer)
			{
				await konvaWrapper.InvokeVoidAsync("add", jSStage, Layer.jSLayer);
			}

			public async Task<Double> GetWidthAsync()
			{
				return await jSStage.InvokeAsync<Double>("width");
			}

			public async Task SetWidthAsync(Double value)
			{
				await jSStage.InvokeVoidAsync("width", value);
			}

			public async Task<Double> GetHeightAsync()
			{
				return await jSStage.InvokeAsync<Double>("height");
			}

			public async Task SetHeightAsync(Double value)
			{
				await jSStage.InvokeVoidAsync("height", value);
			}
		}
	}
}
//=====================================================================================================================
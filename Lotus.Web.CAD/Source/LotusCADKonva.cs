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
		public class Konva
		{
			private readonly IJSRuntime jSRuntime;
			
			public Konva(IJSRuntime jSRuntime)
			{
				this.jSRuntime = jSRuntime;
			}

			public async Task<Stage> CreateStageAsync(String container, Double width, Double height)
			{
				var konvaWrapper = await jSRuntime.InvokeAsync<IJSObjectReference>("import", "./cad_konva.js");
				var jSStage = await konvaWrapper.InvokeAsync<IJSObjectReference>("createStage", container, width, height);
				return new Stage(jSStage, konvaWrapper);
			}

			public async Task<Layer> CreateLayerAsync()
			{
				var konvaWrapper = await jSRuntime.InvokeAsync<IJSObjectReference>("import", "./cad_konva.js");
				var jSLayer = await konvaWrapper.InvokeAsync<IJSObjectReference>("createLayer");
				return new Layer(jSLayer, konvaWrapper);
			}

			public async Task<Circle> CreateCircleAsync(Double x, Double y, Double radius, String fill, String stroke, Double strokeWidth)
			{
				var konvaWrapper = await jSRuntime.InvokeAsync<IJSObjectReference>("import", "./cad_konva.js");
				var jSCircle = await konvaWrapper.InvokeAsync<IJSObjectReference>("createCircle", x, y, radius, fill, stroke, strokeWidth);
				return new Circle(jSCircle, konvaWrapper);
			}
		}
	}
}
//=====================================================================================================================
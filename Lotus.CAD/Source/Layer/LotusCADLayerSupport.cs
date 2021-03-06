//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Общая подсистема
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADLayerSupport.cs
*		Слой для расположения графических элементов.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 27.03.2022
//=====================================================================================================================
using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Xml;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
//=====================================================================================================================
namespace Lotus
{
	namespace CAD
	{
		//-------------------------------------------------------------------------------------------------------------
		//! \addtogroup CadDrawing
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Интерфейс для определения графического элемента поддерживающего/имеющего слой
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public interface ICadLayerSupport
		{
			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Идентификатор слоя
			/// </summary>
			Int64 LayerId { get; set; }

			/// <summary>
			/// Названия слоя
			/// </summary>
			String LayerName { get; set; }

			/// <summary>
			/// Слой
			/// </summary>
			CCadLayer Layer { get; set; }
			#endregion

			#region ======================================= МЕТОДЫ ====================================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка слоя
			/// </summary>
			/// <param name="layer">Слой</param>
			//---------------------------------------------------------------------------------------------------------
			void SetLayer(CCadLayer layer);

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление данных слоя
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			void UpdateLayer();
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
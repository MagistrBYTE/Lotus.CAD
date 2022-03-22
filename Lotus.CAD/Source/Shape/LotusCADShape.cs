﻿//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Интерактивные графические фигуры
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADShape.cs
*		Базовая графическая фигура
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 04.04.2021
//=====================================================================================================================
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Maths;
using Lotus.Core;
//=====================================================================================================================
namespace Lotus
{
	namespace CAD
	{
		//-------------------------------------------------------------------------------------------------------------
		//! \addtogroup CadShape
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Базовая графическая фигура с базовым взаимодействием
		/// </summary>
		/// <typeparam name="TPrimitive">Примитив</typeparam>
		public abstract class CCadShape<TPrimitive> : CCadShapeBase, ICadShape where TPrimitive : CCadPrimitive
		{
			#region ======================================= ДАННЫЕ ====================================================
			// Служебные данные
			internal TPrimitive mPrimitive;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public CCadShape()
			{
				// Данные по умолчанию
				mHandleIndex = -1;
				mHandleSubIndex = -1;
				mHandleRects = new List<Rect2Df>();
				mLayer = XCadLayerManager.DefaultLayer;
				mLayerId = mLayer.Id;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор копирования
			/// </summary>
			/// <param name="source">Источник копии</param>
			/// <param name="add_to_draft">Добавлять ли в чертеж источника</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadShape(CCadShape<TPrimitive> source, Boolean add_to_draft = true)
				: base(source)
			{
				mHandleIndex = -1;
				mHandleSubIndex = -1;
				mHandleRects = new List<Rect2Df>();
				mLayer = XCadLayerManager.DefaultLayer;
				mLayerId = mLayer.Id;
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
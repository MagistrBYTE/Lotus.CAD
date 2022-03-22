﻿//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Графические примитивы
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusWindowsCADPrimitiveLine.cs
*		Графический примитив линии.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.01.2022
//=====================================================================================================================
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
using Lotus.Maths;
//=====================================================================================================================
namespace Lotus
{
	namespace CAD
	{
		//-------------------------------------------------------------------------------------------------------------
		//! \addtogroup CadPrimitivs
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Графический примитив линии
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CCadWindowsPrimitiveLine : CCadPrimitiveLine
		{
			#region ======================================= ДАННЫЕ ====================================================
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public CCadWindowsPrimitiveLine()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="start_point">Начальная точка</param>
			/// <param name="end_point">Конечная точка</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadWindowsPrimitiveLine(Vector2Df start_point, Vector2Df end_point)
			{
				mStartPoint = start_point;
				mEndPoint = end_point;
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление данных линии
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void Update()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Копирование графического примитива
			/// </summary>
			/// <returns>Дубликат графического примитива со всеми параметрами и данными</returns>
			//---------------------------------------------------------------------------------------------------------
			public override CCadPrimitive Duplicate()
			{
				CCadWindowsPrimitiveLine line = new CCadWindowsPrimitiveLine();
				line.CopyParamemtrs(this);
				line.Update();
				return (line);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Копирование параметров с графического примитива
			/// </summary>
			/// <param name="primitiv">Графический примитив</param>
			//---------------------------------------------------------------------------------------------------------
			public override void CopyParamemtrs(CCadPrimitive primitiv)
			{
				base.CopyParamemtrs(primitiv);

				CCadWindowsPrimitiveLine source = primitiv as CCadWindowsPrimitiveLine;
				mStartPoint = source.mStartPoint;
				mEndPoint = source.mEndPoint;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка раположения точки на контуре графического примитива
			/// </summary>
			/// <param name="point">Проверяемая точка</param>
			/// <param name="thickness">Толщина контура</param>
			/// <returns>Статус расположения</returns>
			//---------------------------------------------------------------------------------------------------------
			public override Boolean StrokeContains(ref Vector2Df point, Single thickness)
			{
				return (XIntersect2D.PointOnSegment(ref mStartPoint, ref mEndPoint, ref point, mStroke.Thickness));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка раположения точки внутри области графического примитива
			/// </summary>
			/// <param name="point">Проверяемая точка</param>
			/// <returns>Статус расположения</returns>
			//---------------------------------------------------------------------------------------------------------
			public override Boolean FillContains(ref Vector2Df point)
			{
				return (false);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вычисление площади графического примитива
			/// </summary>
			/// <returns>Площадь графического примитива</returns>
			//---------------------------------------------------------------------------------------------------------
			public override Double ComputeArea()
			{
				return (0);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вычисление ограничивающего прямоугольника графического примитива
			/// </summary>
			/// <param name="dest_bounds_rect">Прямоугольник</param>
			//---------------------------------------------------------------------------------------------------------
			public override void ComputeBounds(ref Rect2Df dest_bounds_rect)
			{
				dest_bounds_rect = mBoundsRect;
			}
			#endregion

			#region ======================================= МЕТОДЫ РИСОВАНИЯ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Рисование линии
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void Draw()
			{
				//LotusCadCanvas.DrawingDevice.DrawLine(mStroke.WindowsPen, mStartPoint.ToWinPoint(),
				//	mEndPoint.ToWinPoint());
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
﻿//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Подсистема расширений методов
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADWindowsExtension.cs
*		Реализация методов расширений для Windows.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.01.2022
//=====================================================================================================================
using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
//---------------------------------------------------------------------------------------------------------------------
using System.Windows;
using System.Windows.Media;
using Media3D = System.Windows.Media.Media3D;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
using Lotus.Maths;
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
		/// Статический класс для реализации методов расширений платформы Windows
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public static class XWindowsExtension
		{
			#region ======================================= МЕТОДЫ ПРЕОБРАЗОВАНИЯ TO Windows ==========================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Преобразование в тип Color
			/// </summary>
			/// <param name="@this">Color</param>
			/// <returns>Объект <see cref="Color"/></returns>
			//---------------------------------------------------------------------------------------------------------
			public static Color ToWinColor(this TColor @this)
			{
				return (Color.FromArgb(@this.A, @this.R, @this.G, @this.B));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Преобразование в тип Vector3D
			/// </summary>
			/// <param name="@this">Вектор</param>
			/// <returns>Объект <see cref="Media3D.Vector3D"/></returns>
			//---------------------------------------------------------------------------------------------------------
			public static Vector3D ToWinVector3D(this Vector3Df @this)
			{
				return (new Vector3D(@this.X, @this.Y, @this.Z));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Преобразование в тип Point3D
			/// </summary>
			/// <param name="@this">Вектор</param>
			/// <returns>Объект <see cref="Media3D.Point3D"/></returns>
			//---------------------------------------------------------------------------------------------------------
			public static Media3D.Point3D ToWinPoint3D(this Vector3Df @this)
			{
				return (new Media3D.Point3D(@this.X, @this.Y, @this.Z));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Преобразование в тип Point
			/// </summary>
			/// <param name="@this">Вектор</param>
			/// <returns>Объект <see cref="Point"/></returns>
			//---------------------------------------------------------------------------------------------------------
			public static Point ToWinPoint(this Vector2Df @this)
			{
				return (new Point(@this.X, @this.Y));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Преобразование в тип Point
			/// </summary>
			/// <param name="@this">Вектор</param>
			/// <returns>Объект <see cref="Point"/></returns>
			//---------------------------------------------------------------------------------------------------------
			public static Point ToWinPoint(this Vector3Df @this)
			{
				return (new Point(@this.X, @this.Y));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Преобразование в тип Rect
			/// </summary>
			/// <param name="@this">Прямоугольник</param>
			/// <returns>Объект <see cref="Rect"/></returns>
			//---------------------------------------------------------------------------------------------------------
			public static Rect ToWinRect(this Rect2Df @this)
			{
				return (new Rect(@this.X, @this.Y, @this.Width, @this.Height));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Преобразование в тип Matrix
			/// </summary>
			/// <param name="@this">Матрица</param>
			/// <returns>Объект <see cref="Matrix"/></returns>
			//---------------------------------------------------------------------------------------------------------
			public static Matrix ToWinMatrix(this Matrix3Dx2f @this)
			{
				return (new Matrix
				{
					M11 = @this.M11,
					M12 = @this.M12,
					M21 = @this.M21,
					M22 = @this.M22,
					OffsetX = @this.M31,
					OffsetY = @this.M32
				});
			}
			#endregion

			#region ======================================= МЕТОДЫ ПРЕОБРАЗОВАНИЯ FROM Windows ========================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Преобразование в тип Rect
			/// </summary>
			/// <param name="@this">Прямоугольник</param>
			/// <returns>Объект <see cref="Rect2Df"/></returns>
			//---------------------------------------------------------------------------------------------------------
			public static Rect2Df ToRect(this Rect @this)
			{
				return (new Rect2Df((Single)@this.X, (Single)@this.Y, (Single)@this.Width, (Single)@this.Height));
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
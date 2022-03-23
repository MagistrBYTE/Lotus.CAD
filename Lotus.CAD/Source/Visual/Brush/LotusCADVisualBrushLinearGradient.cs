//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Ресурсы для отображения графического объекта
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADVisualBrushLinearGradient.cs
*		Кисть для линейного градиентного заполнения.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 27.03.2022
//=====================================================================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Xml;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
using Lotus.Maths;
//=====================================================================================================================
namespace Lotus
{
	namespace CAD
	{
		//-------------------------------------------------------------------------------------------------------------
		//! \addtogroup CadVisual
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Кисть для линейного градиентного заполнения
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CCadBrushLinearGradient : CCadBrush
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			protected static PropertyChangedEventArgs PropertyArgsStartColor = new PropertyChangedEventArgs(nameof(StartColor));
			protected static PropertyChangedEventArgs PropertyArgsStartPoint = new PropertyChangedEventArgs(nameof(StartPoint));
			protected static PropertyChangedEventArgs PropertyArgsEndColor = new PropertyChangedEventArgs(nameof(EndColor));
			protected static PropertyChangedEventArgs PropertyArgsEndPoint = new PropertyChangedEventArgs(nameof(EndPoint));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			internal TColor mStartColor;
			internal Vector2Df mStartPoint;
			internal TColor mEndColor;
			internal Vector2Df mEndPoint;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			//
			// ОСНОВНЫЕ ПАРАМЕТРЫ
			//
			/// <summary>
			/// Начальный цвет
			/// </summary>
			[DisplayName("Начальный цвет")]
			[Description("Начальный цвет заливки")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 0)]
			public TColor StartColor
			{
				get { return (mStartColor); }
				set
				{
					if (TColor.Approximately(ref mStartColor, ref value))
					{
						mStartColor = value;

						// 1) Информируем об изменении
						NotifyPropertyChanged(PropertyArgsStartColor);

						// 2) Обновляем
						RaiseColorsChanged();
					}
				}
			}

			/// <summary>
			/// Начальный точка
			/// </summary>
			[DisplayName("Начальный точка")]
			[Description("Начальный точка заливки")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 1)]
			public Vector2Df StartPoint
			{
				get { return (mStartPoint); }
				set
				{
					if (Vector2Df.Approximately(ref mStartPoint, ref value))
					{
						mStartPoint = value;

						// 1) Информируем об изменении
						NotifyPropertyChanged(PropertyArgsStartPoint);

						// 2) Обновляем
						RaisePointsChanged();
					}
				}
			}

			/// <summary>
			/// Конечный цвет
			/// </summary>
			[DisplayName("Конечный цвет")]
			[Description("Конечный цвет заливки")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 2)]
			public TColor EndColor
			{
				get { return (mEndColor); }
				set
				{
					if (TColor.Approximately(ref mEndColor, ref value))
					{
						mEndColor = value;

						// 1) Информируем об изменении
						NotifyPropertyChanged(PropertyArgsEndColor);

						// 2) Обновляем
						RaiseColorsChanged();
					}
				}
			}

			/// <summary>
			/// Конечный точка
			/// </summary>
			[DisplayName("Конечный точка")]
			[Description("Конечный точка заливки")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 3)]
			public Vector2Df EndPoint
			{
				get { return (mEndPoint); }
				set
				{
					if (Vector2Df.Approximately(ref mEndPoint, ref value))
					{
						mEndPoint = value;

						// 1) Информируем об изменении
						NotifyPropertyChanged(PropertyArgsEndPoint);

						// 2) Обновляем
						RaisePointsChanged();
					}
				}
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public CCadBrushLinearGradient()
				:this("Новая кисть")
			{
				
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="name">Имя кисти</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadBrushLinearGradient(String name)
			{
				mName = name;
				mBrushFill = TCadBrushFillType.LinearGradient;
#if USE_WINDOWS
				mWindowsBrush = new System.Windows.Media.LinearGradientBrush(mStartColor.ToWinColor(), 
					mEndColor.ToWinColor(), mStartPoint.ToWinPoint(), mEndPoint.ToWinPoint());
#endif
#if USE_GDI
				//mDrawingBrush = new System.Drawing.Drawing2D.LinearGradientBrush(mStartPoint, mEndPoint, mStartColor, mEndColor);
#endif
			}
			#endregion

			#region ======================================= СЛУЖЕБНЫЕ МЕТОДЫ СОБЫТИЙ ==================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение цвета кисти.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseColorsChanged()
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение точек кисти.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaisePointsChanged()
			{

			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
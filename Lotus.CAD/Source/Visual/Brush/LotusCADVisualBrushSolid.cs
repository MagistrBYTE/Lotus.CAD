//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Ресурсы для отображения графического объекта
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADVisualBrushSolid.cs
*		Кисть однотонного цвета. Применяется для сплошной заливки одним цветом.
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
		/// Кисть однотонного цвета
		/// </summary>
		/// <remarks>
		/// Применяется для сплошной заливки одним цветом
		/// </remarks>
		//-------------------------------------------------------------------------------------------------------------
		public class CCadBrushSolid : CCadBrush
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			protected static PropertyChangedEventArgs PropertyArgsColor = new PropertyChangedEventArgs(nameof(Color));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			// Основные параметры
			internal TColor mColor;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			//
			// ОСНОВНЫЕ ПАРАМЕТРЫ
			//
			/// <summary>
			/// Основное цвет заливки
			/// </summary>
			[DisplayName("Цвет")]
			[Description("Основное цвет заливки")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 0)]
			public TColor Color
			{
				get { return (mColor); }
				set
				{
					if (!TColor.Approximately(ref mColor, ref value))
					{
						mColor = value;

						// 1) Информируем об изменении
						NotifyPropertyChanged(PropertyArgsColor);

						// 2) Обновляем
						RaiseColorChanged();
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
			public CCadBrushSolid()
				: this("Новая кисть")
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="name">Имя кисти</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadBrushSolid(String name)
			{
				mName = name;
				mBrushFill = TCadBrushFillType.Solid;
#if USE_WINDOWS
				mWindowsBrush = System.Windows.Media.Brushes.Aquamarine.CloneCurrentValue();
#endif
#if USE_GDI
				mDrawingBrush = System.Drawing.Brushes.Aquamarine.Clone() as System.Drawing.Brush;
#endif
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="color">Цвет кисти</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadBrushSolid(TColor color)
			{
				mName = "Новая кисть";
				mBrushFill = TCadBrushFillType.Solid;
				mColor = color;
#if USE_WINDOWS
				mWindowsBrush = new System.Windows.Media.SolidColorBrush(mColor.ToWinColor());
#endif
#if USE_GDI
				mDrawingBrush = new System.Drawing.SolidBrush(mColor);
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
			protected virtual void RaiseColorChanged()
			{
#if USE_WINDOWS
				System.Windows.Media.SolidColorBrush solid_brush = mWindowsBrush as System.Windows.Media.SolidColorBrush;
				solid_brush.Color = Color.ToWinColor();

				// 2) Информируем об изменении
				NotifyPropertyChanged(PropertyArgsWindowsBrush);
#endif
#if USE_GDI
				System.Drawing.SolidBrush gdi_solid_brush = mDrawingBrush as System.Drawing.SolidBrush;
				gdi_solid_brush.Color = Color;
#endif
#if USE_SHARPDX
				if (mD2DBrush != null)
				{
					SharpDX.Direct2D1.SolidColorBrush d2d_solid_brush = mD2DBrush as SharpDX.Direct2D1.SolidColorBrush;
					d2d_solid_brush.Color = mColor;
				}
#endif
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
#if USE_WINDOWS
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление ресурса WPF
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void UpdateWindowsResource()
			{
				System.Windows.Media.SolidColorBrush solid_brush = mWindowsBrush as System.Windows.Media.SolidColorBrush;
				solid_brush.Color = Color.ToWinColor();
			}
#endif
#if USE_GDI
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление ресурса System.Drawing
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void UpdateDrawingResource()
			{
				System.Drawing.SolidBrush gdi_solid_brush = mDrawingBrush as System.Drawing.SolidBrush;
				gdi_solid_brush.Color = Color;
			}
#endif
#if USE_SHARPDX
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление ресурса Direct2D
			/// </summary>
			/// <param name="forced">Принудительное создание ресурса Direct2D</param>
			//---------------------------------------------------------------------------------------------------------
			public override void UpdateDirect2DResource(Boolean forced = false)
			{
				if (XDirect2DManager.D2DRenderTarget != null)
				{
					// Принудительное создание ресурса
					if (forced) XDisposer.SafeDispose(ref mD2DBrush);

					if (mD2DBrush == null)
					{
						mD2DBrush = new SharpDX.Direct2D1.SolidColorBrush(XDirect2DManager.D2DRenderTarget, mColor);
					}
					else
					{
						SharpDX.Direct2D1.SolidColorBrush d2d_solid_brush = mD2DBrush as SharpDX.Direct2D1.SolidColorBrush;
						d2d_solid_brush.Color = mColor;
					}
				}
			}
#endif
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
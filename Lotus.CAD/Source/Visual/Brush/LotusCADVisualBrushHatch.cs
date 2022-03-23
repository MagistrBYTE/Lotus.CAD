//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Ресурсы для отображения графического объекта
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADVisualBrushHatch.cs
*		Кисть штриховки.
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
		/// Кисть штриховки
		/// </summary>
		/// <remarks>
		/// Применяется для заполнения области определенным узором
		/// </remarks>
		//-------------------------------------------------------------------------------------------------------------
		public class CCadBrushHatch : CCadBrush
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			protected static PropertyChangedEventArgs PropertyArgsResourceName = new PropertyChangedEventArgs(nameof(ResourceName));
			protected static PropertyChangedEventArgs PropertyArgsForegroundColor = new PropertyChangedEventArgs(nameof(ForegroundColor));
			protected static PropertyChangedEventArgs PropertyArgsBackgroundColor = new PropertyChangedEventArgs(nameof(BackgroundColor));
			protected static PropertyChangedEventArgs PropertyArgsIsBackground = new PropertyChangedEventArgs(nameof(IsBackground));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			// Основные параметры
			internal String mResourceName;
			internal TColor mForegroundColor = TColor.Blue;
			internal TColor mBackgroundColor;
			internal Boolean mIsBackground;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			//
			// ОСНОВНЫЕ ПАРАМЕТРЫ
			//
			/// <summary>
			/// Имя ресурса штриховки кисти
			/// </summary>
			[DisplayName("Имя ресурса")]
			[Description("Имя ресурса изображения кисти")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 0)]
			public String ResourceName
			{
				get { return (mResourceName); }
				set
				{
					if (mResourceName != value)
					{
						mResourceName = value;

						// 1) Информируем об изменении
						NotifyPropertyChanged(PropertyArgsResourceName);

						// 2) Обновляем
						RaiseResourceNameChanged();
					}
				}
			}

			/// <summary>
			/// Цвет узора
			/// </summary>
			[DisplayName("Цвет узора")]
			[Description("Цвет узора")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 1)]
			public TColor ForegroundColor
			{
				get { return (mForegroundColor); }
				set
				{
					if (TColor.Approximately(ref mForegroundColor, ref value))
					{
						mForegroundColor = value;

						// 1) Информируем об изменении
						NotifyPropertyChanged(PropertyArgsForegroundColor);

						// 2) Обновляем
						RaiseColorChanged();
					}
				}
			}

			/// <summary>
			/// Цвет фона
			/// </summary>
			[DisplayName("Цвет фона")]
			[Description("Цвет фона")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 2)]
			public TColor BackgroundColor
			{
				get { return (mBackgroundColor); }
				set
				{
					if (TColor.Approximately(ref mBackgroundColor, ref value))
					{
						mBackgroundColor = value;

						// 1) Информируем об изменении
						NotifyPropertyChanged(PropertyArgsBackgroundColor);

						// 2) Обновляем
						RaiseColorChanged();
					}
				}
			}

			/// <summary>
			/// Статус цвета фона
			/// </summary>
			[DisplayName("Статус фона")]
			[Description("Статус цвета фона")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 3)]
			public Boolean IsBackground
			{
				get { return (mIsBackground); }
				set
				{
					if (mIsBackground != value)
					{
						mIsBackground = value;

						// 1) Информируем об изменении
						NotifyPropertyChanged(PropertyArgsIsBackground);

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
			public CCadBrushHatch()
				: this("Новая кисть")
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="name">Имя кисти</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadBrushHatch(String name)
			{
				mName = name;
				mBrushFill = TCadBrushFillType.Hatching;
#if USE_WINDOWS
				mWindowsBrush = new System.Windows.Media.DrawingBrush();
#endif
#if USE_GDI
				mDrawingBrush = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.Cross,
					mForegroundColor);
#endif
			}
			#endregion

			#region ======================================= СЛУЖЕБНЫЕ МЕТОДЫ СОБЫТИЙ ==================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение имени ресурса кисти.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseResourceNameChanged()
			{
#if USE_WINDOWS
				// Копируем кисть с ресурсов
				System.Windows.Media.DrawingBrush resource_brush = XCadModuleInitializer.GraphicsResources[mResourceName] as System.Windows.Media.DrawingBrush;
				mWindowsBrush = resource_brush.CloneCurrentValue();

				System.Windows.Media.DrawingBrush hatch_brush = mWindowsBrush as System.Windows.Media.DrawingBrush;
				System.Windows.Media.DrawingGroup dg = hatch_brush.Drawing as System.Windows.Media.DrawingGroup;
				System.Windows.Media.GeometryDrawing gd = dg.Children[0] as System.Windows.Media.GeometryDrawing;
				if (mIsBackground)
				{
					gd.Brush = Windows.XWindowsColorManager.GetBrushByColor(mBackgroundColor.ToWinColor());
				}
				else
				{
					gd.Brush = null;
				}
				gd.Pen.Brush = Windows.XWindowsColorManager.GetBrushByColor(mForegroundColor.ToWinColor());

				// 2) Информируем об изменении
				NotifyPropertyChanged(PropertyArgsWindowsBrush);
#endif
#if USE_GDI
				if (mDrawingBrush != null)
				{
					mDrawingBrush.Dispose();
				}
				mDrawingBrush = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.Cross,
					mForegroundColor, mBackgroundColor);
#endif
#if USE_SHARPDX
				if (mD2DBrush != null)
				{
					SharpDX.Direct2D1.SolidColorBrush d2d_solid_brush = mD2DBrush as SharpDX.Direct2D1.SolidColorBrush;
					d2d_solid_brush.Color = mForegroundColor;
				}
#endif
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение цветов кисти.
			/// Метод автоматически вызывается после установки соответствующих свойств
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseColorChanged()
			{
#if USE_WINDOWS
				System.Windows.Media.DrawingBrush hatch_brush = mWindowsBrush as System.Windows.Media.DrawingBrush;
				System.Windows.Media.DrawingGroup dg = hatch_brush.Drawing as System.Windows.Media.DrawingGroup;
				System.Windows.Media.GeometryDrawing gd = dg.Children[0] as System.Windows.Media.GeometryDrawing;
				if (mIsBackground)
				{
					gd.Brush = Windows.XWindowsColorManager.GetBrushByColor(mBackgroundColor.ToWinColor());
				}
				else
				{
					gd.Brush = null;
				}
				gd.Pen.Brush = Windows.XWindowsColorManager.GetBrushByColor(mForegroundColor.ToWinColor());

				// 2) Информируем об изменении
				NotifyPropertyChanged(PropertyArgsWindowsBrush);
#endif
#if USE_GDI
				if (mDrawingBrush != null)
				{
					mDrawingBrush.Dispose();
				}
				mDrawingBrush = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.Cross,
					mForegroundColor, mBackgroundColor);
#endif
#if USE_SHARPDX
				if (mD2DBrush != null)
				{
					SharpDX.Direct2D1.SolidColorBrush d2d_solid_brush = mD2DBrush as SharpDX.Direct2D1.SolidColorBrush;
					d2d_solid_brush.Color = mForegroundColor;
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
				System.Windows.Media.DrawingBrush hatch_brush = mWindowsBrush as System.Windows.Media.DrawingBrush;
				if (hatch_brush.Drawing == null)
				{
					// Копируем кисть с ресурсов
					System.Windows.Media.DrawingBrush resource_brush = XCadModuleInitializer.GraphicsResources[mResourceName] as System.Windows.Media.DrawingBrush;
					mWindowsBrush = resource_brush.CloneCurrentValue();

					hatch_brush = mWindowsBrush as System.Windows.Media.DrawingBrush;
					System.Windows.Media.DrawingGroup dg = hatch_brush.Drawing as System.Windows.Media.DrawingGroup;
					System.Windows.Media.GeometryDrawing gd = dg.Children[0] as System.Windows.Media.GeometryDrawing;
					if (mIsBackground)
					{
						gd.Brush = Windows.XWindowsColorManager.GetBrushByColor(mBackgroundColor.ToWinColor());
					}
					else
					{
						gd.Brush = null;
					}
					gd.Pen.Brush = Windows.XWindowsColorManager.GetBrushByColor(mForegroundColor.ToWinColor());
				}
				else
				{
					System.Windows.Media.DrawingGroup dg = hatch_brush.Drawing as System.Windows.Media.DrawingGroup;
					System.Windows.Media.GeometryDrawing gd = dg.Children[0] as System.Windows.Media.GeometryDrawing;
					if (mIsBackground)
					{
						gd.Brush = Windows.XWindowsColorManager.GetBrushByColor(mBackgroundColor.ToWinColor());
					}
					else
					{
						gd.Brush = null;
					}
					gd.Pen.Brush = Windows.XWindowsColorManager.GetBrushByColor(mForegroundColor.ToWinColor());
				}
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
				if (mDrawingBrush != null)
				{
					mDrawingBrush.Dispose();
				}
				mDrawingBrush = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.Cross,
					mForegroundColor, mBackgroundColor);
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
						mD2DBrush = new SharpDX.Direct2D1.SolidColorBrush(XDirect2DManager.D2DRenderTarget, mForegroundColor);
					}
					else
					{
						SharpDX.Direct2D1.SolidColorBrush d2d_solid_brush = mD2DBrush as SharpDX.Direct2D1.SolidColorBrush;
						d2d_solid_brush.Color = mForegroundColor;
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
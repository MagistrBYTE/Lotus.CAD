//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Ресурсы для отображения графического объекта
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADVisualBrushImage.cs
*		Кисть для заполнения области изображением.
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
		/// Режим заполнения кистью изображением
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		[TypeConverter(typeof(EnumToStringConverter<TCadBrushExtendMode>))]
		public enum TCadBrushExtendMode
		{
			/// <summary>
			/// Единичное
			/// </summary>
			Clamp,

			/// <summary>
			/// Повтор
			/// </summary>
			Wrap,

			/// <summary>
			/// Зеркальное отображение через одного
			/// </summary>
			Mirror
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Кисть для заполнения области изображением
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CCadBrushImage : CCadBrush
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			protected static PropertyChangedEventArgs PropertyArgsResourceName = new PropertyChangedEventArgs(nameof(ResourceName));
			protected static PropertyChangedEventArgs PropertyArgsExtendModeX = new PropertyChangedEventArgs(nameof(ExtendModeX));
			protected static PropertyChangedEventArgs PropertyArgsExtendModeY = new PropertyChangedEventArgs(nameof(ExtendModeY));
			protected static PropertyChangedEventArgs PropertyArgsScale = new PropertyChangedEventArgs(nameof(Scale));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			internal String mResourceName;
			internal TCadBrushExtendMode mExtendModeX;
			internal TCadBrushExtendMode mExtendModeY;
			internal Single mScale;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Имя ресурса изображения кисти
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
			/// Режим заполнение области кистью по горизонтали
			/// </summary>
			[DisplayName("Режим по горизонтали")]
			[Description("Режим заполнение области кистью по горизонтали")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 1)]
			public TCadBrushExtendMode ExtendModeX
			{
				get { return (mExtendModeX); }
				set
				{
					if (mExtendModeX != value)
					{
						mExtendModeX = value;

						// 1) Информируем об изменении
						NotifyPropertyChanged(PropertyArgsExtendModeX);

						// 2) Обновляем
						RaiseExtendModeChanged();
					}
				}
			}

			/// <summary>
			/// Режим заполнение области кистью по вертикали
			/// </summary>
			[DisplayName("Режим по вертикали")]
			[Description("Режим заполнение области кистью по вертикали")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 2)]
			public TCadBrushExtendMode ExtendModeY
			{
				get { return (mExtendModeY); }
				set
				{
					if (mExtendModeY != value)
					{
						mExtendModeY = value;

						// 1) Информируем об изменении
						NotifyPropertyChanged(PropertyArgsExtendModeY);

						// 2) Обновляем
						RaiseExtendModeChanged();
					}
				}
			}

			/// <summary>
			/// Масштабирование образца заполнения кисти
			/// </summary>
			[DisplayName("Масштаб образца")]
			[Description("Масштабирование образца заполнения кисти")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 3)]
			public Single Scale
			{
				get { return (mScale); }
				set
				{
					if (mScale != value)
					{
						mScale = value;

						// 1) Информируем об изменении
						NotifyPropertyChanged(PropertyArgsScale);

						// 2) Обновляем
						RaiseScaleChanged();
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
			public CCadBrushImage()
				: this("Новая кисть")
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="name">Имя кисти</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadBrushImage(String name)
			{
				mName = name;
				mBrushFill = TCadBrushFillType.Image;
				mExtendModeX = TCadBrushExtendMode.Wrap;
				mExtendModeY = TCadBrushExtendMode.Wrap;
				mScale = 1.0f;
#if USE_WINDOWS
				mWindowsBrush = new System.Windows.Media.ImageBrush();
#endif
			}
			#endregion

			#region ======================================= СЛУЖЕБНЫЕ МЕТОДЫ ==========================================
#if USE_WINDOWS
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение режима мозаичного заполнения по WPF
			/// </summary>
			/// <returns>Режим мозаичного заполнения по WPF</returns>
			//---------------------------------------------------------------------------------------------------------
			internal System.Windows.Media.TileMode GetTileMode()
			{
				System.Windows.Media.TileMode tile_mode = System.Windows.Media.TileMode.None;
				switch (mExtendModeX)
				{
					case TCadBrushExtendMode.Clamp:
						{
							switch (mExtendModeY)
							{
								case TCadBrushExtendMode.Clamp:
									tile_mode = System.Windows.Media.TileMode.None;
									break;
								case TCadBrushExtendMode.Wrap:
									tile_mode = System.Windows.Media.TileMode.None;
									break;
								case TCadBrushExtendMode.Mirror:
									tile_mode = System.Windows.Media.TileMode.FlipY;
									break;
								default:
									break;
							}
						}
						break;
					case TCadBrushExtendMode.Wrap:
						{
							switch (mExtendModeY)
							{
								case TCadBrushExtendMode.Clamp:
									tile_mode = System.Windows.Media.TileMode.Tile;
									break;
								case TCadBrushExtendMode.Wrap:
									tile_mode = System.Windows.Media.TileMode.Tile;
									break;
								case TCadBrushExtendMode.Mirror:
									tile_mode = System.Windows.Media.TileMode.FlipY;
									break;
								default:
									break;
							}
						}
						break;
					case TCadBrushExtendMode.Mirror:
						{
							switch (mExtendModeY)
							{
								case TCadBrushExtendMode.Clamp:
									tile_mode = System.Windows.Media.TileMode.FlipX;
									break;
								case TCadBrushExtendMode.Wrap:
									tile_mode = System.Windows.Media.TileMode.FlipX;
									break;
								case TCadBrushExtendMode.Mirror:
									tile_mode = System.Windows.Media.TileMode.FlipXY;
									break;
								default:
									break;
							}
						}
						break;
					default:
						break;
				}

				return (tile_mode);
			}
#endif
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
				System.Windows.Media.ImageBrush image_brush = mWindowsBrush as System.Windows.Media.ImageBrush;
				image_brush.ImageSource = Windows.XWindowsLoaderBitmap.LoadBitmapFromResource(Windows.Properties.Resources.ResourceManager, mResourceName);
				image_brush.Viewport = new System.Windows.Rect(0, 0, image_brush.ImageSource.Width, image_brush.ImageSource.Height);

				// 2) Информируем об изменении
				NotifyPropertyChanged(PropertyArgsWindowsBrush);
#endif
#if USE_GDI
				if(mDrawingBrush != null)
				{
					mDrawingBrush.Dispose();
				}
				Object image = Properties.Resources.ResourceManager.GetObject(mResourceName);
				System.Drawing.Bitmap source = (System.Drawing.Bitmap)image;
				mDrawingBrush = new System.Drawing.TextureBrush(source, (System.Drawing.Drawing2D.WrapMode)GetTileMode());
#endif
#if USE_SHARPDX
				if (mD2DBrush != null)
				{
					SharpDX.Direct2D1.BitmapBrush d2d_image_brush = mD2DBrush as SharpDX.Direct2D1.BitmapBrush;
					d2d_image_brush.Bitmap = XDirect2DManager.LoadFromResource(mResourceName);
				}
#endif
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение режима заполнения кисти.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseExtendModeChanged()
			{
#if USE_WINDOWS
				System.Windows.Media.ImageBrush image_brush = mWindowsBrush as System.Windows.Media.ImageBrush;
				image_brush.TileMode = GetTileMode();

				// 2) Информируем об изменении
				NotifyPropertyChanged(PropertyArgsWindowsBrush);
#endif
#if USE_GDI
				if (mDrawingBrush != null)
				{
					System.Drawing.TextureBrush gdi_image_brush = mDrawingBrush as System.Drawing.TextureBrush;
					gdi_image_brush.WrapMode = (System.Drawing.Drawing2D.WrapMode)GetTileMode();
				}
#endif
#if USE_SHARPDX
				if (mD2DBrush != null)
				{
					SharpDX.Direct2D1.BitmapBrush d2d_image_brush = mD2DBrush as SharpDX.Direct2D1.BitmapBrush;
					d2d_image_brush.ExtendModeX = (SharpDX.Direct2D1.ExtendMode)mExtendModeX;
					d2d_image_brush.ExtendModeY = (SharpDX.Direct2D1.ExtendMode)mExtendModeY;
				}
#endif
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение масштаба отображения кисти.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseScaleChanged()
			{
#if USE_SHARPDX
				if (mD2DBrush != null)
				{
					mD2DBrush.Transform = SharpDX.Matrix3x2.Scaling(mScale);
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
				System.Windows.Media.ImageBrush image_brush = mWindowsBrush as System.Windows.Media.ImageBrush;
				image_brush.ImageSource = Windows.XWindowsLoaderBitmap.LoadBitmapFromResource(Windows.Properties.Resources.ResourceManager, mResourceName);
				image_brush.Stretch = System.Windows.Media.Stretch.Fill;
				image_brush.ViewportUnits = System.Windows.Media.BrushMappingMode.Absolute;
				image_brush.Viewport = new System.Windows.Rect(0, 0, image_brush.ImageSource.Width, image_brush.ImageSource.Height);
				image_brush.TileMode = GetTileMode();
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

				Object image = Properties.Resources.ResourceManager.GetObject(mResourceName);
				System.Drawing.Bitmap source = (System.Drawing.Bitmap)image;
				mDrawingBrush = new System.Drawing.TextureBrush(source, (System.Drawing.Drawing2D.WrapMode)GetTileMode());
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
						SharpDX.Direct2D1.BitmapBrushProperties bbp = new SharpDX.Direct2D1.BitmapBrushProperties();
						bbp.ExtendModeX = (SharpDX.Direct2D1.ExtendMode)mExtendModeX;
						bbp.ExtendModeY = (SharpDX.Direct2D1.ExtendMode)mExtendModeY;
						bbp.InterpolationMode = SharpDX.Direct2D1.BitmapInterpolationMode.NearestNeighbor;
						mD2DBrush = new SharpDX.Direct2D1.BitmapBrush(XDirect2DManager.D2DRenderTarget,
							XDirect2DManager.LoadFromResource(mResourceName), bbp);

						mD2DBrush.Transform = SharpDX.Matrix3x2.Scaling(mScale);

					}
					else
					{
						SharpDX.Direct2D1.BitmapBrush d2d_image_brush = mD2DBrush as SharpDX.Direct2D1.BitmapBrush;
						d2d_image_brush.Bitmap = XDirect2DManager.LoadFromResource(mResourceName);
						d2d_image_brush.ExtendModeX = (SharpDX.Direct2D1.ExtendMode)mExtendModeX;
						d2d_image_brush.ExtendModeY = (SharpDX.Direct2D1.ExtendMode)mExtendModeY;
						d2d_image_brush.Transform = SharpDX.Matrix3x2.Scaling(mScale);
						d2d_image_brush.InterpolationMode = SharpDX.Direct2D1.BitmapInterpolationMode.NearestNeighbor;
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
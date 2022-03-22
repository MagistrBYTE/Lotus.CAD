//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Ресурсы для отображения графического объекта
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADVisualPen.cs
*		Перо для отображения пера графического объекта.
*		Перо также используется для различных кистей штриховок
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.01.2022
//=====================================================================================================================
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Xml;
using System.Linq;
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
		/// Перо для отображения контура графического объекта
		/// </summary>
		/// <remarks>
		/// Перо также используется для различных кистей штриховок
		/// </remarks>
		//-------------------------------------------------------------------------------------------------------------
		public sealed class CCadPen : CCadEntity, IComparable<CCadPen>, ILotusSupportViewInspector
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			private static PropertyChangedEventArgs PropertyArgsGroup = new PropertyChangedEventArgs(nameof(Group));
			private static PropertyChangedEventArgs PropertyArgsBrush = new PropertyChangedEventArgs(nameof(Brush));
			private static PropertyChangedEventArgs PropertyArgsThickness = new PropertyChangedEventArgs(nameof(Thickness));
			private static PropertyChangedEventArgs PropertyArgsPenStyle = new PropertyChangedEventArgs(nameof(PenStyle));
#if USE_WINDOWS
			private static PropertyChangedEventArgs PropertyArgsWindowsPen = new PropertyChangedEventArgs(nameof(WindowsPen));
#endif
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			// Общие данные
			internal String mGroup;

			// Основные параметры
			internal CCadBrush mBrush;
			internal Single mThickness;
			internal CCadPenStyle mPenStyle;

			// Служебные данные
			internal String mInternalName = "";

			// Платформенные-зависимые данные
#if USE_WINDOWS
			internal System.Windows.Media.Pen mWindowsPen;
#endif
#if USE_GDI
			internal System.Drawing.Pen mDrawingPen;
#endif
#if USE_SHARPDX
			internal SharpDX.Direct2D1.StrokeStyle mD2DStrokeStyle;
#endif
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			//
			// ОБЩИЕ ДАННЫЕ
			//
			/// <summary>
			/// Логическая группа которой принадлежит перо
			/// </summary>
			[DisplayName("Группа")]
			[Description("Логическая группа которой принадлежит перо")]
			[Category(XInspectorGroupDesc.ID)]
			[Display(GroupName = XInspectorGroupDesc.ID, Order = 1)]
			public String Group
			{
				get { return (mGroup); }
				set
				{
					mGroup = value;

					// 1) Информируем об изменении
					NotifyPropertyChanged(PropertyArgsGroup);

					// 2) Обновляем
					RaiseGroupChanged();
				}
			}

			/// <summary>
			/// Тип сущности модуля чертежной графики
			/// </summary>
			[DisplayName("Тип объекта")]
			[Description("Тип сущности модуля чертежной графики")]
			[Category(XInspectorGroupDesc.ID)]
			[Display(GroupName = XInspectorGroupDesc.ID, Order = 3)]
			public override TCadEntityType EntityType
			{
				get { return (TCadEntityType.Pen); }
			}

			//
			// ОСНОВНЫЕ ПАРАМЕТРЫ
			//
			/// <summary>
			/// Кисть
			/// </summary>
			[DisplayName("Кисть")]
			[Description("Кисть для определения отрисовки пера")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 0)]
			public CCadBrush Brush
			{
				get { return (mBrush); }
				set
				{
					if (mBrush != value)
					{
						mBrush = value;

						// 1) Информируем об изменении
						NotifyPropertyChanged(PropertyArgsBrush);

						// 2) Обновляем
						RaiseBrushChanged();
					}
				}
			}

			/// <summary>
			/// Толщина
			/// </summary>
			[DisplayName("Толщина")]
			[Description("Толщина пера")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 1)]
			public Single Thickness
			{
				get { return (mThickness); }
				set
				{
					if (mThickness != value)
					{
						mThickness = value;

						// 1) Информируем об изменении
						NotifyPropertyChanged(PropertyArgsThickness);

						// 2) Обновляем
						RaiseThicknessChanged();
					}
				}
			}

			/// <summary>
			/// Стиль
			/// </summary>
			[DisplayName("Стиль")]
			[Description("Стиль пера")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 2)]
			public CCadPenStyle PenStyle
			{
				get { return (mPenStyle); }
				set
				{
					if (mPenStyle != value)
					{
						mPenStyle = value;

						// 1) Информируем об изменении
						NotifyPropertyChanged(PropertyArgsPenStyle);

						// 2) Обновляем
						RaisePenStyleChanged();
					}
				}
			}

#if USE_WINDOWS
			/// <summary>
			/// Перо WPF
			/// </summary>
			[Browsable(false)]
			public System.Windows.Media.Pen WindowsPen
			{
				get { return (mWindowsPen); }
			}
#endif
#if USE_GDI
			/// <summary>
			/// Перо System.Drawing
			/// </summary>
			[Browsable(false)]
			public System.Drawing.Pen DrawingPen
			{
				get { return (mDrawingPen); }
			}
#endif
			//
			// ПОДДЕРЖКА ИНСПЕКТОРА СВОЙСТВ
			//
			/// <summary>
			/// Отображаемое имя типа в инспекторе свойств
			/// </summary>
			[Browsable(false)]
			public String InspectorTypeName
			{
				get { return ("ПЕРО"); }
			}

			/// <summary>
			/// Отображаемое имя объекта в инспекторе свойств
			/// </summary>
			[Browsable(false)]
			public String InspectorObjectName
			{
				get
				{
					if (String.IsNullOrEmpty(mName))
					{
						return ("<Без имени>");
					}
					else
					{
						return (mName);
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
			public CCadPen()
				: this("Новое перо")
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="name">Имя пера</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadPen(String name)
				: base(name)
			{
				mBrush = XCadBrushManager.DefaultBrush;
				mThickness = 1.0f;
				mPenStyle = XCadPenStyleManager.Solid;
#if USE_WINDOWS
				mWindowsPen = new System.Windows.Media.Pen(System.Windows.Media.Brushes.Black, mThickness);
#endif
#if USE_GDI
				mDrawingPen = new System.Drawing.Pen(System.Drawing.Brushes.Black, mThickness);
#endif
			}
			#endregion

			#region ======================================= СИСТЕМНЫЕ МЕТОДЫ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Сравнение перьев для упорядочивания
			/// </summary>
			/// <param name="other">Сравниваемое перо</param>
			/// <returns>Статус сравнения перьев</returns>
			//---------------------------------------------------------------------------------------------------------
			public Int32 CompareTo(CCadPen other)
			{
				return (XCadDrawing.DefaultComprare(this, other));
			}
			#endregion

			#region ======================================= СЛУЖЕБНЫЕ МЕТОДЫ СОБЫТИЙ ==================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение имени пера.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected override void RaiseNameChanged()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение логической группы пера.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			private void RaiseGroupChanged()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение кисти пера.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			private void RaiseBrushChanged()
			{
#if USE_WINDOWS
				mWindowsPen.Brush = mBrush.WindowsBrush;

				// 2) Информируем об изменении
				NotifyPropertyChanged(PropertyArgsWindowsPen);
#endif
#if USE_GDI
				mDrawingPen.Brush = mBrush.DrawingBrush;
#endif
#if USE_SHARPDX
				// 3) Обновляем ресурс Direct2D
				UpdateDirect2DResource();
#endif
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение толщины пера.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			private void RaiseThicknessChanged()
			{
#if USE_WINDOWS
				mWindowsPen.Thickness = mThickness;

				// 2) Информируем об изменении
				NotifyPropertyChanged(PropertyArgsWindowsPen);
#endif
#if USE_GDI
				mDrawingPen.Width = mThickness;
#endif
#if USE_SHARPDX
				// 3) Обновляем ресурс Direct2D
				UpdateDirect2DResource();
#endif
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение стиля пера.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			private void RaisePenStyleChanged()
			{
#if USE_WINDOWS
				mWindowsPen.StartLineCap = mPenStyle.WindowsPen.StartLineCap;
				mWindowsPen.EndLineCap = mPenStyle.WindowsPen.EndLineCap;
				mWindowsPen.LineJoin = mPenStyle.WindowsPen.LineJoin;
				mWindowsPen.MiterLimit = mPenStyle.WindowsPen.MiterLimit;
				mWindowsPen.DashCap = mPenStyle.WindowsPen.DashCap;
				mWindowsPen.DashStyle = mPenStyle.WindowsPen.DashStyle;

				// 2) Информируем об изменении
				NotifyPropertyChanged(PropertyArgsWindowsPen);
#endif
#if USE_GDI
				mDrawingPen.StartCap = mPenStyle.DrawingPen.StartCap;
				mDrawingPen.EndCap = mPenStyle.DrawingPen.EndCap;
				mDrawingPen.LineJoin = mPenStyle.DrawingPen.LineJoin;
				mDrawingPen.MiterLimit = mPenStyle.DrawingPen.MiterLimit;
				mDrawingPen.DashCap = mPenStyle.DrawingPen.DashCap;
				mDrawingPen.DashOffset = mPenStyle.DrawingPen.DashOffset;
				//mDrawingPen.DashPattern = mPenStyle.DrawingPen.DashPattern;
#endif
#if USE_SHARPDX
				// 3) Обновляем ресурс Direct2D
				UpdateDirect2DResource();
#endif
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Общие обновление пера
			/// </summary>
			/// <remarks>
			/// Применяется когда надо обновить внутренние ресурсы пера по напрямую заданным параметрам пера, 
			/// минуя механизм свойств, например при загрузке или создании
			/// </remarks>
			//---------------------------------------------------------------------------------------------------------
			public void Update()
			{
#if USE_WINDOWS
				UpdateWindowsResource();
#endif
#if USE_GDI
				UpdateDrawingResource();
#endif
#if USE_SHARPDX
				//UpdateDirect2DResource(true);
#endif
			}

#if USE_WINDOWS
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление ресурса WPF
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void UpdateWindowsResource()
			{
				mWindowsPen.Brush = mBrush.WindowsBrush;
				mWindowsPen.Thickness = mThickness;
				mWindowsPen.StartLineCap = mPenStyle.WindowsPen.StartLineCap;
				mWindowsPen.EndLineCap = mPenStyle.WindowsPen.EndLineCap;
				mWindowsPen.LineJoin = mPenStyle.WindowsPen.LineJoin;
				mWindowsPen.MiterLimit = mPenStyle.WindowsPen.MiterLimit;
				mWindowsPen.DashCap = mPenStyle.WindowsPen.DashCap;
				mWindowsPen.DashStyle = mPenStyle.WindowsPen.DashStyle;
			}
#endif
#if USE_GDI
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление ресурса System.Drawing
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void UpdateDrawingResource()
			{
				mDrawingPen.Brush = mBrush.DrawingBrush;
				mDrawingPen.Width = mThickness;
				mDrawingPen.StartCap = mPenStyle.DrawingPen.StartCap;
				mDrawingPen.EndCap = mPenStyle.DrawingPen.EndCap;
				mDrawingPen.LineJoin = mPenStyle.DrawingPen.LineJoin;
				mDrawingPen.MiterLimit = mPenStyle.DrawingPen.MiterLimit;
				mDrawingPen.DashCap = mPenStyle.DrawingPen.DashCap;
				mDrawingPen.DashOffset = mPenStyle.DrawingPen.DashOffset;
				//mDrawingPen.DashPattern = mPenStyle.DrawingPen.DashPattern;
			}
#endif
#if USE_SHARPDX
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление ресурса Direct2D
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void UpdateDirect2DResource()
			{
				if (XDirect2DManager.D2DFactory != null)
				{
					XDisposer.SafeDispose(ref mD2DStrokeStyle);

					SharpDX.Direct2D1.StrokeStyleProperties ssp = new SharpDX.Direct2D1.StrokeStyleProperties();
					ssp.StartCap = (SharpDX.Direct2D1.CapStyle)mPenStyle.StartLineCap;
					ssp.EndCap = (SharpDX.Direct2D1.CapStyle)mPenStyle.EndLineCap;
					ssp.LineJoin = (SharpDX.Direct2D1.LineJoin)mPenStyle.LineJoin;
					ssp.MiterLimit = mPenStyle.MiterLimit;

					if (mPenStyle.DashPattern.Count == 0)
					{
						ssp.DashCap = (SharpDX.Direct2D1.CapStyle)mPenStyle.DashCap;
						ssp.DashOffset = mPenStyle.DashOffset;
						ssp.DashStyle = SharpDX.Direct2D1.DashStyle.Solid;

						mD2DStrokeStyle = new SharpDX.Direct2D1.StrokeStyle(XDirect2DManager.D2DFactory, ssp);
					}
					else
					{
						ssp.DashCap = (SharpDX.Direct2D1.CapStyle)mPenStyle.DashCap;
						ssp.DashOffset = mPenStyle.DashOffset;
						ssp.DashStyle = SharpDX.Direct2D1.DashStyle.Custom;

						mD2DStrokeStyle = new SharpDX.Direct2D1.StrokeStyle(XDirect2DManager.D2DFactory, ssp,
							mPenStyle.DashPattern.ToArray());
					}
				}
			}
#endif

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Копирования пера
			/// </summary>
			/// <returns>Копия пера со всеми параметрами и данными</returns>
			//---------------------------------------------------------------------------------------------------------
			public CCadPen Duplicate()
			{
				CCadPen obj = (CCadPen)MemberwiseClone();

#if USE_WINDOWS
				obj.mWindowsPen = mWindowsPen.CloneCurrentValue();
#endif
#if USE_GDI
				obj.mDrawingPen = mDrawingPen.Clone() as System.Drawing.Pen;
#endif
#if USE_SHARPDX
				// Обновляем ресурс Direct2D
				obj.UpdateDirect2DResource();
#endif
				return (obj);
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
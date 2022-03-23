//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Ресурсы для отображения графического объекта
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADVisualPenStyle.cs
*		Стиль пера отображения контура графического объекта.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 27.03.2022
//=====================================================================================================================
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Xml;
using System.Linq;
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
		/// Форма фигуры расположенной в конце линии или сегмента
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		[TypeConverter(typeof(EnumToStringConverter<TCadStrokeLineCap>))]
		public enum TCadStrokeLineCap
		{
			/// <summary>
			/// Наконечник не распространяется за последнюю точку линии. Сравним с отсутствием наконечника
			/// </summary>
			Flat,

			/// <summary>
			/// Наконечник имеет высоту, равную толщине линии и длину, равную половине толщины линии
			/// </summary>
			Square,

			/// <summary>
			/// Полукруг, диаметр которого равен толщине линии
			/// </summary>
			Round,

			/// <summary>
			/// Равнобедренный прямоугольный треугольник с длиной основания, равной толщине линии
			/// </summary>
			Triangle
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Форма фигуры, соединяющую две линии или два сегмента
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		[TypeConverter(typeof(EnumToStringConverter<TCadStrokeLineCap>))]
		public enum TCadStrokeLineJoin
		{
			/// <summary>
			/// Обычные угловые вершины
			/// </summary>
			Miter,

			/// <summary>
			/// Скошенные вершины
			/// </summary>
			Bevel,

			/// <summary>
			/// Закругленные вершины
			/// </summary>
			Round
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Стиль пера отображения контура графического объекта
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public sealed class CCadPenStyle : CCadEntity, IComparable<CCadPenStyle>, ILotusSupportViewInspector
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			private static PropertyChangedEventArgs PropertyArgsGroup = new PropertyChangedEventArgs(nameof(Group));
			private static PropertyChangedEventArgs PropertyArgsStartLineCap = new PropertyChangedEventArgs(nameof(StartLineCap));
			private static PropertyChangedEventArgs PropertyArgsEndLineCap = new PropertyChangedEventArgs(nameof(EndLineCap));
			private static PropertyChangedEventArgs PropertyArgsLineJoin = new PropertyChangedEventArgs(nameof(LineJoin));
			private static PropertyChangedEventArgs PropertyArgsMiterLimit = new PropertyChangedEventArgs(nameof(MiterLimit));
			private static PropertyChangedEventArgs PropertyArgsDashCap = new PropertyChangedEventArgs(nameof(DashCap));
			private static PropertyChangedEventArgs PropertyArgsDashOffset = new PropertyChangedEventArgs(nameof(DashOffset));
			private static PropertyChangedEventArgs PropertyArgsDashPattern = new PropertyChangedEventArgs(nameof(DashPattern));
#if USE_WINDOWS
			private static PropertyChangedEventArgs PropertyArgsWindowsPen = new PropertyChangedEventArgs(nameof(WindowsPen));
#endif

			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			// Общие данные
			internal String mGroup;

			// Основные параметры
			internal TCadStrokeLineCap mStartLineCap;
			internal TCadStrokeLineCap mEndLineCap;
			internal TCadStrokeLineJoin mLineJoin;
			internal Single mMiterLimit;
			internal TCadStrokeLineCap mDashCap;
			internal Single mDashOffset;
			internal List<Single> mDashPattern;

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
			/// Логическая группа которой принадлежит стиль пера
			/// </summary>
			[DisplayName("Группа")]
			[Description("Логическая группа которой принадлежит стиль пера")]
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
				get { return (TCadEntityType.PenStyle); }
			}

			//
			// ОСНОВНЫЕ ПАРАМЕТРЫ
			//
			/// <summary>
			/// Форма фигуры края в начале линии
			/// </summary>
			[DisplayName("Фигура в начале")]
			[Description("Форма фигуры края в начале линии")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 0)]
			public TCadStrokeLineCap StartLineCap
			{
				get { return (mStartLineCap); }
				set
				{
					if(mStartLineCap != value)
					{
						mStartLineCap = value;

						// 1) Информируем об изменении
						NotifyPropertyChanged(PropertyArgsStartLineCap);

						// 2) Обновляем
						RaiseLineParamChanged();
					}
				}
			}

			/// <summary>
			/// Форма фигуры края в конце линии
			/// </summary>
			[DisplayName("Фигура в конце")]
			[Description("Форма фигуры края в конце линии")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 1)]
			public TCadStrokeLineCap EndLineCap
			{
				get { return (mEndLineCap); }
				set
				{
					if (mEndLineCap != value)
					{
						mEndLineCap = value;

						// 1) Информируем об изменении
						NotifyPropertyChanged(PropertyArgsEndLineCap);

						// 2) Обновляем
						RaiseLineParamChanged();
					}
				}
			}

			/// <summary>
			/// Тип фигуры соединяющая две линии или два сегмента
			/// </summary>
			[DisplayName("Тип фигуры")]
			[Description("Фигура соединяющая две линии или два сегмента")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 2)]
			public TCadStrokeLineJoin LineJoin
			{
				get { return (mLineJoin); }
				set
				{
					if (mLineJoin != value)
					{
						mLineJoin = value;

						// 1) Информируем об изменении
						NotifyPropertyChanged(PropertyArgsLineJoin);

						// 2) Обновляем
						RaiseLineParamChanged();
					}
				}
			}

			/// <summary>
			/// Предельное значение отношения длины уголка к половине толщине контура
			/// </summary>
			[DisplayName("Отношение")]
			[Description("Предельное значение отношения длины уголка к половине толщине контура")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 3)]
			public Single MiterLimit
			{
				get { return (mMiterLimit); }
				set
				{
					if (mMiterLimit != value)
					{
						mMiterLimit = value;

						// 1) Информируем об изменении
						NotifyPropertyChanged(PropertyArgsMiterLimit);

						// 2) Обновляем
						RaiseLineParamChanged();
					}
				}
			}

			/// <summary>
			/// Контур штриховой линии
			/// </summary>
			[DisplayName("Контур штриха")]
			[Description("Контур штриховой линии")]
			[Category(XInspectorGroupDesc.Pattern)]
			[Display(GroupName = XInspectorGroupDesc.Pattern, Order = 0)]
			public TCadStrokeLineCap DashCap
			{
				get { return (mDashCap); }
				set
				{
					if (mDashCap != value)
					{
						mDashCap = value;

						// 1) Информируем об изменении
						NotifyPropertyChanged(PropertyArgsDashCap);

						// 2) Обновляем
						RaiseDashParamChanged();
					}
				}
			}

			/// <summary>
			/// Место в последовательности штрихов с которого начнется обводка
			/// </summary>
			[DisplayName("Смещение штриха")]
			[Description("Место в последовательности штрихов с которого начнется обводка")]
			[Category(XInspectorGroupDesc.Pattern)]
			[Display(GroupName = XInspectorGroupDesc.Pattern, Order = 1)]
			public Single DashOffset
			{
				get { return (mDashOffset); }
				set
				{
					if (mDashOffset != value)
					{
						mDashOffset = value;

						// 1) Информируем об изменении
						NotifyPropertyChanged(PropertyArgsDashOffset);

						// 2) Обновляем
						RaiseDashParamChanged();
					}
				}
			}

			/// <summary>
			/// Последовательность штрихов
			/// </summary>
			[DisplayName("Паттерн заполнения")]
			[Description("Последовательность штрихов")]
			[Category(XInspectorGroupDesc.Pattern)]
			[Display(GroupName = XInspectorGroupDesc.Pattern, Order = 2)]
			public List<Single> DashPattern
			{
				get { return (mDashPattern); }
				set
				{
					mDashPattern = value;

					// 1) Информируем об изменении
					NotifyPropertyChanged(PropertyArgsDashPattern);

					// 2) Обновляем
					RaiseDashParamChanged();
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
#if USE_SHARPDX
			/// <summary>
			/// стиль пера Direct2D
			/// </summary>
			[Browsable(false)]
			public SharpDX.Direct2D1.StrokeStyle D2DStrokeStyle
			{
				get { return (mD2DStrokeStyle); }
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
				get { return ("СТИЛЬ ПЕРА"); }
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
			public CCadPenStyle()
				:this("Новый стиль")
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="name">Имя стиля пера</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadPenStyle(String name)
				: base(name)
			{
				mDashPattern = new List<Single>();
#if USE_WINDOWS
				mWindowsPen = new System.Windows.Media.Pen(System.Windows.Media.Brushes.Black, 2);
#endif
#if USE_GDI
				mDrawingPen = new System.Drawing.Pen(System.Drawing.Color.Black, 2);
#endif
			}
			#endregion

			#region ======================================= СИСТЕМНЫЕ МЕТОДЫ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Сравнение стилей контуров для упорядочивания
			/// </summary>
			/// <param name="other">Сравниваемый стиль пера</param>
			/// <returns>Статус сравнения стилей</returns>
			//---------------------------------------------------------------------------------------------------------
			public Int32 CompareTo(CCadPenStyle other)
			{
				return (XCadDrawing.DefaultComprare(this, other));
			}
			#endregion

			#region ======================================= СЛУЖЕБНЫЕ МЕТОДЫ СОБЫТИЙ ==================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение имени стиля пера.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected override void RaiseNameChanged()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение логической группы стиля пера.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			private void RaiseGroupChanged()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение параметров фигур стиля пера.
			/// Метод автоматически вызывается после установки соответствующих свойств
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			private void RaiseLineParamChanged()
			{
#if USE_WINDOWS
				mWindowsPen.StartLineCap = (System.Windows.Media.PenLineCap)mStartLineCap;
				mWindowsPen.EndLineCap = (System.Windows.Media.PenLineCap)mEndLineCap;
				mWindowsPen.LineJoin = (System.Windows.Media.PenLineJoin)mLineJoin;
				mWindowsPen.MiterLimit = mMiterLimit;

				// 2) Информируем об изменении
				NotifyPropertyChanged(PropertyArgsWindowsPen);
#endif
#if USE_GDI
				mDrawingPen.StartCap = (System.Drawing.Drawing2D.LineCap)mStartLineCap;
				mDrawingPen.EndCap = (System.Drawing.Drawing2D.LineCap)mEndLineCap;
				mDrawingPen.LineJoin = (System.Drawing.Drawing2D.LineJoin)mLineJoin;
				mDrawingPen.MiterLimit = mMiterLimit;
#endif
#if USE_SHARPDX
				// 3) Обновляем ресурс Direct2D
				UpdateDirect2DResource();
#endif
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение параметров штриха стиля пера.
			/// Метод автоматически вызывается после установки соответствующих свойств
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			private void RaiseDashParamChanged()
			{
#if USE_WINDOWS
				mWindowsPen.DashCap = (System.Windows.Media.PenLineCap)mDashCap;
				//mWindowsPen.DashStyle = new System.Windows.Media.DashStyle(mDashPattern.ConvertToDoubles(), mDashOffset);

				// 2) Информируем об изменении
				NotifyPropertyChanged(PropertyArgsWindowsPen);
#endif
#if USE_GDI
				mDrawingPen.DashCap = (System.Drawing.Drawing2D.DashCap)mDashCap;
				mDrawingPen.DashOffset = mDashOffset;
				mDrawingPen.DashPattern = mDashPattern.ToArray();
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
			/// Общие обновление стиля пера
			/// </summary>
			/// <remarks>
			/// Применяется когда надо обновить внутренние ресурсы стиля пера по напрямую заданным параметрам стиля пера, 
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
				mWindowsPen.StartLineCap = (System.Windows.Media.PenLineCap)mStartLineCap;
				mWindowsPen.EndLineCap = (System.Windows.Media.PenLineCap)mEndLineCap;
				mWindowsPen.LineJoin = (System.Windows.Media.PenLineJoin)mLineJoin;
				mWindowsPen.MiterLimit = mMiterLimit;

				if (mDashPattern.Count == 0)
				{
					mWindowsPen.DashStyle = System.Windows.Media.DashStyles.Solid;
				}
				else
				{
					mWindowsPen.DashCap = (System.Windows.Media.PenLineCap)mDashCap;
					mWindowsPen.DashStyle = new System.Windows.Media.DashStyle(XConverterCollection.ToDouble(mDashPattern), mDashOffset);
				}
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
				mDrawingPen.StartCap = (System.Drawing.Drawing2D.LineCap)mStartLineCap;
				mDrawingPen.EndCap = (System.Drawing.Drawing2D.LineCap)mEndLineCap;
				mDrawingPen.LineJoin = (System.Drawing.Drawing2D.LineJoin)mLineJoin;
				mDrawingPen.MiterLimit = mMiterLimit;

				if (mDashPattern.Count == 0)
				{
					mDrawingPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
				}
				else
				{
					mDrawingPen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDotDot;
					//mDrawingPen.DashCap = (System.Drawing.Drawing2D.DashCap)mDashCap;
					//mDrawingPen.DashOffset = mDashOffset;
					//mDrawingPen.DashPattern = mDashPattern.ToArray();
				}
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
					ssp.StartCap = (SharpDX.Direct2D1.CapStyle)mStartLineCap;
					ssp.EndCap = (SharpDX.Direct2D1.CapStyle)mEndLineCap;
					ssp.LineJoin = (SharpDX.Direct2D1.LineJoin)mLineJoin;
					ssp.MiterLimit = mMiterLimit;

					if (mDashPattern.Count == 0)
					{
						ssp.DashCap = (SharpDX.Direct2D1.CapStyle)mDashCap;
						ssp.DashOffset = mDashOffset;
						ssp.DashStyle = SharpDX.Direct2D1.DashStyle.Solid;

						mD2DStrokeStyle = new SharpDX.Direct2D1.StrokeStyle(XDirect2DManager.D2DFactory, ssp);
					}
					else
					{
						ssp.DashCap = (SharpDX.Direct2D1.CapStyle)mDashCap;
						ssp.DashOffset = mDashOffset;
						ssp.DashStyle = SharpDX.Direct2D1.DashStyle.Custom;

						mD2DStrokeStyle = new SharpDX.Direct2D1.StrokeStyle(XDirect2DManager.D2DFactory, ssp, mDashPattern.ToArray());
					}
				}
			}
#endif

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Копирования стиля пера
			/// </summary>
			/// <returns>Копия стиля пера со всеми параметрами и данными</returns>
			//---------------------------------------------------------------------------------------------------------
			public CCadPenStyle Duplicate()
			{
				CCadPenStyle obj = (CCadPenStyle)MemberwiseClone();

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
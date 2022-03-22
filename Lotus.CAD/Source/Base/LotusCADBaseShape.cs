//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Общая подсистема
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADBaseShape.cs
*		Базовая графическая фигура.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.01.2022
//=====================================================================================================================
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
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
		//! \addtogroup CadDrawing
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Интерфейс базовой графической фигуры с базовым взаимодействием
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public interface ICadShape : ICadElement, ICadStrokeSupport, ICadFillSupport, ICadTransform, 
			ICadControlElementPointer
		{
			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Доступность графической фигуры для редактирования
			/// </summary>
			Boolean IsEnabled { get; set; }

			/// <summary>
			/// Статус предварительного выбора графической фигуры
			/// </summary>
			Boolean IsPreSelect { get; set; }

			/// <summary>
			/// Статус выбора графической фигуры
			/// </summary>
			Boolean IsSelect { get; set; }

			//
			// ПАРАМЕТРЫ РУЧЕК
			//
			/// <summary>
			/// Количество доступных ручек для управления
			/// </summary>
			Int32 HandleCount { get; }

			/// <summary>
			/// Индекс текущей выбранной ручки
			/// </summary>
			Int32 HandleIndex { get; }

			/// <summary>
			/// Прямоугольник текущей выбранной ручки
			/// </summary>
			Rect2Df HandleRect { get; }
			#endregion

			#region ======================================= МЕТОДЫ РАБОТЫ С РУЧКАМИ ===================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка прямоугольников ручек
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			void SetHandleRects();

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка соответствующего курсора на ручкой
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			void SetHandleCursor();
			#endregion

			#region ======================================= ОБРАБОТКА СОБЫТИЙ КОНТЕКСТНОГО МЕНЮ =======================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Открытие контекстного меню
			/// </summary>
			/// <param name="point">Точка открытия контекстного меню в пространстве канвы</param>
			//---------------------------------------------------------------------------------------------------------
			void OnOpenContextMenu(ref Vector2Df point);

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Закрытие контекстного меню
			/// </summary>
			/// <param name="point">Точка закрытия контекстного меню в пространстве канвы</param>
			//---------------------------------------------------------------------------------------------------------
			void OnClosedContextMenu(ref Vector2Df point);

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Выполнения команды контекстного меню
			/// </summary>
			/// <param name="command_name">Имя команды</param>
			/// <param name="context">Контекст данных</param>
			//---------------------------------------------------------------------------------------------------------
			void OnCommandContextMenu(String command_name, Object context);
			#endregion

			#region ======================================= ОБРАБОТКА СОБЫТИЙ ПАРАМЕТРОВ ОТОБРАЖЕНИЯ ==================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение масштаба представления текущего элемента
			/// </summary>
			/// <param name="scale">Масштаб представления</param>
			//---------------------------------------------------------------------------------------------------------
			void OnScaleChanged(Double scale);
			#endregion
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Базовая графическая фигура с базовым взаимодействием
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		[DataContract]
		public class CCadShape : CCadElement, ICadShape, IComparable<ICadShape>, IComparable<CCadShape>,
			ILotusViewItemOwner, ILotusViewSelected, ILotusViewPresented, ILotusSupportEditInspector
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			// Основные параметры
			protected static PropertyChangedEventArgs PropertyArgsIsEnabled = new PropertyChangedEventArgs(nameof(IsEnabled));
			protected static PropertyChangedEventArgs PropertyArgsIsSelect = new PropertyChangedEventArgs(nameof(IsSelect));

			// Контур графической фигуры
			protected static PropertyChangedEventArgs PropertyArgsStrokeIsEnabled = new PropertyChangedEventArgs(nameof(StrokeIsEnabled));
			protected static PropertyChangedEventArgs PropertyArgsStrokePen = new PropertyChangedEventArgs(nameof(StrokePen));
			protected static PropertyChangedEventArgs PropertyArgsStrokePenId = new PropertyChangedEventArgs(nameof(StrokePenId));
			protected static PropertyChangedEventArgs PropertyArgsStrokePenName = new PropertyChangedEventArgs(nameof(StrokePenName));
			protected static PropertyChangedEventArgs PropertyArgsStrokeThickness = new PropertyChangedEventArgs(nameof(StrokeThickness));

			// Заливка графической фигуры
			protected static PropertyChangedEventArgs PropertyArgsFillIsEnabled = new PropertyChangedEventArgs(nameof(FillIsEnabled));
			protected static PropertyChangedEventArgs PropertyArgsFillBrush = new PropertyChangedEventArgs(nameof(FillBrush));
			protected static PropertyChangedEventArgs PropertyArgsFillBrushId = new PropertyChangedEventArgs(nameof(FillBrushId));
			protected static PropertyChangedEventArgs PropertyArgsFillBrushName = new PropertyChangedEventArgs(nameof(FillBrushName));
			protected static PropertyChangedEventArgs PropertyArgsFillOpacity = new PropertyChangedEventArgs(nameof(FillOpacity));

			/// <summary>
			/// Описание свойств
			/// </summary>
			public readonly static CPropertyDesc[] CadShapePropertiesDesc = new CPropertyDesc[]
			{
				// Идентификация
				CPropertyDesc.OverrideDisplayNameAndDescription<CCadShape>(nameof(Name), "Наименование", "Наименование фигуры"),
				CPropertyDesc.OverrideCategory<CCadShape>(nameof(Name), XInspectorGroupDesc.ID),
				CPropertyDesc.OverrideOrder<CCadShape>(nameof(Name), 1),
				CPropertyDesc.OverrideDisplayNameAndDescription<CCadShape>(nameof(Id), "Идентификатор", "Локальный идентификатор фигуры"),
				CPropertyDesc.OverrideCategory<CCadShape>(nameof(Id), XInspectorGroupDesc.ID),
				CPropertyDesc.OverrideOrder<CCadShape>(nameof(Id), 100),
			};
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			// Основные параметры
			internal Boolean mIsEnabled = true;
			internal Boolean mIsSelect;
			internal Boolean mIsPreSelect;

			// Контур графической фигуры
			internal Boolean mStrokeIsEnabled = true;
			internal CCadPen mStrokePen;
			internal Int64 mStrokeId;
			internal Single mStrokeThickness;

			// Заливка графической фигуры
			internal Boolean mFillIsEnabled = true;
			internal CCadBrush mFillBrush;
			internal Int64 mFillId;
			internal Int32 mFillOpacity;

			// Данные для работы с управляющими элементами
			internal Int32 mHandleIndex;
			internal Int32 mHandleSubIndex;
			internal List<Rect2Df> mHandleRects;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			//
			// ОСНОВНЫЕ ПАРАМЕТРЫ
			//
			/// <summary>
			/// Доступность графической фигуры для редактирования
			/// </summary>
			[DisplayName("Доступность")]
			[Description("Доступность графической фигуры для редактирования")]
			[Category(XInspectorGroupDesc.Graphics)]
			[Display(GroupName = XInspectorGroupDesc.Graphics, Order = 3)]
			[DataMember]
			public Boolean IsEnabled
			{
				get { return (mIsEnabled); }
				set
				{
					if (mIsEnabled != value)
					{
						SavePropertyToMemory(nameof(IsEnabled));

						mIsEnabled = value;
						NotifyPropertyChanged(PropertyArgsIsEnabled);
						RaiseIsEnabledChanged();
					}
				}
			}

			/// <summary>
			/// Статус выбора графической фигуры
			/// </summary>
			[Browsable(false)]
			public Boolean IsSelect
			{
				get { return (mIsSelect); }
				set
				{
					if (mIsSelect != value)
					{
						SavePropertyToMemory(nameof(IsSelect));

						mIsSelect = value;
						NotifyPropertyChanged(PropertyArgsIsSelect);
						RaiseIsSelectedChanged();

						if(OwnerViewItem != null && OwnerViewItem.IsSelected != mIsSelect)
						{
							OwnerViewItem.IsSelected = mIsSelect;
						}
					}
				}
			}

			/// <summary>
			/// Статус предварительного выбора графической фигуры
			/// </summary>
			[Browsable(false)]
			public Boolean IsPreSelect
			{
				get { return (mIsPreSelect); }
				set
				{
					if (mIsPreSelect != value)
					{
						mIsPreSelect = value;
					}
				}
			}

			//
			// ПАРАМЕТРЫ КОНТУРА
			//
			/// <summary>
			/// Отображение контура
			/// </summary>
			/// <remarks>
			/// Свойство определяет надо ли рисовать контур графического объекта
			/// </remarks>
			[DisplayName("Включить")]
			[Description("Свойство определяет надо ли рисовать контур графического объекта")]
			[Category(XInspectorGroupDesc.Stroke)]
			[LotusCategoryOrder(3)]
			[Display(GroupName = XInspectorGroupDesc.Stroke, Order = 0)]
			[DataMember]
			public Boolean StrokeIsEnabled
			{
				get { return (mStrokeIsEnabled); }
				set
				{
					if (mStrokeIsEnabled != value)
					{
						SavePropertyToMemory(nameof(StrokeIsEnabled));

						mStrokeIsEnabled = value;
						NotifyPropertyChanged(PropertyArgsStrokeIsEnabled);
						RaiseStrokeChanged();
					}
				}
			}

			/// <summary>
			/// Идентификатор пера для отображения контура графической фигуры
			/// </summary>
			[Browsable(false)]
			[DataMember]
			public Int64 StrokePenId
			{
				get { return (mStrokeId); }
				set
				{
					if (mStrokeId != value)
					{
						mStrokePen = XCadPenManager.GetFromId(value);
						mStrokeId = value;

						NotifyPropertyChanged(PropertyArgsStrokePen);
						NotifyPropertyChanged(PropertyArgsStrokePenId);
						NotifyPropertyChanged(PropertyArgsStrokePenName);

						RaiseStrokeChanged();
					}
				}
			}

			/// <summary>
			/// Название кисти пера отображения контура графической фигуры
			/// </summary>
			[Browsable(false)]
			public String StrokePenName
			{
				get { return (mStrokePen.Name); }
				set
				{
					if (mStrokePen.Name != value)
					{
						mStrokePen = XCadPenManager.GetFromName(value);
						mStrokeId = mStrokePen.Id;

						NotifyPropertyChanged(PropertyArgsStrokePen);
						NotifyPropertyChanged(PropertyArgsStrokePenId);
						NotifyPropertyChanged(PropertyArgsStrokePenName);

						RaiseStrokeChanged();
					}
				}
			}

			/// <summary>
			/// Перо для отображения контура графической фигуры
			/// </summary>
			[DisplayName("Перо")]
			[Description("Перо для отображения контура графической фигуры")]
			[Category(XInspectorGroupDesc.Stroke)]
			[Display(GroupName = XInspectorGroupDesc.Stroke, Order = 1)]
#if USE_WINDOWS
			[LotusInspectorTypeEditor(typeof(LotusEditorSelectorPen))]
#endif
			public CCadPen StrokePen
			{
				get { return (mStrokePen); }
				set
				{
					if (mStrokePen != value)
					{
						SavePropertyToMemory(nameof(StrokePen));

						mStrokePen = value;
						mStrokeId = mStrokePen.Id;

						NotifyPropertyChanged(PropertyArgsStrokePen);
						NotifyPropertyChanged(PropertyArgsStrokePenId);
						NotifyPropertyChanged(PropertyArgsStrokePenName);

						RaiseStrokeChanged();
					}
				}
			}

			/// <summary>
			/// Толщины контура графической фигуры
			/// </summary>
			[DisplayName("Толщина")]
			[Description("Толщины контура графической фигуры")]
			[Category(XInspectorGroupDesc.Stroke)]
			[Display(GroupName = XInspectorGroupDesc.Stroke, Order = 2)]
			[DataMember]
			public Single StrokeThickness
			{
				get { return (mStrokeThickness); }
				set
				{
					if (mStrokeThickness != value)
					{
						SavePropertyToMemory(nameof(StrokeThickness));

						mStrokeThickness = value;
						NotifyPropertyChanged(PropertyArgsStrokeThickness);
					}
				}
			}

			//
			// ПАРАМЕТРЫ ЗАЛИВКИ
			//
			/// <summary>
			/// Отображение заливки
			/// </summary>
			/// <remarks>
			/// Свойство определяет надо ли заливать графический объект
			/// </remarks>
			[DisplayName("Включить")]
			[Description("Использовать кисть для заливки графической фигуры")]
			[Category(XInspectorGroupDesc.Fill)]
			[LotusCategoryOrder(4)]
			[Display(GroupName = XInspectorGroupDesc.Fill, Order = 0)]
			[DataMember]
			public Boolean FillIsEnabled
			{
				get { return (mFillIsEnabled); }
				set
				{
					SavePropertyToMemory(nameof(FillIsEnabled));

					mFillIsEnabled = value;
					NotifyPropertyChanged(PropertyArgsFillIsEnabled);
				}
			}

			/// <summary>
			/// Идентификатор кисти для заполнения замкнутой области графической фигуры
			/// </summary>
			[Browsable(false)]
			[DataMember]
			public Int64 FillBrushId
			{
				get { return (mFillId); }
				set
				{
					if (mFillId != value)
					{
						mFillBrush = XCadBrushManager.GetFromId(value);
						mFillId = value;

						NotifyPropertyChanged(PropertyArgsFillBrush);
						NotifyPropertyChanged(PropertyArgsFillBrushId);
						NotifyPropertyChanged(PropertyArgsFillBrushName);

						RaiseFillChanged();
					}
				}
			}

			/// <summary>
			/// Названия кисти для заполнения замкнутой области графической фигуры
			/// </summary>
			[Browsable(false)]
			public String FillBrushName
			{
				get { return (mFillBrush.Name); }
				set
				{
					if (mFillBrush.Name != value)
					{
						mFillBrush = XCadBrushManager.GetFromName(value);
						mFillId = mFillBrush.Id;

						NotifyPropertyChanged(PropertyArgsFillBrush);
						NotifyPropertyChanged(PropertyArgsFillBrushId);
						NotifyPropertyChanged(PropertyArgsFillBrushName);

						RaiseFillChanged();
					}
				}
			}

			/// <summary>
			/// Кисть для заполнения замкнутой области графической фигуры
			/// </summary>
			[DisplayName("Кисть")]
			[Description("Кисть для заполнения замкнутой области графической фигуры")]
			[Category(XInspectorGroupDesc.Fill)]
			[Display(GroupName = XInspectorGroupDesc.Fill, Order = 1)]
#if USE_WINDOWS
			[LotusInspectorTypeEditor(typeof(LotusEditorSelectorBrush))]
#endif
			public CCadBrush FillBrush
			{
				get { return (mFillBrush); }
				set
				{
					if (mFillBrush != value)
					{
						SavePropertyToMemory(nameof(FillBrush));

						mFillBrush = value;
						mFillId = mFillBrush.Id;

						NotifyPropertyChanged(PropertyArgsFillBrush);
						NotifyPropertyChanged(PropertyArgsFillBrushId);
						NotifyPropertyChanged(PropertyArgsFillBrushName);

						RaiseFillChanged();
					}
				}
			}

			/// <summary>
			/// Прозрачность заливки графической фигуры
			/// </summary>
			[DisplayName("Прозрачность")]
			[Description("Прозрачность заливки графической фигуры")]
			[Category(XInspectorGroupDesc.Fill)]
			[Display(GroupName = XInspectorGroupDesc.Fill, Order = 2)]
			[DataMember]
			public Int32 FillOpacity
			{
				get { return (mFillOpacity); }
				set
				{
					if (mFillOpacity != value)
					{
						SavePropertyToMemory(nameof(FillOpacity));

						mFillOpacity = value;
						NotifyPropertyChanged(PropertyArgsFillOpacity);
					}
				}
			}

			//
			// ПАРАМЕТРЫ РУЧЕК
			//
			/// <summary>
			/// Количество доступных ручек для управления
			/// </summary>
			[Browsable(false)]
			public virtual Int32 HandleCount
			{
				get { return (0); }
			}

			/// <summary>
			/// Индекс текущей выбранной ручки
			/// </summary>
			[Browsable(false)]
			public Int32 HandleIndex
			{
				get { return (mHandleIndex); }
			}

			/// <summary>
			/// Прямоугольник текущей выбранной ручки
			/// </summary>
			[Browsable(false)]
			public Rect2Df HandleRect
			{
				get { return (mHandleRects[mHandleIndex]); }
			}
			#endregion

			#region ======================================= СВОЙСТВА ILotusSupportViewInspector =======================
			/// <summary>
			/// Элемент отображения
			/// </summary>
			public ILotusViewItem OwnerViewItem { get; set; }
			#endregion

			#region ======================================= СВОЙСТВА ILotusSupportViewInspector =======================
			/// <summary>
			/// Отображаемое имя типа в инспекторе свойств
			/// </summary>
			[Browsable(false)]
			public virtual String InspectorTypeName
			{
				get {
					return ("ФИГУРА");
				}
			}

			/// <summary>
			/// Отображаемое имя объекта в инспекторе свойств
			/// </summary>
			[Browsable(false)]
			public virtual String InspectorObjectName
			{
				get {
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
			public CCadShape()
				: this("Фигура")
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="name">Имя графической фигуры</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadShape(String name)
				: base(name)
			{
				// Данные по умолчанию
				mHandleIndex = -1;
				mHandleSubIndex = -1;
				mHandleRects = new List<Rect2Df>();
				mStrokePen = XCadPenManager.DefaultPen;
				mStrokeId = mStrokePen.Id;
				mFillBrush = XCadBrushManager.DefaultBrush;
				mFillId = mFillBrush.Id;
			}
			#endregion

			#region ======================================= СИСТЕМНЫЕ МЕТОДЫ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Сравнение графических фигур для упорядочивания (по Z индексу)
			/// </summary>
			/// <param name="other">Сравниваемая графическая фигура</param>
			/// <returns>Статус сравнения графических фигур</returns>
			//---------------------------------------------------------------------------------------------------------
			public Int32 CompareTo(ICadShape other)
			{
				if (ZIndex > other.ZIndex)
				{
					return (1);
				}
				else
				{
					if (ZIndex < other.ZIndex)
					{
						return (-1);
					}
					else
					{
						return (0);
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Сравнение графических фигур для упорядочивания (по Z индексу)
			/// </summary>
			/// <param name="other">Сравниваемая графическая фигура</param>
			/// <returns>Статус сравнения графических фигур</returns>
			//---------------------------------------------------------------------------------------------------------
			public Int32 CompareTo(CCadShape other)
			{
				return (this.CompareTo(other as ICadShape));
			}
			#endregion

			#region ======================================= CЛУЖЕБНЫЕ МЕТОДЫ СОБЫТИЙ ==================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение видимости графической фигуры.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected override void RaiseIsVisibleChanged()
			{
				CheckVisibleElement();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение полутона графической фигуры.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected override void RaiseIsHalftoneChanged()
			{
				CheckHalftoneElement();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение доступности для редактирования графической фигуры.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseIsEnabledChanged()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение слоя графической фигуры.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected override void RaiseLayerChanged()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение статуса расположения на канве графической фигуры.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected override void RaiseIsCanvasChanged()
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение статуса печати графической фигуры.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected override void RaiseIsPrintingChanged()
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение статуса выбора графической фигуры.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseIsSelectedChanged()
			{
				if (mIsSelect == false)
				{
					mHandleIndex = -1;
				}

				SetHandleRects();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение кисти контура графической фигуры.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseStrokeChanged()
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение кисти для заполнения замкнутой области графической фигуры.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseFillChanged()
			{

			}
			#endregion

			#region ======================================= МЕТОДЫ ILotusSupportEditInspector =========================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получить массив описателей свойств объекта
			/// </summary>
			/// <returns>Массив описателей</returns>
			//---------------------------------------------------------------------------------------------------------
			public virtual CPropertyDesc[] GetPropertiesDesc()
			{
				return (CadShapePropertiesDesc);
			}
			#endregion

			#region ======================================= МЕТОДЫ ILotusCopyParameters ===============================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Копирование параметров с указанного объекта
			/// </summary>
			/// <param name="source_object">Объект-источник с которого будут скопированы параметры</param>
			/// <param name="parameters">Контекст копирования параметров</param>
			//---------------------------------------------------------------------------------------------------------
			public override void CopyParameters(System.Object source_object, CParameters parameters)
			{
				base.CopyParameters(source_object, parameters);

				if (source_object is ICadShape cad_shape)
				{
					// 2) Основные параметры
					mIsEnabled = cad_shape.IsEnabled;

					// 4) Контур
					mStrokeId = cad_shape.StrokePenId;
					mStrokeThickness = cad_shape.StrokeThickness;
					mStrokePen = XCadPenManager.GetFromId(mStrokeId);

					// 5) Заливка
					mFillId = cad_shape.FillBrushId;
					mFillOpacity = cad_shape.FillOpacity;
					mFillBrush = XCadBrushManager.GetFromId(mFillId);


					NotifyPropertyChanged(PropertyArgsIsEnabled);
					NotifyPropertyChanged(PropertyArgsStrokeIsEnabled);
					NotifyPropertyChanged(PropertyArgsStrokePen);
					NotifyPropertyChanged(PropertyArgsStrokePenId);
					NotifyPropertyChanged(PropertyArgsStrokePenName);
					NotifyPropertyChanged(PropertyArgsStrokeThickness);
					NotifyPropertyChanged(PropertyArgsFillIsEnabled);
					NotifyPropertyChanged(PropertyArgsFillBrush);
					NotifyPropertyChanged(PropertyArgsFillBrushId);
					NotifyPropertyChanged(PropertyArgsFillBrushName);
					NotifyPropertyChanged(PropertyArgsFillOpacity);
				}
			}
			#endregion

			#region ======================================= МЕТОДЫ ILotusMementoOriginator ============================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получить состояние объекта
			/// </summary>
			/// <remarks>
			/// Под наименованием состояния объекта будем подразумевать имя свойства
			/// </remarks>
			/// <param name="name_state">Наименование состояния объекта</param>
			/// <returns>Состояние объекта</returns>
			//---------------------------------------------------------------------------------------------------------
			public override System.Object GetMemento(String name_state)
			{
				System.Object result = base.GetMemento(name_state);
				if (result != null)
				{
					return (result);
				}

				switch (name_state)
				{
					case nameof(IsEnabled):
						{
							result = IsEnabled;
						}
						break;
					case nameof(IsSelect):
						{
							result = IsSelect;
						}
						break;
					case nameof(StrokeIsEnabled):
						{
							result = StrokeIsEnabled;
						}
						break;
					case nameof(StrokePen):
						{
							result = StrokePen;
						}
						break;
					case nameof(StrokeThickness):
						{
							result = StrokeThickness;
						}
						break;
					case nameof(FillIsEnabled):
						{
							result = FillIsEnabled;
						}
						break;
					case nameof(FillBrush):
						{
							result = FillBrush;
						}
						break;
					case nameof(FillOpacity):
						{
							result = FillOpacity;
						}
						break;
					default:
						break;
				}

				return (result);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установить состояние объекта
			/// </summary>
			/// <remarks>
			/// Под наименованием состояния объекта будем подразумевать имя свойства
			/// </remarks>
			/// <param name="memento">Состояние объекта</param>
			/// <param name="name_state">Наименование состояния объекта</param>
			//---------------------------------------------------------------------------------------------------------
			public override void SetMemento(System.Object memento, String name_state)
			{
				base.SetMemento(memento, name_state);

				switch (name_state)
				{
					case nameof(IsEnabled):
						{
							mIsEnabled = (Boolean)memento;
							NotifyPropertyChanged(PropertyArgsIsEnabled);
							RaiseIsEnabledChanged();
						}
						break;
					case nameof(IsSelect):
						{
							mIsSelect = (Boolean)memento;
							NotifyPropertyChanged(PropertyArgsIsSelect);
							RaiseIsSelectedChanged();
						}
						break;
					case nameof(StrokeIsEnabled):
						{
							mStrokeIsEnabled = (Boolean)memento;
							NotifyPropertyChanged(PropertyArgsStrokeIsEnabled);
							RaiseStrokeChanged();
						}
						break;
					case nameof(StrokePen):
						{
							if (memento is CCadPen pen)
							{
								mStrokePen = pen;
								mStrokeId = pen.Id;
								NotifyPropertyChanged(PropertyArgsStrokePen);
								NotifyPropertyChanged(PropertyArgsStrokePenId);
								NotifyPropertyChanged(PropertyArgsStrokePenName);
								RaiseStrokeChanged();
							}
						}
						break;
					case nameof(StrokeThickness):
						{
							mStrokeThickness = (Single)memento;
							NotifyPropertyChanged(PropertyArgsStrokeThickness);
							RaiseStrokeChanged();
						}
						break;
					case nameof(FillIsEnabled):
						{
							mFillIsEnabled = (Boolean)memento;
							NotifyPropertyChanged(PropertyArgsFillIsEnabled);
							RaiseFillChanged();
						}
						break;
					case nameof(FillBrush):
						{
							if (memento is CCadBrush brush)
							{
								mFillBrush = brush;
								mFillId = brush.Id;
								NotifyPropertyChanged(PropertyArgsFillBrush);
								NotifyPropertyChanged(PropertyArgsFillBrushId);
								NotifyPropertyChanged(PropertyArgsFillBrushName);
								RaiseFillChanged();
							}
						}
						break;
					case nameof(FillOpacity):
						{
							mFillOpacity = (Int32)memento;
							NotifyPropertyChanged(PropertyArgsFillOpacity);
							RaiseFillChanged();
						}
						break;
					default:
						break;
				}
			}
			#endregion

			#region ======================================= МЕТОДЫ ILotusViewPresented ================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка статуса выбора объекта
			/// </summary>
			/// <param name="view_item">Элемент отображения</param>
			/// <param name="selected">Статус выбора объекта</param>
			//---------------------------------------------------------------------------------------------------------
			public void SetViewSelected(ILotusViewItem view_item, Boolean selected)
			{
				mIsSelect = selected;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Возможность выбора объекта
			/// </summary>
			/// <remarks>
			/// Имеется виду возможность выбора объекта в данный момент
			/// </remarks>
			/// <param name="view_item">Элемент отображения</param>
			/// <returns>Возможность выбора</returns>
			//---------------------------------------------------------------------------------------------------------
			public Boolean CanViewSelected(ILotusViewItem view_item)
			{
				return (true);
			}
			#endregion

			#region ======================================= МЕТОДЫ ILotusViewSelected =================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка статуса представления объекта
			/// </summary>
			/// <param name="view_item">Элемент отображения</param>
			/// <param name="presented">Статус представления объекта</param>
			//---------------------------------------------------------------------------------------------------------
			public void SetViewPresented(ILotusViewItem view_item, Boolean presented)
			{
				if (presented && mIsCanvas)
				{
					if (mCanvasViewer != null)
					{
						this.UpdateBoundsRect();
						mCanvasViewer.AnimatedZoomTo(mBoundsRect);
					}
				}
			}
			#endregion

			#region ======================================= МЕТОДЫ ИНТЕРФЕЙСОВ СОХРАНЕНИЯ/ЗАГРУЗКИ ====================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Подготовка элемента перед сохранением
			/// </summary>
			/// <param name="parameters">Параметры контекста</param>
			//---------------------------------------------------------------------------------------------------------
			public override void OnBeforeSave(CParameters parameters)
			{
				base.OnBeforeSave(parameters);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Восстановление параметров элемента после сохранения
			/// </summary>
			/// <param name="parameters">Параметры контекста</param>
			//---------------------------------------------------------------------------------------------------------
			public override void OnAfterSave(CParameters parameters)
			{
				base.OnAfterSave(parameters);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Подготовка элемента перед загрузкой
			/// </summary>
			/// <param name="parameters">Параметры контекста</param>
			//---------------------------------------------------------------------------------------------------------
			public override void OnBeforeLoad(CParameters parameters)
			{
				base.OnBeforeLoad(parameters);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Восстановление параметров элемента после загрузки
			/// </summary>
			/// <param name="parameters">Параметры контекста</param>
			//---------------------------------------------------------------------------------------------------------
			public override void OnAfterLoad(CParameters parameters)
			{
				base.OnAfterLoad(parameters);

				mStrokePen = XCadPenManager.GetFromId(mStrokeId);
				mFillBrush = XCadBrushManager.GetFromId(mFillId);
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Дубликат объекта
			/// </summary>
			/// <param name="context">Контекст дублирования объекта</param>
			//---------------------------------------------------------------------------------------------------------
			public override ICadObject Duplicate(System.Object context)
			{
				CCadShape cad_shape = new CCadShape();
				cad_shape.CopyParameters(this, null);
				return (cad_shape);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение точек привязки графического элемента
			/// </summary>
			/// <remarks>
			/// Точки привязки позволяют более удобно привязываться к различным частям графического элемента
			/// </remarks>
			/// <returns>Точки привязки графического элемента</returns>
			//---------------------------------------------------------------------------------------------------------
			public override IList<Vector2Df> GetSnapNodes()
			{
				return (null);
			}
			#endregion

			#region ======================================= МЕТОДЫ РАБОТЫ С КОНТУРОМ ==================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка пера для отображения контура графической фигуры
			/// </summary>
			/// <param name="pen">Перо</param>
			//---------------------------------------------------------------------------------------------------------
			public virtual void SetStrokePen(CCadPen pen)
			{
				mStrokePen = pen;
				mStrokeId = mStrokePen.Id;

				NotifyPropertyChanged(PropertyArgsStrokePen);
				NotifyPropertyChanged(PropertyArgsStrokePenId);
				NotifyPropertyChanged(PropertyArgsStrokePenName);
			}
			#endregion

			#region ======================================= МЕТОДЫ РАБОТЫ С ЗАЛИВКОЙ ==================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка кисти для заливки графической фигуры
			/// </summary>
			/// <param name="fill">Кисть</param>
			//---------------------------------------------------------------------------------------------------------
			public virtual void SetFillBrush(CCadBrush fill)
			{
				mFillBrush = fill;
				mFillId = mFillBrush.Id;

				NotifyPropertyChanged(PropertyArgsFillBrush);
				NotifyPropertyChanged(PropertyArgsFillBrushId);
				NotifyPropertyChanged(PropertyArgsFillBrushName);
			}
			#endregion

			#region ======================================= МЕТОДЫ ТРАНСФОРМАЦИИ ======================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перемещение графической фигуры
			/// </summary>
			/// <param name="offset">Значение смещения в условных единицах</param>
			//---------------------------------------------------------------------------------------------------------
			public virtual void Move(ref Vector2Df offset)
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перемещение графической фигуры вверх
			/// </summary>
			/// <param name="offset">Значение смещения в условных единицах</param>
			//---------------------------------------------------------------------------------------------------------
			public virtual void MoveUp(Single offset)
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перемещение графической фигуры вниз
			/// </summary>
			/// <param name="offset">Значение смещения в условных единицах</param>
			//---------------------------------------------------------------------------------------------------------
			public virtual void MoveDown(Single offset)
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перемещение графической фигуры влево
			/// </summary>
			/// <param name="offset">Значение смещения в условных единицах</param>
			//---------------------------------------------------------------------------------------------------------
			public virtual void MoveLeft(Single offset)
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перемещение графической фигуры вправо
			/// </summary>
			/// <param name="offset">Значение смещения в условных единицах</param>
			//---------------------------------------------------------------------------------------------------------
			public virtual void MoveRight(Single offset)
			{
			}
			#endregion

			#region ======================================= МЕТОДЫ УПРАВЛЕНИЯ =========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Начало захвата от управления курсора
			/// </summary>
			/// <param name="point">Точка в координатах канвы</param>
			//---------------------------------------------------------------------------------------------------------
			public virtual void StartCapturePosition(ref Vector2Df point)
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновления захвата управления от курсора
			/// </summary>
			/// <param name="point">Точка в координатах канвы</param>
			//---------------------------------------------------------------------------------------------------------
			public virtual void UpdateCapturePosition(ref Vector2Df point)
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Окончание захвата от управления курсора
			/// </summary>
			/// <param name="point">Точка в координатах канвы</param>
			//---------------------------------------------------------------------------------------------------------
			public virtual void EndCapturePosition(ref Vector2Df point)
			{
			}
			#endregion

			#region ======================================= МЕТОДЫ РАБОТЫ С РУЧКАМИ ===================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка точки на вхождение в прямоугольник ручки
			/// </summary>
			/// <param name="point">Проверяемая точка</param>
			/// <returns>Индекс прямоугольника или -1</returns>
			//---------------------------------------------------------------------------------------------------------
			public Int32 CheckPointInHandleRect(ref Vector2Df point)
			{
				Int32 index = -1;
				for (Int32 i = 0; i < mHandleRects.Count; i++)
				{
					if (mHandleRects[i].Contains(ref point))
					{
						index = i;
						break;
					}
				}

				return (index);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка точки на вхождение в прямоугольник ручки
			/// </summary>
			/// <param name="point">Проверяемая точка</param>
			/// <returns>Индекс прямоугольника или -1</returns>
			//---------------------------------------------------------------------------------------------------------
			public Int32 CheckPointInHandleRect(Vector2Df point)
			{
				Int32 index = -1;
				for (Int32 i = 0; i < mHandleRects.Count; i++)
				{
					if (mHandleRects[i].Contains(ref point))
					{
						index = i;
						break;
					}
				}

				return (index);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка прямоугольника ручки по соответствующей точке
			/// </summary>
			/// <param name="point">Требуемая точка</param>
			/// <param name="index">Индекс</param>
			//---------------------------------------------------------------------------------------------------------
			public void SetHandleRectFromPoint(Vector2Df point, Int32 index)
			{
				mHandleRects[index] = mCanvasViewer.GetHandleRect(ref point);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка прямоугольника ручки по соответствующей точке
			/// </summary>
			/// <param name="point">Требуемая точка</param>
			/// <param name="index">Индекс</param>
			//---------------------------------------------------------------------------------------------------------
			public void SetHandleRectFromPoint(ref Vector2Df point, Int32 index)
			{
				mHandleRects[index] = mCanvasViewer.GetHandleRect(ref point);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка прямоугольников для ручек
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public virtual void SetHandleRects()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка соответствующего курсора на ручкой
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public virtual void SetHandleCursor()
			{

			}
			#endregion

			#region ======================================= ОБРАБОТКА СОБЫТИЙ КОНТЕКСТНОГО МЕНЮ =======================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Открытие контекстного меню
			/// </summary>
			/// <param name="point">Точка открытия контекстного меню в пространстве канвы</param>
			//---------------------------------------------------------------------------------------------------------
			public virtual void OnOpenContextMenu(ref Vector2Df point)
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Закрытие контекстного меню
			/// </summary>
			/// <param name="point">Точка закрытия контекстного меню в пространстве канвы</param>
			//---------------------------------------------------------------------------------------------------------
			public virtual void OnClosedContextMenu(ref Vector2Df point)
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Выполнения команды контекстного меню
			/// </summary>
			/// <param name="command_name">Имя команды</param>
			/// <param name="context">Контекст данных</param>
			//---------------------------------------------------------------------------------------------------------
			public virtual void OnCommandContextMenu(String command_name, Object context)
			{

			}
			#endregion

			#region ======================================= ОБРАБОТКА СОБЫТИЙ ПАРАМЕТРОВ ОТОБРАЖЕНИЯ ==================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение масштаба представления текущего элемента
			/// </summary>
			/// <param name="scale">Масштаб представления</param>
			//---------------------------------------------------------------------------------------------------------
			public virtual void OnScaleChanged(Double scale)
			{
				if(mIsCanvas && IsSelect)
				{
					SetHandleRects();
				}
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
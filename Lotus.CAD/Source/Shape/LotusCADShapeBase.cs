//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Интерактивные графические фигуры
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADShapeBase.cs
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
		//! \defgroup CadShape Графические фигуры
		//! Интерактивные графические фигуры
		//! \ingroup CadDrawing
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Интерфейс базовой графической фигуры с базовым взаимодействием
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public interface ICadShape : ICadObject, ICadStrokeSupport, ICadFillSupport, ICadLayerSupport,
			ICadTransform, ICadControlElementMouse
		{
			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Позиция геометрического центра ограничивающего прямоугольника графической фигуры
			/// </summary>
			Vector2Df Location { get; set; }

			/// <summary>
			/// Видимость графической фигуры
			/// </summary>
			Boolean IsVisible { get; set; }

			/// <summary>
			/// Полутон графической фигуры
			/// </summary>
			Boolean IsHalftone { get; set; }

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
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение точек привязки графического элемента
			/// </summary>
			/// <remarks>
			/// Точки привязки позволяют более удобно привязываться к различным частям графического элемента
			/// </remarks>
			/// <returns>Точки привязки графического элемента</returns>
			//---------------------------------------------------------------------------------------------------------
			IList<Vector2Df> GetSnapNodes();
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
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Базовая графическая фигура с базовым взаимодействием
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public abstract class CCadShapeBase : CCadObject, ICadShape, ILotusSupportEditInspector
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			// Основные параметры
			protected static PropertyChangedEventArgs PropertyArgsIsVisible = new PropertyChangedEventArgs(nameof(IsVisible));
			protected static PropertyChangedEventArgs PropertyArgsIsHalftone = new PropertyChangedEventArgs(nameof(IsHalftone));
			protected static PropertyChangedEventArgs PropertyArgsIsEnabled = new PropertyChangedEventArgs(nameof(IsEnabled));
			protected static PropertyChangedEventArgs PropertyArgsIsSelect = new PropertyChangedEventArgs(nameof(IsSelect));

			// Слой графической фигуры
			protected static PropertyChangedEventArgs PropertyArgsLayer = new PropertyChangedEventArgs(nameof(Layer));
			protected static PropertyChangedEventArgs PropertyArgsLayerId = new PropertyChangedEventArgs(nameof(LayerId));
			protected static PropertyChangedEventArgs PropertyArgsLayerName = new PropertyChangedEventArgs(nameof(LayerName));

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
			public readonly static CPropertyDesc[] CadShapeBasePropertiesDesc = new CPropertyDesc[]
			{
				// Идентификация
				CPropertyDesc.OverrideDisplayNameAndDescription<CCadShapeBase>(nameof(Name), "Наименование", "Наименование фигуры"),
				CPropertyDesc.OverrideCategory<CCadShapeBase>(nameof(Name), XInspectorGroupDesc.ID),
				CPropertyDesc.OverrideOrder<CCadShapeBase>(nameof(Name), 1),
				CPropertyDesc.OverrideDisplayNameAndDescription<CCadShapeBase>(nameof(Id), "Идентификатор", "Локальный идентификатор фигуры"),
				CPropertyDesc.OverrideCategory<CCadShapeBase>(nameof(Id), XInspectorGroupDesc.ID),
				CPropertyDesc.OverrideOrder<CCadShapeBase>(nameof(Id), 100),
			};
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			// Основные параметры
			internal Boolean mIsVisible = true;
			internal Boolean mIsHalftone;
			internal Boolean mIsEnabled;
			internal Boolean mIsSelect;
			internal Boolean mIsPreSelect;

			// Слой графической фигуры
			internal CCadLayer mLayer;
			internal Int32 mLayerId;

			// Контур графической фигуры
			internal Boolean mStrokeIsEnabled;
			internal CCadPen mStrokePen;
			internal Int32 mStrokeId;
			internal Single mStrokeThickness;

			// Заливка графической фигуры
			internal Boolean mFillIsEnabled;
			internal CCadBrush mFillBrush;
			internal Int64 mFillId;
			internal Int32 mFillOpacity;

			// Данные для работы с управляющими элементами
			internal Int32 mHandleIndex;
			internal Int32 mHandleSubIndex;
			internal List<Rect2Df> mHandleRects;

			// Служебные данные
			internal Boolean mIsVisibleElement = true;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			//
			// ОСНОВНЫЕ ПАРАМЕТРЫ
			//
			/// <summary>
			/// Позиция геометрического центра ограничивающего прямоугольника графической фигуры
			/// </summary>
			public virtual Vector2Df Location { get; set; }

			//
			// ПАРАМЕТРЫ ГРАФИКИ
			//
			/// <summary>
			/// Видимость графической фигуры
			/// </summary>
			[DisplayName("Видимость")]
			[Description("Видимость графической фигуры")]
			[Category(XInspectorGroupDesc.Graphics)]
			[LotusCategoryOrder(2)]
			[Display(GroupName = XInspectorGroupDesc.Graphics, Order = 0)]
			public Boolean IsVisible
			{
				get { return (mIsVisible); }
				set
				{
					if (mIsVisible != value)
					{
						mIsVisible = value;
						NotifyPropertyChanged(PropertyArgsIsVisible);
						RaiseIsVisibleChanged();
					}
				}
			}

			/// <summary>
			/// Полутон графической фигуры
			/// </summary>
			[DisplayName("Полутон")]
			[Description("Полутон графической фигуры")]
			[Category(XInspectorGroupDesc.Graphics)]
			[Display(GroupName = XInspectorGroupDesc.Graphics, Order = 1)]
			public Boolean IsHalftone
			{
				get { return (mIsHalftone); }
				set
				{
					if (mIsHalftone != value)
					{
						mIsHalftone = value;
						NotifyPropertyChanged(PropertyArgsIsHalftone);
						RaiseIsHalftoneChanged();
					}
				}
			}

			/// <summary>
			/// Доступность графической фигуры для редактирования
			/// </summary>
			[DisplayName("Доступность")]
			[Description("Доступность графической фигуры для редактирования")]
			[Category(XInspectorGroupDesc.Graphics)]
			[Display(GroupName = XInspectorGroupDesc.Graphics, Order = 3)]
			public Boolean IsEnabled
			{
				get { return (mIsEnabled); }
				set
				{
					if (mIsEnabled != value)
					{
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
						mIsSelect = value;
						NotifyPropertyChanged(PropertyArgsIsSelect);
						RaiseIsSelectedChanged();
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
			// ПАРАМЕТРЫ СЛОЯ
			//
			/// <summary>
			/// Идентификатор слоя в котором расположен графическая фигура
			/// </summary>
			[Browsable(false)]
			public Int64 LayerId
			{
				get { return (mLayerId); }
				set
				{
					if (mLayerId != value)
					{
						mLayer = XCadLayerManager.GetFromId(mLayerId);
						mLayerId = mLayer.Id;
						NotifyPropertyChanged(PropertyArgsLayer);
						NotifyPropertyChanged(PropertyArgsLayerId);
						NotifyPropertyChanged(PropertyArgsLayerName);
						RaiseLayerChanged();
					}
				}
			}

			/// <summary>
			/// Названия слоя в котором расположен графическая фигура
			/// </summary>
			[DisplayName("Название слоя")]
			[Description("Названия слоя в котором расположен графическая фигура")]
			[Category(XInspectorGroupDesc.Graphics)]
			[Display(GroupName = XInspectorGroupDesc.Graphics, Order = 4)]
			public String LayerName
			{
				get { return (mLayer.Name); }
				set
				{
					if (mLayer.Name != value)
					{
						mLayer = XCadLayerManager.GetFromName(value);
						mLayerId = mLayer.Id;
						NotifyPropertyChanged(PropertyArgsLayer);
						NotifyPropertyChanged(PropertyArgsLayerName);
						NotifyPropertyChanged(PropertyArgsLayerId);
						RaiseLayerChanged();
					}
				}
			}

			/// <summary>
			/// Слой в котором расположен графическая фигура
			/// </summary>
			[DisplayName("Слой")]
			[Description("Слой в котором расположен графическая фигура")]
			[Category(XInspectorGroupDesc.Graphics)]
			[Display(GroupName = XInspectorGroupDesc.Graphics, Order = 5)]
			public CCadLayer Layer
			{
				get { return (mLayer); }
				set
				{
					if (mLayer != value)
					{
						mLayer = value;
						mLayerId = mLayer.Id;
						NotifyPropertyChanged(PropertyArgsLayer);
						NotifyPropertyChanged(PropertyArgsLayerId);
						NotifyPropertyChanged(PropertyArgsLayerName);
						RaiseLayerChanged();
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
			public Boolean StrokeIsEnabled
			{
				get { return (mStrokeIsEnabled); }
				set
				{
					if (mStrokeIsEnabled != value)
					{
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
			public Int32 StrokePenId
			{
				get { return (mStrokeId); }
				set
				{
					if (mStrokeId != value)
					{
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
			[LotusInspectorTypeEditor(typeof(LotusEditorSelectorPen))]
			public CCadPen StrokePen
			{
				get { return (mStrokePen); }
				set
				{
					if (mStrokePen != value)
					{
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
			public Single StrokeThickness
			{
				get { return (mStrokeThickness); }
				set
				{
					if (mStrokeThickness != value)
					{
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
			public Boolean FillIsEnabled
			{
				get { return (mFillIsEnabled); }
				set
				{
					mFillIsEnabled = value;
					NotifyPropertyChanged(PropertyArgsFillIsEnabled);
				}
			}

			/// <summary>
			/// Идентификатор кисти для заполнения замкнутой области графической фигуры
			/// </summary>
			[Browsable(false)]
			public Int64 FillBrushId
			{
				get { return (mFillId); }
				set
				{
					if (mFillId != value)
					{
						mFillBrush = XCadBrushManager.GetFromId(mFillId);
						mFillId = mFillBrush.Id;

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
			public CCadBrush FillBrush
			{
				get { return (mFillBrush); }
				set
				{
					if (mFillBrush != value)
					{
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
			public Int32 FillOpacity
			{
				get { return (mFillOpacity); }
				set
				{
					if (mFillOpacity != value)
					{
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
			/// Отображаемое имя типа в инспекторе свойств
			/// </summary>
			[Browsable(false)]
			public override String InspectorTypeName
			{
				get {
					return (nameof(CCadShapeBase));
				}
			}

			/// <summary>
			/// Отображаемое имя объекта в инспекторе свойств
			/// </summary>
			[Browsable(false)]
			public override String InspectorObjectName
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
			public CCadShapeBase()
			{
				// Данные по умолчанию
				mHandleIndex = -1;
				mHandleSubIndex = -1;
				mHandleRects = new List<Rect2Df>();
				mLayer = XCadLayerManager.DefaultLayer;
				mLayerId = mLayer.Id;
				mStrokePen = XCadPenManager.DefaultPen;
				mStrokeId = mStrokePen.Id;
				mFillBrush = XCadBrushManager.DefaultBrush;
				mFillId = mFillBrush.Id;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор копирования
			/// </summary>
			/// <param name="source">Источник копии</param>
			/// <param name="add_to_draft">Добавлять ли в чертеж источника</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadShapeBase(CCadShapeBase source, Boolean add_to_draft = true)
				: base(source)
			{
				mHandleIndex = -1;
				mHandleSubIndex = -1;
				mHandleRects = new List<Rect2Df>();
				mLayer = XCadLayerManager.DefaultLayer;
				mLayerId = mLayer.Id;
				mStrokePen = XCadPenManager.DefaultPen;
				mStrokeId = mStrokePen.Id;
				mFillBrush = XCadBrushManager.DefaultBrush;
				mFillId = mFillBrush.Id;
			}
			#endregion

			#region ======================================= СИСТЕМНЫЕ МЕТОДЫ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Сравнение графических элементов для упорядочивания (по Z индексу)
			/// </summary>
			/// <param name="other">Сравниваемый графический элемент</param>
			/// <returns>Статус сравнения графических элементов</returns>
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
			/// Сравнение графических элементов для упорядочивания (по Z индексу)
			/// </summary>
			/// <param name="other">Сравниваемый графический элемент</param>
			/// <returns>Статус сравнения графических элементов</returns>
			//---------------------------------------------------------------------------------------------------------
			public Int32 CompareTo(CCadShapeBase other)
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
			/// Получение хэш кода объекта
			/// </summary>
			/// <returns>Стандартный хэш код объекта</returns>
			//---------------------------------------------------------------------------------------------------------
			public override Int32 GetHashCode()
			{
				return (base.GetHashCode());
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Копирования графического элемента
			/// </summary>
			/// <remarks>
			/// Созданная копия графического элемента не принадлежит чертежу исходного графического элемента
			/// </remarks>
			/// <returns>Дубликат графического элемента со всеми параметрами и данными</returns>
			//---------------------------------------------------------------------------------------------------------
			public virtual CCadShapeBase Duplicate()
			{
				return (null);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Преобразование к текстовому представлению
			/// </summary>
			/// <returns>Наименование графического элемента</returns>
			//---------------------------------------------------------------------------------------------------------
			public override String ToString()
			{
				return (mName);
			}
			#endregion

			#region ======================================= СЛУЖЕБНЫЕ МЕТОДЫ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка на видимость графического элемента с учетом настроек слоя
			/// </summary>
			/// <returns>Видимость графического элемента </returns>
			//---------------------------------------------------------------------------------------------------------
			internal virtual Boolean CheckVisibleElement()
			{
				if ((mLayer.VisibleMode == TCadLayerVisibleMode.Hidden) ||
					(mLayer.VisibleMode == TCadLayerVisibleMode.VisibleIsHidden && mIsVisible) ||
					(mLayer.VisibleMode == TCadLayerVisibleMode.Visible && mIsVisible == false))
				{

					mIsVisibleElement = false;
				}
				else
				{
					mIsVisibleElement = true;
				}

				return (mIsVisibleElement);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка на полутон графического элемента с учетом настроек слоя
			/// </summary>
			/// <returns>Полутон графического элемента </returns>
			//---------------------------------------------------------------------------------------------------------
			internal virtual Boolean CheckHalftoneElement()
			{
				if ((mLayer.HalftoneMode == TCadLayerHalftoneMode.Disable && mIsHalftone) ||
					mLayer.HalftoneMode == TCadLayerHalftoneMode.Halftone ||
					mLayer.HalftoneMode == TCadLayerHalftoneMode.HalftoneGray)
				{
					//this.Opacity = 0.5f;
					return (true);
				}
				else
				{
					//this.Opacity = 1;
					return (false);
				}
			}
			#endregion

			#region ======================================= CЛУЖЕБНЫЕ МЕТОДЫ СОБЫТИЙ ==================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение видимости графического элемента.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseIsVisibleChanged()
			{
				CheckVisibleElement();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение полутона графического элемента.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseIsHalftoneChanged()
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
			/// Изменение слоя графического элемента.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseLayerChanged()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение статуса расположения на канве графического элемента.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected override void RaiseIsCanvasChanged()
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение статуса печати графического элемента.
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
				return (CadShapeBasePropertiesDesc);
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Общее обновление графического элемента
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void Update()
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Копирование параметров с графической фигуры
			/// </summary>
			/// <param name="element">Графическая фигура</param>
			/// <param name="context">Контекст копирования данных</param>
			//---------------------------------------------------------------------------------------------------------
			public virtual void CopyParamemtrs(ICadShape element, Object context)
			{
				//base.CopyParamemtrs(element, context);

				ICadShape source = element as ICadShape;
				if (source != null)
				{

					// 2) Основные параметры
					mIsVisible = source.IsVisible;
					mIsHalftone = source.IsHalftone;
					mIsEnabled = source.IsEnabled;

					// 3) Слой
					mLayerId = source.LayerId;
					mLayer = XCadLayerManager.GetFromId(mLayerId);

					// 4) Контур
					mStrokeId = source.StrokePenId;
					mStrokeThickness = source.StrokeThickness;

					// 5) Заливка
					mFillId = source.FillBrushId;
					mFillOpacity = source.FillOpacity;

					// 2) ОБНОВЛЕНИЕ Основные параметры
					NotifyPropertyChanged(PropertyArgsIsVisible);
					NotifyPropertyChanged(PropertyArgsIsHalftone);
					NotifyPropertyChanged(PropertyArgsIsEnabled);

					// 3) ОБНОВЛЕНИЕ Слой
					NotifyPropertyChanged(PropertyArgsLayer);
					NotifyPropertyChanged(PropertyArgsLayerId);
					NotifyPropertyChanged(PropertyArgsLayerName);

					// 4) ОБНОВЛЕНИЕ Контур
					NotifyPropertyChanged(PropertyArgsStrokeIsEnabled);
					NotifyPropertyChanged(PropertyArgsStrokePen);
					NotifyPropertyChanged(PropertyArgsStrokePenId);
					NotifyPropertyChanged(PropertyArgsStrokePenName);
					NotifyPropertyChanged(PropertyArgsStrokeThickness);

					// 5) ОБНОВЛЕНИЕ Заливка
					NotifyPropertyChanged(PropertyArgsFillIsEnabled);
					NotifyPropertyChanged(PropertyArgsFillBrush);
					NotifyPropertyChanged(PropertyArgsFillBrushId);
					NotifyPropertyChanged(PropertyArgsFillBrushName);
					NotifyPropertyChanged(PropertyArgsFillOpacity);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление слоя для графического элемента
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public virtual void UpdateLayer()
			{
				CheckVisibleElement();
				CheckHalftoneElement();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка слоя для графического элемента
			/// </summary>
			/// <param name="layer">Слой</param>
			//---------------------------------------------------------------------------------------------------------
			public virtual void SetLayer(CCadLayer layer)
			{
				mLayer = layer;
				mLayerId = mLayer.Id;

				NotifyPropertyChanged(PropertyArgsLayer);
				NotifyPropertyChanged(PropertyArgsLayerId);
				NotifyPropertyChanged(PropertyArgsLayerName);

				CheckVisibleElement();
				CheckHalftoneElement();
			}

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

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение точек привязки графического элемента
			/// </summary>
			/// <remarks>
			/// Точки привязки позволяют более удобно привязываться к различным частям графического элемента
			/// </remarks>
			/// <returns>Точки привязки графического элемента</returns>
			//---------------------------------------------------------------------------------------------------------
			public virtual IList<Vector2Df> GetSnapNodes()
			{
				return (null);
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
				mHandleRects[index] = mCanvas.GetHandleRect(ref point);
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
				mHandleRects[index] = mCanvas.GetHandleRect(ref point);
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

			#region ======================================= МЕТОДЫ РИСОВАНИЯ ==========================================
			////---------------------------------------------------------------------------------------------------------
			///// <summary>
			///// Абстрактный метод для рисования графической фигуры
			///// </summary>
			////---------------------------------------------------------------------------------------------------------
			//public virtual void Draw()
			//{
			//}
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

			#region ======================================= ОБРАБОТЧИКИ СОБЫТИЙ =======================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Подготовка элемента перед сохранением
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public virtual void OnBeforeSave()
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Восстановление параметров элемента после сохранения
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public virtual void OnAfterSave()
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Подготовка элемента перед загрузкой
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public virtual void OnBeforeLoad()
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Восстановление параметров элемента после загрузки
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public virtual void OnAfterLoad()
			{
				//base.OnAfterLoad();

				mLayer = XCadLayerManager.GetFromId(mLayerId);
				mStrokePen = XCadPenManager.GetFromId(mStrokeId);
				mFillBrush = XCadBrushManager.GetFromId(mFillId);

				CheckHalftoneElement();
				CheckVisibleElement();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Подготовка элемента перед печатью
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public virtual void OnBeforePrinting()
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Восстановление параметров элемента после печати
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public virtual void OnAfterPrinting()
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение масштаба представления текущего элемента
			/// </summary>
			/// <param name="scale">Масштаб представления</param>
			//---------------------------------------------------------------------------------------------------------
			public virtual void OnScaleChanged(Double scale)
			{

			}
			#endregion

			#region ======================================= МЕТОДЫ СЕРИАЛИЗАЦИИ =======================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Запись свойств и данных графической фигуры в формат атрибутов XML
			/// </summary>
			/// <param name="xml_writer">Средство записи данных в формат XML</param>
			//---------------------------------------------------------------------------------------------------------
			protected void WriteShapeToAttribute(XmlWriter xml_writer)
			{
				xml_writer.WriteBooleanToAttribute("IsVisible", mIsVisible);
				xml_writer.WriteBooleanToAttribute("IsHalftone", mIsHalftone);
				xml_writer.WriteBooleanToAttribute("IsEnabled", mIsEnabled);
				xml_writer.WriteLongToAttribute("ZIndex", mZIndex);

				xml_writer.WriteLongToAttribute("LayerId", mLayerId);

				xml_writer.WriteBooleanToAttribute("StrokeIsEnabled", StrokeIsEnabled);
				xml_writer.WriteLongToAttribute("StrokeId", mStrokeId);
				xml_writer.WriteSingleToAttribute("StrokeThickness", mStrokeThickness);

				xml_writer.WriteBooleanToAttribute("FillIsEnabled", FillIsEnabled);
				xml_writer.WriteLongToAttribute("FillId", mFillId);
				xml_writer.WriteIntegerToAttribute("FillOpacity", mFillOpacity);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Чтение свойств и данных графической фигуры из формата атрибутов XML
			/// </summary>
			/// <param name="xml_reader">Средство чтения данных формата XML</param>
			//---------------------------------------------------------------------------------------------------------
			protected void ReadShapeFromAttribute(XmlReader xml_reader)
			{
				mIsVisible = xml_reader.ReadBooleanFromAttribute("IsVisible", mIsVisible);
				mIsHalftone = xml_reader.ReadBooleanFromAttribute("IsHalftone", mIsHalftone);
				mIsEnabled = xml_reader.ReadBooleanFromAttribute("IsEnabled", mIsEnabled);
				mZIndex = xml_reader.ReadIntegerFromAttribute("ZIndex", mZIndex);

				mLayerId = xml_reader.ReadLongFromAttribute("LayerId", mLayerId);

				mStrokeIsEnabled = xml_reader.ReadBooleanFromAttribute("StrokeIsEnabled", mStrokeIsEnabled);
				mStrokeId = xml_reader.ReadLongFromAttribute("StrokeId", mStrokeId);
				mStrokeThickness = xml_reader.ReadSingleFromAttribute("StrokeThickness", mStrokeThickness);

				mFillIsEnabled = xml_reader.ReadBooleanFromAttribute("FillIsEnabled", mFillIsEnabled);
				mFillId = xml_reader.ReadLongFromAttribute("FillId", mFillId);
				mFillOpacity = xml_reader.ReadIntegerFromAttribute("FillOpacity", mFillOpacity);
			}

			////---------------------------------------------------------------------------------------------------------
			///// <summary>
			///// Запись свойств и данных графической фигуры в бинарный поток
			///// </summary>
			///// <param name="binary_writer">Бинарный поток открытый для записи</param>
			////---------------------------------------------------------------------------------------------------------
			//public void WriteToStream(BinaryWriter binary_writer)
			//{

			//}

			////---------------------------------------------------------------------------------------------------------
			///// <summary>
			///// Запись свойств и данных графической фигуры в формат данных XML
			///// </summary>
			///// <param name="xml_writer">Средство записи данных в формат XML</param>
			////---------------------------------------------------------------------------------------------------------
			//public void WriteToXml(XmlWriter xml_writer)
			//{

			//}

			////---------------------------------------------------------------------------------------------------------
			///// <summary>
			///// Чтение свойств и данных графической фигуры из бинарного потока
			///// </summary>
			///// <param name="binary_reader">Бинарный поток открытый для чтения</param>
			////---------------------------------------------------------------------------------------------------------
			//public void ReadFromStream(BinaryReader binary_reader)
			//{

			//}

			////---------------------------------------------------------------------------------------------------------
			///// <summary>
			///// Чтение свойств и данных графической фигуры из потока данных в формате XML
			///// </summary>
			///// <param name="xml_reader">Средство чтения данных формата XML</param>
			////---------------------------------------------------------------------------------------------------------
			//public void ReadFromXml(XmlReader xml_reader)
			//{
			//}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
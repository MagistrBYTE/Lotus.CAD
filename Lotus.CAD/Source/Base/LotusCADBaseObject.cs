//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Общая подсистема
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADBaseObject.cs
*		Базовый графический объект способный рисовать различное графическое содержимое.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 27.03.2022
//=====================================================================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Runtime.Serialization;
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
		//! \addtogroup CadDrawing
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Тип графического объекта
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		[TypeConverter(typeof(EnumToStringConverter<TCadObjectType>))]
		public enum TCadObjectType
		{
			/// <summary>
			/// Базовый графический объект
			/// </summary>
			[Description("ОБЪЕКТ")]
			Object,

			/// <summary>
			/// Графический элемент
			/// </summary>
			[Description("ЭЛЕМЕНТ")]
			Element,

			/// <summary>
			/// Графическая фигура
			/// </summary>
			[Description("ФИГУРА")]
			Shape
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Интерфейс для определения базового графического объекта способного рисовать различное графическое содержимое
		/// </summary>
		/// <remarks>
		/// К типам графического содержимого относится векторная графика, глифы и текст, растровые изображения
		/// </remarks>
		//-------------------------------------------------------------------------------------------------------------
		public interface ICadObject : ICadEntity, ICadPrintingElement, ILotusCopyParameters, IComparable<ICadObject>
		{
			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Чертеж которому принадлежит элемент
			/// </summary>
			ICadDraft IDraft { get; set; }

			/// <summary>
			/// Логическая группа которой принадлежит графический объект
			/// </summary>
			String Group { get; set; }

			/// <summary>
			/// Статус элемента находящегося в стадии создания
			/// </summary>
			Boolean IsCreating { get; }

			/// <summary>
			/// Интерфейс элемента пользовательского интерфейса для управления канвой
			/// </summary>
			ICadCanvasViewer CanvasViewer { get; set; }

			/// <summary>
			/// Статус расположения графического объекта на канве
			/// </summary>
			Boolean IsCanvas { get; set; }

			/// <summary>
			/// Позиция графического объекта в Z плоскости
			Int32 ZIndex { get; set; }

			/// <summary>
			/// Позиция геометрического центра ограничивающего прямоугольника геометрии объекта
			/// </summary>
			Vector2Df Location { get; }

			/// <summary>
			/// Ограничивающий прямоугольник геометрии графического объекта
			/// </summary>
			Rect2Df BoundsRect { get; }
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление ограничивающего прямоугольника геометрии графического объекта
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			void UpdateBoundsRect();

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка на одну позицию вниз в Z плоскости
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			void SendToBack();

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка на одну позицию вверх в Z плоскости
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			void BringToFront();

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Копирование графического объекта в буфер обмена
			/// </summary>
			/// <param name="context">Контекст копирования </param>
			//---------------------------------------------------------------------------------------------------------
			void Copy(System.Object context);

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вставка графического объекта из буфера обмена на текущий чертеж
			/// </summary>
			/// <param name="context">Контекст вставки</param>
			//---------------------------------------------------------------------------------------------------------
			void Paste(System.Object context);

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Дубликат объекта
			/// </summary>
			/// <param name="context">Контекст дублирования объекта</param>
			//---------------------------------------------------------------------------------------------------------
			ICadObject Duplicate(System.Object context);
			#endregion

			#region ======================================= МЕТОДЫ ПРОВЕРКИ ПОПАДАНИЙ =================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка на попадание в область графического объекта указанной точки
			/// </summary>
			/// <param name="point">Проверяемая точка</param>
			/// <param name="epsilon">Точность проверки</param>
			/// <returns>Статус проверки</returns>
			//---------------------------------------------------------------------------------------------------------
			Boolean CheckPoint(ref Vector2Df point, Single epsilon);

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка на попадание ВСЕЙ ГЕОМЕТРИИ графического объекта внутрь прямоугольной области
			/// </summary>
			/// <param name="rect">Прямоугольник</param>
			/// <returns>Статус проверки</returns>
			//---------------------------------------------------------------------------------------------------------
			Boolean CheckInsideRect(ref Rect2Df rect);

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка на попадание ВСЕЙ ИЛИ ЧАСТИ ГЕОМЕТРИИ графического объекта внутрь прямоугольной области
			/// </summary>
			/// <param name="rect">Прямоугольник</param>
			/// <returns>Статус проверки</returns>
			//---------------------------------------------------------------------------------------------------------
			Boolean CheckInsideOrIntersectRect(ref Rect2Df rect);

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка на видимость всей геометрии графического объекта в области просмотра
			/// </summary>
			/// <returns>Статус видимости</returns>
			//---------------------------------------------------------------------------------------------------------
			Boolean CheckVisibleInViewport();
			#endregion

			#region ======================================= МЕТОДЫ РИСОВАНИЯ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Метод для рисования графического объекта
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			void Draw();
			#endregion
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Класс предоставляющий базовый графический объект способный рисовать различное графическое содержимое
		/// </summary>
		/// <remarks>
		/// К типам графического содержимого относится векторная графика, глифы и текст, растровые изображения.
		/// Это базовый визуальный объект, на основании его строится и системные графические объекты и все остальные
		/// </remarks>
		//-------------------------------------------------------------------------------------------------------------
		[DataContract]
		public class CCadObject : CCadEntity, ICadObject, ILotusMementoOriginator, ILotusOwnerObject, ILotusOwnedObject,
			IComparable<ICadObject>, IComparable<CCadObject>
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			protected static PropertyChangedEventArgs PropertyArgsGroup = new PropertyChangedEventArgs(nameof(Group));
			protected static PropertyChangedEventArgs PropertyArgsIsCanvas = new PropertyChangedEventArgs(nameof(IsCanvas));
			protected static PropertyChangedEventArgs PropertyArgsIsPrinting = new PropertyChangedEventArgs(nameof(IsPrinting));
			protected static PropertyChangedEventArgs PropertyArgsZIndex = new PropertyChangedEventArgs(nameof(ZIndex));
			protected static PropertyChangedEventArgs PropertyArgsLocation = new PropertyChangedEventArgs(nameof(Location));
			protected static PropertyChangedEventArgs PropertyArgsBoundsRect = new PropertyChangedEventArgs(nameof(BoundsRect));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			// Основные параметры
			protected internal ICadCanvasViewer mCanvasViewer;
			protected internal String mGroup;
			protected internal Boolean mIsCreating;
			protected internal Boolean mIsCanvas;
			protected internal Boolean mIsPrinting;
			protected internal Int32 mZIndex = 0;
			protected internal Rect2Df mBoundsRect;

			// Платформенные-зависимые данные
#if USE_WINDOWS
			protected internal System.Windows.Media.DrawingVisual mVisual;
#endif
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			//
			// ИДЕНТИФИКАЦИЯ
			//
			/// <summary>
			/// Логическая группа которой принадлежит графический объект
			/// </summary>
			[DisplayName("Группа")]
			[Description("Логическая группа которой принадлежит графический объект")]
			[Category(XInspectorGroupDesc.ID)]
			[LotusPropertyOrder(2)]
			[Display(GroupName = XInspectorGroupDesc.ID, Order = 2)]
			[DataMember]
			public String Group
			{
				get { return (mGroup); }
				set
				{
					SavePropertyToMemory(nameof(Group));

					mGroup = value;
					NotifyPropertyChanged(PropertyArgsGroup);
					RaiseGroupChanged();
				}
			}

			/// <summary>
			/// Тип сущности модуля чертежной графики
			/// </summary>
			[DisplayName("Тип объекта")]
			[Description("Тип сущности модуля чертежной графики")]
			[Category(XInspectorGroupDesc.ID)]
			[LotusPropertyOrder(3)]
			[Display(GroupName = XInspectorGroupDesc.ID, Order = 3)]
			public override TCadEntityType EntityType
			{
				get { return (TCadEntityType.Object); }
			}

			/// <summary>
			/// Статус элемента находящегося в стадии создания
			/// </summary>
			[Browsable(false)]
			public Boolean IsCreating 
			{
				get { return (mIsCreating); } 
			}

			//
			// ОСНОВНЫЕ ПАРАМЕТРЫ
			//
			/// <summary>
			/// Тип графического объекта
			/// </summary>
			[Browsable(false)]
			public virtual TCadObjectType ObjectType
			{
				get { return (TCadObjectType.Object); }
			}

			/// <summary>
			/// Интерфейс элемента пользовательского интерфейса для управления канвой
			/// </summary>
			[Browsable(false)]
			public ICadCanvasViewer CanvasViewer
			{
				get { return (mCanvasViewer); }
				set
				{
					mCanvasViewer = value;
				}
			}

			/// <summary>
			/// Статус расположения графического объекта на канве
			/// </summary>
			[Browsable(false)]
			public Boolean IsCanvas
			{
				get { return (mIsCanvas); }
				set
				{
					if (mIsCanvas != value)
					{
						mIsCanvas = value;
						NotifyPropertyChanged(PropertyArgsIsCanvas);
						RaiseIsCanvasChanged();
					}
				}
			}

			/// <summary>
			/// Статус печати графического объекта
			/// </summary>
			[Browsable(false)]
			[DataMember]
			public Boolean IsPrinting
			{
				get { return (mIsPrinting); }
				set
				{
					if (mIsPrinting != value)
					{
						SavePropertyToMemory(nameof(IsPrinting));

						mIsPrinting = value;
						NotifyPropertyChanged(PropertyArgsIsPrinting);
						RaiseIsPrintingChanged();
					}
				}
			}

			/// <summary>
			/// Позиция графического объекта в Z плоскости
			/// </summary>
			[DisplayName("ZIndex")]
			[Description("Позиция графического объекта в Z плоскости")]
			[Category(XInspectorGroupDesc.Size)]
			[LotusCategoryOrder(1)]
			[Display(GroupName = XInspectorGroupDesc.Size, Order = 10)]
			[DataMember]
			public Int32 ZIndex
			{
				get { return (mZIndex); }
				set
				{
					if (mZIndex != value)
					{
						SavePropertyToMemory(nameof(ZIndex));

						mZIndex = value;
						NotifyPropertyChanged(PropertyArgsZIndex);

						// Обновляем расположение
						mCanvasViewer.SortByZIndex();
					}
				}
			}

			/// <summary>
			/// Позиция геометрического центра ограничивающего прямоугольника геометрии объекта
			/// </summary>
			[Browsable(false)]
			public virtual Vector2Df Location { get; set; }

			/// <summary>
			/// Ограничивающий прямоугольник геометрии объекта
			/// </summary>
			[Browsable(false)]
			public virtual Rect2Df BoundsRect
			{
				get { return (mBoundsRect); }
			}
			#endregion

			#region ======================================= СВОЙСТВА ILotusOwnedObject ================================
			/// <summary>
			/// Владелец объекта (чертеж)
			/// </summary>
			public ILotusOwnerObject IOwner
			{
				get { return (IDraft as ILotusOwnerObject); }
				set { IDraft = value as ICadDraft; }
			}

			/// <summary>
			/// Владелец объекта (чертеж)
			/// </summary>
			public ICadDraft IDraft { get; set; }
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public CCadObject()
				: this("Новый объект")
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="name">Имя графического объекта</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadObject(String name)
				: base(name)
			{
			}
			#endregion

			#region ======================================= СИСТЕМНЫЕ МЕТОДЫ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Сравнение графических объектов для упорядочивания (по Z индексу)
			/// </summary>
			/// <param name="other">Сравниваемый графический объект</param>
			/// <returns>Статус сравнения графических объектов</returns>
			//---------------------------------------------------------------------------------------------------------
			public Int32 CompareTo(ICadObject other)
			{
				if (mZIndex > other.ZIndex)
				{
					return (1);
				}
				else
				{
					if (mZIndex < other.ZIndex)
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
			/// Сравнение графических объектов для упорядочивания (по Z индексу)
			/// </summary>
			/// <param name="other">Сравниваемый графический объект</param>
			/// <returns>Статус сравнения графических объектов</returns>
			//---------------------------------------------------------------------------------------------------------
			public Int32 CompareTo(CCadObject other)
			{
				return (CompareTo(other as ICadObject));
			}
			#endregion

			#region ======================================= CЛУЖЕБНЫЕ МЕТОДЫ СОБЫТИЙ ==================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение имени графического объекта.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected override void RaiseNameChanged()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение логической группы графического объекта.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected void RaiseGroupChanged()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение статуса расположения на канве графического объекта.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseIsCanvasChanged()
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение статуса печати графического объекта.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseIsPrintingChanged()
			{

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
			public virtual void CopyParameters(System.Object source_object, CParameters parameters)
			{
				if(source_object is ICadObject cad_object)
				{
					mName = cad_object.Name;
					mGroup = cad_object.Group;
					mIsPrinting = cad_object.IsPrinting;
					mIsCanvas = cad_object.IsCanvas;
					mZIndex = cad_object.ZIndex;
					Location = cad_object.Location;
					mBoundsRect = cad_object.BoundsRect;

					NotifyPropertyChanged(PropertyArgsName);
					NotifyPropertyChanged(PropertyArgsGroup);
					NotifyPropertyChanged(PropertyArgsIsPrinting);
					NotifyPropertyChanged(PropertyArgsIsCanvas);
					NotifyPropertyChanged(PropertyArgsZIndex);
					NotifyPropertyChanged(PropertyArgsLocation);
					NotifyPropertyChanged(PropertyArgsBoundsRect);
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
			public virtual System.Object GetMemento(String name_state)
			{
				System.Object result = null;
				switch (name_state)
				{
					case nameof(Name):
						{
							result = mName;
						}
						break;
					case nameof(Group):
						{
							result = mGroup;
						}
						break;
					case nameof(IsPrinting):
						{
							result = mIsPrinting;
						}
						break;
					case nameof(ZIndex):
						{
							result = mZIndex;
						}
						break;
					case nameof(Location):
						{
							result = Location;
						}
						break;
					case nameof(BoundsRect):
						{
							result = mBoundsRect;
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
			public virtual void SetMemento(System.Object memento, String name_state)
			{
				switch (name_state)
				{
					case nameof(Name):
						{
							mName = (String)memento;
							NotifyPropertyChanged(PropertyArgsName);
						}
						break;
					case nameof(Group):
						{
							mGroup = (String)memento;
							NotifyPropertyChanged(PropertyArgsGroup);
						}
						break;
					case nameof(IsPrinting):
						{
							mIsPrinting = (Boolean)memento;
							NotifyPropertyChanged(PropertyArgsIsPrinting);
						}
						break;
					case nameof(ZIndex):
						{
							mZIndex = (Int32)memento;
							NotifyPropertyChanged(PropertyArgsIsPrinting);

							// Обновляем расположение
							mCanvasViewer.SortByZIndex();
						}
						break;
					case nameof(Location):
						{
							Location = (Vector2Df)memento;
							NotifyPropertyChanged(PropertyArgsLocation);
						}
						break;
					case nameof(BoundsRect):
						{
							mBoundsRect = (Rect2Df)memento;
							NotifyPropertyChanged(PropertyArgsBoundsRect);
						}
						break;
					default:
						break;
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Сохранение состояния свойства
			/// </summary>
			/// <param name="property_name">Имя свойства</param>
			//---------------------------------------------------------------------------------------------------------
			public virtual void SavePropertyToMemory(String property_name)
			{
				if (mCanvasViewer != null && mCanvasViewer.Memento != null)
				{
					mCanvasViewer.Memento.AddStateToHistory(new CMementoCaretakerChanged(this,property_name));
				}
			}
			#endregion

			#region ======================================= МЕТОДЫ ICadPrintingElement ================================
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
			#endregion

			#region ======================================= МЕТОДЫ ILotusOwnerObject ==================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Присоединение указанного зависимого объекта
			/// </summary>
			/// <param name="owned_object">Объект</param>
			/// <param name="add">Статус добавления в коллекцию</param>
			//---------------------------------------------------------------------------------------------------------
			public void AttachOwnedObject(ILotusOwnedObject owned_object, Boolean add)
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Отсоединение указанного зависимого объекта
			/// </summary>
			/// <param name="owned_object">Объект</param>
			/// <param name="remove">Статус удаления из коллекции</param>
			//---------------------------------------------------------------------------------------------------------
			public void DetachOwnedObject(ILotusOwnedObject owned_object, Boolean remove)
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление связей для зависимых объектов
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void UpdateOwnedObjects()
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Информирование данного объекта о начале изменения данных указанного зависимого объекта
			/// </summary>
			/// <param name="owned_object">Зависимый объект</param>
			/// <param name="data">Объект, данные которого будут меняться</param>
			/// <param name="data_name">Имя данных</param>
			/// <returns>Статус разрешения/согласования изменения данных</returns>
			//---------------------------------------------------------------------------------------------------------
			public Boolean OnNotifyUpdating(ILotusOwnedObject owned_object, System.Object data, String data_name)
			{
				return (true);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Информирование данного объекта об окончании изменении данных указанного объекта
			/// </summary>
			/// <param name="owned_object">Зависимый объект</param>
			/// <param name="data">Объект, данные которого изменились</param>
			/// <param name="data_name">Имя данных</param>
			//---------------------------------------------------------------------------------------------------------
			public void OnNotifyUpdated(ILotusOwnedObject owned_object, System.Object data, String data_name)
			{

			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление ограничивающего прямоугольника геометрии графического объекта
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public virtual void UpdateBoundsRect()
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка на одну позицию вниз в Z плоскости
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public virtual void SendToBack()
			{
				ZIndex--;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка на одну позицию вверх в Z плоскости
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public virtual void BringToFront()
			{
				ZIndex++;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Копирование графического объекта в буфер обмена
			/// </summary>
			/// <param name="context">Контекст копирования </param>
			//---------------------------------------------------------------------------------------------------------
			public virtual void Copy(System.Object context)
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вставка графического объекта из буфера обмена на текущий чертеж
			/// </summary>
			/// <param name="context">Контекст вставки</param>
			//---------------------------------------------------------------------------------------------------------
			public virtual void Paste(System.Object context)
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Дубликат объекта
			/// </summary>
			/// <param name="context">Контекст дублирования объекта</param>
			/// <returns>Объект</returns>
			//---------------------------------------------------------------------------------------------------------
			public virtual ICadObject Duplicate(System.Object context)
			{
				CCadObject cad_object = new CCadObject();
				cad_object.CopyParameters(this, null);
				return (cad_object);
			}
			#endregion

			#region ======================================= МЕТОДЫ ПРОВЕРКИ ПОПАДАНИЙ =================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка на попадание в область графического объекта указанной точки
			/// </summary>
			/// <param name="point">Проверяемая точка</param>
			/// <param name="epsilon">Точность проверки</param>
			/// <returns>Статус проверки</returns>
			//---------------------------------------------------------------------------------------------------------
			public virtual Boolean CheckPoint(ref Vector2Df point, Single epsilon)
			{
				return (mBoundsRect.Contains(ref point));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка на попадание ВСЕЙ ГЕОМЕТРИИ графического объекта внутрь прямоугольной области
			/// </summary>
			/// <param name="rect">Прямоугольник</param>
			/// <returns>Статус проверки</returns>
			//---------------------------------------------------------------------------------------------------------
			public virtual Boolean CheckInsideRect(ref Rect2Df rect)
			{
				return (rect.Contains(mBoundsRect.PointTopLeft) &&
						rect.Contains(mBoundsRect.PointTopRight) &&
						rect.Contains(mBoundsRect.PointBottomLeft) &&
						rect.Contains(mBoundsRect.PointBottomRight));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка на попадание ВСЕЙ ИЛИ ЧАСТИ ГЕОМЕТРИИ графического объекта внутрь прямоугольной области
			/// </summary>
			/// <param name="rect">Прямоугольник</param>
			/// <returns>Статус проверки</returns>
			//---------------------------------------------------------------------------------------------------------
			public virtual Boolean CheckInsideOrIntersectRect(ref Rect2Df rect)
			{
				return (rect.Contains(mBoundsRect.PointTopLeft) ||
						rect.Contains(mBoundsRect.PointTopRight) ||
						rect.Contains(mBoundsRect.PointBottomLeft) ||
						rect.Contains(mBoundsRect.PointBottomRight));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка на видимость всей геометрии графического объекта в области просмотра
			/// </summary>
			/// <returns>Статус видимости</returns>
			//---------------------------------------------------------------------------------------------------------
			public virtual Boolean CheckVisibleInViewport()
			{
				return (mCanvasViewer.CheckRectVisibleInViewport(ref mBoundsRect));
			}
			#endregion

			#region ======================================= МЕТОДЫ РИСОВАНИЯ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Абстрактный метод для рисования графического объекта
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public virtual void Draw()
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
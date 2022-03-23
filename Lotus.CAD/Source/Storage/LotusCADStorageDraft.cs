//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Объекты хранения данных
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADStorageDraft.cs
*		Чертеж для хранения графических объектов.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 27.03.2022
//=====================================================================================================================
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Specialized;
using System.Runtime.Serialization;
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
		//! \defgroup CadStorage Объекты хранения данных
		//! Объекты хранения данных обеспечиваю иерархическое хранение графических элементов и общих ресурсов.
		//! \ingroup CadDrawing
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Определение интерфейса чертежа как хранилища всех графических объектов
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public interface ICadDraft : ICadEntity
		{
			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Список графических объектов
			/// </summary>
			ListArray<ICadObject> Elements { get; }
			#endregion
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Чертеж для хранения графических объектов
		/// </summary>
		/// <remarks>
		/// Чертеж представляет собой логическое хранилище совокупности связанных графических объектов
		/// </remarks>
		//-------------------------------------------------------------------------------------------------------------
		[DataContract]
		public class CCadDraft : CCadEntity, ICadDraft, ILotusDocument, ILotusOwnerObject, ILotusOwnedObject,
			ILotusSupportViewInspector, IComparable<CCadDraft>
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			//
			// Константы для информирования об изменении свойств
			//
			protected static PropertyChangedEventArgs PropertyArgsFileName = new PropertyChangedEventArgs(nameof(FileName));
			protected static PropertyChangedEventArgs PropertyArgsPathFile = new PropertyChangedEventArgs(nameof(PathFile));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			// Основные параметры
			protected internal ListArray<ICadObject> mElements;

			// Параметры документа
			protected internal String mFileName;
			protected internal String mPathFile;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Список графических объектов
			/// </summary>
			[Browsable(false)]
			[DataMember]
			public ListArray<ICadObject> Elements
			{
				get
				{
					return (mElements);
				}
				set
				{
					mElements = value;
				}
			}

			/// <summary>
			/// Тип объекта модуля чертежной графики
			/// </summary>
			[DisplayName("Тип объекта")]
			[Description("Тип объекта модуля чертежной графики")]
			[Category(XInspectorGroupDesc.ID)]
			[Display(GroupName = XInspectorGroupDesc.ID, Order = 5)]
			public override TCadEntityType EntityType
			{
				get { return (TCadEntityType.Draft); }
			}


			//
			// ПОДДЕРЖКА ИНСПЕКТОРА СВОЙСТВ
			//
			/// <summary>
			/// Отображаемое имя типа в инспекторе свойств
			/// </summary>
			[Browsable(false)]
			public String InspectorTypeName
			{
				get { return ("ЧЕРТЕЖ"); }
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

			/// <summary>
			/// Обработчик события изменения коллекции
			/// </summary>
			[Browsable(false)]
			public override event NotifyCollectionChangedEventHandler CollectionChanged
			{
				add
				{
					mElements.CollectionChanged += value;
				}
				remove
				{
					mElements.CollectionChanged -= value;
				}
			}
			#endregion

			#region ======================================= СВОЙСТВА ILotusDocument ===================================
			/// <summary>
			/// Имя физического файла
			/// </summary>
			public String FileName
			{
				get { return (mFileName); }
				set
				{
					mFileName = value;
					NotifyPropertyChanged(PropertyArgsFileName);
				}
			}

			/// <summary>
			/// Путь до файла
			/// </summary>
			public String PathFile
			{
				get { return (mPathFile); }
				set
				{
					mPathFile = value;
					NotifyPropertyChanged(PropertyArgsPathFile);
				}
			}

			#endregion

			#region ======================================= СВОЙСТВА ILotusOwnedObject ================================
			/// <summary>
			/// Владелец объекта (проект)
			/// </summary>
			public ILotusOwnerObject IOwner 
			{
				get { return (Project); }
				set { Project = value as CCadProject; }
			}

			/// <summary>
			/// Владелец объекта (проект)
			/// </summary>
			public CCadProject Project { get; set; }
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public CCadDraft()
				: this("Новый чертеж")
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="name">Имя чертежа</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadDraft(String name)
				: base(name)
			{
				mElements = new ListArray<ICadObject>();
				mElements.IsNotify = true;
			}
			#endregion

			#region ======================================= СИСТЕМНЫЕ МЕТОДЫ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Сравнение чертежей для упорядочивания
			/// </summary>
			/// <param name="other">Сравниваемый чертеж</param>
			/// <returns>Статус сравнения чертежей</returns>
			//---------------------------------------------------------------------------------------------------------
			public Int32 CompareTo(CCadDraft other)
			{
				return (XCadDrawing.DefaultComprare(this, other));
			}
			#endregion

			#region ======================================= МЕТОДЫ ILotusViewItemBuilder ==============================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение количества дочерних узлов
			/// </summary>
			/// <returns>Количество дочерних узлов</returns>
			//---------------------------------------------------------------------------------------------------------
			public override Int32 GetCountChildrenNode()
			{
				return (mElements.Count);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение дочернего узла по индексу
			/// </summary>
			/// <param name="index">Индекс дочернего узла</param>
			/// <returns>Дочерней узел</returns>
			//---------------------------------------------------------------------------------------------------------
			public override System.Object GetChildrenNode(Int32 index)
			{
				return (mElements[index]);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка объекта на удовлетворение указанного предиката
			/// </summary>
			/// <remarks>
			/// Объект удовлетворяет условию предиката если хотя бы один его элемент удовлетворяет условию предиката
			/// </remarks>
			/// <param name="match">Предикат проверки</param>
			/// <returns>Статус проверки</returns>
			//---------------------------------------------------------------------------------------------------------
			public override Boolean CheckOne(Predicate<ICadEntity> match)
			{
				if (match(this))
				{
					return (true);
				}
				else
				{
					for (Int32 i = 0; i < mElements.Count; i++)
					{
						if (mElements[i].CheckOne(match))
						{
							return (true);
						}
					}
				}

				return (false);
			}
			#endregion

			#region ======================================= МЕТОДЫ ILotusDocument =====================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение расширения файла без точки
			/// </summary>
			/// <returns>Расширение файла без точки</returns>
			//---------------------------------------------------------------------------------------------------------
			public String GetFileExtension()
			{
				return (XFileExtension.JSON);
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
				// Присоединять можем только объекты
				if (owned_object is ICadObject cad_object)
				{
					// Если владелец есть
					if (owned_object.IOwner != null)
					{
						// И он не равен текущему
						if (owned_object.IOwner != this)
						{
							// Отсоединяем
							owned_object.IOwner.DetachOwnedObject(owned_object, add);
						}
					}

					if (add)
					{
						cad_object.IDraft = this;
						mElements.Add(cad_object);
					}
				}
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
				// Отсоединять можем только объекты
				if (owned_object is ICadObject cad_object)
				{
					owned_object.IOwner = null;

					if (remove)
					{
						// Ищем его
						Int32 index = mElements.IndexOf(cad_object);
						if (index != -1)
						{
							// Удаляем
							mElements.RemoveAt(index);
						}
					}
				}
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

			#region ======================================= МЕТОДЫ ИНТЕРФЕЙСОВ СОХРАНЕНИЯ/ЗАГРУЗКИ ====================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Подготовка элемента перед сохранением
			/// </summary>
			/// <param name="parameters">Параметры контекста</param>
			//---------------------------------------------------------------------------------------------------------
			public override void OnBeforeSave(CParameters parameters)
			{
				for (Int32 i = 0; i < mElements.Count; i++)
				{
					mElements[i].OnBeforeSave(parameters);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Восстановление параметров элемента после сохранения
			/// </summary>
			/// <param name="parameters">Параметры контекста</param>
			//---------------------------------------------------------------------------------------------------------
			public override void OnAfterSave(CParameters parameters)
			{
				for (Int32 i = 0; i < mElements.Count; i++)
				{
					mElements[i].OnAfterSave(parameters);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Подготовка элемента перед загрузкой
			/// </summary>
			/// <param name="parameters">Параметры контекста</param>
			//---------------------------------------------------------------------------------------------------------
			public override void OnBeforeLoad(CParameters parameters)
			{
				for (Int32 i = 0; i < mElements.Count; i++)
				{
					mElements[i].OnBeforeLoad(parameters);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Восстановление параметров элемента после загрузки
			/// </summary>
			/// <param name="parameters">Параметры контекста</param>
			//---------------------------------------------------------------------------------------------------------
			public override void OnAfterLoad(CParameters parameters)
			{
				for (Int32 i = 0; i < mElements.Count; i++)
				{
					mElements[i].OnAfterLoad(parameters);
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
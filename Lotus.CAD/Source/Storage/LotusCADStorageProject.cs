//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Объекты хранения данных
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADStorageProject.cs
*		Проект для хранения всех графических данных.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.01.2022
//=====================================================================================================================
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;
//---------------------------------------------------------------------------------------------------------------------
using Newtonsoft.Json;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
//=====================================================================================================================
namespace Lotus
{
	namespace CAD
	{
		//-------------------------------------------------------------------------------------------------------------
		//! \addtogroup CadStorage
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Проект для хранения всех графических данных
		/// </summary>
		/// <remarks>
		/// Проект хранит совокупность чертежей. Проект также может иметь и свои собственные графические объекты и
		/// графические ресурсы
		/// </remarks>
		//-------------------------------------------------------------------------------------------------------------
		[DataContract]
		public class CCadProject : CCadEntity, ILotusDocument, ILotusOwnerObject, ILotusSupportViewInspector
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
			protected internal ListArray<ICadDraft> mDrafts;

			// Параметры документа
			protected internal String mFileName;
			protected internal String mPathFile;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			//
			// ОСНОВНЫЕ ПАРАМЕТРЫ
			//
			/// <summary>
			/// Список чертежей проекта
			/// </summary>
			[Browsable(false)]
			[DataMember]
			public ListArray<ICadDraft> Drafts
			{
				get
				{
					return (mDrafts);
				}
				set
				{
					mDrafts = value;
				}
			}

			/// <summary>
			/// Первый чертеж проекта
			/// </summary>
			[Browsable(false)]
			public ICadDraft FirstDraft
			{
				get
				{
					return (mDrafts.ItemFirst);
				}
			}

			/// <summary>
			/// Последний чертеж проекта
			/// </summary>
			[Browsable(false)]
			public ICadDraft LastDraft
			{
				get
				{
					return (mDrafts.ItemLast);
				}
			}

			/// <summary>
			/// Тип объекта модуля чертежной графики
			/// </summary>
			[DisplayName("Тип объекта")]
			[Description("Тип объекта модуля чертежной графики")]
			[Category(XInspectorGroupDesc.ID)]
			public override TCadEntityType EntityType
			{
				get { return (TCadEntityType.Project); }
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
				get { return ("ПРОЕКТ"); }
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
					mDrafts.CollectionChanged += value;
				}
				remove
				{
					mDrafts.CollectionChanged -= value;
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

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public CCadProject()
				: this("Новый проект")
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="name">Имя проекта</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadProject(String name)
				: base(name)
			{
				mDrafts = new ListArray<ICadDraft>()
				{
					IsNotify = true
				};
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="name">Имя проекта</param>
			/// <param name="drafts">Список чертежей</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadProject(String name, params ICadDraft[] drafts)
				: base(name)
			{
				mDrafts = new ListArray<ICadDraft>(drafts)
				{
					IsNotify = true
				};
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="name">Имя проекта</param>
			/// <param name="draft_names">Список имен чертежей</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadProject(String name, params String[] draft_names)
				: base(name)
			{
				mDrafts = new ListArray<ICadDraft>(draft_names.Length)
				{
					IsNotify = true
				};

				for (Int32 i = 0; i < draft_names.Length; i++)
				{
					mDrafts.Add(new CCadDraft(draft_names[i]));
				}
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
				return (mDrafts.Count);
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
				return (mDrafts[index]);
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
					for (Int32 i = 0; i < mDrafts.Count; i++)
					{
						if (mDrafts[i].CheckOne(match))
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
			/// Получить коллекцию элементов отображения
			/// </summary>
			/// <returns>Иерархическая коллекция элементов отображения</returns>
			//---------------------------------------------------------------------------------------------------------
			public CCollectionViewHierarchyCadEntity GetCollectionViewHierarchy()
			{
				CCollectionViewHierarchyCadEntity view_project = new CCollectionViewHierarchyCadEntity();
				view_project.Source = this;
				return (view_project);
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
				for (Int32 i = 0; i < mDrafts.Count; i++)
				{
					mDrafts[i].OnBeforeSave(parameters);
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
				for (Int32 i = 0; i < mDrafts.Count; i++)
				{
					mDrafts[i].OnAfterSave(parameters);
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
				for (Int32 i = 0; i < mDrafts.Count; i++)
				{
					mDrafts[i].OnBeforeLoad(parameters);
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
				for (Int32 i = 0; i < mDrafts.Count; i++)
				{
					mDrafts[i].OnAfterLoad(parameters);
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
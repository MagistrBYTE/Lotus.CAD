//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Общая подсистема
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADLayerManager.cs
*		Статический класс менеджер для управления слоями.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.01.2022
//=====================================================================================================================
using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;
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
		//! \addtogroup CadDrawing
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Статический класс менеджер для управления слоями
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public static class XCadLayerManager
		{
			#region ======================================= ДАННЫЕ ====================================================
			internal static ListArray<CCadLayer> mLayers;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Текущий набор слоев
			/// </summary>
			public static ListArray<CCadLayer> Layers
			{
				get 
				{
					if (mLayers == null)
					{
						Init();
					}
					return (mLayers); 
				}
			}

			/// <summary>
			/// Текущий слой по умолчанию
			/// </summary>
			public static CCadLayer DefaultLayer
			{
				get 
				{
					if (mLayers == null)
					{
						Init();
					}
					return (mLayers[0]);
				}
			}
			#endregion

			#region ======================================= МЕТОДЫ ====================================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Первоначальная инициализация стандартных слоев
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public static void Init()
			{
				mLayers = new ListArray<CCadLayer>()
				{
					IsNotify = true
				};

				Add("Основной", "СИСТЕМА", 0);
				Add("Служебный", "СИСТЕМА", 1, false);
				Add("Оформление", "СИСТЕМА", 2);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Поиск слоя по имени
			/// </summary>
			/// <param name="name">Имя слоя</param>
			/// <returns>Найденный слой</returns>
			//---------------------------------------------------------------------------------------------------------
			public static CCadLayer GetFromName(String name)
			{
				CCadLayer layer = DefaultLayer;
				for (Int32 i = 0; i < mLayers.Count; i++)
				{
					if (mLayers[i].Name == name)
					{
						return (mLayers[i]);
					}
				}

				return (layer);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Поиск слоя по идентификатору
			/// </summary>
			/// <param name="id">Идентификатор слоя</param>
			/// <returns>Найденный слой</returns>
			//---------------------------------------------------------------------------------------------------------
			public static CCadLayer GetFromId(Int64 id)
			{
				CCadLayer layer = DefaultLayer;
				for (Int32 i = 0; i < mLayers.Count; i++)
				{
					if (mLayers[i].Id == id)
					{
						return (mLayers[i]);
					}
				}

				return (layer);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление параметров слоя
			/// </summary>
			/// <param name="layer">Слой</param>
			//---------------------------------------------------------------------------------------------------------
			public static void Update(CCadLayer layer)
			{
				//1) Проходим все проекты и перерисовываем элемент если он находится на данном слое и представлен на канве
				//for (Int32 ip = 0; ip < XManager.Projects.Count; ip++)
				//{
				//	ICadProject project = XManager.Projects[ip] as ICadProject;
				//	if (project != null)
				//	{
				//		//for (Int32 id = 0; id < project.Documents.Count; id++)
				//		//{
				//		//	ICadDraft draft = project.Documents[id] as ICadDraft;
				//		//	if (draft != null)
				//		//	{
				//		//		for (Int32 ie = 0; ie < draft.Elements.Count; ie++)
				//		//		{
				//		//			ICadLayerSupport element = draft.Elements[ie] as ICadLayerSupport;
				//		//			if (element != null)
				//		//			{
				//		//				element.UpdateLayer();
				//		//			}
				//		//		}
				//		//	}
				//		//}
				//	}
				//}

				//2) Проходим все документы и перерисовываем элемент если он находится на данном слое и представлен на канве
				//for (Int32 id = 0; id < XManager.Documents.Count; id++)
				//{
				//	ICadDraft draft = XManager.Documents[id] as ICadDraft;
				//	if (draft != null)
				//	{
				//		//for (Int32 ie = 0; ie < draft.Elements.Count; ie++)
				//		//{
				//		//	ICadLayerSupport element = draft.Elements[ie] as ICadLayerSupport;
				//		//	if (element != null)
				//		//	{
				//		//		element.UpdateLayer();
				//		//	}
				//		//}
				//	}
				//}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавление слоя
			/// </summary>
			/// <param name="name">Имя слоя</param>
			/// <returns>Добавленный слой</returns>
			//---------------------------------------------------------------------------------------------------------
			public static CCadLayer Add(String name)
			{
				CCadLayer layer = new CCadLayer(name);
				Layers.Add(layer);
				return (layer);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавление слоя
			/// </summary>
			/// <param name="name">Имя слоя</param>
			/// <param name="group">Группа слоя</param>
			/// <returns>Добавленный слой</returns>
			//---------------------------------------------------------------------------------------------------------
			public static CCadLayer Add(String name, String group)
			{
				CCadLayer layer = new CCadLayer(name);
				layer.Group = group;
				Layers.Add(layer);
				return (layer);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавление слоя
			/// </summary>
			/// <param name="name">Имя слоя</param>
			/// <param name="group">Группа слоя</param>
			/// <param name="id">Идентификатор слоя</param>
			/// <param name="is_print">Печать слоя</param>
			/// <returns>Добавленный слой</returns>
			//---------------------------------------------------------------------------------------------------------
			public static CCadLayer Add(String name, String group, Int32 id, Boolean is_print = true)
			{
				CCadLayer layer = new CCadLayer(name);
				layer.Group = group;
				layer.Id = id;
				layer.IsPrint = is_print;
				Layers.Add(layer);
				return (layer);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Удаления слоя
			/// </summary>
			/// <param name="name">Имя слоя</param>
			//---------------------------------------------------------------------------------------------------------
			public static void Remove(String name)
			{
				for (Int32 i = 0; i < mLayers.Count; i++)
				{
					if (mLayers[i].Name == name)
					{
						Remove(mLayers[i]);
						break;
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Удаления слоя
			/// </summary>
			/// <param name="id">Идентификатор слоя</param>
			//---------------------------------------------------------------------------------------------------------
			public static void Remove(Int64 id)
			{
				for (Int32 i = 0; i < mLayers.Count; i++)
				{
					if (mLayers[i].Id == id)
					{
						Remove(mLayers[i]);
						break;
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Удаления слоя
			/// </summary>
			/// <param name="layer">Слой</param>
			//---------------------------------------------------------------------------------------------------------
			public static void Remove(CCadLayer layer)
			{
				// 1) Если объект константный - выходим
				if (layer.IsConst) return;

				//1) Проходим все проекты и перерисовываем элемент если он находится на данном слое и представлен на канве
				//for (Int32 ip = 0; ip < XManager.Projects.Count; ip++)
				//{
				//	ICadProject project = XManager.Projects[ip] as ICadProject;
				//	if (project != null)
				//	{
				//		//for (Int32 id = 0; id < project.Documents.Count; id++)
				//		//{
				//		//	ICadDraft draft = project.Documents[id] as ICadDraft;
				//		//	if (draft != null)
				//		//	{
				//		//		for (Int32 ie = 0; ie < draft.Elements.Count; ie++)
				//		//		{
				//		//			ICadLayerSupport element = draft.Elements[ie] as ICadLayerSupport;
				//		//			if (element != null && element.Layer == layer)
				//		//			{
				//		//				element.SetLayer(DefaultLayer);
				//		//			}
				//		//		}
				//		//	}
				//		//}
				//	}
				//}

				//2) Проходим все документы и перерисовываем элемент если он находится на данном слое и представлен на канве
				//for (Int32 id = 0; id < XManager.Documents.Count; id++)
				//{
				//	ICadDraft draft = XManager.Documents[id] as ICadDraft;
				//	if (draft != null)
				//	{
				//		//for (Int32 ie = 0; ie < draft.Elements.Count; ie++)
				//		//{
				//		//	ICadLayerSupport element = draft.Elements[ie] as ICadLayerSupport;
				//		//	if (element != null && element.Layer == layer)
				//		//	{
				//		//		element.SetLayer(DefaultLayer);
				//		//	}
				//		//}
				//	}
				//}

				// 3) Удаляем слой
				mLayers.Remove(layer);
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
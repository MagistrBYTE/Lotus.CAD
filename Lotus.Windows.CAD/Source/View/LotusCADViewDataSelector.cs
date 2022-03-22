//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Общая подсистема
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADViewDataSelector.cs
*		Селекторы для выбора модели отображения данных.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.01.2022
//=====================================================================================================================
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
//---------------------------------------------------------------------------------------------------------------------
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
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
		/// Селектор шаблона данных для отображения иерархии элементов
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CCadEntityDataSelector : DataTemplateSelector
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			/// <summary>
			/// Глобальный экземпляр селектора
			/// </summary>
			public static readonly CCadEntityDataSelector Instance = Application.Current.Resources["CadEntityDataSelectorKey"] as CCadEntityDataSelector;
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			/// <summary>
			/// Шаблон для представления проекта
			/// </summary>
			public DataTemplate Project { get; set; }

			/// <summary>
			/// Шаблон для представления чертежа
			/// </summary>
			public DataTemplate Draft { get; set; }

			/// <summary>
			/// Шаблон для представления элемента
			/// </summary>
			public DataTemplate Element { get; set; }

			/// <summary>
			/// Шаблон для представления неизвестного узла
			/// </summary>
			public DataTemplate Unknow { get; set; }
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Выбор шаблона привязки данных
			/// </summary>
			/// <param name="item">Объект</param>
			/// <param name="container">Контейнер</param>
			/// <returns>Нужный шаблон</returns>
			//---------------------------------------------------------------------------------------------------------
			public override DataTemplate SelectTemplate(Object item, DependencyObject container)
			{
				if(item is ILotusViewItemHierarchy view_item_hierarchy)
				{
					if (view_item_hierarchy.DataContext is CCadProject)
					{
						return (Project);
					}

					if (view_item_hierarchy.DataContext is ICadDraft)
					{
						return (Draft);
					}

					if (view_item_hierarchy.DataContext is ICadShape)
					{
						return (Element);
					}
				}

				return (Unknow);
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
﻿//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Элементы управления
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusPenStyleManager.xaml.cs
*		Менеджер управления стилями перьев.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 27.03.2022
//=====================================================================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//---------------------------------------------------------------------------------------------------------------------
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Data;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
//=====================================================================================================================
namespace Lotus
{
	namespace CAD
	{
		//-------------------------------------------------------------------------------------------------------------
		//! \addtogroup CadControls
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Менеджер управления стилями перьев
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public partial class LotusPenStyleManager : UserControl
		{
			#region ======================================= ДАННЫЕ ====================================================
			private ListCollectionView mCollectionViewPenStyles;
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public LotusPenStyleManager()
			{
				InitializeComponent();

				if (XCadPenStyleManager.PenStyles == null) XCadPenStyleManager.Init();
				mCollectionViewPenStyles = new ListCollectionView(XCadPenStyleManager.PenStyles);
				mCollectionViewPenStyles.GroupDescriptions.Add(new PropertyGroupDescription(nameof(CCadPenStyle.Group)));
				dataPenStyles.ItemsSource = mCollectionViewPenStyles;
			}
			#endregion

			#region ======================================= УПРАВЛЕНИЕ СТИЛЯМИ ПЕРЬВ ==================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Выбор стиля пера для редактирования свойств
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnPenStyleManager_SelectionChanged(Object sender, SelectionChangedEventArgs args)
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Двойной щелчок по объекту - Выбор стиля пера для редактирования свойств
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnPenStyleManager_MouseDoubleClick(Object sender, MouseButtonEventArgs args)
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавление стиля пера к проекту
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnPenStyleManager_Add(Object sender, RoutedEventArgs args)
			{
				XCadPenStyleManager.Add("Новый стиль", "Пользовательские", new Single[] { 2, 2, 0, 2 }, -1);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Удаление стиля пера из проекта
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnPenStyleManager_Remove(Object sender, RoutedEventArgs args)
			{
				CCadPenStyle pen_style = mCollectionViewPenStyles.CurrentItem as CCadPenStyle;
				if (pen_style != null)
				{
					XCadPenStyleManager.Remove(pen_style);
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
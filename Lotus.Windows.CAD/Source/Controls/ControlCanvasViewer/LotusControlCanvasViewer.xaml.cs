//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Элементы управления
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusControlCanvasViewer.xaml.cs
*		Элемент пользовательского интерфейса для представления и управления канвой.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.01.2022
//=====================================================================================================================
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
using Lotus.Maths;
using Lotus.Windows;
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
		/// Элемент пользовательского интерфейса для представления и управления канвой
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public partial class LotusControlCanvasViewer : UserControl, INotifyPropertyChanged
		{
			#region ======================================= СВОЙСТВА ==================================================
			//
			// ГЛОБАЛЬНЫЕ ДАННЫЕ
			//
			/// <summary>
			/// Статус нахождения компонента в режиме разработки
			/// </summary>
			public static Boolean IsDesignMode
			{
				get
				{
					var prop = DesignerProperties.IsInDesignModeProperty;
					var isDesignMode = (Boolean)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;
					return isDesignMode;
				}
			}

			//
			// ОСНОВНЫЕ ПАРАМЕТРЫ
			//
			/// <summary>
			/// Элемент для управления канвой
			/// </summary>
			public LotusCadCanvasViewer Viewer
			{
				get { return (canvasViewer); }
			}

			/// <summary>
			/// Канва
			/// </summary>
			public LotusCadCanvas Canvas
			{
				get { return (canvas); }
			}

			/// <summary>
			/// Менеджер для отмены/повторения действия
			/// </summary>
			public ILotusMementoManager Memento 
			{
				get { return (canvasViewer.Memento); } 
			}

			//
			// ПОЗИЦИЯ И СМЕЩЕНИЕ КУРСОРА
			//
			/// <summary>
			/// Смещение курсора в координатах канвы
			/// </summary>
			public Vector2Df PointerDelta
			{
				get { return (canvasViewer.MouseDeltaCurrent); }
			}

			/// <summary>
			/// Позиция курсора мыши в координатах канвы
			/// </summary>
			public Vector2Df PointerPosition
			{
				get { return (canvasViewer.MousePositionCurrent); }
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public LotusControlCanvasViewer()
			{
				InitializeComponent();
			}
			#endregion

			#region ======================================= МЕТОДЫ РАБОТЫ С КОНТЕКСТНЫМ МЕНЮ ==========================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавление в контекстное меню команды
			/// </summary>
			/// <param name="command_name">Имя команды</param>
			//---------------------------------------------------------------------------------------------------------
			public void AddCommandContextMenu(String command_name)
			{
				MenuItem menu_item = new MenuItem();
				menu_item.Header = command_name;
				contextMenu.Items.Add(menu_item);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Удаление из контекстное меню команды
			/// </summary>
			/// <param name="command_name">Имя команды</param>
			//---------------------------------------------------------------------------------------------------------
			public void RemoveCommandContextMenu(String command_name)
			{
				for (Int32 i = 0; i < contextMenu.Items.Count; i++)
				{
					MenuItem menu_item = contextMenu.Items[i] as MenuItem;
					if (menu_item.Header.ToString() == command_name)
					{
						contextMenu.Items.RemoveAt(i);
						break;
					}
				}
			}
			#endregion

			#region ======================================= ОБРАБОТЧИКИ СОБЫТИЙ =======================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Открытие контекстного меню
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnContextMenu_Opened(Object sender, RoutedEventArgs args)
			{
				//for (Int32 i = 0; i < Selecting.SelectedElements.Count; i++)
				//{
				//	if(Selecting.SelectedElements[i] is ICadShape cadShape)
				//	{
				//		Vector2Df point = PointerPosition;
				//		cadShape.OnOpenContextMenu(ref point);
				//	}
				//}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Закрытие контекстного меню
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnContextMenu_Closed(Object sender, RoutedEventArgs args)
			{
				//for (Int32 i = 0; i < Selecting.SelectedElements.Count; i++)
				//{
				//	if (Selecting.SelectedElements[i] is ICadShape cadShape)
				//	{
				//		Vector2Df point = PointerPosition;
				//		cadShape.OnClosedContextMenu(ref point);
				//	}
				//}
			}
			#endregion

			#region ======================================= ДАННЫЕ INotifyPropertyChanged =============================
			/// <summary>
			/// Событие срабатывает ПОСЛЕ изменения свойства
			/// </summary>
			public event PropertyChangedEventHandler PropertyChanged;

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вспомогательный метод для нотификации изменений свойства
			/// </summary>
			/// <param name="property_name">Имя свойства</param>
			//---------------------------------------------------------------------------------------------------------
			public void NotifyPropertyChanged(String property_name = "")
			{
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs(property_name));
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вспомогательный метод для нотификации изменений свойства
			/// </summary>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			public void NotifyPropertyChanged(PropertyChangedEventArgs args)
			{
				if (PropertyChanged != null)
				{
					PropertyChanged(this, args);
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
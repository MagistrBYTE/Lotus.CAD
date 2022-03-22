//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Элементы управления
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusEditorSelectorPen.xaml.cs
*		Элемент-редактор для выбора пера.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.01.2022
//=====================================================================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//---------------------------------------------------------------------------------------------------------------------
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
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
		/// Элемент-редактор для выбора пера
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public partial class LotusEditorSelectorPen : ComboBox
		{
			#region ======================================= ОПРЕДЕЛЕНИЕ СВОЙСТВ ЗАВИСИМОСТИ ===========================
			public static readonly DependencyProperty ValueProperty =
					DependencyProperty.Register(nameof(Value), typeof(CCadPen), typeof(LotusEditorSelectorPen),
												new PropertyMetadata(null, Value_PropertyChanged));
			#endregion

			#region ======================================= МЕТОДЫ СВОЙСТВ ЗАВИСИМОСТИ ================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение пера
			/// </summary>
			/// <param name="obj">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private static void Value_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
			{
				LotusEditorSelectorPen selector = (LotusEditorSelectorPen)obj;
				CCadPen pen = args.NewValue as CCadPen;
				if (pen == null)
				{
					selector.SelectedIndex = 0;
				}
				else
				{
					for (Int32 i = 0; i < XCadPenManager.Pens.Count; i++)
					{
						if(XCadPenManager.Pens[i].Id == pen.Id)
						{
							selector.SelectedIndex = i;
							break;
						}

					}
				}
			}
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Перо
			/// </summary>
			public CCadPen Value
			{
				get { return (CCadPen)GetValue(ValueProperty); }
				set { SetValue(ValueProperty, value); }
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public LotusEditorSelectorPen()
			{
				InitializeComponent();
				SetResourceReference(StyleProperty, typeof(ComboBox));

				this.ItemsSource = XCadPenManager.Pens;
			}
			#endregion

			#region ======================================= ОБРАБОТЧИКИ СОБЫТИЙ =======================================
			//-------------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение выбора объекта
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//-------------------------------------------------------------------------------------------------------------
			private void OnComboBox_SelectionChanged(Object sender, SelectionChangedEventArgs args)
			{
				Value = SelectedItem as CCadPen;
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
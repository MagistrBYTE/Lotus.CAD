//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Элементы управления
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusEditorSelectorPenStyle.xaml.cs
*		Элемент-редактор для выбора стиля пера.
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
		/// Элемент-редактор для выбора стиля пера
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public partial class LotusEditorSelectorPenStyle : ComboBox
		{
			#region ======================================= ОПРЕДЕЛЕНИЕ СВОЙСТВ ЗАВИСИМОСТИ ===========================
			public static readonly DependencyProperty ValueProperty =
					DependencyProperty.Register(nameof(Value), typeof(CCadPenStyle), typeof(LotusEditorSelectorPenStyle),
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
				LotusEditorSelectorPenStyle selector = (LotusEditorSelectorPenStyle)obj;
				CCadPenStyle pen_style = args.NewValue as CCadPenStyle;
				if (pen_style == null)
				{
					selector.SelectedIndex = 0;
				}
				else
				{
					for (Int32 i = 0; i < XCadPenStyleManager.PenStyles.Count; i++)
					{
						if (XCadPenStyleManager.PenStyles[i].Id == pen_style.Id)
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
			/// Стиль пера
			/// </summary>
			public CCadPenStyle Value
			{
				get { return (CCadPenStyle)GetValue(ValueProperty); }
				set { SetValue(ValueProperty, value); }
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public LotusEditorSelectorPenStyle()
			{
				InitializeComponent();
				SetResourceReference(StyleProperty, typeof(ComboBox));

				this.ItemsSource = XCadPenStyleManager.PenStyles;
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
				Value = SelectedItem as CCadPenStyle;
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
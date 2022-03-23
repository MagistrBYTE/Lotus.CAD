//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Элементы управления
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusEditorSelectorFont.xaml.cs
*		Элемент-редактор для выбора шрифта.
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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.CAD;
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
		/// Элемент-редактор для выбора шрифта
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public partial class LotusEditorSelectorFont : ComboBox
		{
			#region ======================================= ОПРЕДЕЛЕНИЕ СВОЙСТВ ЗАВИСИМОСТИ ===========================
			public static readonly DependencyProperty ValueProperty =
					DependencyProperty.Register(nameof(Value), typeof(CCadFont), typeof(LotusEditorSelectorFont),
												new PropertyMetadata(null, Value_PropertyChanged));
			#endregion

			#region ======================================= МЕТОДЫ СВОЙСТВ ЗАВИСИМОСТИ ================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение кисти
			/// </summary>
			/// <param name="obj">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private static void Value_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
			{
				LotusEditorSelectorFont selector = (LotusEditorSelectorFont)obj;
				CCadFont font = args.NewValue as CCadFont;
				if (font == null)
				{
					selector.SelectedIndex = 0;
				}
				else
				{
					for (Int32 i = 0; i < XCadFontManager.Fonts.Count; i++)
					{
						if (XCadFontManager.Fonts[i].Id == font.Id)
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
			/// Шрифт
			/// </summary>
			public CCadDraft Value
			{
				get { return (CCadDraft)GetValue(ValueProperty); }
				set { SetValue(ValueProperty, value); }
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public LotusEditorSelectorFont()
			{
				InitializeComponent();
				SetResourceReference(StyleProperty, typeof(ComboBox));

				this.ItemsSource = XCadFontManager.Fonts;
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
				Value = SelectedItem as CCadDraft;
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
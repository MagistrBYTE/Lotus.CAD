//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Элементы управления
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusEditorImageSource.xaml.cs
*		Элемент-редактор для выбора источника изображения.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 27.03.2022
//=====================================================================================================================
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//---------------------------------------------------------------------------------------------------------------------
using System.Windows;
using System.Windows.Controls;
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
		/// Элемент-редактор для выбора источника изображения
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public partial class LotusEditorImageSource : UserControl
		{
			#region ======================================= ОПРЕДЕЛЕНИЕ СВОЙСТВ ЗАВИСИМОСТИ ===========================
			public static readonly DependencyProperty ValueProperty =
					DependencyProperty.Register(nameof(Value), typeof(TCadImageSource), typeof(LotusEditorImageSource),
												new FrameworkPropertyMetadata(TCadImageSource.Empty, ImageSource_PropertyChanged));
			#endregion

			#region ======================================= МЕТОДЫ СВОЙСТВ ЗАВИСИМОСТИ ================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение свойства зависимости
			/// </summary>
			/// <param name="obj">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private static void ImageSource_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
			{
				LotusEditorImageSource editor_source = obj as LotusEditorImageSource;
				TCadImageSource? value = ((TCadImageSource)(args.NewValue));
				editor_source.textFileName.SetValue(TextBox.TextProperty, Path.GetFileName(value.Value.PathData));
			}
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Источник данных для изображения
			/// </summary>
			public TCadImageSource Value
			{
				get { return (TCadImageSource)GetValue(ValueProperty); }
				set { SetValue(ValueProperty, value); }
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public LotusEditorImageSource()
			{
				InitializeComponent();
				SetResourceReference(StyleProperty, typeof(UserControl));
			}
			#endregion

			#region ======================================= ОБРАБОТЧИКИ СОБЫТИЙ =======================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Открытие диалога выбора файла
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnOpenFileDialog(Object sender, RoutedEventArgs args)
			{
				// Конфигурация диалога
				Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
				dlg.DefaultExt = ".jpg";
				dlg.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";

				// Показываем диалог открытия
				Nullable<Boolean> result = dlg.ShowDialog();

				// Если успешно
				if (result == true)
				{
					textFileName.Text = Path.GetFileName(dlg.FileName);
					Value = new TCadImageSource(dlg.FileName);
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
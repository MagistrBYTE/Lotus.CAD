//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Элементы управления
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusEditorSelectorLayer.xaml.cs
*		Элемент-редактор для выбора слоя.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 27.03.2022
//=====================================================================================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
//---------------------------------------------------------------------------------------------------------------------
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
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
		/// Конвертер типа <see cref="TCadLayerVisibleMode"/> в соответствующую графическую пиктограмму
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public sealed class VisibleModeToImageConverter : IValueConverter
		{
			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Пиктограмма для слоя когда графические элементы слоя видны (за исключением тех у 
			/// которых свойство IsVisible равно false)
			/// </summary>
			public BitmapSource Visible { get; set; }

			/// <summary>
			/// Пиктограмма для слоя когда все графические элементы слоя видны (даже те у которых 
			/// свойство IsVisible равно false)
			/// </summary>
			public BitmapSource VisibleAll { get; set; }

			/// <summary>
			/// Пиктограмма для слоя когда графические элементы слоя скрыты
			/// </summary>
			public BitmapSource Hidden { get; set; }

			/// <summary>
			/// Пиктограмма для слоя когда  видны только графические элементы у которых свойство IsVisible равно false
			/// </summary>
			public BitmapSource VisibleIsHidden { get; set; }
			#endregion

			#region ======================================= МЕТОДЫ ====================================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертер типа TCadCadLayerVisibleMode в соответствующую графическую пиктограмму
			/// </summary>
			/// <param name="value">Значение</param>
			/// <param name="target_type">Целевой тип</param>
			/// <param name="parameter">Дополнительный параметр</param>
			/// <param name="culture">Культура</param>
			/// <returns>Графическая пиктограмма</returns>
			//---------------------------------------------------------------------------------------------------------
			public Object Convert(Object value, Type target_type, Object parameter, CultureInfo culture)
			{
				TCadLayerVisibleMode val = (TCadLayerVisibleMode)value;
				switch (val)
				{
					case TCadLayerVisibleMode.Visible:
						{
							return (Visible);
						}
					case TCadLayerVisibleMode.VisibleAll:
						{
							return (VisibleAll);
						}
					case TCadLayerVisibleMode.Hidden:
						{
							return (Hidden);
						}
					case TCadLayerVisibleMode.VisibleIsHidden:
						{
							return (VisibleIsHidden);
						}
					default:
						break;
				}

				return (null);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертация графической пиктограммы в тип TCadLayerVisibleMode
			/// </summary>
			/// <param name="value">Значение</param>
			/// <param name="target_type">Целевой тип</param>
			/// <param name="parameter">Дополнительный параметр</param>
			/// <param name="culture">Культура</param>
			/// <returns>Тип CadLayerVisibleMode</returns>
			//---------------------------------------------------------------------------------------------------------
			public Object ConvertBack(Object value, Type target_type, Object parameter, CultureInfo culture)
			{
				return (null);
			}
			#endregion
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Конвертер типа <see cref="TCadLayerHalftoneMode"/> в соответствующую графическую пиктограмму
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public sealed class HalftoneModeToImageConverter : IValueConverter
		{
			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Пиктограмма для слоя когда полутон отключен, цвет зависит от графического объекта
			/// </summary>
			public BitmapSource Disable { get; set; }

			/// <summary>
			/// Пиктограмма для слоя когда полутон цвета графического объекта
			/// </summary>
			public BitmapSource Halftone { get; set; }

			/// <summary>
			/// Пиктограмма для слоя когда цвет всех графических элементов - полутон серого цвета
			/// </summary>
			public BitmapSource HalftoneGray { get; set; }
			#endregion

			#region ======================================= МЕТОДЫ ====================================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертер типа TCadLayerHalftoneMode в соответствующую графическую пиктограмму
			/// </summary>
			/// <param name="value">Значение</param>
			/// <param name="target_type">Целевой тип</param>
			/// <param name="parameter">Дополнительный параметр</param>
			/// <param name="culture">Культура</param>
			/// <returns>Графическая пиктограмма</returns>
			//---------------------------------------------------------------------------------------------------------
			public Object Convert(Object value, Type target_type, Object parameter, CultureInfo culture)
			{
				TCadLayerHalftoneMode val = (TCadLayerHalftoneMode)value;
				switch (val)
				{
					case TCadLayerHalftoneMode.Disable:
						{
							return (Disable);
						}
					case TCadLayerHalftoneMode.Halftone:
						{
							return (Halftone);
						}
					case TCadLayerHalftoneMode.HalftoneGray:
						{
							return (HalftoneGray);
						}
					default:
						break;
				}

				return (null);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конвертация графической пиктограммы в тип TCadLayerHalftoneMode
			/// </summary>
			/// <param name="value">Значение</param>
			/// <param name="target_type">Целевой тип</param>
			/// <param name="parameter">Дополнительный параметр</param>
			/// <param name="culture">Культура</param>
			/// <returns>Тип CadLayerHalftoneMode</returns>
			//---------------------------------------------------------------------------------------------------------
			public Object ConvertBack(Object value, Type target_type, Object parameter, CultureInfo culture)
			{
				return (null);
			}
			#endregion
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Элемент-редактор для выбора слоя
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public partial class LotusEditorSelectorLayer : ComboBox
		{
			#region ======================================= ОПРЕДЕЛЕНИЕ СВОЙСТВ ЗАВИСИМОСТИ ===========================
			public static readonly DependencyProperty ValueProperty =
					DependencyProperty.Register(nameof(Value), typeof(CCadLayer), typeof(LotusEditorSelectorLayer),
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
				LotusEditorSelectorLayer selector = (LotusEditorSelectorLayer)obj;
				CCadLayer layer = args.NewValue as CCadLayer;
				if (layer == null)
				{
					selector.SelectedIndex = 0;
				}
				else
				{
					for (Int32 i = 0; i < XCadLayerManager.Layers.Count; i++)
					{
						if (XCadLayerManager.Layers[i].Id == layer.Id)
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
			/// Слой
			/// </summary>
			public CCadLayer Value
			{
				get { return (CCadLayer)GetValue(ValueProperty); }
				set { SetValue(ValueProperty, value); }
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public LotusEditorSelectorLayer()
			{
				InitializeComponent();
				SetResourceReference(StyleProperty, typeof(ComboBox));

				this.ItemsSource = XCadLayerManager.Layers;
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
				Value = SelectedItem as CCadLayer;
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
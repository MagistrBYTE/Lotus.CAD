//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Ресурсы для отображения графического объекта
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADVisualFontEntity.cs
*		Шрифт как совокупность параметров начертания текста.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.01.2022
//=====================================================================================================================
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
		//! \addtogroup CadVisual
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Шрифт как совокупность параметров начертания текста
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CCadFont : CCadEntity, IComparable<CCadFont>, ILotusSupportViewInspector
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			protected static PropertyChangedEventArgs PropertyArgsGroup = new PropertyChangedEventArgs(nameof(Group));
			protected static PropertyChangedEventArgs PropertyArgsFamily = new PropertyChangedEventArgs(nameof(Family));
			protected static PropertyChangedEventArgs PropertyArgsStyle = new PropertyChangedEventArgs(nameof(Style));
			protected static PropertyChangedEventArgs PropertyArgsStretch = new PropertyChangedEventArgs(nameof(Stretch));
			protected static PropertyChangedEventArgs PropertyArgsWeight = new PropertyChangedEventArgs(nameof(Weight));
			protected static PropertyChangedEventArgs PropertyArgsWindowsFont = new PropertyChangedEventArgs("WindowsFont");
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			// Общие данные
			internal String mGroup;

			// Основные параметры
			internal String mFamily;
			internal TCadFontStyle mStyle;
			internal TCadFontStretch mStretch;
			internal TCadFontWeight mWeight;

			// Платформенные-зависимые данные
#if USE_WINDOWS
			internal System.Windows.Media.Typeface mWindowsFont;
#endif
#if USE_GDI
			internal System.Drawing.Font mDrawingFont;
#endif
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			//
			// ОБЩИЕ ДАННЫЕ
			//
			/// <summary>
			/// Логическая группа которой принадлежит шрифт
			/// </summary>
			[DisplayName("Группа")]
			[Description("Логическая группа которой принадлежит шрифт")]
			[Category(XInspectorGroupDesc.ID)]
			[Display(GroupName = XInspectorGroupDesc.ID, Order = 1)]
			public String Group
			{
				get { return (mGroup); }
				set
				{
					mGroup = value;

					// 1) Информируем об изменении
					NotifyPropertyChanged(PropertyArgsGroup);

					// 2) Обновляем
					RaiseGroupChanged();
				}
			}

			/// <summary>
			/// Тип сущности модуля чертежной графики
			/// </summary>
			[DisplayName("Тип объекта")]
			[Description("Тип сущности модуля чертежной графики")]
			[Category(XInspectorGroupDesc.ID)]
			[Display(GroupName = XInspectorGroupDesc.ID, Order = 3)]
			public override TCadEntityType EntityType
			{
				get { return (TCadEntityType.Font); }
			}

			//
			// ОСНОВНЫЕ ПАРАМЕТРЫ
			//
			/// <summary>
			/// Имя семейства шрифтов
			/// </summary>
			[DisplayName("Семейство")]
			[Description("Имя семейства шрифтов")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 0)]
			public String Family
			{
				get { return (mFamily); }
				set
				{
					mFamily = value;

					// 1) Информируем об изменении
					NotifyPropertyChanged(PropertyArgsFamily);

					// 2) Обновляем
					RaiseParametrsChanged();
				}
			}

			/// <summary>
			/// Наклон шрифта
			/// </summary>
			[DisplayName("Наклон")]
			[Description("Наклон шрифта")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 1)]
			public TCadFontStyle Style
			{
				get { return (mStyle); }
				set
				{
					if (mStyle != value)
					{
						mStyle = value;

						// 1) Информируем об изменении
						NotifyPropertyChanged(PropertyArgsStyle);

						// 2) Обновляем
						RaiseParametrsChanged();
					}
				}
			}

			/// <summary>
			/// Коэффициент растяжения/сжатия шрифта
			/// </summary>
			[DisplayName("Сжатие")]
			[Description("Коэффициент растяжения/сжатия шрифта")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 2)]
			public TCadFontStretch Stretch
			{
				get { return (mStretch); }
				set
				{
					if (mStretch != value)
					{
						mStretch = value;

						// 1) Информируем об изменении
						NotifyPropertyChanged(PropertyArgsStretch);

						// 2) Обновляем
						RaiseParametrsChanged();
					}
				}
			}

			/// <summary>
			/// Плотность шрифта
			/// </summary>
			[DisplayName("Плотность")]
			[Description("Плотность шрифта")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 3)]
			public TCadFontWeight Weight
			{
				get { return (mWeight); }
				set
				{
					if (mWeight != value)
					{
						mWeight = value;

						// 1) Информируем об изменении
						NotifyPropertyChanged(PropertyArgsWeight);

						// 2) Обновляем
						RaiseParametrsChanged();
					}
				}
			}
		
#if USE_WINDOWS
			/// <summary>
			/// Шрифт WPF
			/// </summary>
			[Browsable(false)]
			public System.Windows.Media.Typeface WindowsFont
			{
				get { return (mWindowsFont); }
			}
#endif
#if USE_GDI
			/// <summary>
			/// Шрифт Drawing
			/// </summary>
			[Browsable(false)]
			public System.Drawing.Font DrawingFont
			{
				get { return (mDrawingFont); }
			}
#endif
#if USE_SHARPDX
			/// <summary>
			/// Шрифт SharpDX.Direct2D
			/// </summary>
			//[Browsable(false)]
			//public SharpDX.Direct2D1.Brush D2DBrush
			//{
			//	get { return (mD2DBrush); }
			//}
#endif
			//
			// ПОДДЕРЖКА ИНСПЕКТОРА СВОЙСТВ
			//
			/// <summary>
			/// Отображаемое имя типа в инспекторе свойств
			/// </summary>
			[Browsable(false)]
			public virtual String InspectorTypeName
			{
				get { return ("ШРИФТ"); }
			}

			/// <summary>
			/// Отображаемое имя объекта в инспекторе свойств
			/// </summary>
			[Browsable(false)]
			public virtual String InspectorObjectName
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
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public CCadFont()
				: this("Шрифт 1", "Arial", TCadFontStyle.Normal, TCadFontStretch.Normal, TCadFontWeight.Normal)
			{
				
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="name">Имя шрифта</param>
			/// <param name="family">Имя семейства шрифта</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadFont(String name, String family)
				: this(name, family, TCadFontStyle.Normal, TCadFontStretch.Normal, TCadFontWeight.Normal)
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="name">Имя шрифта</param>
			/// <param name="family">Имя семейства шрифта</param>
			/// <param name="font_style">Наклон шрифта</param>
			/// <param name="font_stretch">Коэффициент растяжения или сжатия шрифта</param>
			/// <param name="font_weight">Плотность шрифта</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadFont(String name, String family, TCadFontStyle font_style,
				TCadFontStretch font_stretch, TCadFontWeight font_weight)
				: base(name)
			{
				mFamily = family;
				mStyle = font_style;
				mStretch = font_stretch;
				mWeight = font_weight;
				RaiseParametrsChanged();
			}
			#endregion

			#region ======================================= СИСТЕМНЫЕ МЕТОДЫ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Сравнение шрифтов для упорядочивания
			/// </summary>
			/// <param name="other">Сравниваемый шрифт</param>
			/// <returns>Статус сравнения шрифтов</returns>
			//---------------------------------------------------------------------------------------------------------
			public Int32 CompareTo(CCadFont other)
			{
				return (XCadDrawing.DefaultComprare(this, other));
			}
			#endregion

			#region ======================================= СЛУЖЕБНЫЕ МЕТОДЫ ==========================================
#if USE_WINDOWS
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение стиля шрифта по WPF
			/// </summary>
			/// <returns>Стиль шрифта по WPF</returns>
			//---------------------------------------------------------------------------------------------------------
			internal System.Windows.FontStyle GetWindowsFontStyle()
			{
				System.Windows.FontStyle font_style = System.Windows.FontStyles.Normal;
				switch (mStyle)
				{
					case TCadFontStyle.Normal:
						font_style = System.Windows.FontStyles.Normal;
						break;
					case TCadFontStyle.Oblique:
						font_style = System.Windows.FontStyles.Oblique;
						break;
					case TCadFontStyle.Italic:
						font_style = System.Windows.FontStyles.Italic;
						break;
					default:
						break;
				}

				return (font_style);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение коэффициента растяжения/сжатия шрифта по WPF
			/// </summary>
			/// <returns>Коэффициент растяжения/сжатия шрифта по WPF</returns>
			//---------------------------------------------------------------------------------------------------------
			internal System.Windows.FontStretch GetWindowsFontStretch()
			{
				System.Windows.FontStretch font_stretch = System.Windows.FontStretches.Normal;

				switch (mStretch)
				{
					case TCadFontStretch.Undefined:
						font_stretch = System.Windows.FontStretches.Normal;
						break;
					case TCadFontStretch.UltraCondensed:
						font_stretch = System.Windows.FontStretches.UltraCondensed;
						break;
					case TCadFontStretch.ExtraCondensed:
						font_stretch = System.Windows.FontStretches.ExtraCondensed;
						break;
					case TCadFontStretch.Condensed:
						font_stretch = System.Windows.FontStretches.Condensed;
						break;
					case TCadFontStretch.SemiCondensed:
						font_stretch = System.Windows.FontStretches.SemiCondensed;
						break;
					case TCadFontStretch.Normal:
						font_stretch = System.Windows.FontStretches.Normal;
						break;
					case TCadFontStretch.Medium:
						font_stretch = System.Windows.FontStretches.Medium;
						break;
					case TCadFontStretch.SemiExpanded:
						font_stretch = System.Windows.FontStretches.SemiExpanded;
						break;
					case TCadFontStretch.Expanded:
						font_stretch = System.Windows.FontStretches.Expanded;
						break;
					case TCadFontStretch.ExtraExpanded:
						font_stretch = System.Windows.FontStretches.ExtraExpanded;
						break;
					case TCadFontStretch.UltraExpanded:
						font_stretch = System.Windows.FontStretches.UltraExpanded;
						break;
					default:
						break;
				}

				return (font_stretch);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение плотности шрифта по WPF
			/// </summary>
			/// <returns>Плотность шрифта по WPF</returns>
			//---------------------------------------------------------------------------------------------------------
			internal System.Windows.FontWeight GetWindowsFontWeight()
			{
				System.Windows.FontWeight font_weight = System.Windows.FontWeights.Normal;

				switch (mWeight)
				{
					case TCadFontWeight.Thin:
						font_weight = System.Windows.FontWeights.Thin;
						break;
					case TCadFontWeight.ExtraLight:
						font_weight = System.Windows.FontWeights.ExtraLight;
						break;
					case TCadFontWeight.Light:
						font_weight = System.Windows.FontWeights.Light;
						break;
					case TCadFontWeight.SemiLight:
						font_weight = System.Windows.FontWeights.Light;
						break;
					case TCadFontWeight.Normal:
						font_weight = System.Windows.FontWeights.Normal;
						break;
					case TCadFontWeight.Medium:
						font_weight = System.Windows.FontWeights.Medium;
						break;
					case TCadFontWeight.DemiBold:
						font_weight = System.Windows.FontWeights.DemiBold;
						break;
					case TCadFontWeight.Bold:
						font_weight = System.Windows.FontWeights.Bold;
						break;
					case TCadFontWeight.ExtraBold:
						font_weight = System.Windows.FontWeights.ExtraBold;
						break;
					case TCadFontWeight.Black:
						font_weight = System.Windows.FontWeights.Black;
						break;
					case TCadFontWeight.ExtraBlack:
						font_weight = System.Windows.FontWeights.ExtraBlack;
						break;
					default:
						break;
				}

				return (font_weight);
			}
#endif
			#endregion

			#region ======================================= СЛУЖЕБНЫЕ МЕТОДЫ СОБЫТИЙ ==================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение имени шрифта.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected override void RaiseNameChanged()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение логической группы шрифта.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseGroupChanged()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение параметров шрифта.
			/// Метод автоматически вызывается после установки соответствующих свойств
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseParametrsChanged()
			{
				Update();
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Общие обновление шрифта
			/// </summary>
			/// <remarks>
			/// Применяется когда надо обновить внутренние ресурсы шрифта по напрямую заданным параметрам шрифта, 
			/// минуя механизм свойств, например при загрузке или создании
			/// </remarks>
			//---------------------------------------------------------------------------------------------------------
			public virtual void Update()
			{
#if USE_WINDOWS
				UpdateWindowsResource();
#endif
#if USE_GDI
				UpdateDrawingResource();
#endif
#if USE_SHARPDX
				UpdateDirect2DResource(true);
#endif
			}

#if USE_WINDOWS


			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление ресурса WPF
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public virtual void UpdateWindowsResource()
			{
				mWindowsFont = new System.Windows.Media.Typeface(new System.Windows.Media.FontFamily(mFamily),
					GetWindowsFontStyle(),
					GetWindowsFontWeight(),
					GetWindowsFontStretch());

				NotifyPropertyChanged(PropertyArgsWindowsFont);
			}
#endif
#if USE_GDI
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление ресурса System.Drawing
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public virtual void UpdateDrawingResource()
			{
				mDrawingFont = new System.Drawing.Font(mFamily, 12);
			}
#endif
#if USE_SHARPDX
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление ресурса Direct2D
			/// </summary>
			/// <param name="forced">Принудительное создание ресурса Direct2D</param>
			//---------------------------------------------------------------------------------------------------------
			public virtual void UpdateDirect2DResource(Boolean forced = false)
			{
			}
#endif

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Копирования шрифта
			/// </summary>
			/// <returns>Копия шрифта со всеми параметрами и данными</returns>
			//---------------------------------------------------------------------------------------------------------
			public virtual CCadFont Duplicate()
			{
				CCadFont obj = (CCadFont)MemberwiseClone();
				obj.RaiseParametrsChanged();
				return (obj);
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Ресурсы для отображения графического объекта
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADVisualFontManager.cs
*		Статический класс - менеджер для управления шрифтами.
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
		/// Статический класс - менеджер для управления шрифтами
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public static class XCadFontManager
		{
			#region ======================================= ДАННЫЕ ====================================================
			internal static ListArray<CCadFont> mFonts;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Текущий набор шрифтов
			/// </summary>
			public static ListArray<CCadFont> Fonts
			{
				get { return (mFonts); }
			}

			/// <summary>
			/// Текущий шрифт по умолчанию
			/// </summary>
			public static CCadFont DefaultFont
			{
				get { return (mFonts[0]); }
			}

			/// <summary>
			/// Шрифт Arial
			/// </summary>
			public static CCadFont Arial
			{
				get { return (mFonts[0]); }
			}

			/// <summary>
			/// Шрифт Verdana
			/// </summary>
			public static CCadFont Verdana
			{
				get { return (mFonts[1]); }
			}
			#endregion

			#region ======================================= МЕТОДЫ ====================================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Первоначальная инициализация стандартных шрифтов
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public static void Init()
			{
				// 1) Инициализируем базу шрифтов
				mFonts = new ListArray<CCadFont>()
				{
					IsNotify = true
				};

				// 2) Заполняем шрифты
				AddFont("Arial", "Стандартные", "Arial", TCadFontStyle.Normal, TCadFontStretch.Normal, TCadFontWeight.Normal, 0);
				AddFont("Verdana", "Стандартные", "Verdana", TCadFontStyle.Normal, TCadFontStretch.Normal, TCadFontWeight.Normal, 1);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Поиск шрифта по имени
			/// </summary>
			/// <param name="name">Имя шрифта</param>
			/// <returns>Найденный шрифт или шрифт по умолчанию</returns>
			//---------------------------------------------------------------------------------------------------------
			public static CCadFont GetFromName(String name)
			{
				CCadFont font = DefaultFont;
				for (Int32 i = 0; i < mFonts.Count; i++)
				{
					if (mFonts[i].Name == name)
					{
						return (mFonts[i]);
					}
				}

				return (font);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Поиск шрифта по идентификатору
			/// </summary>
			/// <param name="id">Идентификатор шрифта</param>
			/// <returns>Найденный шрифт или шрифт по умолчанию</returns>
			//---------------------------------------------------------------------------------------------------------
			public static CCadFont GetFromId(Int64 id)
			{
				CCadFont font = DefaultFont;
				for (Int32 i = 0; i < mFonts.Count; i++)
				{
					if (mFonts[i].Id == id)
					{
						return (mFonts[i]);
					}
				}

				return (font);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавление шрифта с указанными параметрами
			/// </summary>
			/// <param name="name">Имя шрифта</param>
			/// <param name="group">Имя группы</param>
			/// <param name="family">Имя семейства шрифта</param>
			/// <param name="font_style">Наклон шрифта</param>
			/// <param name="font_stretch">Коэффициент растяжения или сжатия шрифта</param>
			/// <param name="font_weight">Плотность шрифта</param>
			/// <returns>Шрифт</returns>
			//---------------------------------------------------------------------------------------------------------
			public static CCadFont AddFont(String name, String group, String family, TCadFontStyle font_style,
				TCadFontStretch font_stretch, TCadFontWeight font_weight)
			{
				// Создаем шрифт
				CCadFont font = new CCadFont(name, family, font_style, font_stretch, font_weight);
				font.mGroup = group;

				// Добавляем
				mFonts.Add(font);

				return (font);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавление шрифта с указанными параметрами
			/// </summary>
			/// <param name="name">Имя шрифта</param>
			/// <param name="group">Имя группы</param>
			/// <param name="family">Имя семейства шрифта</param>
			/// <param name="font_style">Наклон шрифта</param>
			/// <param name="font_stretch">Коэффициент растяжения или сжатия шрифта</param>
			/// <param name="font_weight">Плотность шрифта</param>
			/// <param name="id">Идентификатор шрифта</param>
			/// <returns>Шрифт</returns>
			//---------------------------------------------------------------------------------------------------------
			public static CCadFont AddFont(String name, String group, String family, TCadFontStyle font_style,
				TCadFontStretch font_stretch, TCadFontWeight font_weight, Int32 id)
			{
				// Создаем шрифт
				CCadFont font = new CCadFont(name, family, font_style, font_stretch, font_weight);
				font.mGroup = group;
				font.Id = id;

				// Добавляем
				mFonts.Add(font);

				return (font);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Удаления шрифта
			/// </summary>
			/// <param name="name">Имя шрифта</param>
			//---------------------------------------------------------------------------------------------------------
			public static void Remove(String name)
			{
				for (Int32 i = 0; i < mFonts.Count; i++)
				{
					if (mFonts[i].Name == name)
					{
						Remove(mFonts[i]);
						break;
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Удаления шрифта
			/// </summary>
			/// <param name="id">Идентификатор шрифта</param>
			//---------------------------------------------------------------------------------------------------------
			public static void Remove(Int64 id)
			{
				for (Int32 i = 0; i < mFonts.Count; i++)
				{
					if (mFonts[i].Id == id)
					{
						Remove(mFonts[i]);
						break;
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Удаления шрифта
			/// </summary>
			/// <param name="font">Шрифт</param>
			//---------------------------------------------------------------------------------------------------------
			public static void Remove(CCadFont font)
			{
				// 1) Если объект константный - выходим
				if (font.IsConst) return;

				// 3) Удаляем шрифт
				mFonts.Remove(font);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление ресурсов Direct2D
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
#if USE_SHARPDX
			public static void UpdateDirect2DResources()
			{
				//for (Int32 i = 0; i < mBrushes.Count; i++)
				//{
				//	mBrushes[i].UpdateDirect2DResource(true);
				//}
			}
#endif
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
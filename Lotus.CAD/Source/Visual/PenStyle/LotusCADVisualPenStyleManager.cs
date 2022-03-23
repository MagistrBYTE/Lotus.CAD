//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Ресурсы для отображения графического объекта
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADVisualPenStyleManager.cs
*		Статический класс - менеджер для управления стилями перьев.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 27.03.2022
//=====================================================================================================================
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Xml;
using System.Linq;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
using Lotus.Maths;
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
		/// Статический класс - менеджер для управления стилями перьев
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public static class XCadPenStyleManager
		{
			#region ======================================= ДАННЫЕ ====================================================
			internal static ListArray<CCadPenStyle> mPenStyles;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Текущий набор стилей перьев
			/// </summary>
			public static ListArray<CCadPenStyle> PenStyles
			{
				get { return (mPenStyles); }
			}

			/// <summary>
			/// стиль пера по умолчанию
			/// </summary>
			public static CCadPenStyle DefaultStyle
			{
				get { return (mPenStyles[0]); }
			}

			/// <summary>
			/// Сплошная
			/// </summary>
			public static CCadPenStyle Solid
			{
				get { return (mPenStyles[0]); }
			}

			/// <summary>
			/// Штриховая
			/// </summary>
			public static CCadPenStyle Dash
			{
				get { return (mPenStyles[1]); }
			}

			/// <summary>
			/// Штрихпунктирная
			/// </summary>
			public static CCadPenStyle DashDot
			{
				get { return (mPenStyles[4]); }
			}

			/// <summary>
			/// Пунктирная
			/// </summary>
			public static CCadPenStyle Dot
			{
				get { return (mPenStyles[7]); }
			}

			/// <summary>
			/// Двойная штрихпунктирная
			/// </summary>
			public static CCadPenStyle DashDotDot
			{
				get { return (mPenStyles[10]); }
			}

			/// <summary>
			/// Стиль дорожной разметки 1.1
			/// </summary>
			public static CCadPenStyle RoadMarking_1_1
			{
				get { return (mPenStyles[13]); }
			}

			/// <summary>
			/// Стиль дорожной разметки 1.2.1
			/// </summary>
			public static CCadPenStyle RoadMarking_1_2_1
			{
				get { return (mPenStyles[14]); }
			}

			/// <summary>
			/// Стиль дорожной разметки 1.5
			/// </summary>
			public static CCadPenStyle RoadMarking_1_5
			{
				get { return (mPenStyles[15]); }
			}

			/// <summary>
			/// Стиль дорожной разметки 1.6
			/// </summary>
			public static CCadPenStyle RoadMarking_1_6
			{
				get { return (mPenStyles[16]); }
			}

			/// <summary>
			/// Стиль дорожной разметки 1.11
			/// </summary>
			public static CCadPenStyle RoadMarking_1_11
			{
				get { return (mPenStyles[17]); }
			}
			#endregion

			#region ======================================= МЕТОДЫ ====================================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Первоначальная инициализация стандартных стилей перьев
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public static void Init()
			{
				// 1) Инициализируем базу стилей перьев
				mPenStyles = new ListArray<CCadPenStyle>()
				{
					IsNotify = true
				};

				// 2) Заполняем стандартные стили
				Add("Сплошная", "Стандартные", null, 0);
				Add("Штриховая - 2", "Стандартные", new Single[] { 2, 2 }, 12);
				Add("Штриховая - 4", "Стандартные", new Single[] { 4, 4 }, 14);
				Add("Штриховая - 6", "Стандартные", new Single[] { 6, 6 }, 14);
				Add("Штрихпунктирная - 2", "Стандартные", new Single[] { 2, 2, 0, 2 }, 22);
				Add("Штрихпунктирная - 4", "Стандартные", new Single[] { 4, 4, 0, 4 }, 24);
				Add("Штрихпунктирная - 6", "Стандартные", new Single[] { 6, 6, 0, 6 }, 26);
				Add("Пунктирная - 2", "Стандартные", new Single[] { 0, 2 }, 32);
				Add("Пунктирная - 4", "Стандартные", new Single[] { 0, 4 }, 34);
				Add("Пунктирная - 6", "Стандартные", new Single[] { 0, 6 }, 36);
				Add("Двойная штрихпунктирная - 2", "Стандартные", new Single[] { 2, 2, 0, 2, 0, 2 }, 42);
				Add("Двойная штрихпунктирная - 4", "Стандартные", new Single[] { 4, 4, 0, 4, 0, 4 }, 44);
				Add("Двойная штрихпунктирная - 6", "Стандартные", new Single[] { 6, 6, 0, 6, 0, 6 }, 46);

				// 2) Заполняем стили для дорожной разметки
				Add("Линия разделения (1.1)", "Дорожная разметка", TCadStrokeLineCap.Square, null, 110);
				Add("Край проезжей части (1.2.1)", "Дорожная разметка", TCadStrokeLineCap.Square, null, 121);
				Add("Линия разделения (1.5)", "Дорожная разметка", TCadStrokeLineCap.Square, new Single[] { 4, 4 }, 150);
				Add("Линия приближения (1.6)", "Дорожная разметка", TCadStrokeLineCap.Square, new Single[] { 12, 4 }, 160);
				Add("Барьерная линия (1.11)", "Дорожная разметка", TCadStrokeLineCap.Square, new Single[] { 6, 3 }, 190);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Поиск стиля пера по имени
			/// </summary>
			/// <param name="name">Имя стиля пера</param>
			/// <returns>Найденный стиль пера или стиль по умолчанию</returns>
			//---------------------------------------------------------------------------------------------------------
			public static CCadPenStyle GetFromName(String name)
			{
				CCadPenStyle stroke_style = Solid;
				for (Int32 i = 0; i < mPenStyles.Count; i++)
				{
					if (mPenStyles[i].Name == name)
					{
						return (mPenStyles[i]);
					}
				}

				return (stroke_style);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Поиск стиля пера по идентификатору
			/// </summary>
			/// <param name="id">Идентификатор стиля пера</param>
			/// <returns>Найденный стиль пера или стиль по умолчанию</returns>
			//---------------------------------------------------------------------------------------------------------
			public static CCadPenStyle GetFromId(Int64 id)
			{
				CCadPenStyle stroke_style = Solid;
				for (Int32 i = 0; i < mPenStyles.Count; i++)
				{
					if (mPenStyles[i].Id == id)
					{
						return (mPenStyles[i]);
					}
				}

				return (stroke_style);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавление стиля пера с указанными параметрами
			/// </summary>
			/// <param name="name">Имя стиля пера</param>
			/// <param name="group">Имя группы</param>
			/// <param name="dash_pattern">Последовательность штрихов и пробелов</param>
			/// <param name="id">Идентификатор стиля пера</param>
			/// <returns>Стиль пера</returns>
			//---------------------------------------------------------------------------------------------------------
			public static CCadPenStyle Add(String name, String group, Single[] dash_pattern, Int32 id)
			{
				return (Add(name, group, TCadStrokeLineCap.Round, dash_pattern, id));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавление стиля пера с указанными параметрами
			/// </summary>
			/// <param name="name">Имя стиля пера</param>
			/// <param name="group">Имя группы</param>
			/// <param name="dash_cap">Фигура в конце/начале сегмента</param>
			/// <param name="dash_pattern">Последовательность штрихов и пробелов</param>
			/// <param name="id">Идентификатор стиля пера</param>
			/// <returns>Стиль пера</returns>
			//---------------------------------------------------------------------------------------------------------
			public static CCadPenStyle Add(String name, String group, TCadStrokeLineCap dash_cap, Single[] dash_pattern, Int32 id)
			{
				// Создаем стиль пера
				CCadPenStyle pen_style = new CCadPenStyle(name);
				pen_style.mGroup = group;
				pen_style.Id = id;
				if (dash_pattern != null)
				{
					pen_style.mDashPattern.AddRange(dash_pattern);
					pen_style.mDashCap = dash_cap;
				}

				// Обновляем стиль пера
				pen_style.Update();


				// Добавляем
				mPenStyles.Add(pen_style);

				return (pen_style);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Удаления стиля пера
			/// </summary>
			/// <param name="name">Имя стиля пера</param>
			//---------------------------------------------------------------------------------------------------------
			public static void Remove(String name)
			{
				for (Int32 i = 0; i < mPenStyles.Count; i++)
				{
					if (mPenStyles[i].Name == name)
					{
						Remove(mPenStyles[i]);
						break;
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Удаления стиля пера
			/// </summary>
			/// <param name="id">Идентификатор стиля пера</param>
			//---------------------------------------------------------------------------------------------------------
			public static void Remove(Int64 id)
			{
				for (Int32 i = 0; i < mPenStyles.Count; i++)
				{
					if (mPenStyles[i].Id == id)
					{
						Remove(mPenStyles[i]);
						break;
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Удаления стиля пера
			/// </summary>
			/// <param name="pen_style">Стиль пера</param>
			//---------------------------------------------------------------------------------------------------------
			public static void Remove(CCadPenStyle pen_style)
			{
				// 1) Если объект константный - выходим
				if (pen_style.IsConst) return;

				// 2) Проходим все перья и перекидываем используемый стиль пера на стиль пера по умолчанию
				for (Int32 i = 0; i < XCadPenManager.Pens.Count; i++)
				{
					if(XCadPenManager.Pens[i].PenStyle == pen_style)
					{
						XCadPenManager.Pens[i].PenStyle = DefaultStyle;
					}
				}

				// 3) Удаляем стиль пера
				mPenStyles.Remove(pen_style);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление ресурсов Direct2D
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
#if USE_SHARPDX
			public static void UpdateDirect2DResources()
			{
				for (Int32 i = 0; i < mPenStyles.Count; i++)
				{
					mPenStyles[i].UpdateDirect2DResource();
				}
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
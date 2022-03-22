//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Ресурсы для отображения графического объекта
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADVisualBrushManager.cs
*		Статический класс - менеджер для управления кистями.
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
		/// Статический класс - менеджер для управления кистями
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public static class XCadBrushManager
		{
			#region ======================================= ДАННЫЕ ====================================================
			internal static ListArray<CCadBrush> mBrushes;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Текущий набор кистей
			/// </summary>
			public static ListArray<CCadBrush> Brushes
			{
				get { return (mBrushes); }
			}

			/// <summary>
			/// Текущая кисть по умолчанию
			/// </summary>
			public static CCadBrush DefaultBrush
			{
				get { return (mBrushes[0]); }
			}

			/// <summary>
			/// Прозрачная кисть
			/// </summary>
			public static CCadBrush Transparent
			{
				get { return (null); }
			}

			/// <summary>
			/// Черная кисть
			/// </summary>
			public static CCadBrush Black
			{
				get { return (mBrushes[0]); }
			}

			/// <summary>
			/// Темно-серая кисть
			/// </summary>
			public static CCadBrush DarkGray
			{
				get { return (mBrushes[1]); }
			}

			/// <summary>
			/// Серая кисть
			/// </summary>
			public static CCadBrush Gray
			{
				get { return (mBrushes[2]); }
			}

			/// <summary>
			/// Светло-серая
			/// </summary>
			public static CCadBrush LightGray
			{
				get { return (mBrushes[3]); }
			}

			/// <summary>
			/// Белая
			/// </summary>
			public static CCadBrush White
			{
				get { return (mBrushes[4]); }
			}

			/// <summary>
			/// Красная
			/// </summary>
			public static CCadBrush Red
			{
				get { return (mBrushes[5]); }
			}

			/// <summary>
			/// Зеленая
			/// </summary>
			public static CCadBrush Green
			{
				get { return (mBrushes[6]); }
			}

			/// <summary>
			/// Синяя
			/// </summary>
			public static CCadBrush Blue
			{
				get { return (mBrushes[7]); }
			}

			/// <summary>
			/// Кисть для пешеходного ограждения
			/// </summary>
			public static CCadBrush RoadPedestrian_1
			{
				get { return (mBrushes[8]); }
			}

			/// <summary>
			/// Кисть для барьерного ограждения
			/// </summary>
			public static CCadBrush RoadBarrier_1
			{
				get { return (mBrushes[9]); }
			}

			/// <summary>
			/// Кисть для ограждения сигнальных столбиков
			/// </summary>
			public static CCadBrush RoadSignalingColumns_1
			{
				get { return (mBrushes[10]); }
			}
			#endregion

			#region ======================================= МЕТОДЫ ====================================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Первоначальная инициализация стандартных кистей
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public static void Init()
			{
				// 1) Инициализируем базу кистей
				mBrushes = new ListArray<CCadBrush>()
				{
					IsNotify = true
				};

				// 2) Заполняем кисти
				AddSolid("Черная", "Сплошные", TColor.Black, 0);
				AddSolid("Темно-серая", "Сплошные", TColor.DarkGray, 1);
				AddSolid("Серая", "Сплошные", TColor.Gray, 2);
				AddSolid("Светло-серая", "Сплошные", TColor.LightGray, 3);
				AddSolid("Белая", "Сплошные", TColor.White, 4);

				AddSolid("Красная", "Сплошные", TColor.Red, 20);
				AddSolid("Зеленая", "Сплошные", TColor.Green, 30);
				AddSolid("Синяя", "Сплошные", TColor.Blue, 40);

				AddHatch("Пешеходное ограждение", "Дорожная деятельность", "brushPedestrian_1", 70);
				AddHatch("Барьерное ограждение", "Дорожная деятельность", "brushBarrier_1", 71);
				AddHatch("Сигнальные столбики", "Дорожная деятельность", "brushSignalingColumns_1", 72);
				AddHatch("Разметка неровности", "Дорожная деятельность", "brushMarkingHump", 73);
				AddHatch("Обозначение неровности", "Дорожная деятельность", "brushHump", 74);

				AddImage("Диагональ (45)", "Штриховка", "brush_diagonal45", 100);
				AddImage("Диагональ (135)", "Штриховка","brush_diagonal135", 101);
				AddImage("Вертикаль", "Штриховка", "brush_vertical", 102);
				AddImage("Горизонталь", "Штриховка", "brush_horizontal", 103);
				AddImage("Шахматы (100)", "Штриховка", "brush_chess_large", 104);
				AddImage("Шахматы (50)", "Штриховка", "brush_chess_small", 105);
				AddImage("Крест (100)", "Штриховка", "brush_cross_large", 106);
				AddImage("Линия - окружность", "Штриховка", "brush_line_circle", 107);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Поиск кисти по имени
			/// </summary>
			/// <param name="name">Имя кисти</param>
			/// <returns>Найденная кисть или кисть по умолчанию</returns>
			//---------------------------------------------------------------------------------------------------------
			public static CCadBrush GetFromName(String name)
			{
				CCadBrush brush = DefaultBrush;
				for (Int32 i = 0; i < mBrushes.Count; i++)
				{
					if (mBrushes[i].Name == name)
					{
						return (mBrushes[i]);
					}
				}

				return (brush);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Поиск кисти по идентификатору
			/// </summary>
			/// <param name="id">Идентификатор кисти</param>
			/// <returns>Найденная кисть или кисть по умолчанию</returns>
			//---------------------------------------------------------------------------------------------------------
			public static CCadBrush GetFromId(Int64 id)
			{
				CCadBrush brush = DefaultBrush;
				for (Int32 i = 0; i < mBrushes.Count; i++)
				{
					if (mBrushes[i].Id == id)
					{
						return (mBrushes[i]);
					}
				}

				return (brush);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Поиск кисти по цвету
			/// </summary>
			/// <param name="color">Цвет кисти</param>
			/// <returns>Найденная кисть или кисть по умолчанию</returns>
			//---------------------------------------------------------------------------------------------------------
			public static CCadBrush GetFromColor(TColor color)
			{
				CCadBrush brush = DefaultBrush;
				for (Int32 i = 0; i < mBrushes.Count; i++)
				{
					if(mBrushes[i].BrushFill == TCadBrushFillType.Solid)
					{
						CCadBrushSolid solid_brush = mBrushes[i] as CCadBrushSolid;
						if(solid_brush.Color.Equals(color))
						{
							return (solid_brush);
						}
					}
				}

				CCadBrushSolid new_solid_brush = new CCadBrushSolid(color);
				mBrushes.Add(new_solid_brush);

				return (new_solid_brush);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавление сплошной кисти с указанными параметрами
			/// </summary>
			/// <param name="name">Имя кисти</param>
			/// <param name="group">Имя группы</param>
			/// <param name="color">Основной цвет кисти</param>
			/// <param name="id">Идентификатор кисти</param>
			/// <returns>Сплошная кисть</returns>
			//---------------------------------------------------------------------------------------------------------
			public static CCadBrushSolid AddSolid(String name, String group, TColor color, Int32 id)
			{
				// Создаем кисть
				CCadBrushSolid brush_solid = new CCadBrushSolid(name);
				brush_solid.Id = id;
				brush_solid.mGroup = group;
				brush_solid.mColor = color;

				// Обновляем кисть
				brush_solid.Update();

				// Добавляем
				mBrushes.Add(brush_solid);

				return (brush_solid);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавление кисти изображения с указанными параметрами
			/// </summary>
			/// <param name="name">Имя кисти</param>
			/// <param name="group">Имя группы</param>
			/// <param name="resource_name">Имя ресурса кисти</param>
			/// <param name="id">Идентификатор кисти</param>
			/// <returns>Кисть изображения</returns>
			//---------------------------------------------------------------------------------------------------------
			public static CCadBrushImage AddImage(String name, String group, String resource_name, Int32 id)
			{
				// Создаем кисть
				CCadBrushImage brush_image = new CCadBrushImage(name);
				brush_image.Id = id;
				brush_image.mGroup = group;
				brush_image.mResourceName = resource_name;

				// Обновляем кисть
				brush_image.Update();

				// Добавляем
				mBrushes.Add(brush_image);

				return (brush_image);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавление кисти штриховки с указанными параметрами
			/// </summary>
			/// <param name="name">Имя кисти</param>
			/// <param name="group">Имя группы</param>
			/// <param name="resource_name">Имя ресурса кисти</param>
			/// <param name="id">Идентификатор кисти</param>
			/// <returns>Кисть штриховки</returns>
			//---------------------------------------------------------------------------------------------------------
			public static CCadBrushHatch AddHatch(String name, String group, String resource_name, Int32 id)
			{
				// Создаем кисть
				CCadBrushHatch brush_hatch = new CCadBrushHatch(name);
				brush_hatch.Id = id;
				brush_hatch.mGroup = group;
				brush_hatch.mResourceName = resource_name;

				// Обновляем кисть
				brush_hatch.Update();

				// Добавляем
				mBrushes.Add(brush_hatch);

				return (brush_hatch);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Удаления кисти
			/// </summary>
			/// <param name="name">Имя кисти</param>
			//---------------------------------------------------------------------------------------------------------
			public static void Remove(String name)
			{
				for (Int32 i = 0; i < mBrushes.Count; i++)
				{
					if (mBrushes[i].Name == name)
					{
						Remove(mBrushes[i]);
						break;
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Удаления кисти
			/// </summary>
			/// <param name="id">Идентификатор кисти</param>
			//---------------------------------------------------------------------------------------------------------
			public static void Remove(Int64 id)
			{
				for (Int32 i = 0; i < mBrushes.Count; i++)
				{
					if (mBrushes[i].Id == id)
					{
						Remove(mBrushes[i]);
						break;
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Удаления кисти
			/// </summary>
			/// <param name="brush">Кисть</param>
			//---------------------------------------------------------------------------------------------------------
			public static void Remove(CCadBrush brush)
			{
				// 1) Если объект константный - выходим
				if (brush.IsConst) return;

				// 2) Проходим все проекты и перекидываем используемую кисть на кисть по умолчанию
				//for (Int32 ip = 0; ip < XManager.Projects.Count; ip++)
				//{
				//	ICadProject project = XManager.Projects[ip] as ICadProject;
				//	if (project != null)
				//	{
				//		//for (Int32 id = 0; id < project.Documents.Count; id++)
				//		//{
				//		//	ICadDraft draft = project.Documents[id] as ICadDraft;
				//		//	if (draft != null)
				//		//	{
				//		//		for (Int32 ie = 0; ie < draft.Elements.Count; ie++)
				//		//		{
				//		//			ICadFillSupport element = draft.Elements[ie] as ICadFillSupport;
				//		//			if (element != null && element.FillBrush == brush)
				//		//			{
				//		//				element.SetFillBrush(DefaultBrush);
				//		//			}
				//		//		}
				//		//	}
				//		//}
				//	}
				//}

				// 2) Проходим все документы и перекидываем используемую кисть на кисть по умолчанию
				//for (Int32 id = 0; id < XManager.Documents.Count; id++)
				//{
				//	ICadDraft draft = XManager.Documents[id] as ICadDraft;
				//	if (draft != null)
				//	{
				//		//for (Int32 ie = 0; ie < draft.Elements.Count; ie++)
				//		//{
				//		//	ICadFillSupport element = draft.Elements[ie] as ICadFillSupport;
				//		//	if (element != null && element.FillBrush == brush)
				//		//	{
				//		//		element.SetFillBrush(DefaultBrush);
				//		//	}
				//		//}
				//	}
				//}

				// 3) Удаляем кисть
				mBrushes.Remove(brush);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление ресурсов Direct2D
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
#if USE_SHARPDX
			public static void UpdateDirect2DResources()
			{
				for (Int32 i = 0; i < mBrushes.Count; i++)
				{
					mBrushes[i].UpdateDirect2DResource(true);
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
//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Ресурсы для отображения графического объекта
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADVisualPenManager.cs
*		Статический класс - менеджер для управления перьями.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 27.03.2022
//=====================================================================================================================
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Xml;
using System.Linq;
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
		/// Статический класс - менеджер для управления перьями
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public static class XCadPenManager
		{
			#region ======================================= ДАННЫЕ ====================================================
			internal static ListArray<CCadPen> mPens;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Текущий набор перьев
			/// </summary>
			public static ListArray<CCadPen> Pens
			{
				get { return (mPens); }
			}

			/// <summary>
			/// Перо по умолчанию
			/// </summary>
			public static CCadPen DefaultPen
			{
				get { return (mPens[1]); }
			}

			/// <summary>
			/// Черное сплошное перо толщиной 05
			/// </summary>
			public static CCadPen Black05
			{
				get { return (mPens[0]); }
			}

			/// <summary>
			/// Черное сплошное перо толщиной 1
			/// </summary>
			public static CCadPen Black1
			{
				get { return (mPens[1]); }
			}

			/// <summary>
			/// Черное сплошное перо толщиной 2
			/// </summary>
			public static CCadPen Black2
			{
				get { return (mPens[2]); }
			}

			/// <summary>
			/// Черное сплошное перо толщиной 3
			/// </summary>
			public static CCadPen Black3
			{
				get { return (mPens[3]); }
			}

			/// <summary>
			/// Черное сплошное перо толщиной 4
			/// </summary>
			public static CCadPen Black4
			{
				get { return (mPens[4]); }
			}

			/// <summary>
			/// Черное сплошное перо толщиной 5
			/// </summary>
			public static CCadPen Black5
			{
				get { return (mPens[5]); }
			}

			/// <summary>
			/// Серое сплошное перо толщиной 1
			/// </summary>
			public static CCadPen Gray1
			{
				get { return (mPens[6]); }
			}

			/// <summary>
			/// Серое сплошное перо толщиной 2
			/// </summary>
			public static CCadPen Gray2
			{
				get { return (mPens[7]); }
			}

			/// <summary>
			/// Серое сплошное перо толщиной 3
			/// </summary>
			public static CCadPen Gray3
			{
				get { return (mPens[8]); }
			}

			/// <summary>
			/// Серое сплошное перо толщиной 4
			/// </summary>
			public static CCadPen Gray4
			{
				get { return (mPens[9]); }
			}

			/// <summary>
			/// Серое сплошное перо толщиной 5
			/// </summary>
			public static CCadPen Gray5
			{
				get { return (mPens[10]); }
			}

			/// <summary>
			/// Красное сплошное перо толщиной 1
			/// </summary>
			public static CCadPen Red1
			{
				get { return (mPens[11]); }
			}

			/// <summary>
			/// Красное сплошное перо толщиной 2
			/// </summary>
			public static CCadPen Red2
			{
				get { return (mPens[12]); }
			}

			/// <summary>
			/// Красное сплошное перо толщиной 3
			/// </summary>
			public static CCadPen Red3
			{
				get { return (mPens[13]); }
			}

			/// <summary>
			/// Красное сплошное перо толщиной 4
			/// </summary>
			public static CCadPen Red4
			{
				get { return (mPens[14]); }
			}

			/// <summary>
			/// Красное сплошное перо толщиной 5
			/// </summary>
			public static CCadPen Red5
			{
				get { return (mPens[15]); }
			}

			/// <summary>
			/// Зеленое сплошное перо толщиной 1
			/// </summary>
			public static CCadPen Green1
			{
				get { return (mPens[16]); }
			}

			/// <summary>
			/// Зеленое сплошное перо толщиной 2
			/// </summary>
			public static CCadPen Green2
			{
				get { return (mPens[17]); }
			}

			/// <summary>
			/// Зеленое сплошное перо толщиной 3
			/// </summary>
			public static CCadPen Green3
			{
				get { return (mPens[18]); }
			}

			/// <summary>
			/// Зеленое сплошное перо толщиной 4
			/// </summary>
			public static CCadPen Green4
			{
				get { return (mPens[19]); }
			}

			/// <summary>
			/// Зеленое сплошное перо толщиной 5
			/// </summary>
			public static CCadPen Green5
			{
				get { return (mPens[20]); }
			}

			/// <summary>
			/// Синие сплошное перо толщиной 1
			/// </summary>
			public static CCadPen Blue1
			{
				get { return (mPens[21]); }
			}

			/// <summary>
			/// Синие сплошное перо толщиной 2
			/// </summary>
			public static CCadPen Blue2
			{
				get { return (mPens[22]); }
			}

			/// <summary>
			/// Синие сплошное перо толщиной 3
			/// </summary>
			public static CCadPen Blue3
			{
				get { return (mPens[23]); }
			}

			/// <summary>
			/// Синие сплошное перо толщиной 4
			/// </summary>
			public static CCadPen Blue4
			{
				get { return (mPens[24]); }
			}

			/// <summary>
			/// Синие сплошное перо толщиной 5
			/// </summary>
			public static CCadPen Blue5
			{
				get { return (mPens[25]); }
			}

			/// <summary>
			/// Перо для дорожной разметки 1.1
			/// </summary>
			public static CCadPen RoadMarking_1_1
			{
				get { return (mPens[26]); }
			}

			/// <summary>
			/// Перо для дорожной разметки 1.2.1
			/// </summary>
			public static CCadPen RoadMarking_1_2_1
			{
				get { return (mPens[27]); }
			}

			/// <summary>
			/// Перо для дорожной разметки 1.5
			/// </summary>
			public static CCadPen RoadMarking_1_5
			{
				get { return (mPens[28]); }
			}

			/// <summary>
			/// Перо для дорожной разметки 1.6
			/// </summary>
			public static CCadPen RoadMarking_1_6
			{
				get { return (mPens[29]); }
			}

			/// <summary>
			/// Перо для дорожной разметки 1.11
			/// </summary>
			public static CCadPen RoadMarking_1_11
			{
				get { return (mPens[30]); }
			}

			/// <summary>
			/// Перо для дорожного пешеходного ограждения
			/// </summary>
			public static CCadPen RoadFencePedestrian
			{
				get { return (mPens[31]); }
			}

			/// <summary>
			/// Перо для дорожного барьерного ограждения
			/// </summary>
			public static CCadPen RoadFenceBarrier
			{
				get { return (mPens[32]); }
			}

			/// <summary>
			/// Перо для дорожного ограждения в виде сигнальных столбиков
			/// </summary>
			public static CCadPen RoadFenceSignalingColumns
			{
				get { return (mPens[33]); }
			}
			#endregion

			#region ======================================= МЕТОДЫ ====================================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Первоначальная инициализация стандартных перьев
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public static void Init()
			{
				// 1) Инициализируем базу перьев
				mPens = new ListArray<CCadPen>()
				{
					IsNotify = true
				};

				// 2) Заполняем стандартные перья
				Add("Черный", "Сплошные", XCadBrushManager.Black, 0.5f, XCadPenStyleManager.Solid, 0);
				Add("Черный", "Сплошные", XCadBrushManager.Black, 1f, XCadPenStyleManager.Solid, 1);
				Add("Черный", "Сплошные", XCadBrushManager.Black, 2f, XCadPenStyleManager.Solid, 2);
				Add("Черный", "Сплошные", XCadBrushManager.Black, 3f, XCadPenStyleManager.Solid, 3);
				Add("Черный", "Сплошные", XCadBrushManager.Black, 4f, XCadPenStyleManager.Solid, 4);
				Add("Черный", "Сплошные", XCadBrushManager.Black, 5f, XCadPenStyleManager.Solid, 5);

				Add("Серый", "Сплошные", XCadBrushManager.Gray, 1f, XCadPenStyleManager.Solid, 11);
				Add("Серый", "Сплошные", XCadBrushManager.Gray, 2f, XCadPenStyleManager.Solid, 12);
				Add("Серый", "Сплошные", XCadBrushManager.Gray, 3f, XCadPenStyleManager.Solid, 13);
				Add("Серый", "Сплошные", XCadBrushManager.Gray, 4f, XCadPenStyleManager.Solid, 14);
				Add("Серый", "Сплошные", XCadBrushManager.Gray, 5f, XCadPenStyleManager.Solid, 15);

				Add("Красный", "Сплошные", XCadBrushManager.Red, 1f, XCadPenStyleManager.Solid, 21);
				Add("Красный", "Сплошные", XCadBrushManager.Red, 2f, XCadPenStyleManager.Solid, 22);
				Add("Красный", "Сплошные", XCadBrushManager.Red, 3f, XCadPenStyleManager.Solid, 23);
				Add("Красный", "Сплошные", XCadBrushManager.Red, 4f, XCadPenStyleManager.Solid, 24);
				Add("Красный", "Сплошные", XCadBrushManager.Red, 5f, XCadPenStyleManager.Solid, 25);

				Add("Зеленый", "Сплошные", XCadBrushManager.Green, 1f, XCadPenStyleManager.Solid, 31);
				Add("Зеленый", "Сплошные", XCadBrushManager.Green, 2f, XCadPenStyleManager.Solid, 32);
				Add("Зеленый", "Сплошные", XCadBrushManager.Green, 3f, XCadPenStyleManager.Solid, 33);
				Add("Зеленый", "Сплошные", XCadBrushManager.Green, 4f, XCadPenStyleManager.Solid, 34);
				Add("Зеленый", "Сплошные", XCadBrushManager.Green, 5f, XCadPenStyleManager.Solid, 35);

				Add("Синий", "Сплошные", XCadBrushManager.Blue, 1f, XCadPenStyleManager.Solid, 41);
				Add("Синий", "Сплошные", XCadBrushManager.Blue, 2f, XCadPenStyleManager.Solid, 42);
				Add("Синий", "Сплошные", XCadBrushManager.Blue, 3f, XCadPenStyleManager.Solid, 43);
				Add("Синий", "Сплошные", XCadBrushManager.Blue, 4f, XCadPenStyleManager.Solid, 44);
				Add("Синий", "Сплошные", XCadBrushManager.Blue, 5f, XCadPenStyleManager.Solid, 45);

				Add("Черный", "Штриховые", XCadBrushManager.Black, 2, XCadPenStyleManager.Dash, 100);
				Add("Серый", "Штриховые", XCadBrushManager.Gray, 2, XCadPenStyleManager.Dash, 110);
				Add("Зеленый", "Штриховые", XCadBrushManager.Green, 2, XCadPenStyleManager.Dash, 120);
				Add("Синий", "Штриховые", XCadBrushManager.Blue, 2, XCadPenStyleManager.Dash, 130);
				Add("Красный", "Штриховые", XCadBrushManager.Red, 2, XCadPenStyleManager.Dash, 140);

				Add("Черный", "Штрихпунктирные", XCadBrushManager.Black, 2, XCadPenStyleManager.DashDot, 200);
				Add("Серый", "Штрихпунктирные", XCadBrushManager.Gray, 2, XCadPenStyleManager.DashDot, 210);
				Add("Зеленый", "Штрихпунктирные", XCadBrushManager.Green, 2, XCadPenStyleManager.DashDot, 220);
				Add("Синий", "Штрихпунктирные", XCadBrushManager.Blue, 2, XCadPenStyleManager.DashDot, 230);
				Add("Красный", "Штрихпунктирные", XCadBrushManager.Red, 2, XCadPenStyleManager.DashDot, 240);

				Add("Черный", "Штрихпунктирные - 2", XCadBrushManager.Black, 2, XCadPenStyleManager.DashDotDot, 300);
				Add("Серый", "Штрихпунктирные - 2", XCadBrushManager.Gray, 2, XCadPenStyleManager.DashDotDot, 310);
				Add("Зеленый", "Штрихпунктирные - 2", XCadBrushManager.Green, 2, XCadPenStyleManager.DashDotDot, 320);
				Add("Синий", "Штрихпунктирные - 2", XCadBrushManager.Blue, 2, XCadPenStyleManager.DashDotDot, 330);
				Add("Красный", "Штрихпунктирные - 2", XCadBrushManager.Red, 2, XCadPenStyleManager.DashDotDot, 340);

				Add("Черный", "Пунктирные", XCadBrushManager.Black, 2, XCadPenStyleManager.Dot, 400);
				Add("Серый", "Пунктирные", XCadBrushManager.Gray, 2, XCadPenStyleManager.Dot, 410);
				Add("Зеленый", "Пунктирные", XCadBrushManager.Green, 2, XCadPenStyleManager.Dot, 420);
				Add("Синий", "Пунктирные", XCadBrushManager.Blue, 2, XCadPenStyleManager.Dot, 430);
				Add("Красный", "Пунктирные", XCadBrushManager.Red, 2, XCadPenStyleManager.Dot, 440);

				// 3) Перья для ПОДД - Дорожная разметка
				Add("Линия разделения (1.1)", "Дорожная разметка", XCadBrushManager.Black, 2, 
					XCadPenStyleManager.RoadMarking_1_1, 5000);

				Add("Край проезжей части (1.2.1)", "Дорожная разметка", XCadBrushManager.Black, 3,
					XCadPenStyleManager.RoadMarking_1_2_1, 5001);

				Add("Линия разделения (1.5)", "Дорожная разметка", XCadBrushManager.Black, 2,
					XCadPenStyleManager.RoadMarking_1_5, 5002);

				Add("Линия приближения (1.6)", "Дорожная разметка", XCadBrushManager.Black, 2,
					XCadPenStyleManager.RoadMarking_1_6, 5003);

				Add("Барьерная линия (1.11)", "Дорожная разметка", XCadBrushManager.Black, 1,
					XCadPenStyleManager.RoadMarking_1_11, 5004);

				// 4) Перья для ПОДД - Ограждение
				Add("Пешеходное ограждение", "Ограждение", XCadBrushManager.RoadPedestrian_1, 7, XCadPenStyleManager.Solid, 6000);
				Add("Барьерное ограждение", "Ограждение", XCadBrushManager.RoadBarrier_1, 7, XCadPenStyleManager.Solid, 6001);
				Add("Сигнальные столбики", "Ограждение", XCadBrushManager.RoadSignalingColumns_1, 7, XCadPenStyleManager.Solid, 6002);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Поиск пера по внутреннему имени
			/// </summary>
			/// <param name="name">Внутренние имя пера</param>
			/// <returns>Найденное перо или перо по умолчанию</returns>
			//---------------------------------------------------------------------------------------------------------
			public static CCadPen GetFromInternalName(String name)
			{
				CCadPen pen = DefaultPen;
				for (Int32 i = 0; i < mPens.Count; i++)
				{
					if (mPens[i].mInternalName == name)
					{
						return (mPens[i]);
					}
				}

				return (pen);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Поиск пера по имени
			/// </summary>
			/// <param name="name">Имя пера</param>
			/// <returns>Найденное перо или перо по умолчанию</returns>
			//---------------------------------------------------------------------------------------------------------
			public static CCadPen GetFromName(String name)
			{
				CCadPen pen = DefaultPen;
				for (Int32 i = 0; i < mPens.Count; i++)
				{
					if (mPens[i].Name == name)
					{
						return (mPens[i]);
					}
				}

				return (pen);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Поиск пера по идентификатору
			/// </summary>
			/// <param name="id">Идентификатор пера</param>
			/// <returns>Найденное перо или перо по умолчанию</returns>
			//---------------------------------------------------------------------------------------------------------
			public static CCadPen GetFromId(Int64 id)
			{
				CCadPen pen = DefaultPen;
				for (Int32 i = 0; i < mPens.Count; i++)
				{
					if (mPens[i].Id == id)
					{
						return (mPens[i]);
					}
				}

				return (pen);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавление пера с указанными параметрами
			/// </summary>
			/// <param name="name">Имя пера</param>
			/// <param name="color">Основной цвет пера</param>
			/// <param name="thickness">Толщина пера</param>
			/// <returns>Перо</returns>
			//---------------------------------------------------------------------------------------------------------
			public static CCadPen Add(String name, TColor color, Single thickness = 2.0f)
			{
				return (Add(name, "", XCadBrushManager.GetFromColor(color), thickness, XCadPenStyleManager.Solid));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавление пера с указанными параметрами
			/// </summary>
			/// <param name="name">Имя пера</param>
			/// <param name="group">Имя группы</param>
			/// <param name="color">Основной цвет пера</param>
			/// <param name="thickness">Толщина пера</param>
			/// <returns>Перо</returns>
			//---------------------------------------------------------------------------------------------------------
			public static CCadPen Add(String name, String group, TColor color, Single thickness = 2.0f)
			{
				return (Add(name, group, XCadBrushManager.GetFromColor(color), thickness, XCadPenStyleManager.Solid));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавление пера с указанными параметрами
			/// </summary>
			/// <param name="name">Имя пера</param>
			/// <param name="group">Имя группы</param>
			/// <param name="color">Основной цвет пера</param>
			/// <param name="thickness">Толщина пера</param>
			/// <param name="pen_style">Последовательность штрихов и пробелов</param>
			/// <returns>Перо</returns>
			//---------------------------------------------------------------------------------------------------------
			public static CCadPen Add(String name, String group, TColor color, Single thickness, CCadPenStyle pen_style)
			{
				return (Add(name, group, XCadBrushManager.GetFromColor(color), thickness, pen_style));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавление пера с указанными параметрами
			/// </summary>
			/// <param name="name">Имя пера</param>
			/// <param name="group">Имя группы</param>
			/// <param name="brush">Кисть пера</param>
			/// <param name="thickness">Толщина пера</param>
			/// <param name="pen_style">Последовательность штрихов и пробелов</param>
			/// <returns>Перо</returns>
			//---------------------------------------------------------------------------------------------------------
			public static CCadPen Add(String name, String group, CCadBrush brush, Single thickness, CCadPenStyle pen_style)
			{
				// Создаем перо
				CCadPen pen = new CCadPen(name);
				pen.mGroup = group;
				pen.mThickness = thickness;
				pen.mBrush = brush;
				pen.mPenStyle = pen_style;

				// Обновляем перо
				pen.Update();

				// Добавляем
				mPens.Add(pen);

				return (pen);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавление пера с указанными параметрами
			/// </summary>
			/// <param name="name">Имя пера</param>
			/// <param name="group">Имя группы</param>
			/// <param name="brush">Кисть пера</param>
			/// <param name="thickness">Толщина пера</param>
			/// <param name="pen_style">Последовательность штрихов и пробелов</param>
			/// <param name="id">Идентификатор пера</param>
			/// <returns>Перо</returns>
			//---------------------------------------------------------------------------------------------------------
			public static CCadPen Add(String name, String group, CCadBrush brush, Single thickness, CCadPenStyle pen_style, Int32 id)
			{
				CCadPen pen = Add(name, group, brush, thickness, pen_style);
				pen.Id = id;
				return (pen);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Удаления пера
			/// </summary>
			/// <param name="name">Имя пера</param>
			//---------------------------------------------------------------------------------------------------------
			public static void Remove(String name)
			{
				for (Int32 i = 0; i < mPens.Count; i++)
				{
					if (mPens[i].Name == name)
					{
						Remove(mPens[i]);
						break;
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Удаления пера
			/// </summary>
			/// <param name="id">Идентификатор пера</param>
			//---------------------------------------------------------------------------------------------------------
			public static void Remove(Int64 id)
			{
				for (Int32 i = 0; i < mPens.Count; i++)
				{
					if (mPens[i].Id == id)
					{
						Remove(mPens[i]);
						break;
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Удаления пера
			/// </summary>
			/// <param name="pen">Перо</param>
			//---------------------------------------------------------------------------------------------------------
			public static void Remove(CCadPen pen)
			{
				// 1) Если объект константный - выходим
				if (pen.IsConst) return;

				// 2) Проходим все проекты и перекидываем используемое перо по умолчанию
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
				//		//			ICadStrokeSupport element = draft.Elements[ie] as ICadStrokeSupport;
				//		//			if (element != null && element.StrokePen == pen)
				//		//			{
				//		//				element.SetStrokePen(DefaultPen);
				//		//			}
				//		//		}
				//		//	}
				//		//}
				//	}
				//}

				// 2) Проходим все документы и перекидываем используемое перо по умолчанию
				//for (Int32 id = 0; id < XManager.Documents.Count; id++)
				//{
				//	ICadDraft draft = XManager.Documents[id] as ICadDraft;
				//	if (draft != null)
				//	{
				//		//for (Int32 ie = 0; ie < draft.Elements.Count; ie++)
				//		//{
				//		//	ICadStrokeSupport element = draft.Elements[ie] as ICadStrokeSupport;
				//		//	if (element != null && element.StrokePen == pen)
				//		//	{
				//		//		element.SetStrokePen(DefaultPen);
				//		//	}
				//		//}
				//	}
				//}

				// 3) Удаляем перо
				mPens.Remove(pen);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление ресурсов Direct2D
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
#if USE_SHARPDX
			public static void UpdateDirect2DResources()
			{
				for (Int32 i = 0; i < mPens.Count; i++)
				{
					mPens[i].UpdateDirect2DResource();
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
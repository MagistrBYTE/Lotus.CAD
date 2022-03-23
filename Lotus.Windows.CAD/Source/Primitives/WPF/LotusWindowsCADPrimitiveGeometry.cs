//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Графические примитивы
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusWindowsCADPrimitiveGeometry.cs
*		Геометрия.
*		Геометрия представляет из себя набор типовых геометрических примитивов - сегментов. Набор сегментов строго 
*	предопределён.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 27.03.2022
//=====================================================================================================================
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Maths;
using Lotus.Windows;
//=====================================================================================================================
namespace Lotus
{
	namespace CAD
	{
		//-------------------------------------------------------------------------------------------------------------
		//! \addtogroup CadPrimitivs
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Геометрия
		/// </summary>
		/// <remarks>
		/// Геометрия представляет из себя набор типовых геометрических примитивов - сегментов. Набор сегментов строго 
		/// предопределён
		/// </remarks>
		//-------------------------------------------------------------------------------------------------------------
		public class CCadWindowsPrimitiveGeometry : CCadPrimitiveGeometry
		{
			#region ======================================= ДАННЫЕ ====================================================
			// Основные параметры
			internal StreamGeometry mGeometry;
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public CCadWindowsPrimitiveGeometry()
			{
				mGeometry = new StreamGeometry();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="start_point">Начальная точка</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadWindowsPrimitiveGeometry(Vector2Df start_point)
			{
				mGeometry = new StreamGeometry();
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление данных отображения контура примитива
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void UpdateIsStroked()
			{
				Update();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление данных отображения заливки примитива
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void UpdateIsFilled()
			{
				Update();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление данных геометрии
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void Update()
			{
				using (StreamGeometryContext sgc = mGeometry.Open())
				{
					CCadSegment current_figure = mSegments[0];

					sgc.BeginFigure(current_figure.BasePoint.ToWinPoint(), current_figure.IsFilled, current_figure.IsClosed);

					Int32 current_new_figure = 0;

					for (Int32 i = 0; i < mSegments.Count; i++)
					{
						CCadSegment current_segment = mSegments[i];

						// Если новая фигура
						if (current_segment.IsNewFigure && current_new_figure != i)
						{
							// Закрывает текущую фигуру

							// Открываем новую фигуру
							sgc.BeginFigure(current_segment.BasePoint.ToWinPoint(), current_segment.IsFilled, current_segment.IsClosed);

							current_figure = current_segment;
							current_new_figure = i;
						}

						switch (current_segment.SegmentType)
						{
							case TCadSegmentType.Points:
								{
									CCadSegmentPoints points = current_segment as CCadSegmentPoints;
									sgc.PolyLineTo(points.Points.ConvertToWindowsPoints(), mIsStroked, true);
								}
								break;
							case TCadSegmentType.Line:
								{
									CCadSegmentLine line = current_segment as CCadSegmentLine;
									sgc.LineTo(line.EndPoint.ToWinPoint(), mIsStroked, true);
								}
								break;
							case TCadSegmentType.Arc:
								{
									CCadSegmentArc arc = current_segment as CCadSegmentArc;
									sgc.ArcTo(arc.EndPoint.ToWinPoint(), new Size(arc.RadiusX, arc.RadiusY),
										arc.RotationAngle, arc.IsLargeArc, arc.IsClockwiseDirection ? SweepDirection.Clockwise :
										 SweepDirection.Counterclockwise, mIsStroked, true);
								}
								break;
							case TCadSegmentType.Bezier:
								break;
							default:
								break;
						}
					}

					// Закрывает текущую фигуру
					sgc.Close();
				}

				mBoundsRect = mGeometry.Bounds.ToRect();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Копирование графического примитива
			/// </summary>
			/// <returns>Дубликат графического примитива со всеми параметрами и данными</returns>
			//---------------------------------------------------------------------------------------------------------
			public override CCadPrimitive Duplicate()
			{
				CCadWindowsPrimitiveGeometry geometry = new CCadWindowsPrimitiveGeometry();
				geometry.CopyParamemtrs(this);
				geometry.Update();
				return (geometry);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Копирование параметров с графического примитива
			/// </summary>
			/// <param name="primitiv">Графический примитив</param>
			//---------------------------------------------------------------------------------------------------------
			public override void CopyParamemtrs(CCadPrimitive primitiv)
			{
				base.CopyParamemtrs(primitiv);

				CCadWindowsPrimitiveGeometry source = primitiv as CCadWindowsPrimitiveGeometry;

				mSegments.Clear();
				for (Int32 i = 0; i < source.Count; i++)
				{
					switch (source.mSegments[i].SegmentType)
					{
						case TCadSegmentType.Points:
							{
								mSegments.Add(new CCadSegmentPoints(this, source.mSegments[i] as CCadSegmentPoints));
							}
							break;
						case TCadSegmentType.Line:
							{
								mSegments.Add(new CCadSegmentLine(this, source.mSegments[i] as CCadSegmentLine));
							}
							break;
						case TCadSegmentType.Arc:
							{
								mSegments.Add(new CCadSegmentArc(this, source.mSegments[i] as CCadSegmentArc));
							}
							break;
						case TCadSegmentType.Bezier:
							break;
						default:
							break;
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка раположения точки на контуре графического примитива
			/// </summary>
			/// <param name="point">Проверяемая точка</param>
			/// <param name="thickness">Толщина контура</param>
			/// <returns>Статус расположения</returns>
			//---------------------------------------------------------------------------------------------------------
			public override Boolean StrokeContains(ref Vector2Df point, Single thickness)
			{
				return (mGeometry.StrokeContains(mStroke.WindowsPen, new Point(point.X, point.Y)));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка раположения точки внутри области графического примитива
			/// </summary>
			/// <param name="point">Проверяемая точка</param>
			/// <returns>Статус расположения</returns>
			//---------------------------------------------------------------------------------------------------------
			public override Boolean FillContains(ref Vector2Df point)
			{
				return (mGeometry.FillContains(new Point(point.X, point.Y)));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вычисление площади графического примитива
			/// </summary>
			/// <returns>Площадь графического примитива</returns>
			//---------------------------------------------------------------------------------------------------------
			public override Double ComputeArea()
			{
				return (mGeometry.GetArea());
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вычисление ограничивающего прямоугольника графического примитива
			/// </summary>
			/// <param name="dest_bounds_rect">Прямоугольник</param>
			//---------------------------------------------------------------------------------------------------------
			public override void ComputeBounds(ref Rect2Df dest_bounds_rect)
			{
				dest_bounds_rect = mGeometry.Bounds.ToRect();
			}
			#endregion

			#region ======================================= МЕТОДЫ РИСОВАНИЯ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Рисование геометрии
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void Draw()
			{
				//LotusCadCanvas.DrawingDevice.DrawGeometry(mIsFilled ? mFill.WindowsBrush : null,
				//						mIsStroked ? Stroke.WindowsPen : null, mGeometry);
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
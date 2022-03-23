//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Графические примитивы
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADPrimitiveGeometrySegments.cs
*		Сегменты графических примитивов геометрии.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 27.03.2022
//=====================================================================================================================
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
using Lotus.Maths;
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
		/// Тип сегмента
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public enum TCadSegmentType
		{
			/// <summary>
			/// Набор точек
			/// </summary>
			Points,
			
			/// <summary>
			/// Линия
			/// </summary>
			Line,

			/// <summary>
			/// Дуга
			/// </summary>
			Arc,

			/// <summary>
			/// Кривая Безье
			/// </summary>
			Bezier
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Базовый класс для сегмента геометрии
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CCadSegment
		{
			#region ======================================= ДАННЫЕ ====================================================
			// Основные параметры
			internal CCadPrimitiveGeometry mGeometry;
			internal Vector2Df mBasePoint;
			internal TCadSegmentType mSegmentType;
			internal Boolean mIsNewFigure;
			internal Boolean mIsFilled;
			internal Boolean mIsClosed;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			//
			// ОСНОВНЫЕ ПАРАМЕТРЫ
			//
			/// <summary>
			/// Геометрия
			/// </summary>
			public CCadPrimitiveGeometry Geometry
			{
				get { return (mGeometry); }
				set
				{
					Geometry = value;
				}
			}

			/// <summary>
			/// Тип сегмента геометрии
			/// </summary>
			public TCadSegmentType SegmentType
			{
				get { return (mSegmentType); }
			}

			/// <summary>
			/// Базовая точка сегмента
			/// </summary>
			public Vector2Df BasePoint
			{
				get { return (mBasePoint); }
				set
				{
					if (!Vector2Df.Approximately(ref mBasePoint, ref value, 0.01f))
					{
						mBasePoint = value;

						if (mGeometry.IsCanvas)
						{
							// Обновляем данные
							mGeometry.Update();

							// Обновляем отображение примитива
							//XCadManager.Canvas.Update();
						}
					}
				}
			}

			/// <summary>
			/// Статус новой фигуры с данного сегмента геометрии
			/// </summary>
			public Boolean IsNewFigure
			{
				get { return (mIsNewFigure); }
				set
				{
					if (mIsNewFigure != value)
					{
						mIsNewFigure = value;

						if (mGeometry.IsCanvas)
						{
							// Обновляем данные
							mGeometry.Update();

							// Обновляем отображение примитива
							//XCadManager.Canvas.Update();
						}
					}
				}
			}

			/// <summary>
			/// Статус заливки текущей фигуры
			/// </summary>
			public Boolean IsFilled
			{
				get { return (mIsFilled); }
				set
				{
					if (mIsFilled != value)
					{
						mIsFilled = value;

						if (mGeometry.IsCanvas)
						{
							// Обновляем данные
							mGeometry.Update();

							// Обновляем отображение примитива
							//XCadManager.Canvas.Update();
						}
					}
				}
			}

			/// <summary>
			/// Статус замкнутости текущей фигуры
			/// </summary>
			public Boolean IsClosed
			{
				get { return (mIsClosed); }
				set
				{
					if (mIsClosed != value)
					{
						mIsClosed = value;

						if (mGeometry.IsCanvas)
						{
							// Обновляем данные
							mGeometry.Update();

							// Обновляем отображение примитива
							//XCadManager.Canvas.Update();
						}
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
			protected CCadSegment()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="geometry">Геометрия</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadSegment(CCadPrimitiveGeometry geometry)
			{
				mGeometry = geometry;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="geometry">Геометрия</param>
			/// <param name="base_point">Базовая точка</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadSegment(CCadPrimitiveGeometry geometry, Vector2Df base_point)
			{
				mGeometry = geometry;
				mBasePoint = base_point;
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			#endregion
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Сегмент геометрии - Набор точек (используется прямых участков полилинии и полигона)
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CCadSegmentPoints : CCadSegment
		{
			#region ======================================= ДАННЫЕ ====================================================
			// Основные параметры
			internal List<Vector2Df> mPoints;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Набор точек
			/// </summary>
			public List<Vector2Df> Points
			{
				get { return (mPoints); }
			}

			/// <summary>
			/// Начальная точка
			/// </summary>
			public Vector2Df StartPoint
			{
				get { return (mPoints[0]); }
				set
				{
					mPoints[0] = value;
					mBasePoint = value;

					if (mGeometry.IsCanvas)
					{
						// Обновляем данные
						mGeometry.Update();

						// Обновляем отображение примитива
						//XCadManager.Canvas.Update();
					}
				}
			}

			/// <summary>
			/// Конечная точка
			/// </summary>
			public Vector2Df EndPoint
			{
				get { return (mPoints[mPoints.Count - 1]); }
				set
				{
					mPoints[mPoints.Count - 1] = value;

					if (mGeometry.IsCanvas)
					{
						// Обновляем данные
						mGeometry.Update();

						// Обновляем отображение примитива
						//XCadManager.Canvas.Update();
					}
				}
			}

			/// <summary>
			/// Количество точек
			/// </summary>
			public Int32 Count
			{
				get { return (mPoints.Count); }
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected CCadSegmentPoints()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="geometry">Геометрия</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadSegmentPoints(CCadPrimitiveGeometry geometry)
			{
				mGeometry = geometry;
				mSegmentType = TCadSegmentType.Points;
				mPoints = new List<Vector2Df>();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="geometry">Геометрия</param>
			/// <param name="base_point">Базовая точка</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadSegmentPoints(CCadPrimitiveGeometry geometry, Vector2Df base_point)
			{
				mGeometry = geometry;
				mSegmentType = TCadSegmentType.Points;
				mPoints = new List<Vector2Df>();
				mBasePoint = base_point;
				mPoints.Add(base_point);
				mPoints.Add(base_point);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="geometry">Геометрия</param>
			/// <param name="base_point">Базовая точка</param>
			/// <param name="next_point">Следующая точка</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadSegmentPoints(CCadPrimitiveGeometry geometry, Vector2Df base_point, Vector2Df next_point)
			{
				mGeometry = geometry;
				mSegmentType = TCadSegmentType.Points;
				mPoints = new List<Vector2Df>();
				mBasePoint = base_point;
				mPoints.Add(base_point);
				mPoints.Add(next_point);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="geometry">Геометрия</param>
			/// <param name="segment_points">Сегмент геометрии - Набор точек</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadSegmentPoints(CCadPrimitiveGeometry geometry, CCadSegmentPoints segment_points)
			{
				mGeometry = geometry;
				mSegmentType = TCadSegmentType.Points;
				mPoints = new List<Vector2Df>();
				mPoints.AddRange(segment_points.mPoints);
			}
			#endregion

			#region ======================================= ИНДЕКСАТОР ================================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Индексация точек на основе индекса
			/// </summary>
			/// <param name="index">Индекс точки</param>
			/// <returns>Точка</returns>
			//---------------------------------------------------------------------------------------------------------
			public Vector2Df this[Int32 index]
			{
				get { return (mPoints[index]); }
				set
				{
					mPoints[index] = value;

					if (mGeometry.IsCanvas)
					{
						// Обновляем данные
						mGeometry.Update();

						// Обновляем отображение примитива
						//XCadManager.Canvas.Update();
					}
				}
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавление точки
			/// </summary>
			/// <param name="point">Точка</param>
			//---------------------------------------------------------------------------------------------------------
			public void AddPoint(ref Vector2Df point)
			{
				mPoints.Add(point);

				if (mGeometry.IsCanvas)
				{
					// Обновляем данные
					mGeometry.Update();

					// Обновляем отображение примитива
					//XCadManager.Canvas.Update();
				}
			}
			#endregion
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Сегмент отдельной линии
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CCadSegmentLine : CCadSegment
		{
			#region ======================================= ДАННЫЕ ====================================================
			// Основные параметры
			internal Vector2Df mEndPoint;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			//
			// ОСНОВНЫЕ ПАРАМЕТРЫ
			//
			/// <summary>
			/// Начальная точка
			/// </summary>
			public Vector2Df StartPoint
			{
				get { return (mBasePoint); }
				set
				{
					if (!Vector2Df.Approximately(ref mBasePoint, ref value, 0.01f))
					{
						mBasePoint = value;

						if (mGeometry.IsCanvas)
						{
							// Обновляем данные
							mGeometry.Update();

							// Обновляем отображение примитива
							//XCadManager.Canvas.Update();
						}
					}
				}
			}

			/// <summary>
			/// Конечная точка
			/// </summary>
			public Vector2Df EndPoint
			{
				get { return (mEndPoint); }
				set
				{
					if (!Vector2Df.Approximately(ref mEndPoint, ref value, 0.01f))
					{
						mEndPoint = value;

						if (mGeometry.IsCanvas)
						{
							// Обновляем данные
							mGeometry.Update();

							// Обновляем отображение примитива
							//XCadManager.Canvas.Update();
						}
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
			protected CCadSegmentLine()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="geometry">Геометрия</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadSegmentLine(CCadPrimitiveGeometry geometry)
			{
				mGeometry = geometry;
				mSegmentType = TCadSegmentType.Line;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="geometry">Геометрия</param>
			/// <param name="start_point">Начальная точка</param>
			/// <param name="end_point">Конечная точка</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadSegmentLine(CCadPrimitiveGeometry geometry, Vector2Df start_point, Vector2Df end_point)
			{
				mGeometry = geometry;
				mBasePoint = start_point;
				mEndPoint = end_point;
				mSegmentType = TCadSegmentType.Line;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="geometry">Геометрия</param>
			/// <param name="segment_line">Сегмент отдельной линии</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadSegmentLine(CCadPrimitiveGeometry geometry, CCadSegmentLine segment_line)
			{
				mGeometry = geometry;
				mBasePoint = segment_line.mBasePoint;
				mEndPoint = segment_line.mEndPoint;
				mSegmentType = TCadSegmentType.Line;
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			#endregion
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Сегмент дуги
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CCadSegmentArc : CCadSegment
		{
			#region ======================================= ДАННЫЕ ====================================================
			// Основные параметры
			internal Boolean mIsLargeArc;
			internal Boolean mIsClockwiseDirection;
			internal Single mRotationAngle;
			internal Single mRadiusX;
			internal Single mRadiusY;
			internal Vector2Df mEndPoint;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			//
			// ОСНОВНЫЕ ПАРАМЕТРЫ
			//
			/// <summary>
			/// Большая или малая дуга
			/// </summary>
			public Boolean IsLargeArc
			{
				get { return (mIsLargeArc); }
				set
				{
					if (mIsLargeArc != value)
					{
						mIsLargeArc = value;

						if (mGeometry.IsCanvas)
						{
							// Обновляем данные
							mGeometry.Update();

							// Обновляем отображение примитива
							//XCadManager.Canvas.Update();
						}
					}
				}
			}

			/// <summary>
			/// Рисование по часовой или против часовой стрелки
			/// </summary>
			public Boolean IsClockwiseDirection
			{
				get { return (mIsClockwiseDirection); }
				set
				{
					if (mIsClockwiseDirection != value)
					{
						mIsClockwiseDirection = value;

						if (mGeometry.IsCanvas)
						{
							// Обновляем данные
							mGeometry.Update();

							// Обновляем отображение примитива
							//XCadManager.Canvas.Update();
						}
					}
				}
			}

			/// <summary>
			/// Угол рисования дуги
			/// </summary>
			public Single RotationAngle
			{
				get { return (mRotationAngle); }
				set
				{
					if (!XMath.Approximately(mRotationAngle, value, 0.01))
					{
						mRotationAngle = value;

						if (mGeometry.IsCanvas)
						{
							// Обновляем данные
							mGeometry.Update();

							// Обновляем отображение примитива
							//XCadManager.Canvas.Update();
						}
					}
				}
			}

			/// <summary>
			/// Размер эллипса дуги по X
			/// </summary>
			public Single RadiusX
			{
				get { return (mRadiusX); }
				set
				{
					if (!XMath.Approximately(mRadiusX, value, 0.01))
					{
						mRadiusX = value;

						if (mGeometry.IsCanvas)
						{
							// Обновляем данные
							mGeometry.Update();

							// Обновляем отображение примитива
							//XCadManager.Canvas.Update();
						}
					}
				}
			}

			/// <summary>
			/// Размер эллипса дуги по Y
			/// </summary>
			public Single RadiusY
			{
				get { return (mRadiusY); }
				set
				{
					if (!XMath.Approximately(mRadiusY, value, 0.01))
					{
						mRadiusY = value;

						if (mGeometry.IsCanvas)
						{
							// Обновляем данные
							mGeometry.Update();

							// Обновляем отображение примитива
							//XCadManager.Canvas.Update();
						}
					}
				}
			}

			/// <summary>
			/// Конечная точка
			/// </summary>
			public Vector2Df EndPoint
			{
				get { return (mEndPoint); }
				set
				{
					if (!Vector2Df.Approximately(ref mEndPoint, ref value, 0.01f))
					{
						mEndPoint = value;

						if (mGeometry.IsCanvas)
						{
							// Обновляем данные
							mGeometry.Update();

							// Обновляем отображение примитива
							//XCadManager.Canvas.Update();
						}
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
			protected CCadSegmentArc()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="geometry">Геометрия</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadSegmentArc(CCadPrimitiveGeometry geometry)
			{
				mGeometry = geometry;
				mSegmentType = TCadSegmentType.Arc;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="geometry">Геометрия</param>
			/// <param name="start_point">Начальная точка</param>
			/// <param name="size_arc">Размеры эллипса дуги</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadSegmentArc(CCadPrimitiveGeometry geometry, Vector2Df start_point, Vector2Df size_arc)
			{
				mGeometry = geometry;
				mSegmentType = TCadSegmentType.Arc;
				mBasePoint = start_point;
				mRadiusX = size_arc.X;
				mRadiusY = size_arc.Y;
				mEndPoint = start_point + size_arc;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="geometry">Геометрия</param>
			/// <param name="segment_arc">Сегмент дуги</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadSegmentArc(CCadPrimitiveGeometry geometry, CCadSegmentArc segment_arc)
			{
				mGeometry = geometry;
				mIsLargeArc = segment_arc.mIsLargeArc;
				mIsClockwiseDirection = segment_arc.mIsClockwiseDirection;
				mRotationAngle = segment_arc.mRotationAngle;
				mRadiusX = segment_arc.mRadiusX;
				mRadiusY = segment_arc.mRadiusY;
				mEndPoint = segment_arc.mEndPoint;
				mSegmentType = TCadSegmentType.Arc;
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
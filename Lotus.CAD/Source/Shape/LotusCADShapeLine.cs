//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Интерактивные графические фигуры
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADShapeLine.cs
*		Линия с набором аналитических свойств.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 27.03.2022
//=====================================================================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Runtime.Serialization;
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
		//! \defgroup CadShape Графические фигуры
		//! Интерактивные графические фигуры
		//! \ingroup CadDrawing
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Линия с набором аналитических свойств
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		[DataContract]
		public class CCadShapeLine : CCadShape, IComparable<CCadShapeLine>
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			//
			// Константы для информирования об изменении свойств
			//
			protected static PropertyChangedEventArgs PropertyArgsStartPoint = new PropertyChangedEventArgs(nameof(StartPoint));
			protected static PropertyChangedEventArgs PropertyArgsEndPoint = new PropertyChangedEventArgs(nameof(EndPoint));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			// Основные параметры
			internal Vector2Df mStartPoint;
			internal Vector2Df mEndPoint;

			// Кэшированные данные
			internal Vector2Df mCachedStartPoint;
			internal Vector2Df mCachedEndPoint;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			//
			// ОСНОВНЫЕ ПАРАМЕТРЫ
			//
			/// <summary>
			/// Начальная точка
			/// </summary>
			[DisplayName("Начальная точка")]
			[Description("Начальная точка линии")]
			[Category(XInspectorGroupDesc.Size)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 0)]
			[DataMember]
			public Vector2Df StartPoint
			{
				get { return (mStartPoint); }
				set
				{
					mStartPoint = value;
					NotifyPropertyChanged(PropertyArgsStartPoint);
					NotifyPropertyChanged(PropertyArgsLocation);

					RaiseStartPointChanged();
				}
			}

			/// <summary>
			/// Конечная точка
			/// </summary>
			[DisplayName("Конечная точка")]
			[Description("Конечная точка линии")]
			[Category(XInspectorGroupDesc.Size)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 1)]
			[DataMember]
			public Vector2Df EndPoint
			{
				get { return (mEndPoint); }
				set
				{
					mEndPoint = value;
					NotifyPropertyChanged(PropertyArgsEndPoint);
					NotifyPropertyChanged(PropertyArgsLocation);
					RaiseEndPointChanged();
				}
			}

			/// <summary>
			/// Позиция центра прямоугольника
			/// </summary>
			[DisplayName("Позиция")]
			[Description("Позиция центра прямоугольника")]
			[Category(XInspectorGroupDesc.Size)]
			[Display(GroupName = XInspectorGroupDesc.Size, Order = 0)]
			public override Vector2Df Location
			{
				get { return ((mStartPoint + mEndPoint)/2); }
				set
				{
					Vector2Df delta = (mStartPoint + mEndPoint)/ 2 - value;
					mStartPoint -= delta;
					mEndPoint -= delta;

					NotifyPropertyChanged(PropertyArgsEndPoint);
					NotifyPropertyChanged(PropertyArgsStartPoint);
					NotifyPropertyChanged(PropertyArgsLocation);

					SetHandleRects();
				}
			}

			//
			// ПАРАМЕТРЫ РУЧЕК
			//
			/// <summary>
			/// Количество доступных ручек для управления
			/// </summary>
			[Browsable(false)]
			public override Int32 HandleCount
			{
				get { return (3); }
			}

			//
			// ПОДДЕРЖКА ИНСПЕКТОРА СВОЙСТВ
			//
			/// <summary>
			/// Отображаемое имя типа в инспекторе свойств
			/// </summary>
			[Browsable(false)]
			public override String InspectorTypeName
			{
				get { return ("ЛИНИЯ"); }
			}

			/// <summary>
			/// Отображаемое имя объекта в инспекторе свойств
			/// </summary>
			[Browsable(false)]
			public override String InspectorObjectName
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
			public CCadShapeLine()
				: this(Vector2Df.Zero, Vector2Df.Zero, "Линия")
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="start_point">Начальная точка</param>
			/// <param name="end_pont">Конечная точка</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadShapeLine(Vector2Df start_point, Vector2Df end_pont)
				: this(start_point, end_pont, "Линия")
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="start_point">Начальная точка</param>
			/// <param name="end_pont">Конечная точка</param>
			/// <param name="name">Имя линии</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadShapeLine(Vector2Df start_point, Vector2Df end_pont, String name)
				: base(name)
			{
				mGroup = "Линии";
				mStartPoint = start_point;
				mEndPoint = end_pont;

				mHandleRects.Add(Rect2Df.Empty);
				mHandleRects.Add(Rect2Df.Empty);
				mHandleRects.Add(Rect2Df.Empty);
			}
			#endregion

			#region ======================================= СИСТЕМНЫЕ МЕТОДЫ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Сравнение графических фигур для упорядочивания (по Z индексу)
			/// </summary>
			/// <param name="other">Сравниваемая графическая фигура</param>
			/// <returns>Статус сравнения графических фигур</returns>
			//---------------------------------------------------------------------------------------------------------
			public Int32 CompareTo(CCadShapeLine other)
			{
				if (ZIndex > other.ZIndex)
				{
					return (1);
				}
				else
				{
					if (ZIndex < other.ZIndex)
					{
						return (-1);
					}
					else
					{
						return (0);
					}
				}
			}
			#endregion

			#region ======================================= CЛУЖЕБНЫЕ МЕТОДЫ СОБЫТИЙ ==================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение начальной позиции линии.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseStartPointChanged()
			{
				SetHandleRects();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение конечной позиции линии.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseEndPointChanged()
			{
				SetHandleRects();
			}
			#endregion

			#region ======================================= МЕТОДЫ ILotusSupportEditInspector =========================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получить массив описателей свойств объекта
			/// </summary>
			/// <returns>Массив описателей</returns>
			//---------------------------------------------------------------------------------------------------------
			public override CPropertyDesc[] GetPropertiesDesc()
			{
				return (CadShapePropertiesDesc);
			}
			#endregion

			#region ======================================= МЕТОДЫ ILotusCopyParameters ===============================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Копирование параметров с указанного объекта
			/// </summary>
			/// <param name="source_object">Объект-источник с которого будут скопированы параметры</param>
			/// <param name="parameters">Контекст копирования параметров</param>
			//---------------------------------------------------------------------------------------------------------
			public override void CopyParameters(System.Object source_object, CParameters parameters)
			{
				base.CopyParameters(source_object, parameters);

				if (source_object is CCadShapeLine cad_shape_line)
				{
					mStartPoint = cad_shape_line.StartPoint;
					mEndPoint = cad_shape_line.EndPoint;

					NotifyPropertyChanged(PropertyArgsStartPoint);
					NotifyPropertyChanged(PropertyArgsEndPoint);
					NotifyPropertyChanged(PropertyArgsLocation);
				}
			}
			#endregion

			#region ======================================= МЕТОДЫ ILotusMementoOriginator ============================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получить состояние объекта
			/// </summary>
			/// <remarks>
			/// Под наименованием состояния объекта будем подразумевать имя свойства
			/// </remarks>
			/// <param name="name_state">Наименование состояния объекта</param>
			/// <returns>Состояние объекта</returns>
			//---------------------------------------------------------------------------------------------------------
			public override System.Object GetMemento(String name_state)
			{
				System.Object result = base.GetMemento(name_state);
				if (result != null)
				{
					return (result);
				}

				switch (name_state)
				{
					case nameof(StartPoint):
						{
							result = StartPoint;
						}
						break;
					case nameof(EndPoint):
						{
							result = EndPoint;
						}
						break;
					default:
						break;
				}

				return (result);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установить состояние объекта
			/// </summary>
			/// <remarks>
			/// Под наименованием состояния объекта будем подразумевать имя свойства
			/// </remarks>
			/// <param name="memento">Состояние объекта</param>
			/// <param name="name_state">Наименование состояния объекта</param>
			//---------------------------------------------------------------------------------------------------------
			public override void SetMemento(System.Object memento, String name_state)
			{
				base.SetMemento(memento, name_state);

				switch (name_state)
				{
					case nameof(StartPoint):
						{
							mStartPoint = (Vector2Df)memento;
							NotifyPropertyChanged(PropertyArgsStartPoint);
							RaiseStartPointChanged();
						}
						break;
					case nameof(EndPoint):
						{
							mEndPoint = (Vector2Df)memento;
							NotifyPropertyChanged(PropertyArgsEndPoint);
							RaiseEndPointChanged();
						}
						break;
					default:
						break;
				}
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление ограничивающего прямоугольника геометрии графического объекта
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void UpdateBoundsRect()
			{
				if (mStartPoint.X < mEndPoint.X)
				{
					mBoundsRect.X = mStartPoint.X;
					mBoundsRect.Right = mEndPoint.X;
				}
				else
				{
					mBoundsRect.X = mEndPoint.X;
					mBoundsRect.Right = mStartPoint.X;
				}

				if (mStartPoint.Y < mEndPoint.Y)
				{
					mBoundsRect.Y = mStartPoint.Y;
					mBoundsRect.Bottom = mEndPoint.Y;
				}
				else
				{
					mBoundsRect.Y = mEndPoint.Y;
					mBoundsRect.Bottom = mStartPoint.Y;
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Дубликат объекта
			/// </summary>
			/// <param name="context">Контекст дублирования объекта</param>
			/// <returns>Объект</returns>
			//---------------------------------------------------------------------------------------------------------
			public override ICadObject Duplicate(System.Object context)
			{
				CCadShapeLine cad_shape_line = new CCadShapeLine();
				cad_shape_line.CopyParameters(this, null);
				return (cad_shape_line);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение точек привязки графического элемента
			/// </summary>
			/// <remarks>
			/// Точки привязки позволяют более удобно привязываться к различным частям графического элемента
			/// </remarks>
			/// <returns>Точки привязки графического элемента</returns>
			//---------------------------------------------------------------------------------------------------------
			public override IList<Vector2Df> GetSnapNodes()
			{
				if (CanvasViewer.Selecting.CaptureElement != this && IsCreating == false)
				{
					return (new Vector2Df[] { mStartPoint, Location, mEndPoint });
				}
				else
				{
					return (null);
				}
			}
			#endregion

			#region ======================================= МЕТОДЫ ПРОВЕРКИ ПОПАДАНИЙ =================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка на попадание в область графического элемента указанной точки
			/// </summary>
			/// <param name="point">Проверяемая точка</param>
			/// <param name="epsilon">Точность проверки</param>
			/// <returns>Статус проверки</returns>
			//---------------------------------------------------------------------------------------------------------
			public override Boolean CheckPoint(ref Vector2Df point, Single epsilon)
			{
				// Если линия выбрана то проверяем ручки
				if (mIsSelect)
				{
					if (mHandleRects[0].Contains(point) || mHandleRects[1].Contains(point) || mHandleRects[2].Contains(point))
					{
						return (true);
					}
				}

				// Проверяем попадание на линию
				Boolean status = XIntersect2D.PointOnSegment(ref mStartPoint, ref mEndPoint, ref point, epsilon);
				if (status)
				{
					return (true);
				}
				else
				{
					return (false);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка на попадание ВСЕЙ ГЕОМЕТРИИ графического объекта внутрь прямоугольной области
			/// </summary>
			/// <param name="rect">Прямоугольник</param>
			/// <returns>Статус проверки</returns>
			//---------------------------------------------------------------------------------------------------------
			public override Boolean CheckInsideRect(ref Rect2Df rect)
			{
				return (rect.Contains(ref mStartPoint) && rect.Contains(ref mEndPoint));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка на попадание ВСЕЙ ИЛИ ЧАСТИ ГЕОМЕТРИИ графического объекта внутрь прямоугольной области
			/// </summary>
			/// <param name="rect">Прямоугольник</param>
			/// <returns>Статус проверки</returns>
			//---------------------------------------------------------------------------------------------------------
			public override Boolean CheckInsideOrIntersectRect(ref Rect2Df rect)
			{
				return (rect.Contains(ref mStartPoint) || rect.Contains(Location) || rect.Contains(ref mEndPoint));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Статус видимости всей геометрии объекта
			/// </summary>
			/// <returns>Статус видимости</returns>
			//---------------------------------------------------------------------------------------------------------
			public override Boolean CheckVisibleInViewport()
			{
				return (mCanvasViewer.CheckPointVisibleInViewport(ref mStartPoint) &&
					mCanvasViewer.CheckPointVisibleInViewport(ref mEndPoint));
			}
			#endregion

			#region ======================================= МЕТОДЫ ТРАНСФОРМАЦИИ ======================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перемещение графического элемента вверх
			/// </summary>
			/// <param name="offset">Значение смещения в условных единицах</param>
			//---------------------------------------------------------------------------------------------------------
			public override void MoveUp(Single offset)
			{
				mStartPoint.Y -= offset;
				mEndPoint.Y -= offset;
				NotifyPropertyChanged(PropertyArgsStartPoint);
				NotifyPropertyChanged(PropertyArgsEndPoint);
				NotifyPropertyChanged(PropertyArgsLocation);
				UpdateBoundsRect();
				SetHandleRects();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перемещение графического элемента вниз
			/// </summary>
			/// <param name="offset">Значение смещения в условных единицах</param>
			//---------------------------------------------------------------------------------------------------------
			public override void MoveDown(Single offset)
			{
				mStartPoint.Y += offset;
				mEndPoint.Y += offset;
				NotifyPropertyChanged(PropertyArgsStartPoint);
				NotifyPropertyChanged(PropertyArgsEndPoint);
				NotifyPropertyChanged(PropertyArgsLocation);
				UpdateBoundsRect();
				SetHandleRects();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перемещение графического элемента влево
			/// </summary>
			/// <param name="offset">Значение смещения в условных единицах</param>
			//---------------------------------------------------------------------------------------------------------
			public override void MoveLeft(Single offset)
			{
				mStartPoint.X -= offset;
				mEndPoint.X -= offset;
				NotifyPropertyChanged(PropertyArgsStartPoint);
				NotifyPropertyChanged(PropertyArgsEndPoint);
				NotifyPropertyChanged(PropertyArgsLocation);
				UpdateBoundsRect();
				SetHandleRects();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перемещение графического элемента вправо
			/// </summary>
			/// <param name="offset">Значение смещения в условных единицах</param>
			//---------------------------------------------------------------------------------------------------------
			public override void MoveRight(Single offset)
			{
				mStartPoint.X += offset;
				mEndPoint.X += offset;
				NotifyPropertyChanged(PropertyArgsStartPoint);
				NotifyPropertyChanged(PropertyArgsEndPoint);
				NotifyPropertyChanged(PropertyArgsLocation);
				UpdateBoundsRect();
				SetHandleRects();
			}
			#endregion

			#region ======================================= МЕТОДЫ УПРАВЛЕНИЯ =========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Начало захвата от управления курсора
			/// </summary>
			/// <param name="point">Точка в координатах канвы</param>
			//---------------------------------------------------------------------------------------------------------
			public override void StartCapturePosition(ref Vector2Df point)
			{
				// Проверяем наведение на ручки
				if (mHandleIndex != -1) return;

				// Если есть попадание
				mHandleIndex = CheckPointInHandleRect(ref point);

				switch (mHandleIndex)
				{
					case 0: // Начальная точка
						{
							mCanvasViewer.Selecting.CaptureElement = this;
							mCanvasViewer.Selecting.CapturePointOffset = mStartPoint - point;
						}
						break;
					case 1: // Точка по середине
						{
							mCanvasViewer.Selecting.CaptureElement = this;
							mCanvasViewer.Selecting.CapturePointOffset = Location - point;
						}
						break;
					case 2: // Конечная точка
						{
							mCanvasViewer.Selecting.CaptureElement = this;
							mCanvasViewer.Selecting.CapturePointOffset = mEndPoint - point;
						}
						break;
					default:
						{
							mCanvasViewer.Selecting.CaptureElement = null;
							mCanvasViewer.Selecting.CapturePointOffset = Vector2Df.Zero;
						}
						break;
				}

				// Ставим соответствующий курсор
				SetHandleCursor();

				// Сохраняем положение
				mCachedStartPoint = mStartPoint;
				mCachedEndPoint = mEndPoint;

				// Копируем в состояние
				SavePropertyToMemory(nameof(StartPoint));
				SavePropertyToMemory(nameof(EndPoint));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновления захвата управления от курсора
			/// </summary>
			/// <param name="point">Точка в координатах канвы</param>
			/// <param name="offset">Смещение в координатах канвы</param>
			//---------------------------------------------------------------------------------------------------------
			public void UpdateCapturePosition(ref Vector2Df point, ref Vector2Df offset)
			{
				switch (mHandleIndex)
				{
					case 0: // Начальная точка
						{
							if (mCanvasViewer.Selecting.EditModeMoving)
							{
								// Перемещаем графический элемент
								mStartPoint += offset;
								mEndPoint += offset;
							}
							else
							{
								mStartPoint = point + mCanvasViewer.Selecting.CapturePointOffset;
							}
						}
						break;
					case 1: // Точка по середине
						{
							// Перемещаем графический элемент
							mStartPoint += offset;
							mEndPoint += offset;
						}
						break;
					case 2: // Конечная точка
						{
							if (mCanvasViewer.Selecting.EditModeMoving)
							{
								// Перемещаем графический элемент
								mStartPoint += offset;
								mEndPoint += offset;
							}
							else
							{
								mEndPoint = point + mCanvasViewer.Selecting.CapturePointOffset;
							}
						}
						break;
					default:
						{
							// Графический элемент в составе множества - только перемещение
							mStartPoint += offset;
							mEndPoint += offset;
						}
						break;
				}

				// Обновляем ручки
				SetHandleRects();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновления захвата управления от курсора
			/// </summary>
			/// <param name="point">Точка в координатах канвы</param>
			//---------------------------------------------------------------------------------------------------------
			public override void UpdateCapturePosition(ref Vector2Df point)
			{
				// Смещение
				Vector2Df offset = mCanvasViewer.PointerDelta;

				// Обновляем
				UpdateCapturePosition(ref point, ref offset);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Окончание захвата от управления курсора
			/// </summary>
			/// <param name="point">Точка в координатах канвы</param>
			//---------------------------------------------------------------------------------------------------------
			public override void EndCapturePosition(ref Vector2Df point)
			{
				// Если была включена привязка
				if (mCanvasViewer.SnapIsExsisting)
				{
					// Смещение курсора мыши в координатах канвы от текущей точки привязки
					Vector2Df snap_offset = mCanvasViewer.SnapMouseOffsetCanvas;
					snap_offset -= mCanvasViewer.Selecting.CapturePointOffset;

					if (this == mCanvasViewer.Selecting.CaptureElement)
					{
						// Корректируем на величину смещения от опорной точки элемента
						Vector2Df snap_point = mCanvasViewer.SnapPoint - mCanvasViewer.Selecting.CapturePointOffset;
						UpdateCapturePosition(ref snap_point, ref snap_offset);
					}
					else
					{
						// Графический элемент в составе множества - только перемещение
						// Перемещаем графический элемент
						mStartPoint += snap_offset;
						mEndPoint += snap_offset;
					}

					SetHandleRects();
				}

				UpdateBoundsRect();
				NotifyPropertyChanged(PropertyArgsStartPoint);
				NotifyPropertyChanged(PropertyArgsEndPoint);
				NotifyPropertyChanged(PropertyArgsLocation);

				mHandleIndex = -1;
				SetHandleCursor();
			}
			#endregion

			#region ======================================= МЕТОДЫ РАБОТЫ С РУЧКАМИ ===================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка прямоугольников ручек
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void SetHandleRects()
			{
				if (mCanvasViewer != null)
				{
					mHandleRects[0] = mCanvasViewer.GetHandleRect(ref mStartPoint);
					mHandleRects[1] = mCanvasViewer.GetHandleRect(Location);
					mHandleRects[2] = mCanvasViewer.GetHandleRect(ref mEndPoint);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка соответствующего курсора на ручкой
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void SetHandleCursor()
			{
				if (mCanvasViewer != null)
				{
					if (mHandleIndex == -1)
					{
						mCanvasViewer.SetCursor(TCursor.Arrow);
					}
					else
					{
						if (mCanvasViewer.Selecting.EditModeMoving || mHandleIndex == 1)
						{
							mCanvasViewer.SetCursor(TCursor.SizeAll);
						}
						else
						{
							mCanvasViewer.SetCursor(TCursor.Cross);
						}
					}
				}
			}
			#endregion

			#region ======================================= МЕТОДЫ РИСОВАНИЯ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Рисование линии
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void Draw()
			{
				if (!mIsVisibleElement) return;

				if (mIsHalftone)
				{
					mCanvasViewer.SetOpacity(0.5f);
				}

				mCanvasViewer.DrawLine(ref mStartPoint, ref mEndPoint, mStrokePen);

				if (mIsHalftone)
				{
					mCanvasViewer.ResetOpacity();
				}

				// Статус выделения
				if (mIsSelect)
				{
					for (Int32 i = 0; i < HandleCount; i++)
					{
						mCanvasViewer.DrawHandleRect(mHandleRects[i], mHandleIndex == i);
					}
				}
				else
				{
					// Статус предварительного выделения
					if (mIsPreSelect)
					{
						mCanvasViewer.DrawLine(ref mStartPoint, ref mEndPoint, 
							XCadBrushManager.Green, mStrokePen.Thickness * 2);
					}
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
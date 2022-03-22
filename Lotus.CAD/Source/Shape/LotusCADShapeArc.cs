//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Интерактивные графические фигуры
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADShapeArc.cs
*		Дуга.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.01.2022
//=====================================================================================================================
using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
using Lotus.Maths;
//=====================================================================================================================
namespace Lotus
{
	namespace CAD
	{
		//-------------------------------------------------------------------------------------------------------------
		//! \addtogroup CadShape
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Дуга
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		[DataContract]
		public class CCadShapeArc : CCadShape, IComparable<CCadShapeArc>
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			protected static PropertyChangedEventArgs PropertyArgsStartPoint = new PropertyChangedEventArgs(nameof(StartPoint));
			protected static PropertyChangedEventArgs PropertyArgsEndPoint = new PropertyChangedEventArgs(nameof(EndPoint));
			protected static PropertyChangedEventArgs PropertyArgsIsLargeArc = new PropertyChangedEventArgs(nameof(IsLargeArc));
			protected static PropertyChangedEventArgs PropertyArgsIsClockwiseDirection = new PropertyChangedEventArgs(nameof(IsClockwiseDirection));
			protected static PropertyChangedEventArgs PropertyArgsRotationAngle = new PropertyChangedEventArgs(nameof(RotationAngle));
			protected static PropertyChangedEventArgs PropertyArgsRadiusX = new PropertyChangedEventArgs(nameof(RadiusX));
			protected static PropertyChangedEventArgs PropertyArgsRadiusY = new PropertyChangedEventArgs(nameof(RadiusY));
			protected static PropertyChangedEventArgs PropertyArgsIsClosed = new PropertyChangedEventArgs(nameof(IsClosed));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			// Основные параметры
			protected internal Vector2Df mStartPoint;
			protected internal Vector2Df mEndPoint;
			protected internal Boolean mIsLargeArc;
			protected internal Boolean mIsClockwiseDirection;
			protected internal Single mRotationAngle;
			protected internal Single mRadiusX;
			protected internal Single mRadiusY;
			protected internal Boolean mIsClosed;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			//
			// ОСНОВНЫЕ ПАРАМЕТРЫ
			//
			/// <summary>
			/// Начальная точка
			/// </summary>
			[DisplayName("Начальная точка")]
			[Description("Начальная точка")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 0)]
			public Vector2Df StartPoint
			{
				get { return (mStartPoint); }
				set
				{
					mStartPoint = value;
					SetHandleRects();
					NotifyPropertyChanged(PropertyArgsStartPoint);
					NotifyPropertyChanged(PropertyArgsLocation);
				}
			}

			/// <summary>
			/// Конечная точка
			/// </summary>
			[DisplayName("Конечная точка")]
			[Description("Конечная точка")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 1)]
			public Vector2Df EndPoint
			{
				get { return (mEndPoint); }
				set
				{
					mEndPoint = value;
					SetHandleRects();
					NotifyPropertyChanged(PropertyArgsEndPoint);
					NotifyPropertyChanged(PropertyArgsLocation);
				}
			}

			/// <summary>
			/// Большая или малая дуга
			/// </summary>
			[DisplayName("Большая дуга")]
			[Description("Большая или малая дуга")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 2)]
			public Boolean IsLargeArc
			{
				get { return (mIsLargeArc); }
				set
				{
					mIsLargeArc = value;
					SetHandleRects();
					NotifyPropertyChanged(PropertyArgsIsLargeArc);
				}
			}

			/// <summary>
			/// Рисование по часовой или против часовой стрелки
			/// </summary>
			[DisplayName("По часовой")]
			[Description("Рисование по часовой или против часовой стрелки")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 3)]
			public Boolean IsClockwiseDirection
			{
				get { return (mIsClockwiseDirection); }
				set
				{
					mIsClockwiseDirection = value;
					SetHandleRects();
					NotifyPropertyChanged(PropertyArgsIsClockwiseDirection);
				}
			}

			/// <summary>
			/// Угол рисования дуги
			/// </summary>
			[DisplayName("Угол")]
			[Description("Угол рисования дуги")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 4)]
			public Single RotationAngle
			{
				get { return (mRotationAngle); }
				set
				{
					mRotationAngle = value;
					SetHandleRects();
					NotifyPropertyChanged(PropertyArgsRotationAngle);
				}
			}

			/// <summary>
			/// Размер эллипса дуги по X
			/// </summary>
			[DisplayName("Размер по X")]
			[Description("Размер эллипса дуги по X")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 5)]
			public Single RadiusX
			{
				get { return (mRadiusX); }
				set
				{
					mRadiusX = value;
					SetHandleRects();
					NotifyPropertyChanged(PropertyArgsRadiusX);
				}
			}

			/// <summary>
			/// Размер эллипса дуги по Y
			/// </summary>
			[DisplayName("Размер по Y")]
			[Description("Размер эллипса дуги по Y")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 6)]
			public Single RadiusY
			{
				get { return (mRadiusY); }
				set
				{
					mRadiusY = value;
					SetHandleRects();
					NotifyPropertyChanged(PropertyArgsRadiusY);
				}
			}

			/// <summary>
			/// Статус замкнутости фигуры дуги
			/// </summary>
			[DisplayName("Замкнуть")]
			[Description("Статус замкнутости фигуры дуги")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 7)]
			public Boolean IsClosed
			{
				get { return (mIsClosed); }
				set
				{
					mIsClosed = value;
					SetHandleRects();
					NotifyPropertyChanged(PropertyArgsIsClosed);
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
				get { return ((mStartPoint + mEndPoint) / 2); }
				set
				{
					Vector2Df delta = (mStartPoint + mEndPoint) / 2 - value;
					mStartPoint -= delta;
					mEndPoint -= delta;

					NotifyPropertyChanged(PropertyArgsEndPoint);
					NotifyPropertyChanged(PropertyArgsStartPoint);
					NotifyPropertyChanged(PropertyArgsLocation);

					SetHandleRects();
				}
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
				get { return ("ДУГА"); }
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
			public CCadShapeArc()
			{
				mName = "Дуга: " + Id.ToString();
				mGroup = "Дуги";

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
			public Int32 CompareTo(CCadShapeArc other)
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
			/// Изменение позиции прямоугольника.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseLocationChanged()
			{
				SetHandleRects();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение ширины прямоугольника.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseWidthChanged()
			{
				SetHandleRects();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение высоты прямоугольника.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseHeightChanged()
			{
				SetHandleRects();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение поворота прямоугольника.
			/// Метод автоматически вызывается после установки соответствующих свойств
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseRotationParamChanged()
			{
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

				if (source_object is CCadShapeRect cad_shape_rect)
				{
					mStartPoint = cad_shape_rect.mStartPoint;
					mBoundsRect = cad_shape_rect.mBoundsRect;

					NotifyPropertyChanged(PropertyArgsStartPoint);
					NotifyPropertyChanged(PropertyArgsEndPoint);
					NotifyPropertyChanged(PropertyArgsIsLargeArc);
					NotifyPropertyChanged(PropertyArgsIsClockwiseDirection);
					NotifyPropertyChanged(PropertyArgsRotationAngle);
					NotifyPropertyChanged(PropertyArgsIsClosed);
					NotifyPropertyChanged(PropertyArgsRadiusX);
					NotifyPropertyChanged(PropertyArgsRadiusY);
					NotifyPropertyChanged(PropertyArgsLocation);
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
				CCadShapeRect cad_shape_rect = new CCadShapeRect();
				cad_shape_rect.CopyParameters(this, null);
				return (cad_shape_rect);
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
				if (CanvasViewer.Selecting.CaptureElement != null && mIsCreating == false)
				{
					return (new Vector2Df[] { StartPoint, Location, EndPoint });
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
				// Если есть заливка то проверяем все пространство дуги
				if (FillIsEnabled)
				{
					//if (FillContains(ref point))
					//{
					//	return (true);
					//}
				}

				// Если выбран то проверяем ручки
				if (mIsSelect)
				{
					for (Int32 i = 0; i < mHandleRects.Count; i++)
					{
						if (mHandleRects[i].Contains(ref point))
						{
							return (true);
						}
					}
				}

				// Если есть контур то проверяем границы дуги
				if (StrokeIsEnabled)
				{
					//return (StrokeContains(ref point, epsilon));
				}

				return (false);
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
				return (rect.Contains(BoundsRect.PointTopLeft) &&
						rect.Contains(BoundsRect.PointTopRight) &&
						rect.Contains(BoundsRect.PointBottomLeft) &&
						rect.Contains(BoundsRect.PointBottomRight));
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
				return (rect.Contains(BoundsRect.PointTopLeft) ||
						rect.Contains(BoundsRect.PointTopRight) ||
						rect.Contains(BoundsRect.PointBottomLeft) ||
						rect.Contains(BoundsRect.PointBottomRight));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Статус видимости всей геометрии объекта
			/// </summary>
			/// <returns>Статус видимости</returns>
			//---------------------------------------------------------------------------------------------------------
			public override Boolean CheckVisibleInViewport()
			{
				return (mCanvasViewer.CheckRectVisibleInViewport(BoundsRect));
			}
			#endregion

			#region ======================================= МЕТОДЫ СОЗДАНИЯ ===========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Начало создания дуги
			/// </summary>
			/// <param name="pos">Начальная позиция</param>
			//---------------------------------------------------------------------------------------------------------
			public void CreateStartArc(Vector2Df pos)
			{
				mEndPoint = pos;
				StartPoint = pos;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Продолжение создание дуги
			/// </summary>
			/// <param name="pos">Текущая позиция</param>
			//---------------------------------------------------------------------------------------------------------
			public void CreateContinueArc(ref Vector2Df pos)
			{
				mEndPoint = pos;
				mRadiusX = Math.Abs(mEndPoint.X - mStartPoint.X);
				mRadiusY = Math.Abs(mEndPoint.Y - mStartPoint.Y);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Окончание создание дуги
			/// </summary>
			/// <param name="pos">Конечная позиция</param>
			//---------------------------------------------------------------------------------------------------------
			public void CreateEndArc(Vector2Df pos)
			{
				mEndPoint = pos;
				mRadiusX = Math.Abs(mEndPoint.X - mStartPoint.X);
				mRadiusY = Math.Abs(mEndPoint.Y - mStartPoint.Y);
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

				SetHandleRects();

				NotifyPropertyChanged(PropertyArgsStartPoint);
				NotifyPropertyChanged(PropertyArgsEndPoint);
				NotifyPropertyChanged(PropertyArgsLocation);
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

				SetHandleRects();

				NotifyPropertyChanged(PropertyArgsStartPoint);
				NotifyPropertyChanged(PropertyArgsEndPoint);
				NotifyPropertyChanged(PropertyArgsLocation);
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

				SetHandleRects();

				NotifyPropertyChanged(PropertyArgsStartPoint);
				NotifyPropertyChanged(PropertyArgsEndPoint);
				NotifyPropertyChanged(PropertyArgsLocation);
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

				SetHandleRects();

				NotifyPropertyChanged(PropertyArgsStartPoint);
				NotifyPropertyChanged(PropertyArgsEndPoint);
				NotifyPropertyChanged(PropertyArgsLocation);
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
							mCanvasViewer.Selecting.CapturePointOffset = StartPoint - point;
						}
						break;
					case 1: // Точка по середине
						{
							mCanvasViewer.Selecting.CaptureElement = this;
						}
						break;
					case 2: // Конечная точка
						{
							mCanvasViewer.Selecting.CaptureElement = this;
							mCanvasViewer.Selecting.CapturePointOffset = EndPoint - point;
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
								mStartPoint = point;
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
								mEndPoint = point;
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
				Vector2Df offset = mCanvasViewer.PointerDelta;

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
					mHandleRects[0] = mCanvasViewer.GetHandleRect(StartPoint);
					mHandleRects[1] = mCanvasViewer.GetHandleRect(Location);
					mHandleRects[2] = mCanvasViewer.GetHandleRect(EndPoint);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка соответствующего курсора на ручкой
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void SetHandleCursor()
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
			#endregion

			#region ======================================= МЕТОДЫ РИСОВАНИЯ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Рисование дуги
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void Draw()
			{
				if (!mIsVisibleElement) return;

				if (mIsHalftone)
				{
					mCanvasViewer.SetOpacity(0.5f);
				}

				//mCanvasViewer.DrawEllipse(ref mStartPoint, ref mEndPoint, mStrokePen);

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
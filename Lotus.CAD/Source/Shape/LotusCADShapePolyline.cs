//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Интерактивные графические фигуры
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADShapePolyline.cs
*		Полилиния.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 04.04.2021
//=====================================================================================================================
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
		/// Полилиния
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		[TypeConverter(typeof(CCadShapePolylineConverter))]
		public class CCadShapePolyline : CCadShape<CCadPrimitiveGeometry>
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			protected static PropertyChangedEventArgs PropertyArgsStartPoint = new PropertyChangedEventArgs(nameof(StartPoint));
			protected static PropertyChangedEventArgs PropertyArgsEndPoint = new PropertyChangedEventArgs(nameof(EndPoint));
			protected static PropertyChangedEventArgs PropertyArgsIsClosed = new PropertyChangedEventArgs(nameof(IsClosed));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			// Основные параметры
			internal Boolean mIsClosed;
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
				get { return (mPrimitive.StartSegment.BasePoint); }
				set
				{
					mPrimitive.StartSegment.BasePoint = value;
					NotifyPropertyChanged(PropertyArgsStartPoint);
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
				get { return (mPrimitive.StartSegmentOfPoints.EndPoint); }
				set
				{
					mPrimitive.StartSegmentOfPoints.EndPoint = value;
					NotifyPropertyChanged(PropertyArgsEndPoint);

					mPrimitive.Update();
				}
			}

			/// <summary>
			/// Статус замкнутости фигуры полилинии
			/// </summary>
			[DisplayName("Замкнуть")]
			[Description("Статус замкнутости фигуры полилинии")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 2)]
			public Boolean IsClosed
			{
				get { return (mIsClosed); }
				set
				{
					if (mIsClosed != value)
					{
						mIsClosed = value;
						NotifyPropertyChanged(PropertyArgsIsClosed);
					}
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
				get { return ("ПОЛИЛИНИЯ"); }
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
			public CCadShapePolyline()
			{
				mName = "Полилиния: " + mID.ToString();
				mGroup = "Полилинии";

				mHandleRects.Add(Rect2Df.Empty);
				mHandleRects.Add(Rect2Df.Empty);

				mPrimitive = mCanvas.CreateGeometry(Vector2Df.Zero);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор копирования
			/// </summary>
			/// <param name="source">Источник копии</param>
			/// <param name="add_to_draft">Добавлять ли в чертеж источника</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadShapePolyline(CCadShapePolyline source, Boolean add_to_draft = true)
				: base(source, add_to_draft)
			{
				mHandleRects.Add(Rect2Df.Empty);
				mHandleRects.Add(Rect2Df.Empty);

				mPrimitive = source.mPrimitive.Duplicate() as CCadPrimitiveGeometry;
			}
			#endregion

			#region ======================================= СИСТЕМНЫЕ МЕТОДЫ ==========================================
			////---------------------------------------------------------------------------------------------------------
			///// <summary>
			///// Копирования полилинии
			///// </summary>
			///// <returns>Дубликат полилинии со всеми параметрами и данными</returns>
			////---------------------------------------------------------------------------------------------------------
			//public override IBaseElement Duplicate()
			//{
			//	return (new CCadShapePolyline(this, false));
			//}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Начало создания полилинии
			/// </summary>
			/// <param name="pos">Начальная позиция</param>
			//---------------------------------------------------------------------------------------------------------
			public void CreateStartPolyline(Vector2Df pos)
			{
				mPrimitive.AddSegmentPoints(ref pos, ref pos);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Продолжение создание полилинии
			/// </summary>
			/// <param name="pos">Текущая позиция</param>
			/// <param name="index">Индекс точки</param>
			//---------------------------------------------------------------------------------------------------------
			public void CreateContinuePolyline(ref Vector2Df pos, Int32 index)
			{
				mPrimitive.StartSegmentOfPoints[index] = pos;

				mPrimitive.Update();
				//mCanvas.Update();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавление точки к полилинии
			/// </summary>
			/// <param name="pos">Текущая позиция</param>
			//---------------------------------------------------------------------------------------------------------
			public void CreateAddPoint(Vector2Df pos)
			{
				mPrimitive.StartSegmentOfPoints.AddPoint(ref pos);

				mPrimitive.Update();
				//mCanvas.Update();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Окончание создание полилинии
			/// </summary>
			/// <param name="pos">Конечная позиция</param>
			//---------------------------------------------------------------------------------------------------------
			public void EndPolyline(Vector2Df pos)
			{
				mPrimitive.Update();
				//mCanvas.Update();

				SetHandleRects();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Копирование параметров с графического элемента
			/// </summary>
			/// <param name="element">Графический элемент</param>
			/// <param name="context">Контекст копирования данных</param>
			//---------------------------------------------------------------------------------------------------------
			public override void CopyParamemtrs(ICadShape element, Object context)
			{
				base.CopyParamemtrs(element, context);

				CCadShapePolyline source = element as CCadShapePolyline;
				if (source != null)
				{
					NotifyPropertyChanged(PropertyArgsStartPoint);
					NotifyPropertyChanged(PropertyArgsEndPoint);
				}
			}

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
				// Если есть заливка то проверяем все пространство полилинии
				if (mPrimitive.IsFilled)
				{
					if (mPrimitive.FillContains(ref point))
					{
						return (true);
					}
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

				// Если есть контур то проверяем границы полилинии
				if (mPrimitive.IsStroked)
				{
					return (mPrimitive.StrokeContains(ref point, epsilon));
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
				return (mCanvasViewer.CheckRectVisibleInViewport(ref mPrimitive.mBoundsRect));
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
				return (new Vector2Df[] { StartPoint, EndPoint });
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
				//mStartPoint.Y -= offset;
				//mEndPoint.Y -= offset;
				NotifyPropertyChanged(PropertyArgsStartPoint);
				NotifyPropertyChanged(PropertyArgsEndPoint);
				//UpdateGeometry();
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
				//mStartPoint.Y += offset;
				//mEndPoint.Y += offset;
				NotifyPropertyChanged(PropertyArgsStartPoint);
				NotifyPropertyChanged(PropertyArgsEndPoint);
				//UpdateGeometry();
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
				//mStartPoint.X -= offset;
				//mEndPoint.X -= offset;
				NotifyPropertyChanged(PropertyArgsStartPoint);
				NotifyPropertyChanged(PropertyArgsEndPoint);
				//UpdateGeometry();
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
				//mStartPoint.X += offset;
				//mEndPoint.X += offset;
				NotifyPropertyChanged(PropertyArgsStartPoint);
				NotifyPropertyChanged(PropertyArgsEndPoint);
				//UpdateGeometry();
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

				if (mHandleIndex != -1)
				{
					mCanvasViewer.Selecting.CaptureElement = this;
					mCanvasViewer.Selecting.CapturePointOffset = mPrimitive.StartSegmentOfPoints[mHandleIndex] - point;
				}
				else
				{
					mCanvasViewer.Selecting.CaptureElement = null;
					mCanvasViewer.Selecting.CapturePointOffset = Vector2Df.Zero;
				}

				// Ставим соответствующий курсор
				SetHandleCursor();

				// Сохраняем положение
				//mCachedStartPoint = mStartPoint;
				//mCachedEndPoint = mEndPoint;

				// Копируем в состояние
				//XManager.MementoManager.AddStateToHistory(new CStateMementoElementChange(this));

				// Обновляем канву
				//mCanvas.Update();
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
				if (mHandleIndex != -1)
				{
					if (mCanvasViewer.Selecting.EditModeMoving)
					{
						// Перемещаем графический элемент
						//mStartPoint += offset;
					}
					else
					{
						//mPoints[mHandleIndex] = point;
						//mHandleRects[mHandleIndex] = mCanvas.GetHandleRect(mPoints[mHandleIndex]);
					}
				}
				else
				{

				}

				//UpdateGeometry();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновления захвата управления от курсора
			/// </summary>
			/// <param name="point">Точка в координатах канвы</param>
			//---------------------------------------------------------------------------------------------------------
			public override void UpdateCapturePosition(ref Vector2Df point)
			{
				Vector2Df offset = mCanvasViewer.MouseDelta;

				UpdateCapturePosition(ref point, ref offset);

				// Обновляем ручки
				SetHandleRects();
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
					if (this == mCanvasViewer.Selecting.CaptureElement)
					{
						//Vector2Df snap_point = //mCanvasViewer.SnapPoint;
						//Vector2Df snap_offset = //mCanvasViewer.SnapMouseOffsetCanvasOfCapture;

						//UpdateCapturePosition(ref snap_point, ref snap_offset);
					}
					else
					{
						// Графический элемент в составе множества - только перемещение
						// Перемещаем графический элемент
						//Vector2Df delta = mCanvasViewer.SnapMouseOffsetCanvasOfCapture;
						//mStartPoint += delta;
						//mEndPoint += delta;
					}

					SetHandleRects();
				}

				// Если было включено копирование
				if (mCanvasViewer.Selecting.ModifyIsCopy)
				{
					// 1) Создаем новый объект
					CCadShapeLine line = Duplicate() as CCadShapeLine;
					//mBaseDocument.AddExistingElement(line);
					line.UpdateBoundsRect();

					// 2) Восстанавливаем старые координаты
					//mStartPoint = mCachedStartPoint;
					//mEndPoint = mCachedEndPoint;

					// 3) Добавляем новый в объекты копирования
					//mCanvasViewer.Selecting.CopyElements.Add(line);
				}

				//UpdateBoundsRect();
				NotifyPropertyChanged(PropertyArgsStartPoint);
				NotifyPropertyChanged(PropertyArgsEndPoint);

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
				if (mHandleRects.Count != mPrimitive.StartSegmentOfPoints.Count)
				{
					mHandleRects.Clear();
					for (Int32 i = 0; i < mPrimitive.StartSegmentOfPoints.Count; i++)
					{
						mHandleRects.Add(mCanvas.GetHandleRect(mPrimitive.StartSegmentOfPoints[i]));
					}
				}
				else
				{
					for (Int32 i = 0; i < mPrimitive.StartSegmentOfPoints.Count; i++)
					{
						mHandleRects[i] = mCanvas.GetHandleRect(mPrimitive.StartSegmentOfPoints[i]);
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка соответствующего курсора на ручкой
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void SetHandleCursor()
			{

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
				if (!mIsVisibleElement) return;

				mPrimitive.Draw();

				if (mIsSelect)
				{
					for (Int32 i = 0; i < HandleCount; i++)
					{
						mCanvas.DrawHandleRect(mHandleRects[i], mHandleIndex == i);
					}
				}
			}
			#endregion
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Конвертер типа полилинии для предоставления свойств
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CCadShapePolylineConverter : TypeConverter
		{
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение возможности использовать определенный набор свойств
			/// </summary>
			/// <param name="context">Контекст</param>
			/// <returns>True</returns>
			//---------------------------------------------------------------------------------------------------------
			public override Boolean GetPropertiesSupported(ITypeDescriptorContext context)
			{
				return (true);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение нужной коллекции свойств
			/// </summary>
			/// <param name="context">Контекст</param>
			/// <param name="value">Объект</param>
			/// <param name="attributes">Атрибуты</param>
			/// <returns>Сформированная коллекция свойств</returns>
			//---------------------------------------------------------------------------------------------------------
			public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, Object value,
				Attribute[] attributes)
			{
				List<PropertyDescriptor> result = new List<PropertyDescriptor>();
				PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(value, true);

				// 1) Общие данные
				result.Add(pdc["Name"]);
				result.Add(pdc["Group"]);
				result.Add(pdc["ID"]);

				// 2) Основные параметры
				result.Add(pdc["StartPoint"]);
				result.Add(pdc["EndPoint"]);
				//result.Add(pdc["IsLargePolyline"]);
				//result.Add(pdc["IsClockwiseDirection"]);
				//result.Add(pdc["RotationAngle"]);
				//result.Add(pdc["RadiusX"]);
				//result.Add(pdc["RadiusY"]);
				result.Add(pdc["IsClosed"]);

				// 2) Графика
				result.Add(pdc["Layer"]);
				result.Add(pdc["StrokeIsEnabled"]);
				result.Add(pdc["StrokeBrush"]);
				result.Add(pdc["StrokeThickness"]);
				result.Add(pdc["StrokeStyle"]);

				result.Add(pdc["FillIsEnabled"]);
				result.Add(pdc["Fill"]);
				result.Add(pdc["FillOpacity"]);
				result.Add(pdc["IsVisible"]);
				result.Add(pdc["IsHalftone"]);
				result.Add(pdc["IsEnabled"]);

				// 3) Размеры
				result.Add(pdc["ZIndex"]);

				return (new PropertyDescriptorCollection(result.ToArray(), true));
			}
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
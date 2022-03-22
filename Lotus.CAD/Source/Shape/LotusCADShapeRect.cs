//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Интерактивные графические фигуры
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADShapeRect.cs
*		Прямоугольник.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.01.2022
//=====================================================================================================================
using System;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;
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
		/// Прямоугольник
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		[DataContract]
		public class CCadShapeRect : CCadShape, IComparable<CCadShapeRect>
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			//
			// Константы для информирования об изменении свойств
			//
			protected static PropertyChangedEventArgs PropertyArgsWidth = new PropertyChangedEventArgs(nameof(Width));
			protected static PropertyChangedEventArgs PropertyArgsHeight = new PropertyChangedEventArgs(nameof(Height));
			protected static PropertyChangedEventArgs PropertyArgsRotationAngle = new PropertyChangedEventArgs(nameof(RotationAngle));
			protected static PropertyChangedEventArgs PropertyArgsRotationOrigin = new PropertyChangedEventArgs(nameof(RotationOrigin));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			// Основные параметры
			internal CCadTransform mTransform;

			// Служебные данные
			internal Vector2Df mStartPoint;
			internal Single mLeftModifyPoint;
			internal Single mTopModifyPoint;
			internal Single mRightModifyPoint;
			internal Single mBottomModifyPoint;

			// Кэшированные данные
			internal Vector2Df mCachedPosition;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			//
			// ОСНОВНЫЕ ПАРАМЕТРЫ
			//
			/// <summary>
			/// Позиция центра прямоугольника
			/// </summary>
			[DisplayName("Позиция")]
			[Description("Позиция центра прямоугольника")]
			[Category(XInspectorGroupDesc.Size)]
			[Display(GroupName = XInspectorGroupDesc.Size, Order = 0)]
			[DataMember]
			public override Vector2Df Location
			{
				get { return (mTransform.Position); }
				set
				{
					mTransform.Position = value;
					NotifyPropertyChanged(PropertyArgsLocation);
					RaiseLocationChanged();
				}
			}

			/// <summary>
			/// Ширина прямоугольника
			/// </summary>
			[DisplayName("Ширина")]
			[Description("Ширина прямоугольника")]
			[Category(XInspectorGroupDesc.Size)]
			[Display(GroupName = XInspectorGroupDesc.Size, Order = 1)]
			[DataMember]
			public Single Width
			{
				get { return (mBoundsRect.Width); }
				set
				{
					// Обновляем ширину
					mBoundsRect.Width = value;

					// Центр прямоугольника
					Vector2Df center = Transform.TransformPointToCanvas(mBoundsRect.Center);
					mBoundsRect.X = -mBoundsRect.Width / 2;
					mBoundsRect.Y = -mBoundsRect.Height / 2;

					// Обновляем трансформацию
					mTransform.mRotationOrigin = Vector2Df.Zero;
					mTransform.Position = center;

					// Обновляем свойства
					NotifyPropertyChanged(PropertyArgsWidth);
					NotifyPropertyChanged(PropertyArgsLocation);

					RaiseWidthChanged();
				}
			}

			/// <summary>
			/// Высота прямоугольника
			/// </summary>
			[DisplayName("Высота")]
			[Description("Высота прямоугольника")]
			[Category(XInspectorGroupDesc.Size)]
			[Display(GroupName = XInspectorGroupDesc.Size, Order = 2)]
			[DataMember]
			public Single Height
			{
				get { return (mBoundsRect.Height); }
				set
				{
					// Обновляем высоту
					mBoundsRect.Height = value;

					// Центр прямоугольника
					Vector2Df center = Transform.TransformPointToCanvas(mBoundsRect.Center);
					mBoundsRect.X = -mBoundsRect.Width / 2;
					mBoundsRect.Y = -mBoundsRect.Height / 2;

					// Обновляем трансформацию
					mTransform.mRotationOrigin = Vector2Df.Zero;
					mTransform.Position = center;

					// Обновляем свойства
					NotifyPropertyChanged(PropertyArgsHeight);
					NotifyPropertyChanged(PropertyArgsLocation);

					RaiseHeightChanged();
				}
			}

			/// <summary>
			/// Трансформация прямоугольника
			/// </summary>
			[Browsable(false)]
			public CCadTransform Transform
			{
				get { return (mTransform); }
			}

			/// <summary>
			/// Угол поворота прямоугольника
			/// </summary>
			[DisplayName("Угол поворота")]
			[Description("Угол поворота прямоугольника")]
			[Category(XInspectorGroupDesc.Size)]
			[Display(GroupName = XInspectorGroupDesc.Size, Order = 3)]
			[DataMember]
			public Single RotationAngle
			{
				get { return (mTransform.RotationAngle); }
				set
				{
					if (!XMath.Approximately(mTransform.RotationAngle, value, 0.01f))
					{
						mTransform.RotationAngle = value;
						NotifyPropertyChanged(PropertyArgsRotationAngle);
						RaiseRotationParamChanged();
					}
				}
			}

			/// <summary>
			/// Точка оси поворота прямоугольника
			/// </summary>
			[DisplayName("Ось поворота")]
			[Description("Точка оси поворота прямоугольника")]
			[Category(XInspectorGroupDesc.Size)]
			[Display(GroupName = XInspectorGroupDesc.Size, Order = 4)]
			[DataMember]
			public Vector2Df RotationOrigin
			{
				get { return (mTransform.RotationOrigin); }
				set
				{
					mTransform.RotationOrigin = value;
					NotifyPropertyChanged(PropertyArgsRotationOrigin);
					RaiseRotationParamChanged();
				}
			}

			/// <summary>
			/// Ограничивающий прямоугольник геометрии графической фигуры
			/// </summary>
			[Browsable(false)]
			public override Rect2Df BoundsRect
			{
				get { return (mTransform.TransformBounds(mBoundsRect)); }
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
				get { return (9); }
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
				get { return ("ПРЯМОУГОЛЬНИК"); }
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
			public CCadShapeRect()
			{
				for (Int32 i = 0; i < 9; i++)
				{
					mHandleRects.Add(Rect2Df.Empty);
				}

				mTransform = new CCadTransform();
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
			public Int32 CompareTo(CCadShapeRect other)
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

					NotifyPropertyChanged(PropertyArgsWidth);
					NotifyPropertyChanged(PropertyArgsHeight);
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

					Vector2Df ptl = mTransform.TransformPointToCanvas(mBoundsRect.PointTopLeft);
					Vector2Df ptr = mTransform.TransformPointToCanvas(mBoundsRect.PointTopRight);
					Vector2Df pbl = mTransform.TransformPointToCanvas(mBoundsRect.PointBottomLeft);
					Vector2Df pbr = mTransform.TransformPointToCanvas(mBoundsRect.PointBottomRight);
					Vector2Df pc = mTransform.TransformPointToCanvas(mBoundsRect.Center);

					return (new Vector2Df[] { ptl, ptr, pc, pbl, pbr, (ptl + ptr)/2,
					(ptl + pbl)/2, (ptr + pbr)/2, (pbl + pbr)/2});
				}
				else
				{
					return (null);
				}
			}
			#endregion

			#region ======================================= МЕТОДЫ СОЗДАНИЯ ===========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Начало создания прямоугольника
			/// </summary>
			/// <param name="pos">Начальная позиция</param>
			//---------------------------------------------------------------------------------------------------------
			public virtual void CreateStartRect(ref Vector2Df pos)
			{
				mStartPoint = pos;
				mBoundsRect.PointTopLeft = mStartPoint;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Продолжение создание прямоугольника
			/// </summary>
			/// <param name="pos">Текущая позиция</param>
			//---------------------------------------------------------------------------------------------------------
			public virtual void CreateContinueRect(ref Vector2Df pos)
			{
				// Корректировка прямоугольника
				if (mStartPoint.X < pos.X)
				{
					mBoundsRect.X = mStartPoint.X;
				}
				else
				{
					mBoundsRect.X = pos.X;
				}

				if (mStartPoint.Y < pos.Y)
				{
					mBoundsRect.Y = mStartPoint.Y;
				}
				else
				{
					mBoundsRect.Y = pos.Y;
				}

				mBoundsRect.Width = Math.Abs(mStartPoint.X - pos.X);
				mBoundsRect.Height = Math.Abs(mStartPoint.Y - pos.Y);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Окончание создания прямоугольника
			/// </summary>
			/// <param name="pos">Конечная позиция</param>
			//---------------------------------------------------------------------------------------------------------
			public virtual void CreateEndRect(ref Vector2Df pos)
			{
				// Корректировка прямоугольника
				if (mStartPoint.X < pos.X)
				{
					mBoundsRect.X = mStartPoint.X;
				}
				else
				{
					mBoundsRect.X = pos.X;
				}

				if (mStartPoint.Y < pos.Y)
				{
					mBoundsRect.Y = mStartPoint.Y;
				}
				else
				{
					mBoundsRect.Y = pos.Y;
				}

				mBoundsRect.Width = Math.Abs(mStartPoint.X - pos.X);
				mBoundsRect.Height = Math.Abs(mStartPoint.Y - pos.Y);

				// Центр прямоугольника
				Vector2Df center = mBoundsRect.Center;
				mBoundsRect.X = -mBoundsRect.Width / 2;
				mBoundsRect.Y = -mBoundsRect.Height / 2;

				// Обновляем трансформацию
				mTransform.mRotationOrigin = Vector2Df.Zero;
				mTransform.Position = center;

				// Обновляем ручки
				SetHandleRects();
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
				// Трансформируем точку из пространства канвы в пространство примитива
				Vector2Df point_check = mTransform.TransformPointToLocal(ref point);

				// Если есть заливка то проверяем все пространство прямоугольника
				if (FillIsEnabled)
				{
					if (mBoundsRect.Contains(ref point_check))
					{
						return (true);
					}
				}

				// Если выбран то проверяем ручки
				if (mIsSelect)
				{
					if ((CheckPointInHandleRect(ref point_check) != -1))
					{
						return (true);
					}
				}

				// Если есть контур то проверяем границы прямоугольника
				if (StrokeIsEnabled)
				{
					Boolean status1 = XIntersect2D.PointOnSegment(mBoundsRect.PointTopLeft, 
						mBoundsRect.PointTopRight, point_check, epsilon);
					if (status1)
					{
						return (true);
					}
					Boolean status2 = XIntersect2D.PointOnSegment(mBoundsRect.PointTopLeft, 
						mBoundsRect.PointBottomLeft, point_check, epsilon);
					if (status2)
					{
						return (true);
					}
					Boolean status3 = XIntersect2D.PointOnSegment(mBoundsRect.PointTopRight, 
						mBoundsRect.PointBottomRight, point_check, epsilon);
					if (status3)
					{
						return (true);
					}
					Boolean status4 = XIntersect2D.PointOnSegment(mBoundsRect.PointBottomLeft, 
						mBoundsRect.PointBottomRight, point_check, epsilon);
					if (status4)
					{
						return (true);
					}
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
				Vector2Df ptl = mTransform.TransformPointToCanvas(mBoundsRect.PointTopLeft);
				Vector2Df ptr = mTransform.TransformPointToCanvas(mBoundsRect.PointTopRight);
				Vector2Df pbl = mTransform.TransformPointToCanvas(mBoundsRect.PointBottomLeft);
				Vector2Df pbr = mTransform.TransformPointToCanvas(mBoundsRect.PointBottomRight);
				return (rect.Contains(ref ptl) &&
						rect.Contains(ref ptr) &&
						rect.Contains(ref pbl) &&
						rect.Contains(ref pbr));
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
				Vector2Df ptl = mTransform.TransformPointToCanvas(mBoundsRect.PointTopLeft);
				Vector2Df ptr = mTransform.TransformPointToCanvas(mBoundsRect.PointTopRight);
				Vector2Df pbl = mTransform.TransformPointToCanvas(mBoundsRect.PointBottomLeft);
				Vector2Df pbr = mTransform.TransformPointToCanvas(mBoundsRect.PointBottomRight);
				return (rect.Contains(ref ptl) ||
						rect.Contains(ref ptr) ||
						rect.Contains(ref pbl) ||
						rect.Contains(ref pbr));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Статус видимости всей геометрии объекта
			/// </summary>
			/// <returns>Статус видимости</returns>
			//---------------------------------------------------------------------------------------------------------
			public override Boolean CheckVisibleInViewport()
			{
				Vector2Df ptl = mTransform.TransformPointToCanvas(mBoundsRect.PointTopLeft);
				Vector2Df ptr = mTransform.TransformPointToCanvas(mBoundsRect.PointTopRight);
				Vector2Df pbl = mTransform.TransformPointToCanvas(mBoundsRect.PointBottomLeft);
				Vector2Df pbr = mTransform.TransformPointToCanvas(mBoundsRect.PointBottomRight);

				return (mCanvasViewer.CheckPointVisibleInViewport(ref ptl) &&
					mCanvasViewer.CheckPointVisibleInViewport(ref ptr) &&
					mCanvasViewer.CheckPointVisibleInViewport(ref pbl) &&
					mCanvasViewer.CheckPointVisibleInViewport(ref pbr));
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
				mTransform.MoveUp(offset);
				NotifyPropertyChanged(PropertyArgsLocation);
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
				mTransform.MoveDown(offset);
				NotifyPropertyChanged(PropertyArgsLocation);
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
				mTransform.MoveLeft(offset);
				NotifyPropertyChanged(PropertyArgsLocation);
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
				mTransform.MoveRight(offset);
				NotifyPropertyChanged(PropertyArgsLocation);
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

				// Трансформируем точку из пространства канвы в пространство примитива
				Vector2Df point_check = mTransform.TransformPointToLocal(ref point);

				// Если есть попадание
				mHandleIndex = CheckPointInHandleRect(ref point_check);

				switch (mHandleIndex)
				{
					case 0: // 1 точка
						{
							mCanvasViewer.Selecting.CaptureElement = this;
							mCanvasViewer.Selecting.CapturePointOffset = mTransform.TransformPointToCanvas(mBoundsRect.PointTopLeft) - point;
						}
						break;
					case 1: // 2 точка
						{
							mCanvasViewer.Selecting.CaptureElement = this;
							mCanvasViewer.Selecting.CapturePointOffset = mTransform.TransformPointToCanvas(mBoundsRect.PointTopLeftRightMiddle) - point;
						}
						break;
					case 2: // 3 точка
						{
							mCanvasViewer.Selecting.CaptureElement = this;
							mCanvasViewer.Selecting.CapturePointOffset = mTransform.TransformPointToCanvas(mBoundsRect.PointTopRight) - point;
						}
						break;
					case 3: // 4 точка
						{
							mCanvasViewer.Selecting.CaptureElement = this;
							mCanvasViewer.Selecting.CapturePointOffset = mTransform.TransformPointToCanvas(mBoundsRect.PointRightTopBottomMiddle) - point;
						}
						break;
					case 4: // 5 точка
						{
							mCanvasViewer.Selecting.CaptureElement = this;
							mCanvasViewer.Selecting.CapturePointOffset = mTransform.TransformPointToCanvas(mBoundsRect.PointBottomRight) - point;
						}
						break;
					case 5: // 6 точка
						{
							mCanvasViewer.Selecting.CaptureElement = this;
							mCanvasViewer.Selecting.CapturePointOffset = mTransform.TransformPointToCanvas(mBoundsRect.PointBottomLeftRightMiddle) - point;
						}
						break;
					case 6: // 7 точка
						{
							mCanvasViewer.Selecting.CaptureElement = this;
							mCanvasViewer.Selecting.CapturePointOffset = mTransform.TransformPointToCanvas(mBoundsRect.PointBottomLeft) - point;
						}
						break;
					case 7: // 8 точка
						{
							mCanvasViewer.Selecting.CaptureElement = this;
							mCanvasViewer.Selecting.CapturePointOffset = mTransform.TransformPointToCanvas(mBoundsRect.PointLeftTopBottomMiddle) - point;
						}
						break;
					case 8: // Центр
						{
							mCanvasViewer.Selecting.CaptureElement = this;
							mCanvasViewer.Selecting.CapturePointOffset = mTransform.TransformPointToCanvas(mBoundsRect.Center) - point;
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

				// Сохраняем размеры
				mStartPoint = new Vector2Df(mBoundsRect.X, mBoundsRect.Y);
				mLeftModifyPoint = mBoundsRect.X;
				mTopModifyPoint = mBoundsRect.Y;
				mRightModifyPoint = mBoundsRect.Right;
				mBottomModifyPoint = mBoundsRect.Bottom;

				// Сохраняем положение
				mCachedPosition = mTransform.Position;

				// Копируем в состояние
				mCanvasViewer.Memento.AddStateToHistory(new CMementoCaretakerChanged(this, nameof(Location)));
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
					case 0:
						{
							if (mCanvasViewer.Selecting.EditModeMoving)
							{
								mTransform.Move(ref offset);
							}
							else
							{
								offset = Transform.TransformVectorToLocal(offset);
								mLeftModifyPoint += offset.X;
								mTopModifyPoint += offset.Y;

								// Корректируем прямоугольник
								mBoundsRect.Width = Math.Abs(mRightModifyPoint - mLeftModifyPoint);
								mBoundsRect.Height = Math.Abs(mBottomModifyPoint - mTopModifyPoint);
								mBoundsRect.X = mLeftModifyPoint;
								mBoundsRect.Y = mTopModifyPoint;
							}
						}
						break;
					case 1:
						{
							if (mCanvasViewer.Selecting.EditModeMoving)
							{
								mTransform.Move(ref offset);
							}
							else
							{
								offset = Transform.TransformVectorToLocal(offset);
								mTopModifyPoint += offset.Y;

								// Корректируем прямоугольник
								mBoundsRect.Width = Math.Abs(mRightModifyPoint - mLeftModifyPoint);
								mBoundsRect.Height = Math.Abs(mBottomModifyPoint - mTopModifyPoint);
								mBoundsRect.X = mLeftModifyPoint;
								mBoundsRect.Y = mTopModifyPoint;
							}
						}
						break;
					case 2:
						{
							if (mCanvasViewer.Selecting.EditModeMoving)
							{
								mTransform.Move(ref offset);
							}
							else
							{
								offset = Transform.TransformVectorToLocal(offset);
								mRightModifyPoint += offset.X;
								mTopModifyPoint += offset.Y;

								// Корректируем прямоугольник
								mBoundsRect.Width = Math.Abs(mRightModifyPoint - mLeftModifyPoint);
								mBoundsRect.Height = Math.Abs(mBottomModifyPoint - mTopModifyPoint);
								mBoundsRect.X = mLeftModifyPoint;
								mBoundsRect.Y = mTopModifyPoint;
							}
						}
						break;
					case 3:
						{
							if (mCanvasViewer.Selecting.EditModeMoving)
							{
								mTransform.Move(ref offset);
							}
							else
							{
								offset = Transform.TransformVectorToLocal(offset);
								mRightModifyPoint += offset.X;

								// Корректируем прямоугольник
								mBoundsRect.Width = Math.Abs(mRightModifyPoint - mLeftModifyPoint);
								mBoundsRect.Height = Math.Abs(mBottomModifyPoint - mTopModifyPoint);
								mBoundsRect.X = mLeftModifyPoint;
								mBoundsRect.Y = mTopModifyPoint;
							}
						}
						break;
					case 4:
						{
							if (mCanvasViewer.Selecting.EditModeMoving)
							{
								mTransform.Move(ref offset);
							}
							else
							{
								offset = Transform.TransformVectorToLocal(offset);
								mRightModifyPoint += offset.X;
								mBottomModifyPoint += offset.Y;

								// Корректируем прямоугольник
								mBoundsRect.Width = Math.Abs(mRightModifyPoint - mLeftModifyPoint);
								mBoundsRect.Height = Math.Abs(mBottomModifyPoint - mTopModifyPoint);
								mBoundsRect.X = mLeftModifyPoint;
								mBoundsRect.Y = mTopModifyPoint;
							}
						}
						break;
					case 5:
						{
							if (mCanvasViewer.Selecting.EditModeMoving)
							{
								mTransform.Move(ref offset);
							}
							else
							{
								offset = Transform.TransformVectorToLocal(offset);
								mBottomModifyPoint += offset.Y;

								// Корректируем прямоугольник
								mBoundsRect.Width = Math.Abs(mRightModifyPoint - mLeftModifyPoint);
								mBoundsRect.Height = Math.Abs(mBottomModifyPoint - mTopModifyPoint);
								mBoundsRect.X = mLeftModifyPoint;
								mBoundsRect.Y = mTopModifyPoint;
							}
						}
						break;
					case 6:
						{
							if (mCanvasViewer.Selecting.EditModeMoving)
							{
								mTransform.Move(ref offset);
							}
							else
							{
								offset = Transform.TransformVectorToLocal(offset);
								mBottomModifyPoint += offset.Y;
								mLeftModifyPoint += offset.X;

								// Корректируем прямоугольник
								mBoundsRect.Width = Math.Abs(mRightModifyPoint - mLeftModifyPoint);
								mBoundsRect.Height = Math.Abs(mBottomModifyPoint - mTopModifyPoint);
								mBoundsRect.X = mLeftModifyPoint;
								mBoundsRect.Y = mTopModifyPoint;
							}
						}
						break;
					case 7:
						{
							if (mCanvasViewer.Selecting.EditModeMoving)
							{
								mTransform.Move(ref offset);
							}
							else
							{
								offset = Transform.TransformVectorToLocal(offset);
								mLeftModifyPoint += offset.X;

								// Корректируем прямоугольник
								mBoundsRect.Width = Math.Abs(mRightModifyPoint - mLeftModifyPoint);
								mBoundsRect.Height = Math.Abs(mBottomModifyPoint - mTopModifyPoint);
								mBoundsRect.X = mLeftModifyPoint;
								mBoundsRect.Y = mTopModifyPoint;
							}
						}
						break;
					case 8:
						{
							mTransform.Move(ref offset);
						}
						break;
					default:
						{
							// Графический элемент в составе множества - только перемещение
							mTransform.Move(ref offset);
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
				// Трансформируем точку из пространства канвы в пространство примитива
				Vector2Df pos = mTransform.TransformPointToLocal(ref point);

				// Смещение
				Vector2Df offset = mCanvasViewer.PointerDelta;

				// Обновляем
				UpdateCapturePosition(ref pos, ref offset);
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

					if (this == mCanvasViewer.Selecting.CaptureElement)
					{
						// Трансформируем точку из пространства канвы в пространство примитива
						Vector2Df snap_point = mTransform.TransformPointToLocal(mCanvasViewer.SnapPoint);

						// Корректируем на величину смещения от опорной точки элемента
						snap_point -= mCanvasViewer.Selecting.CapturePointOffset;
						snap_offset -= mCanvasViewer.Selecting.CapturePointOffset;

						UpdateCapturePosition(ref snap_point, ref snap_offset);
					}
					else
					{
						// Графический элемент в составе множества - только перемещение
						mTransform.Move(ref snap_offset);
					}
				}

				// Центр прямоугольника
				Vector2Df center = Transform.TransformPointToCanvas(mBoundsRect.Center);
				mBoundsRect.X = -mBoundsRect.Width / 2;
				mBoundsRect.Y = -mBoundsRect.Height / 2;

				// Обновляем трансформацию
				mTransform.mRotationOrigin = Vector2Df.Zero;
				mTransform.Position = center;

				// Обновляем свойства
				NotifyPropertyChanged(PropertyArgsLocation);
				NotifyPropertyChanged(PropertyArgsWidth);
				NotifyPropertyChanged(PropertyArgsHeight);

				// Курсор
				mHandleIndex = -1;
				SetHandleCursor();

				// Обновляем ручки
				SetHandleRects();
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
					mHandleRects[0] = mCanvasViewer.GetHandleRect(mBoundsRect.PointTopLeft);
					mHandleRects[1] = mCanvasViewer.GetHandleRect(mBoundsRect.PointTopLeftRightMiddle);
					mHandleRects[2] = mCanvasViewer.GetHandleRect(mBoundsRect.PointTopRight);
					mHandleRects[3] = mCanvasViewer.GetHandleRect(mBoundsRect.PointRightTopBottomMiddle);
					mHandleRects[4] = mCanvasViewer.GetHandleRect(mBoundsRect.PointBottomRight);
					mHandleRects[5] = mCanvasViewer.GetHandleRect(mBoundsRect.PointBottomLeftRightMiddle);
					mHandleRects[6] = mCanvasViewer.GetHandleRect(mBoundsRect.PointBottomLeft);
					mHandleRects[7] = mCanvasViewer.GetHandleRect(mBoundsRect.PointLeftTopBottomMiddle);
					mHandleRects[8] = mCanvasViewer.GetHandleRect(mBoundsRect.Center);
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
					if (mCanvasViewer.Selecting.EditModeMoving || mHandleIndex == 8)
					{
						mCanvasViewer.SetCursor(TCursor.SizeAll);
					}
					else
					{
						if (mHandleIndex == 0 || mHandleIndex == 4)
						{
							mCanvasViewer.SetCursor(TCursor.SizeNWSE);
						}
						if (mHandleIndex == 1 || mHandleIndex == 5)
						{
							mCanvasViewer.SetCursor(TCursor.SizeNS);
						}
						if (mHandleIndex == 2 || mHandleIndex == 6)
						{
							mCanvasViewer.SetCursor(TCursor.SizeNESW);
						}
						if (mHandleIndex == 3 || mHandleIndex == 7)
						{
							mCanvasViewer.SetCursor(TCursor.SizeWE);
						}
					}
				}
			}
			#endregion

			#region ======================================= МЕТОДЫ РИСОВАНИЯ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Рисование прямоугольника
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void Draw()
			{
				if (!mIsVisibleElement) return;

				mCanvasViewer.SetTransform(mTransform);

				if(mIsHalftone)
				{
					mCanvasViewer.SetOpacity(0.5f);
				}

				if (mFillIsEnabled)
				{
					mCanvasViewer.DrawRectangle(ref mBoundsRect, mFillBrush, -1);
				}

				if (mStrokeIsEnabled)
				{
					mCanvasViewer.DrawRectangle(ref mBoundsRect, mStrokePen);
				}


				if (mIsHalftone)
				{
					mCanvasViewer.ResetOpacity();
				}

				if (mIsSelect)
				{
					for (Int32 i = 0; i < HandleCount; i++)
					{
						mCanvasViewer.DrawHandleRect(mHandleRects[i], mHandleIndex == i);
					}
				}

				mCanvasViewer.ResetTransform();
			}
			#endregion

			#region ======================================= ОБРАБОТКА СОБЫТИЙ КОНТЕКСТНОГО МЕНЮ =======================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Открытие контекстного меню
			/// </summary>
			/// <param name="point">Точка открытия контекстного меню в пространстве канвы</param>
			//---------------------------------------------------------------------------------------------------------
			public override void OnOpenContextMenu(ref Vector2Df point)
			{
				Vector2Df check_point = Transform.TransformPointToLocal(ref point);
				if (CheckPointInHandleRect(ref check_point) != -1)
				{
					mCanvasViewer.AddCommandContextMenu("Дублировать");
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Закрытие контекстного меню
			/// </summary>
			/// <param name="point">Точка закрытия контекстного меню в пространстве канвы</param>
			//---------------------------------------------------------------------------------------------------------
			public override void OnClosedContextMenu(ref Vector2Df point)
			{
				mCanvasViewer.RemoveCommandContextMenu("Дублировать");
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Выполнения команды контекстного меню
			/// </summary>
			/// <param name="command_name">Имя команды</param>
			/// <param name="context">Контекст данных</param>
			//---------------------------------------------------------------------------------------------------------
			public override void OnCommandContextMenu(String command_name, Object context)
			{
				if (command_name == "Дублировать")
				{
					CCadShapeRect rect = this.Duplicate(context) as CCadShapeRect;
					rect.Location += new Vector2Df(20, 20);

					mCanvasViewer.AddObject(rect);
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
//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Элементы управления
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADSelecting.cs
*		Подситема выбора объектов.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.01.2022
//=====================================================================================================================
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		//! \defgroup CadSelecting Подситема выбора объектов
		//! Подсистема выбора объектов обеспечивает выбор графических объектов и управлениями им.
		//! \ingroup CadDrawing
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Элемент управления для обработки пользовательского ввода
		/// </summary>
		/// <remarks>
		/// Элемент предназначен для обработки пользовательского ввода и управления коллекцией выбранных
		/// графических элементов, представление общих свойств
		/// </remarks>
		//-------------------------------------------------------------------------------------------------------------
		public class CCadSelecting : PropertyChangedBase
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			// Общие данные
			protected internal ICadCanvasViewer mCanvasViewer;
			protected internal ListArray<ICadObject> mSelectedElements;
			protected internal ICadObject mCaptureElement;
			protected internal Vector2Df mCapturePoint;
			protected internal Vector2Df mCapturePointOffset;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			//
			// ОСНОВНЫЕ ПАРАМЕТРЫ
			//
			/// <summary>
			/// Канва
			/// </summary>
			public ICadCanvasViewer CanvasViewer
			{
				get
				{
					return (mCanvasViewer);
				}
				set
				{
					mCanvasViewer = value;
				}
			}

			/// <summary>
			/// Режим перемещения элементов
			/// </summary>
			public Boolean EditModeMoving
			{
				get
				{
#if USE_WINDOWS
					return (System.Windows.Input.Keyboard.IsKeyDown((((LotusCadCanvasViewer)CanvasViewer).KeyMovingElement)));
#else
					return (false);
#endif
				}
			}

			/// <summary>
			/// Захваченный элемент
			/// </summary>
			public ICadObject CaptureElement
			{
				get
				{
					return (mCaptureElement);
				}
				set
				{
					mCaptureElement = value;
				}
			}

			/// <summary>
			/// Точка захвата элементов
			/// </summary>
			public Vector2Df CapturePoint
			{
				get
				{
					return (mCapturePoint);
				}
				set
				{
					mCapturePoint = value;
				}
			}

			/// <summary>
			/// Смещение точки захвата от опорной точки элемента
			/// </summary>
			public Vector2Df CapturePointOffset
			{
				get
				{
					return (mCapturePointOffset);
				}
				set
				{
					mCapturePointOffset = value;
				}
			}

			//
			// ОСНОВНЫЕ ПАРАМЕТРЫ
			//
			/// <summary>
			/// Выбранные графические элементы
			/// </summary>
			public ListArray<ICadObject> SelectedElements
			{
				get
				{
					return (mSelectedElements);
				}
			}

			/// <summary>
			/// Количество выбранных графических элементов
			/// </summary>
			public Int32 Count
			{
				get
				{
					return (mSelectedElements.Count);
				}
			}

			/// <summary>
			/// Первый выбранный графический элемент
			/// </summary>
			public ICadObject FirstElement
			{
				get
				{
					return (mSelectedElements[0]);
				}
			}

			/// <summary>
			/// Первая выбранная графическая фигура
			/// </summary>
			public ICadShape FirstShape
			{
				get
				{
					return (mSelectedElements[0] as ICadShape);
				}
			}

			/// <summary>
			/// Последний выбранный графический элемент
			/// </summary>
			public ICadObject LastElement
			{
				get
				{
					return (mSelectedElements[mSelectedElements.Count - 1]);
				}
			}

			/// <summary>
			/// Последняя выбранная графическая фигура
			/// </summary>
			public ICadShape LastShape
			{
				get
				{
					return (mSelectedElements[mSelectedElements.Count - 1] as ICadShape);
				}
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public CCadSelecting()
			{
				mSelectedElements = new ListArray<ICadObject>();
			}
			#endregion

			#region ======================================= МЕТОДЫ ПРОВЕРКИ ===========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка на попадание в область графического элемента указанной точки
			/// </summary>
			/// <param name="point">Проверяемая точка</param>
			/// <param name="add">Статус добавления объектов</param>
			/// <param name="epsilon">Точность проверки</param>
			/// <returns>Статус проверки</returns>
			//---------------------------------------------------------------------------------------------------------
			public virtual Boolean CheckPoint(ref Vector2Df point, Boolean add, Single epsilon)
			{
				if (CanvasViewer == null || CanvasViewer.Elements == null) return (false);

				// Список элеметов которые попали
				ListArray<ICadObject> checks = new ListArray<ICadObject>();

				for (Int32 i = 0; i < CanvasViewer.Elements.Count; i++)
				{
					ICadObject item = CanvasViewer.Elements[i];
					if (item != null)
					{
						if(item is ICadShape shape)
						{
							if(shape.IsEnabled == false)
							{
								continue;
							}

							if (shape.IsVisible == false)
							{
								continue;
							}
						}

						// Смотрим есть ли попадание по какой-либо графическому элементу
						if (item.CheckPoint(ref point, epsilon))
						{
							checks.Add(item);
						}
					}
				}

				// Если что-то получили
				if (checks.Count > 0)
				{
					// Если режим добавления
					if (add)
					{
						for (Int32 i = 0; i < checks.Count; i++)
						{
							ICadObject item = checks[i];

							Boolean status = this.Contains(item);
							if (!status)
							{
								// Добавляем в выбранное
								Add(item);
							}
							else
							{
								// Удаляем из выбранного
								Remove(item);
							}
						}
					}
					else
					{
						// Очищаем предыдущей выбор
						Clear();

						// Добавляем
						for (Int32 i = 0; i < checks.Count; i++)
						{
							ICadObject item = checks[i];
							Add(item);
						}

						// Берем управление
						StartCapturePosition(ref point);
					}
				}
				else
				{
					// Очищаем предыдущей выбор
					Clear();
				}


				return (mSelectedElements.Count > 0);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка на попадание ВСЕЙ ГЕОМЕТРИИ графического объекта внутрь прямоугольной области
			/// </summary>
			/// <param name="rect">Прямоугольник</param>
			/// <param name="add">Статус добавления объектов</param>
			/// <returns>Статус проверки</returns>
			//---------------------------------------------------------------------------------------------------------
			public virtual Boolean CheckInsideRect(ref Rect2Df rect, Boolean add)
			{
				if (CanvasViewer == null || CanvasViewer.Draft == null) return (false);

				// Список элеметов которые попали
				ListArray<ICadObject> checks = new ListArray<ICadObject>();
				for (Int32 i = 0; i < CanvasViewer.Draft.Elements.Count; i++)
				{
					ICadObject item = CanvasViewer.Draft.Elements[i];
					if (item != null)
					{
						// Смотрим есть ли попадание по какой-либо графическому элементу
						if (item.CheckInsideRect(ref rect))
						{
							checks.Add(item);
						}
					}
				}

				// Если что-то получили
				if (checks.Count > 0)
				{
					for (Int32 i = 0; i < checks.Count; i++)
					{
						ICadObject item = checks[i];

						// Режим добавления или удаления
						if (add)
						{
							Boolean status = this.Contains(item);
							if (!status)
							{
								// Добавляем в выбранное
								Add(item);
							}
							else
							{
								// Удаляем из выбранного
								Remove(item);
							}
						}
						else
						{
							// Добавляем в выбранное
							Add(item);
						}
					}
				}
				else
				{
					// Очищаем предыдущей выбор
					Clear();
				}

				return (mSelectedElements.Count > 0);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка на попадание ВСЕЙ ИЛИ ЧАСТИ ГЕОМЕТРИИ графического объекта внутрь прямоугольной области
			/// </summary>
			/// <param name="rect">Прямоугольник</param>
			/// <param name="add">Статус добавления объектов</param>
			/// <returns>Статус проверки</returns>
			//---------------------------------------------------------------------------------------------------------
			public virtual Boolean CheckInsideOrIntersectRect(ref Rect2Df rect, Boolean add)
			{
				if (CanvasViewer == null || CanvasViewer.Draft == null) return (false);

				// Список элеметов которые попали
				ListArray<ICadObject> checks = new ListArray<ICadObject>();
				for (Int32 i = 0; i < CanvasViewer.Draft.Elements.Count; i++)
				{
					ICadObject item = CanvasViewer.Draft.Elements[i];
					if (item != null)
					{
						// Смотрим есть ли попадание по какой-либо графическому элементу
						if (item.CheckInsideOrIntersectRect(ref rect))
						{
							checks.Add(item);
						}
					}
				}

				// Если что-то получили
				if (checks.Count > 0)
				{
					for (Int32 i = 0; i < checks.Count; i++)
					{
						ICadObject item = checks[i];

						// Режим добавления или удаления
						if (add)
						{
							Boolean status = this.Contains(item);
							if (!status)
							{
								// Добавляем в выбранное
								Add(item);
							}
							else
							{
								// Удаляем из выбранного
								Remove(item);
							}
						}
						else
						{
							// Добавляем в выбранное
							Add(item);
						}
					}
				}
				else
				{
					// Очищаем предыдущей выбор
					Clear();
				}

				return (mSelectedElements.Count > 0);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка на видимость всей геометрии выбранных графических элементов в области просмотра
			/// </summary>
			/// <returns>Статус видимости</returns>
			//---------------------------------------------------------------------------------------------------------
			public virtual Boolean CheckSelectedElementsVisibleInViewport()
			{
				Boolean status = true;
				for (Int32 i = 0; i < mSelectedElements.Count; i++)
				{
					ICadShape shape = mSelectedElements[i] as ICadShape;
					if (shape != null && shape.CheckVisibleInViewport() == false)
					{
						status = false;
						break;
					}
				}

				return (status);
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Рисование вспомогательных элементов выбранных графических элементов
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public virtual void Draw()
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Копирование выбранных графических элементов в буфер обмена
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public virtual void Copy()
			{
				//mCopyElements.Clear();
				//for (Int32 i = 0; i < mSelectedElements.Count; i++)
				//{
				//	mCopyElements.Add(mSelectedElements[i] as ICadShape);
				//}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вставка графических элементов из буфера обмена в представленный чертеж
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public virtual void Paste()
			{
				// Если есть документ
				//if (XManager.PresenterDocument != null)
				//{
				//	for (Int32 i = 0; i < mCopyElements.Count; i++)
				//	{
				//		//XManager.Presenter.PresenterDocument.AddExistingElement(mCopyElements[i].Duplicate());
				//	}
				//}
				//else
				//{
				//	for (Int32 i = 0; i < mCopyElements.Count; i++)
				//	{
				//		XManager.Presenter.Add(mCopyElements[i].Duplicate());
				//	}
				//}
			}
			#endregion

			#region ======================================= МЕТОДЫ РАБОТЫ C ЭЛЕМЕНТАМИ ================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка свойств в инспекторе свойств.
			/// </summary>
			/// <param name="inspector_properties">Инспектор свойств</param>
			//---------------------------------------------------------------------------------------------------------
			public void SetElementProperties(ILotusPropertyInspector inspector_properties)
			{
				if (SelectedElements.Count > 0)
				{
					if (SelectedElements.Count == 1)
					{
						inspector_properties.SelectedObject = FirstElement;
					}
					else
					{
						inspector_properties.SelectedObject = FirstElement;
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка нахождения графического элемента в списке выбранных
			/// </summary>
			/// <param name="element">Графический элемент</param>
			/// <returns>Статус нахождения</returns>
			//---------------------------------------------------------------------------------------------------------
			public Boolean Contains(ICadObject element)
			{
				return (mSelectedElements.Contains(element));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавление графического элемента в выбранные
			/// </summary>
			/// <param name="element">Графический элемент</param>
			//---------------------------------------------------------------------------------------------------------
			public void Add(ICadObject element)
			{
				// Если только графического элемента нет
				if (!mSelectedElements.Contains(element))
				{
					// Добавляем
					mSelectedElements.Add(element);

					// Для интерактивных фигур устанавливаем выбор
					if (element is ICadShape shape)
					{
						shape.IsSelect = true;
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Удаление графического элемента из выбранных
			/// </summary>
			/// <param name="element">Графический элемент</param>
			//---------------------------------------------------------------------------------------------------------
			public void Remove(ICadObject element)
			{
				// Удаляем
				mSelectedElements.Remove(element);

				// Для интерактивных фигур убираем выбор
				if (element is ICadShape shape)
				{
					shape.IsSelect = false;
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Очистка выбора графических элементов
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void Clear()
			{
				// Сбрасываем выбор
				for (Int32 i = 0; i < mSelectedElements.Count; i++)
				{
					// Для интерактивных фигур убираем выбор
					if (mSelectedElements[i] is ICadShape shape)
					{
						shape.IsSelect = false;
					}
				}

				mSelectedElements.Clear();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Удаляем выбранные графические элементы
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void Delete()
			{
				// 1) Сбрасываем выбор и представления на канве
				for (Int32 i = 0; i < mSelectedElements.Count; i++)
				{
					// Для интерактивных фигур убираем выбор
					if (mSelectedElements[i] is ICadShape shape)
					{
						shape.IsSelect = false;
					}
				}

				// 2) Копируем во временный список
				List<ICadObject> items_to_remove = new List<ICadObject>(mSelectedElements);

				// 2) Копируем в состояние
				if (mCanvasViewer != null && mCanvasViewer.Memento != null)
				{
					for (Int32 i = 0; i < mSelectedElements.Count; i++)
					{
						mCanvasViewer.Memento.AddStateToHistory(new CMementoCaretakerRemove(mCanvasViewer, mSelectedElements[i]));
					}
				}

				// 4) Удаляем из документа
				if (mCanvasViewer?.Draft != null)
				{
					for (Int32 i = 0; i < items_to_remove.Count; i++)
					{
						mCanvasViewer?.RemoveObject(items_to_remove[i]);
					}
				}

				// 5) Очищаем список
				mSelectedElements.Clear();

				// 6) Очищаем временный список
				items_to_remove.Clear();
			}
			#endregion

			#region ======================================= МЕТОДЫ УПРАВЛЕНИЯ =========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Начало захвата от управления курсора
			/// </summary>
			/// <param name="point">Точка в координатах канвы</param>
			//---------------------------------------------------------------------------------------------------------
			public void StartCapturePosition(ref Vector2Df point)
			{
				if (mSelectedElements.Count > 0)
				{
					for (Int32 i = 0; i < mSelectedElements.Count; i++)
					{
						if (mSelectedElements[i] is ICadControlElementPointer control_element)
						{
							control_element.StartCapturePosition(ref point);
						}
					}

					mCapturePoint = point;
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновления захвата управления от курсора
			/// </summary>
			/// <param name="point">Точка в координатах канвы</param>
			//---------------------------------------------------------------------------------------------------------
			public void UpdateCapturePosition(ref Vector2Df point)
			{
				if (mSelectedElements.Count > 0)
				{
					for (Int32 i = 0; i < mSelectedElements.Count; i++)
					{
						if (mSelectedElements[i] is ICadControlElementPointer control_element)
						{
							control_element.UpdateCapturePosition(ref point);
						}
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Окончание захвата от управления курсора
			/// </summary>
			/// <param name="point">Точка в координатах канвы</param>
			//---------------------------------------------------------------------------------------------------------
			public void EndCapturePosition(ref Vector2Df point)
			{
				if (mSelectedElements.Count > 0)
				{
					for (Int32 i = 0; i < mSelectedElements.Count; i++)
					{
						if (mSelectedElements[i] is ICadControlElementPointer control_element)
						{
							control_element.EndCapturePosition(ref point);
						}
					}

					mCapturePoint = Vector2Df.Zero;
				}
			}

#if USE_WINDOWS
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обработка события нажатия клавиши
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			public void OnKeyDown(System.Windows.Input.KeyEventArgs args)
			{
				if (args.Key == ((LotusCadCanvasViewer)CanvasViewer).KeyMoveElementLeft)
				{
					MoveLeft(CanvasViewer.ZoomInverse * 10);
					args.Handled = CheckSelectedElementsVisibleInViewport();
				}
				if (args.Key == ((LotusCadCanvasViewer)CanvasViewer).KeyMoveElementRight)
				{
					MoveRight(CanvasViewer.ZoomInverse * 10);
					args.Handled = CheckSelectedElementsVisibleInViewport();
				}

				if (args.Key == ((LotusCadCanvasViewer)CanvasViewer).KeyMoveElementDown)
				{
					MoveDown(CanvasViewer.ZoomInverse * 10);
					args.Handled = CheckSelectedElementsVisibleInViewport();
				}
				if (args.Key == ((LotusCadCanvasViewer)CanvasViewer).KeyMoveElementUp)
				{
					MoveUp(CanvasViewer.ZoomInverse * 10);
					args.Handled = CheckSelectedElementsVisibleInViewport();
				}

				if (args.Key == System.Windows.Input.Key.Delete)
				{
					Delete();
				}
			}
#endif
			#endregion

			#region ======================================= МЕТОДЫ ТРАНСФОРМАЦИИ ======================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перемещение графического элемента с помощью мыши
			/// </summary>
			/// <param name="offset">Значение смещения в условных единицах</param>
			//---------------------------------------------------------------------------------------------------------
			public void Move(ref Vector2Df offset)
			{
				for (Int32 i = 0; i < mSelectedElements.Count; i++)
				{
					ICadTransform transform = mSelectedElements[i] as ICadTransform;
					if (transform != null)
					{
						transform.Move(ref offset);
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перемещение выбранных графических элементов вверх
			/// </summary>
			/// <param name="offset">Значение смещения в условных единицах</param>
			//---------------------------------------------------------------------------------------------------------
			public void MoveUp(Single offset)
			{
				for (Int32 i = 0; i < mSelectedElements.Count; i++)
				{
					ICadTransform transform = mSelectedElements[i] as ICadTransform;
					if (transform != null)
					{
						transform.MoveUp(offset);
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перемещение выбранных графических элементов вниз
			/// </summary>
			/// <param name="offset">Значение смещения в условных единицах</param>
			//---------------------------------------------------------------------------------------------------------
			public void MoveDown(Single offset)
			{
				for (Int32 i = 0; i < mSelectedElements.Count; i++)
				{
					ICadTransform transform = mSelectedElements[i] as ICadTransform;
					if (transform != null)
					{
						transform.MoveDown(offset);
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перемещение выбранных графических элементов влево
			/// </summary>
			/// <param name="offset">Значение смещения в условных единицах</param>
			//---------------------------------------------------------------------------------------------------------
			public void MoveLeft(Single offset)
			{
				for (Int32 i = 0; i < mSelectedElements.Count; i++)
				{
					ICadTransform transform = mSelectedElements[i] as ICadTransform;
					if (transform != null)
					{
						transform.MoveLeft(offset);
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перемещение выбранных графических элементов вправо
			/// </summary>
			/// <param name="offset">Значение смещения в условных единицах</param>
			//---------------------------------------------------------------------------------------------------------
			public void MoveRight(Single offset)
			{
				for (Int32 i = 0; i < mSelectedElements.Count; i++)
				{
					ICadTransform transform = mSelectedElements[i] as ICadTransform;
					if (transform != null)
					{
						transform.MoveRight(offset);
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
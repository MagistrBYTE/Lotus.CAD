//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Элементы управления
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCadCanvasViewer.cs
*		Элемент для управления канвой.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 27.03.2022
//=====================================================================================================================
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
using Lotus.Maths;
using Lotus.Windows;
//=====================================================================================================================
namespace Lotus
{
	namespace CAD
	{
		//-------------------------------------------------------------------------------------------------------------
		//! \addtogroup CadControls
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Элемент для управления канвой
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class LotusCadCanvasViewer : LotusContentViewer, ICadCanvasViewer
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			// Основные параметры
			protected static PropertyChangedEventArgs PropertyArgsDraft = new PropertyChangedEventArgs(nameof(Draft));
			protected static PropertyChangedEventArgs PropertyArgsTool = new PropertyChangedEventArgs(nameof(Tool));

			// Адаптивный масштаб
			protected static PropertyChangedEventArgs PropertyArgsZoom = new PropertyChangedEventArgs(nameof(Zoom));
			protected static PropertyChangedEventArgs PropertyArgsZoomAdaptive = new PropertyChangedEventArgs(nameof(ZoomAdaptive));
			protected static PropertyChangedEventArgs PropertyArgsZoomInverse = new PropertyChangedEventArgs(nameof(ZoomInverse));
			protected static PropertyChangedEventArgs PropertyArgsZoomInverseAdaptive = new PropertyChangedEventArgs(nameof(ZoomInverseAdaptive));

			// Режим редактирования
			protected static PropertyChangedEventArgs PropertyArgsCreateModeIsAutoCAD = new PropertyChangedEventArgs(nameof(CreateModeIsAutoCAD));
			protected static PropertyChangedEventArgs PropertyArgsEditModeIsAutoCAD = new PropertyChangedEventArgs(nameof(EditModeIsAutoCAD));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			// Основные параметры
			protected internal CCadDraft mDraft;
			protected internal CCadTool mTool;
			protected internal CCadSelecting mSelecting;
			protected internal CMementoManager mMemento;
			protected internal LotusCadCanvas mCanvas;

			// Адаптивный масштаб
			protected internal Single[] mScaleAdaptives = { 0.1f, 0.2f, 0.5f, 1.0f, 2.0f, 5.0f, 10f };
			protected internal Single[] mScaleAdaptiveInverse = { 10f, 5f, 2f, 1.0f, 0.5f, 0.2f, 0.1f };

			// Привязка
			protected internal Boolean mSnapIsEnabled;
			protected internal List<Vector2Df> mSnapPoints;
			protected internal List<Rect2Df> mSnapRects;
			protected internal Int32 mSnapIndexPoint;

			// Режим редактирования
			protected internal Boolean mCreateModeIsAutoCAD;
			protected internal Boolean mEditModeIsAutoCAD;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			//
			// ОСНОВНЫЕ ПАРАМЕТРЫ
			//
			/// <summary>
			/// Канва для отображения объектов чертежной графики
			/// </summary>
			public LotusCadCanvas Canvas 
			{
				get { return (mCanvas); }
			}

			/// <summary>
			/// Список графических объектов отображаемых на канве
			/// </summary>
			public ListArray<ICadObject> Elements
			{
				get
				{
					return (Canvas.Elements);
				}
			}

			/// <summary>
			/// Текущий отображаемый чертеж
			/// </summary>
			public CCadDraft Draft
			{
				get { return (mDraft); }
				set
				{
					if (mDraft != value)
					{
						// Убираем с канвы элементы старого чертежа
						if (mDraft != null)
						{
							for (Int32 i = 0; i < mDraft.Elements.Count; i++)
							{
								var item = mDraft.Elements[i];
								item.IsCanvas = false;
								Canvas.Elements.Remove(item);
							}
						}

						mDraft = value;
						NotifyPropertyChanged(PropertyArgsDraft);

						if (mDraft != null)
						{
							for (Int32 i = 0; i < mDraft.Elements.Count; i++)
							{
								var item = mDraft.Elements[i];
								item.IsCanvas = true;
								item.CanvasViewer = this;
								Canvas.Elements.Add(item);
							}
						}
					}
				}
			}

			/// <summary>
			/// Текущий активный инструмент для создания и редактирования графических объектов
			/// </summary>
			public CCadTool Tool
			{
				get { return (mTool); }
				set 
				{ 
					mTool = value;
					NotifyPropertyChanged(PropertyArgsTool);
				}
			}

			/// <summary>
			/// Объект содержащий список выбранных объектов
			/// </summary>
			public CCadSelecting Selecting
			{
				get { return (mSelecting); }
			}

			/// <summary>
			/// Менеджер для отмены/повторения действия
			/// </summary>
			public ILotusMementoManager Memento
			{
				get { return (mMemento); }
			}

			//
			// ПОЗИЦИЯ И СМЕЩЕНИЕ КУРСОРА
			//
			/// <summary>
			/// Смещение курсора в координатах канвы
			/// </summary>
			public Vector2Df PointerDelta
			{
				get { return (MouseDeltaCurrent); }
			}

			/// <summary>
			/// Позиция курсора мыши в координатах канвы
			/// </summary>
			public Vector2Df PointerPosition
			{
				get { return (MousePositionCurrent); }
			}

			//
			// АДАПТИВНЫЙ МАСШТАБ
			//
			/// <summary>
			/// Текущий масштаб увеличения канвы
			/// </summary>
			public Single Zoom
			{
				get { return ((Single)ContentScale); }
			}

			/// <summary>
			/// Фиксированный адаптивный масштаб канвы
			/// </summary>
			public Single ZoomAdaptive
			{
				get { return (mScaleAdaptives.GetNearestValue(Zoom)); }
			}

			/// <summary>
			/// Обратный масштаб канвы
			/// </summary>
			public Single ZoomInverse
			{
				get { return (1.0f / Zoom); }
			}

			/// <summary>
			/// Фиксированный адаптивный обратный масштаб канвы
			/// </summary>
			public Single ZoomInverseAdaptive
			{
				get { return (mScaleAdaptiveInverse.GetNearestValue(ZoomInverse)); }
			}

			//
			// ПАРАМЕТРЫ ПРИВЯЗКИ
			//
			/// <summary>
			/// Режим включения привязки
			/// </summary>
			public Boolean SnapIsEnabled
			{
				get { return (mSnapIsEnabled); }
				set
				{
					if (mSnapIsEnabled != value)
					{
						mSnapIsEnabled = value;
						if (mSnapIsEnabled)
						{
							UpdateSnapNodes();
						}
					}
				}
			}

			/// <summary>
			/// Индекс текущей точки привязки
			/// </summary>
			public Int32 SnapIndexPoint
			{
				get { return (mSnapIndexPoint); }
			}

			/// <summary>
			/// Режим включения привязки и существующей точки привязки
			/// </summary>
			public Boolean SnapIsExsisting
			{
				get { return (mSnapIsEnabled && mSnapIndexPoint > -1); }
			}

			/// <summary>
			/// Текущая точки привязки
			/// </summary>
			public Vector2Df SnapPoint
			{
				get { return (mSnapPoints[mSnapIndexPoint]); }
			}

			/// <summary>
			/// Смещение курсора мыши в координатах канвы от текущей точки привязки
			/// </summary>
			public Vector2Df SnapMouseOffsetCanvas
			{
				get { return ((mSnapPoints[mSnapIndexPoint] - MousePositionCurrent)); }
			}

			/// <summary>
			/// Список прямоугольников для привязки
			/// </summary>
			public List<Rect2Df> SnapRects
			{
				get { return (mSnapRects); }
			}

			//
			// РЕЖИМ РЕДАКТИРОВАНИЯ
			//
			/// <summary>
			/// Создание графических элементов в режиме AutoCAD
			/// </summary>
			public Boolean CreateModeIsAutoCAD
			{
				get { return (mCreateModeIsAutoCAD); }
				set
				{
					mCreateModeIsAutoCAD = value;
					NotifyPropertyChanged(PropertyArgsCreateModeIsAutoCAD);
				}
			}

			/// <summary>
			/// Редактирование графических элементов в режиме AutoCAD
			/// </summary>
			public Boolean EditModeIsAutoCAD
			{
				get { return (mEditModeIsAutoCAD); }
				set
				{
					mEditModeIsAutoCAD = value;
					NotifyPropertyChanged(PropertyArgsEditModeIsAutoCAD);
				}
			}


			/// <summary>
			/// 
			/// </summary>
			public Key KeyZoomRegion { get; set; } = Key.Z;

			/// <summary>
			/// 
			/// </summary>
			public Key KeyZoomSmallChange { get; set; } = Key.LeftCtrl;

			/// <summary>
			/// 
			/// </summary>
			public Key KeyAddElement { get; set; } = Key.LeftShift;

			/// <summary>
			/// 
			/// </summary>
			public Key KeyMovingElement { get; set; } = Key.M;

			/// <summary>
			/// 
			/// </summary>
			public Key KeyMoveElementLeft { get; set; } = Key.A;

			/// <summary>
			/// 
			/// </summary>
			public Key KeyMoveElementRight { get; set; } = Key.D;

			/// <summary>
			/// 
			/// </summary>
			public Key KeyMoveElementUp { get; set; } = Key.W;

			/// <summary>
			/// 
			/// </summary>
			public Key KeyMoveElementDown { get; set; } = Key.W;
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public LotusCadCanvasViewer()
			{
				mSelecting = new CCadSelecting();
				mSelecting.CanvasViewer = this;
				mMemento = new CMementoManager();
				mSnapPoints = new List<Vector2Df>();
				mSnapRects = new List<Rect2Df>();
				mSnapIndexPoint = -1;
				this.Loaded += OnCanvasViewer_Loaded;
				this.Unloaded += OnCanvasViewer_Unloaded;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="content">Элемент контент</param>
			//---------------------------------------------------------------------------------------------------------
			public LotusCadCanvasViewer(FrameworkElement content)
				: base(content)
			{
				mSelecting = new CCadSelecting();
				mSelecting.CanvasViewer = this;
				mMemento = new CMementoManager();
				mSnapPoints = new List<Vector2Df>();
				mSnapRects = new List<Rect2Df>();
				mSnapIndexPoint = -1;
				this.Loaded += OnCanvasViewer_Loaded;
				this.Unloaded += OnCanvasViewer_Unloaded;
			}
			#endregion

			#region ======================================= СЛУЖЕБНЫЕ МЕТОДЫ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Загрузка канвы для отображения
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnCanvasViewer_Loaded(Object sender, RoutedEventArgs args)
			{
				mCanvas = mContent.GetValue(ContentPresenter.ContentProperty) as LotusCadCanvas;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Выгрузка канвы
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnCanvasViewer_Unloaded(Object sender, RoutedEventArgs e)
			{
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка текущего курсора
			/// </summary>
			/// <param name="cursor">Курсор</param>
			//---------------------------------------------------------------------------------------------------------
			public void SetCursor(TCursor cursor)
			{
				this.Cursor = XWindowsConverters.ConvertToCursor(cursor);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Анимация масштабирования указанной области канвы
			/// </summary>
			/// <param name="content_rect">Прямоугольник области канвы</param>
			//---------------------------------------------------------------------------------------------------------
			public void AnimatedZoomTo(Rect2Df content_rect)
			{
				AnimatedZoomTo(content_rect.ToWinRect());
			}
			#endregion

			#region ======================================= МЕТОДЫ РАБОТЫ С ЭЛЕМЕНТАМИ ================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавления графического объекта на канву
			/// </summary>
			/// <param name="cad_object">Графический объект</param>
			/// <returns>Статус успешности добавления</returns>
			//---------------------------------------------------------------------------------------------------------
			public Boolean AddObject(ICadObject cad_object)
			{
				cad_object.IsCanvas = true;
				Canvas.Elements.Add(cad_object);

				if (mDraft != null)
				{
					if (mDraft.Elements.Contains(cad_object) == false)
					{
						cad_object.IDraft = mDraft;
						mDraft.Elements.Add(cad_object);
					}
					return (true);
				}
				else
				{
					return (false);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Удаления графического объекта с канвы
			/// </summary>
			/// <param name="cad_object">Графический объект</param>
			/// <returns>Статус успешности уд</returns>
			//---------------------------------------------------------------------------------------------------------
			public Boolean RemoveObject(ICadObject cad_object)
			{
				cad_object.IsCanvas = false;
				Canvas.Elements.Remove(cad_object);
				if (mDraft != null)
				{
					Int32 index = mDraft.Elements.IndexOf(cad_object);
					if (index > -1)
					{
						cad_object.IDraft = null;
						mDraft.Elements.RemoveAt(index);
					}
					return (true);
				}
				else
				{
					return (false);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Сортировка графических объектов по глубине
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void SortByZIndex()
			{
				if (mDraft != null)
				{
					mDraft.Elements.Sort(SortingСomparison);
				}

				Canvas.SortByZIndex();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Функция сортировки
			/// </summary>
			/// <param name="a">Первый элемент</param>
			/// <param name="b">Второй элемент</param>
			/// <returns>Статус сравнения</returns>
			//---------------------------------------------------------------------------------------------------------
			private Int32 SortingСomparison(ICadObject a, ICadObject b)
			{
				ICadObject x = a as ICadObject;
				ICadObject y = b as ICadObject;

				if (x.ZIndex > y.ZIndex)
				{
					return (1);
				}
				else
				{
					if (x.ZIndex < y.ZIndex)
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

			#region ======================================= МЕТОДЫ РИСОВАНИЯ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка матрицы трансформации
			/// </summary>
			/// <param name="transform">Матрица трансформации</param>
			//---------------------------------------------------------------------------------------------------------
			public void SetTransform(CCadTransform transform)
			{
				Canvas.SetTransform(transform);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Восстановление матрицы трансформации
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void ResetTransform()
			{
				Canvas.ResetTransform();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка прозрачности рисования графических объектов
			/// </summary>
			/// <param name="opacity">Прозрачность</param>
			//---------------------------------------------------------------------------------------------------------
			public void SetOpacity(Single opacity)
			{
				Canvas.SetOpacity(opacity);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Восстановление прозрачности рисования графических объектов
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void ResetOpacity()
			{
				Canvas.ResetOpacity();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Рисование линии с параметрами отображения по умолчанию
			/// </summary>
			/// <param name="start_point">Начальная точка</param>
			/// <param name="end_point">Конечная точка</param>
			//---------------------------------------------------------------------------------------------------------
			public void DrawLine(ref Vector2Df start_point, ref Vector2Df end_point)
			{
				Canvas.DrawLine(ref start_point, ref end_point);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Рисование линии
			/// </summary>
			/// <param name="start_point">Начальная точка</param>
			/// <param name="end_point">Конечная точка</param>
			/// <param name="stroke">Перо для отображения</param>
			//---------------------------------------------------------------------------------------------------------
			public void DrawLine(ref Vector2Df start_point, ref Vector2Df end_point, CCadPen stroke)
			{
				Canvas.DrawLine(ref start_point, ref end_point, stroke);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Рисование линии
			/// </summary>
			/// <param name="start_point">Начальная точка</param>
			/// <param name="end_point">Конечная точка</param>
			/// <param name="brush">Кисть для отрисовки</param>
			/// <param name="thickness">Толщина линии</param>
			//---------------------------------------------------------------------------------------------------------
			public void DrawLine(ref Vector2Df start_point, ref Vector2Df end_point, CCadBrush brush, Single thickness = 1.0f)
			{
				Canvas.DrawLine(ref start_point, ref end_point, brush, thickness);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Рисование прямоугольника с параметрами отображения по умолчанию
			/// </summary>
			/// <param name="rect">Прямоугольник</param>
			//---------------------------------------------------------------------------------------------------------
			public void DrawRectangle(ref Rect2Df rect)
			{
				Canvas.DrawRectangle(ref rect);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Рисование прямоугольника
			/// </summary>
			/// <param name="rect">Прямоугольник</param>
			/// <param name="stroke">Перо для отображения</param>
			//---------------------------------------------------------------------------------------------------------
			public void DrawRectangle(ref Rect2Df rect, CCadPen stroke)
			{
				Canvas.DrawRectangle(ref rect, stroke);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Рисование прямоугольника
			/// </summary>
			/// <param name="rect">Прямоугольник</param>
			/// <param name="brush">Кисть для отрисовки</param>
			/// <param name="thickness">Толщина линии</param>
			//---------------------------------------------------------------------------------------------------------
			public void DrawRectangle(ref Rect2Df rect, CCadBrush brush, Single thickness = 1.0f)
			{
				Canvas.DrawRectangle(ref rect, brush, thickness);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Рисование эллипса с параметрами отображения по умолчанию
			/// </summary>
			/// <param name="rect">Прямоугольник</param>
			//---------------------------------------------------------------------------------------------------------
			public void DrawEllipse(ref Rect2Df rect)
			{
				Canvas.DrawEllipse(ref rect);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Рисование эллипса
			/// </summary>
			/// <param name="rect">Прямоугольник</param>
			/// <param name="stroke">Перо для отображения</param>
			//---------------------------------------------------------------------------------------------------------
			public void DrawEllipse(ref Rect2Df rect, CCadPen stroke)
			{
				Canvas.DrawEllipse(ref rect, stroke);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Рисование эллипса
			/// </summary>
			/// <param name="rect">Прямоугольник</param>
			/// <param name="brush">Кисть для отрисовки</param>
			/// <param name="thickness">Толщина линии</param>
			//---------------------------------------------------------------------------------------------------------
			public void DrawEllipse(ref Rect2Df rect, CCadBrush brush, Single thickness = 1.0f)
			{
				//Canvas.DrawEllipse(ref rect, stroke);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Рисование прямоугольника ручки указанной точки
			/// </summary>
			/// <param name="point">Точка</param>
			/// <param name="is_selected">Статус выбора фигуры</param>
			//---------------------------------------------------------------------------------------------------------
			public void DrawHandleRect(Vector2Df point, Boolean is_selected)
			{
				Canvas.DrawHandleRect(point, is_selected);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Рисование прямоугольника ручки указанной точки
			/// </summary>
			/// <param name="point">Точка</param>
			/// <param name="is_selected">Статус выбора фигуры</param>
			//---------------------------------------------------------------------------------------------------------
			public void DrawHandleRect(ref Vector2Df point, Boolean is_selected)
			{
				Canvas.DrawHandleRect(ref point, is_selected);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Рисование прямоугольника ручки
			/// </summary>
			/// <param name="rect">Прямоугольник ручки</param>
			/// <param name="is_selected">Статус выбора фигуры</param>
			//---------------------------------------------------------------------------------------------------------
			public void DrawHandleRect(Rect2Df rect, Boolean is_selected)
			{
				Canvas.DrawHandleRect(ref rect, is_selected);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Рисование прямоугольника ручки
			/// </summary>
			/// <param name="rect">Прямоугольник ручки</param>
			/// <param name="is_selected">Статус выбора фигуры</param>
			//---------------------------------------------------------------------------------------------------------
			public void DrawHandleRect(ref Rect2Df rect, Boolean is_selected)
			{
				Canvas.DrawHandleRect(ref rect, is_selected);
			}
			#endregion

			#region ======================================= МЕТОДЫ ПРОВЕРКИ ПОПАДАНИЙ =================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка прямоугольника на видимость в пределах области просмотра
			/// </summary>
			/// <param name="rect">Проверяемый прямоугольник</param>
			/// <returns>Статус видимости</returns>
			//---------------------------------------------------------------------------------------------------------
			public Boolean CheckRectVisibleInViewport(ref Rect2Df rect)
			{
				return (false);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка прямоугольника на видимость в пределах области просмотра
			/// </summary>
			/// <param name="rect">Проверяемый прямоугольник</param>
			/// <returns>Статус видимости</returns>
			//---------------------------------------------------------------------------------------------------------
			public Boolean CheckRectVisibleInViewport(Rect2Df rect)
			{
				return (false);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка точки на видимость в пределах области просмотра
			/// </summary>
			/// <param name="point">Проверяемая точка</param>
			/// <returns>Статус видимости</returns>
			//---------------------------------------------------------------------------------------------------------
			public Boolean CheckPointVisibleInViewport(ref Vector2Df point)
			{
				return (false);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка точки на видимость в пределах области просмотра
			/// </summary>
			/// <param name="point">Проверяемая точка</param>
			/// <returns>Статус видимости</returns>
			//---------------------------------------------------------------------------------------------------------
			public Boolean CheckPointVisibleInViewport(Vector2Df point)
			{
				return (false);
			}
			#endregion

			#region ======================================= МЕТОДЫ РАБОТЫ С КОНТЕКСТНЫМ МЕНЮ ==========================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавление в контекстное меню команды
			/// </summary>
			/// <param name="command_name">Имя команды</param>
			//---------------------------------------------------------------------------------------------------------
			public void AddCommandContextMenu(String command_name)
			{
				//MenuItem menu_item = new MenuItem();
				//menu_item.Header = command_name;
				//contextMenu.Items.Add(menu_item);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Удаление из контекстное меню команды
			/// </summary>
			/// <param name="command_name">Имя команды</param>
			//---------------------------------------------------------------------------------------------------------
			public void RemoveCommandContextMenu(String command_name)
			{
				//for (Int32 i = 0; i < contextMenu.Items.Count; i++)
				//{
				//	MenuItem menu_item = contextMenu.Items[i] as MenuItem;
				//	if (menu_item.Header.ToString() == command_name)
				//	{
				//		contextMenu.Items.RemoveAt(i);
				//		break;
				//	}
				//}
			}
			#endregion

			#region ======================================= МЕТОДЫ РАБОТЫ С РУЧКАМИ ===================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение прямоугольника ручки указанной точки
			/// </summary>
			/// <param name="point">Точка</param>
			/// <returns>Прямоугольник</returns>
			//---------------------------------------------------------------------------------------------------------
			public Rect2Df GetHandleRect(Vector2Df point)
			{
				Single size = 10 * ZoomInverse;
				return (new Rect2Df(point.X - size, point.Y - size, size * 2, size * 2));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение прямоугольника ручки указанной точки
			/// </summary>
			/// <param name="point">Точка</param>
			/// <returns>Прямоугольник</returns>
			//---------------------------------------------------------------------------------------------------------
			public Rect2Df GetHandleRect(ref Vector2Df point)
			{
				Single size = 10 * ZoomInverse;
				return (new Rect2Df(point.X - size, point.Y - size, size * 2, size * 2));
			}
			#endregion

			#region ======================================= МЕТОДЫ РАБОТЫ С ПРИВЯЗКОЙ =================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновить доступные для привязки узлы
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			private void UpdateSnapNodes()
			{
				if (Canvas != null)
				{
					mSnapPoints.Clear();

					// Добавляем опорные точки
					for (Int32 i = 0; i < Canvas.Elements.Count; i++)
					{
						if (Canvas.Elements[i] is ICadShape shape)
						{
							var snaps = shape.GetSnapNodes();
							if (snaps != null)
							{
								mSnapPoints.AddRange(snaps);
							}
						}
					}

					// Добавляем опорные прямоугольники
					mSnapRects.Clear();
					Single offset = 10 * ZoomInverse;
					Single size = 20 * ZoomInverse;
					for (Int32 i = 0; i < mSnapPoints.Count; i++)
					{
						mSnapRects.Add(new Rect2Df(mSnapPoints[i].X - offset, mSnapPoints[i].Y - offset, size, size));
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка на попадание курсора в область привязки узла
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			private void CheckSnapNodes()
			{
				mSnapIndexPoint = -1;
				for (Int32 i = 0; i < mSnapRects.Count; i++)
				{
					if (mSnapRects[i].Contains(ref MousePositionCurrent))
					{
						mSnapIndexPoint = i;
						break;
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Режим предварительного выбора элемента
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			private void PreSelectMode()
			{
				if (Canvas != null)
				{
					for (Int32 i = 0; i < Canvas.Elements.Count; i++)
					{
						if (Canvas.Elements[i] is CCadShape shape)
						{
							shape.IsPreSelect = false;
							if (shape.CheckPoint(ref MousePositionCurrent, ZoomInverse * 2))
							{
								shape.IsPreSelect = true;
							}
						}
					}
				}
			}
			#endregion

			#region ======================================= МЕТОДЫ РАБОТЫ С ВЫДЕЛЕНИЕМ РЕГИОНА ========================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Первичная инициализация данных для работы с выделением региона
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected override void InitSelectingRegion()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Начало операции выделения региона
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected override void StartSelectingRegion()
			{
				if (mSelectingIsSupport)
				{
					mSelectingStarting = true;
					mSelectingStartPoint = new Point(MousePositionLeftDown.X, MousePositionLeftDown.Y);
					mSelectingRectCorrect.X = MousePositionLeftDown.X - mSelectingDragCorrect / 2;
					mSelectingRectCorrect.Y = MousePositionLeftDown.Y - mSelectingDragCorrect / 2;
					mSelectingRectCorrect.Width = mSelectingDragCorrect;
					mSelectingRectCorrect.Height = mSelectingDragCorrect;
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Операция выделения региона (вызывается в MouseMove)
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected override void ProcessSelectingRegion()
			{
				if (mSelectingIsSupport)
				{
					// Если есть выход за пределы корректировочного прямоугольника
					if (!mSelectingRectCorrect.Contains(MousePositionCurrent))
					{
						if (mOperationCurrent != TViewHandling.SelectingRegion)
						{
							mOperationCurrent = TViewHandling.SelectingRegion;
							mOperationDesc = "ВЫДЕЛЕНИЕ РЕГИОНА" + mSelectingStartPoint.ToString();
							NotifyPropertyChanged(PropertyArgsOperationDesc);
						}

						if (mSelectingStartPoint.X < MousePositionCurrent.X)
						{
							mSelectingLeftUpPoint.X = mSelectingStartPoint.X;
							mSelectingRightToLeft = false;
						}
						else
						{
							mSelectingLeftUpPoint.X = MousePositionCurrent.X;
							mSelectingRightToLeft = true;
						}

						if (mSelectingStartPoint.Y < MousePositionCurrent.Y)
						{
							mSelectingLeftUpPoint.Y = mSelectingStartPoint.Y;
						}
						else
						{
							mSelectingLeftUpPoint.Y = MousePositionCurrent.Y;
						}

						mSelectingRect.X = (Single)mSelectingLeftUpPoint.X;
						mSelectingRect.Y = (Single)mSelectingLeftUpPoint.Y;
						mSelectingRect.Width = (Single)Math.Abs(mSelectingStartPoint.X - MousePositionCurrent.X);
						mSelectingRect.Height = (Single)Math.Abs(mSelectingStartPoint.Y - MousePositionCurrent.Y);

						// 2) Проверка попадания
						Boolean add = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
						if (mSelectingRightToLeft)
						{
							Rect2Df rect = mSelectingRect.ToRect();
							Selecting.CheckInsideOrIntersectRect(ref rect, add);
						}
						else
						{
							Rect2Df rect = mSelectingRect.ToRect();
							Selecting.CheckInsideRect(ref rect, add);
						}
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Окончание операции выделения региона
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected override void EndSelectingRegion()
			{
				mSelectingStarting = false;
				mOperationCurrent = TViewHandling.None;
				mOperationDesc = "";
				NotifyPropertyChanged(PropertyArgsOperationDesc);
			}
			#endregion

			#region ======================================= ОБРАБОТЧИКИ СОБЫТИЙ - КОНТЕНТ =============================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Смещение окна просмотра
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected override void OnContentViewerContentOffsetChanged()
			{
				base.OnContentViewerContentOffsetChanged();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение размеров окна просмотра
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected override void OnContentViewerContentSizeChanged()
			{
				base.OnContentViewerContentOffsetChanged();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение масштаба контента
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected override void OnContentViewerContentScaleChanged()
			{
				for (Int32 i = 0; i < Canvas.Elements.Count; i++)
				{
					if (Canvas.Elements[i] is CCadShape shape)
					{
						shape.OnScaleChanged(ContentScale);
					}
				}
			}
			#endregion

			#region ======================================= ОБРАБОТЧИКИ СОБЫТИЙ - ДЕЙСТВИЯ ============================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Элемент загружен
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			protected override void OnContentViewerLoaded(Object sender, RoutedEventArgs args)
			{
				if (mContent == null)
				{
					mContent = Content as FrameworkElement;
				}

				//if (mIsUnit == false)
				//{
				//	if (this.ContextMenu != null)
				//	{
				//		this.ContextMenu.Opened += OnContextMenuOpenedCanvas;
				//		this.ContextMenu.Closed += OnContextMenuClosedCanvas;
				//	}

				//	mIsUnit = true;
				//}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Нажатия кнопки мыши
			/// </summary>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			protected override void OnMouseDown(MouseButtonEventArgs args)
			{
				Keyboard.Focus(this);

				// 1) Получаем позиции курсора в координатах канвы
				MousePositionCurrent = mContentTotalTransform.Inverse.Transform(args.GetPosition(this)).ToVector2Df();

				// Если указатель
				if (mTool == null)
				{
					// 2) Сохраняем текущую операцию
					mOperationPreview = mOperationCurrent;

					// 3) Нажата левая кнопка мыши
					if (args.ChangedButton == MouseButton.Left)
					{
						MousePositionLeftDown = MousePositionCurrent;

						if (Keyboard.IsKeyDown(Key.Z))
						{
							// Увеличиваем регион
							StartZoomingRegion();
						}
						else
						{
							// Попытка выбора графического элемента
							if (Selecting.CheckPoint(ref MousePositionCurrent,
								Keyboard.Modifiers.HasFlag(ModifierKeys.Control), ZoomInverse * 2))
							{
								mOperationCurrent = TViewHandling.SelectedShape;
								
								Selecting.SetElementProperties(XWindowManager.PropertyInspector);

								if (mSnapIsEnabled)
								{
									UpdateSnapNodes();
								}
							}
							else
							{
								// Начало выделения региона
								StartSelectingRegion();
							}
						}
					}

					// Правая кнопка мыши - Открывание контекстного меню
					if (args.ChangedButton == MouseButton.Right)
					{
						if (ContextMenu != null)
						{
							ContextMenu.IsOpen = true;
						}
						MousePositionRightDown = MousePositionCurrent;
					}

					// Перемещение
					if (args.ChangedButton == MouseButton.Middle)
					{
						MousePositionMiddleDown = MousePositionCurrent;
						StartPanning();
					}

					// Захватываем мышь
					if (mOperationCurrent != TViewHandling.None)
					{
						CaptureMouse();
						args.Handled = true;
					}
				}
				else
				{
					mOperationCurrent = TViewHandling.UserOperation;
					mOperationDesc = mTool.ActionName;
					NotifyPropertyChanged(PropertyArgsOperationDesc);
					mTool.OnMouseDown(ref MousePositionCurrent, TMouseButton.Left);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перемещение курсора мыши
			/// </summary>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			protected override void OnMouseMove(MouseEventArgs args)
			{
				// Получаем текущие координаты
				Vector2Df current_content = mContentTotalTransform.Inverse.Transform(args.GetPosition(this)).ToVector2Df();

				// Смотрим смещение
				MouseDeltaCurrent = current_content - MousePositionCurrent;

				// Обновляем координаты
				MousePositionCurrent = current_content;

				if (mTool == null)
				{
					// Если зажата левая кнопка мыши
					if (args.LeftButton == MouseButtonState.Pressed)
					{
						if (mOperationCurrent == TViewHandling.SelectedShape)
						{
							Selecting.UpdateCapturePosition(ref MousePositionCurrent);
						}
						else
						{
							// Увеличение региона
							if (mZoomingStarting)
							{
								ProcessZoomingRegion();
							}
							else
							{
								if (mSelectingStarting)
								{
									// Выделение региона
									ProcessSelectingRegion();
								}
							}
						}
					}
					else
					{
						if (args.MiddleButton == MouseButtonState.Pressed)
						{
							// Перемещение
							if (mOperationCurrent == TViewHandling.Panning)
							{
								ProcessPanning();
							}

							args.Handled = true;
						}
						else
						{
							// Режим предварителього выбора
							PreSelectMode();
						}
					}
				}
				else
				{
					if (mSnapIsEnabled)
					{
						UpdateSnapNodes();
					}

					mTool.OnMouseMove(ref current_content, TMouseButton.Left);
				}

				if (mSnapIsEnabled)
				{
					CheckSnapNodes();
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Отпускание кнопки мыши
			/// </summary>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			protected override void OnMouseUp(MouseButtonEventArgs args)
			{
				Keyboard.Focus(this);

				// Получаем текущие координаты
				Vector2Df current_content = mContentTotalTransform.Inverse.Transform(args.GetPosition(this)).ToVector2Df();

				if (mTool == null)
				{
					if (args.ChangedButton == MouseButton.Left)
					{
						if (mOperationCurrent == TViewHandling.SelectedShape)
						{
							Selecting.EndCapturePosition(ref current_content);
							mOperationCurrent = TViewHandling.None;
						}
						else
						{
							if (mOperationCurrent == TViewHandling.ZoomingRegion)
							{
								EndZoomingRegion();
							}
							else
							{
								if (mOperationCurrent == TViewHandling.SelectingRegion)
								{
									EndSelectingRegion();
								}
							}
						}
					}
					else
					{
						if (args.ChangedButton == MouseButton.Middle)
						{
							EndPanning();
						}
						else
						{

						}
					}

					ReleaseMouseCapture();
					args.Handled = true;
				}
				else
				{
					mOperationCurrent = TViewHandling.None;
					mTool.OnMouseUp(ref current_content, TMouseButton.Left);

					if (mSnapIsEnabled)
					{
						UpdateSnapNodes();
						mSnapIndexPoint = -1;
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вращение колеса мыши
			/// </summary>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			protected override void OnMouseWheel(MouseWheelEventArgs args)
			{
				base.OnMouseWheel(args);

				args.Handled = true;

				if (args.Delta > 0)
				{
					Point curContentMousePoint = args.GetPosition(mContent);
					if (Keyboard.IsKeyDown(KeyZoomSmallChange))
					{
						this.ZoomAboutPoint(this.ContentScale + 0.1, curContentMousePoint);
					}
					else
					{
						this.ZoomAboutPoint(this.ContentScale + 0.01, curContentMousePoint);
					}
				}
				else if (args.Delta < 0)
				{
					Point curContentMousePoint = args.GetPosition(mContent);
					if (Keyboard.IsKeyDown(KeyZoomSmallChange))
					{
						this.ZoomAboutPoint(this.ContentScale - 0.1, curContentMousePoint);
					}
					else
					{
						this.ZoomAboutPoint(this.ContentScale - 0.01, curContentMousePoint);
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Нажатия клавиши клавиатуры
			/// </summary>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			protected override void OnPreviewKeyDown(KeyEventArgs args)
			{
				base.OnPreviewKeyDown(args);
				if (Selecting.SelectedElements.Count > 0)
				{
					Selecting.OnKeyDown(args);
				}
			}
			#endregion

			#region ======================================= ОБРАБОТЧИКИ СОБЫТИЙ - ДЕЙСТВИЯ ============================


			////---------------------------------------------------------------------------------------------------------
			///// <summary>
			///// Нажатия кнопки мыши
			///// </summary>
			///// <param name="args">Аргументы события</param>
			////---------------------------------------------------------------------------------------------------------
			//protected override void OnMouseDown(MouseButtonEventArgs args)
			//{
			//	base.OnMouseDown(args);

			//	Boolean status = mContent.Focus();
			//	Keyboard.Focus(mContent);

			//	// 1) Получаем позиции курсора в координатах канвы
			//	// Получаем текущие координаты
			//	//Vector2Df current_content = 
			//	MousePositionCurrent = mContentTotalTransform.Inverse.Transform(args.GetPosition(this)).ToVector2Df();

			//	// Если инструмент не активирован
			//	if (mTool == null)
			//	{
			//		// 2) Сохраняем текущую операцию
			//		mOperationPreview = mOperationCurrent;

			//		// 3) Нажата левая кнопка мыши
			//		if (args.ChangedButton == MouseButton.Left)
			//		{
			//			MousePositionLeftDown = MousePositionCurrent;

			//			if (Keyboard.IsKeyDown(Key.Z))
			//			{
			//				// Увеличиваем регион
			//				StartZoomingRegion();
			//			}
			//			else
			//			{
			//				// Если предыдущая операция была пользовательской и еще не окончания
			//				if (mOperationPreview == TViewHandling.UserOperation)
			//				{
			//					if (mEditModeIsAutoCAD)
			//					{
			//						Selecting.EndCapturePosition(ref MousePositionCurrent);
			//						mOperationCurrent = TViewHandling.None;
			//					}
			//					else
			//					{
			//						ContinueUserOperation(ref MousePositionCurrent);
			//					}
			//				}
			//				else
			//				{
			//					// Статус пользовательской операции
			//					Boolean is_user = CheckUserOperation(ref MousePositionCurrent);
			//					if (is_user)
			//					{
			//						mOperationCurrent = TViewHandling.UserOperation;
			//						mOperationDesc = "ВЫБОР ЭЛЕМЕНТА";
			//						NotifyPropertyChanged(PropertyArgsOperationDesc);

			//						if (mSnapIsEnabled)
			//						{
			//							UpdateSnapNodes();
			//						}
			//					}
			//					else
			//					{
			//						// Начало выделения региона
			//						StartSelectingRegion();
			//					}
			//				}
			//			}
			//		}

			//		// Правая кнопка мыши - Открывание контекстного меню
			//		if (args.ChangedButton == MouseButton.Right)
			//		{
			//			if (ContextMenu != null)
			//			{
			//				ContextMenu.IsOpen = true;
			//			}
			//			MousePositionRightDown = MousePositionCurrent;
			//		}

			//		// Перемещение
			//		if (args.ChangedButton == MouseButton.Middle)
			//		{
			//			MousePositionMiddleDown = MousePositionCurrent;
			//			StartPanning();
			//		}

			//		// Захватываем мышь
			//		if (mOperationCurrent != TViewHandling.None)
			//		{
			//			CaptureMouse();
			//			args.Handled = true;
			//		}
			//	}
			//	else
			//	{
			//		if (mSnapIsEnabled)
			//		{
			//			UpdateSnapNodes();
			//		}

			//		mTool.OnMouseDown(ref MousePositionCurrent, (TMouseButton)args.ChangedButton);
			//	}
			//}

			////---------------------------------------------------------------------------------------------------------
			///// <summary>
			///// Перемещение курсора мыши
			///// </summary>
			///// <param name="args">Аргументы события</param>
			////---------------------------------------------------------------------------------------------------------
			//protected override void OnMouseMove(MouseEventArgs args)
			//{
			//	base.OnMouseMove(args);

			//	// Получаем текущие координаты
			//	Vector2Df current_content = mContentTotalTransform.Inverse.Transform(args.GetPosition(this)).ToVector2Df();

			//	// Смотрим смещение
			//	MouseDeltaCurrent = current_content - MousePositionCurrent;

			//	// Обновляем координаты
			//	MousePositionCurrent = current_content;

			//	if (mTool == null)
			//	{
			//		// Если в режиме AutoCAD и пользовательская операция
			//		if (mEditModeIsAutoCAD && mOperationCurrent == TViewHandling.UserOperation)
			//		{
			//			ProcessUserOperation(ref MousePositionCurrent);

			//			if (mSnapIsEnabled)
			//			{
			//				CheckSnapNodes();
			//			}
			//		}
			//		else
			//		{
			//			// Если зажата левая кнопка мыши
			//			if (args.LeftButton == MouseButtonState.Pressed)
			//			{
			//				// Увеличение региона
			//				if (mZoomingStarting)
			//				{
			//					ProcessZoomingRegion();
			//				}
			//				else
			//				{
			//					if (mEditModeIsAutoCAD == false && mOperationCurrent == TViewHandling.UserOperation)
			//					{
			//						ProcessUserOperation(ref MousePositionCurrent);

			//						//System.Diagnostics.Debug.Print("ОБНОВЛЕНИЕ ЭЛЕМЕНТА");

			//						if (mSnapIsEnabled)
			//						{
			//							CheckSnapNodes();
			//						}
			//					}
			//					else
			//					{
			//						if (mSelectingStarting)
			//						{
			//							// Выделение региона
			//							ProcessSelectingRegion();
			//						}
			//					}
			//				}
			//			}
			//			else
			//			{
			//				if (args.MiddleButton == MouseButtonState.Pressed)
			//				{
			//					// Перемещение
			//					if (mOperationCurrent == TViewHandling.Panning)
			//					{
			//						ProcessPanning();
			//					}

			//					args.Handled = true;
			//				}
			//				else
			//				{
			//					// Режим предварителього выбора
			//					//PreSelectMode();
			//				}
			//			}
			//		}
			//	}
			//	else
			//	{
			//		if (mSnapIsEnabled)
			//		{
			//			CheckSnapNodes();
			//		}

			//		mTool.OnMouseMove(ref current_content, TMouseButton.Left);
			//	}
			//}

			////---------------------------------------------------------------------------------------------------------
			///// <summary>
			///// Отпускание кнопки мыши
			///// </summary>
			///// <param name="args">Аргументы события</param>
			////---------------------------------------------------------------------------------------------------------
			//protected override void OnMouseUp(MouseButtonEventArgs args)
			//{
			//	base.OnMouseUp(args);

			//	if (mTool == null)
			//	{
			//		if (args.ChangedButton == MouseButton.Left)
			//		{
			//			if (mOperationCurrent == TViewHandling.UserOperation)
			//			{
			//				if (mEditModeIsAutoCAD == false)
			//				{
			//					if (EndUserOperation(ref MousePositionCurrent))
			//					{
			//						mOperationCurrent = TViewHandling.None;
			//						mOperationDesc = "";
			//						NotifyPropertyChanged(PropertyArgsOperationDesc);
			//					}
			//				}
			//			}
			//			else
			//			{
			//				if (mOperationCurrent == TViewHandling.ZoomingRegion)
			//				{
			//					EndZoomingRegion();
			//				}
			//				else
			//				{
			//					if (mOperationCurrent == TViewHandling.SelectingRegion)
			//					{
			//						EndSelectingRegion();
			//					}
			//				}
			//			}
			//		}
			//		else
			//		{
			//			if (args.ChangedButton == MouseButton.Middle)
			//			{
			//				EndPanning();
			//			}
			//			else
			//			{

			//			}
			//		}

			//		ReleaseMouseCapture();
			//		args.Handled = true;
			//	}
			//	else
			//	{
			//		mTool.OnMouseUp(ref MousePositionCurrent, (TMouseButton)args.ChangedButton);
			//	}

			//	if (mSnapIsEnabled)
			//	{
			//		UpdateSnapNodes();
			//		mSnapIndexPoint = -1;
			//	}
			//}

			////---------------------------------------------------------------------------------------------------------
			///// <summary>
			///// Вращение колеса мыши
			///// </summary>
			///// <param name="args">Аргументы события</param>
			////---------------------------------------------------------------------------------------------------------
			//protected override void OnMouseWheel(MouseWheelEventArgs args)
			//{
			//	//base.OnMouseWheel(args);

			//	args.Handled = true;

			//	if (args.Delta > 0)
			//	{
			//		Point curContentMousePoint = args.GetPosition(mContent);
			//		this.ZoomAboutPoint(this.ContentScale + 0.1, curContentMousePoint);
			//	}
			//	else if (args.Delta < 0)
			//	{
			//		Point curContentMousePoint = args.GetPosition(mContent);
			//		this.ZoomAboutPoint(this.ContentScale - 0.1, curContentMousePoint);
			//	}
			//}
			#endregion

			#region ======================================= ОБРАБОТЧИКИ СОБЫТИЙ - КОНТЕКСТНОЕ МЕНЮ ====================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Открытие меню
			/// </summary>
			/// <remarks>
			/// На данном этапе происходит присоединение контекстное меню элемента к общему меню
			/// </remarks>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			protected void OnContextMenuOpenedCanvas(Object sender, RoutedEventArgs args)
			{
				//// Присоединяем контекстное меню элемента к общему меню
				//for (Int32 i = 0; i < XManager.Presenter.Elements.Count; i++)
				//{
				//	ICadShape shape = XManager.Presenter.Elements[i] as ICadShape;
				//	if (shape != null)
				//	{
				//		shape.OnOpenContextMenu(ref MousePositionCurrent);
				//	}
				//}

				//// Связываем обработчик событий
				//for (Int32 i = 0; i < ContextMenu.Items.Count; i++)
				//{
				//	MenuItem mi = ContextMenu.Items[i] as MenuItem;
				//	if (mi != null)
				//	{
				//		mi.Click += OnContextMenuCommandCanvas;
				//	}
				//}

				//args.Handled = true;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Исполнение комманды
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			protected void OnContextMenuCommandCanvas(Object sender, RoutedEventArgs args)
			{
				//MenuItem mi = sender as MenuItem;
				//if (mi != null)
				//{
				//	String command_name = mi.Header.ToString();
				//	Object context = mi.Tag;

				//	// Исполнение только для выбранных элементов
				//	for (Int32 i = 0; i < XManager.Editor.SelectedElements.Count; i++)
				//	{
				//		ICadShape shape = XManager.Editor.SelectedElements[i] as ICadShape;
				//		if (shape != null)
				//		{
				//			shape.OnCommandContextMenu(command_name, context);
				//		}
				//	}
				//}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Закрытие меню
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			protected void OnContextMenuClosedCanvas(Object sender, RoutedEventArgs args)
			{
				//// Отсоединяем обработчик событий
				//for (Int32 i = 0; i < ContextMenu.Items.Count; i++)
				//{
				//	MenuItem mi = ContextMenu.Items[i] as MenuItem;
				//	if (mi != null)
				//	{
				//		mi.Click -= OnContextMenuCommandCanvas;
				//	}
				//}

				//// Отсоединяем контекстное меню элемента от общего меню
				//for (Int32 i = 0; i < XManager.Presenter.Elements.Count; i++)
				//{
				//	ICadShape shape = XManager.Presenter.Elements[i] as ICadShape;
				//	if (shape != null)
				//	{
				//		shape.OnClosedContextMenu(ref MousePositionCurrent);
				//	}
				//}

				//args.Handled = true;
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
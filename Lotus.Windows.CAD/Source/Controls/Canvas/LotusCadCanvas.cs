//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Элементы управления
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCadCanvas.cs
*		Канва для отображения графических объектов.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.01.2022
//=====================================================================================================================
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
		/// Канва для отображения графических объектов
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class LotusCadCanvas : Canvas, INotifyPropertyChanged
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			// Фон
			protected static PropertyChangedEventArgs PropertyArgsBackgroundImage = new PropertyChangedEventArgs(nameof(BackgroundImage));
			protected static PropertyChangedEventArgs PropertyArgsBackgroundColor = new PropertyChangedEventArgs(nameof(BackgroundColor));
			#endregion

			#region ======================================= СТАТИЧЕСКИЕ МЕТОДЫ ========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Draw an Arc of an ellipse or circle. Static extension method of DrawingContext.
			/// </summary>
			/// <param name="dc">DrawingContext</param>
			/// <param name="pen">Pen for outline. set to null for no outline.</param>
			/// <param name="brush">Brush for fill. set to null for no fill.</param>
			/// <param name="rect">Box to hold the whole ellipse described by the arc</param>
			/// <param name="start_degrees">Start angle of the arc degrees within the ellipse. 0 degrees is a line to the right.</param>
			/// <param name="sweep_degrees">Sweep angle, -ve = Counterclockwise, +ve = Clockwise</param>
			//---------------------------------------------------------------------------------------------------------
			public static void DrawArc(DrawingContext dc, Pen pen, Brush brush, Rect rect, Double start_degrees, Double sweep_degrees)
			{
				GeometryDrawing arc = CreateArcDrawing(rect, start_degrees, sweep_degrees);
				dc.DrawGeometry(brush, pen, arc.Geometry);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Create an Arc geometry drawing of an ellipse or circle
			/// </summary>
			/// <param name="rect">Box to hold the whole ellipse described by the arc</param>
			/// <param name="start_degrees">Start angle of the arc degrees within the ellipse. 0 degrees is a line to the right.</param>
			/// <param name="sweep_degrees">Sweep angle, -ve = Counterclockwise, +ve = Clockwise</param>
			/// <returns>GeometryDrawing object</returns>
			//---------------------------------------------------------------------------------------------------------
			private static GeometryDrawing CreateArcDrawing(Rect rect, Double start_degrees, Double sweep_degrees)
			{
				// degrees to radians conversion
				Double start_radians = start_degrees * Math.PI / 180.0;
				Double sweep_radians = sweep_degrees * Math.PI / 180.0;

				// x and y radius
				Double dx = rect.Width / 2;
				Double dy = rect.Height / 2;

				// determine the start point 
				Double xs = rect.X + dx + (Math.Cos(start_radians) * dx);
				Double ys = rect.Y + dy + (Math.Sin(start_radians) * dy);

				// determine the end point 
				Double xe = rect.X + dx + (Math.Cos(start_radians + sweep_radians) * dx);
				Double ye = rect.Y + dy + (Math.Sin(start_radians + sweep_radians) * dy);

				// draw the arc into a stream geometry
				StreamGeometry streamGeom = new StreamGeometry();
				using (StreamGeometryContext ctx = streamGeom.Open())
				{
					Boolean is_large_arc = Math.Abs(sweep_degrees) > 180;
					SweepDirection sweep_direction = sweep_degrees < 0 ? SweepDirection.Counterclockwise : SweepDirection.Clockwise;

					ctx.BeginFigure(new Point(xs, ys), false, false);
					ctx.ArcTo(new Point(xe, ye), new Size(dx, dy), 0, is_large_arc, sweep_direction, true, false);
				}

				// create the drawing
				GeometryDrawing drawing = new GeometryDrawing();
				drawing.Geometry = streamGeom;
				return drawing;
			}
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			// Основные параметры
			protected internal ListArray<ICadObject> mElements;
			protected internal List<Visual> mVisuals;
			protected internal HitTestResultCallback mCallbackInsideGeometry;
			protected internal HitTestResultCallback mCallbackInsideOrIntersectGeometry;

			// Фон
			protected internal BitmapSource mBackgroundImage;
			protected internal TColor mBackgroundColor;

			// Элемент управления канвой
			protected internal LotusCadCanvasViewer mCanvasViewer;

			// Служебные данные
			protected internal Brush mCurrentBrush;
			protected internal DrawingBrush mCurrentBrushDrawing;
			protected internal Pen mCurrentPen;
			protected internal Stopwatch mRenderTimer;
			protected internal DrawingContext mDrawingDevice;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			//
			// ГЛОБАЛЬНЫЕ ДАННЫЕ
			//
			/// <summary>
			/// Статус нахождения компонента в режиме разработки
			/// </summary>
			public static Boolean IsDesignMode
			{
				get
				{
					var prop = DesignerProperties.IsInDesignModeProperty;
					var isDesignMode = (Boolean)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;
					return isDesignMode;
				}
			}

			/// <summary>
			/// Список графических объектов отображаемых на канве
			/// </summary>
			[Browsable(false)]
			public ListArray<ICadObject> Elements
			{
				get
				{
					return (mElements);
				}
			}

			//
			// ФОН
			//
			/// <summary>
			/// Фоновое изображение
			/// </summary>
			[Browsable(false)]
			public BitmapSource BackgroundImage
			{
				get
				{
					return (mBackgroundImage);
				}
				set
				{
					mBackgroundImage = value;
					NotifyPropertyChanged(PropertyArgsBackgroundImage);
					RaiseBackgroundImageChanged();
				}
			}

			/// <summary>
			/// Фоновый цвет
			/// </summary>
			[Browsable(false)]
			public TColor BackgroundColor
			{
				get
				{
					return (mBackgroundColor);
				}
				set
				{
					mBackgroundColor = value;
					NotifyPropertyChanged(PropertyArgsBackgroundColor);
					RaiseBackgroundColorChanged();
				}
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public LotusCadCanvas()
			{
				mElements = new ListArray<ICadObject>();
				mRenderTimer = new Stopwatch();
				this.Loaded += OnCanvas_Loaded;
				this.Unloaded += OnCanvas_Unloaded;
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
			private void OnCanvas_Loaded(Object sender, RoutedEventArgs args)
			{
				mCanvasViewer = Parent as LotusCadCanvasViewer;

				if (IsDesignMode == false)
				{
					mCurrentPen = new Pen(Brushes.Black, 1.0);
					mCurrentBrush = new SolidColorBrush(Colors.Gold);
					mCurrentBrushDrawing = new DrawingBrush();

					if (mCanvasViewer != null)
					{
						StartRendering();
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Выгрузка канвы
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnCanvas_Unloaded(Object sender, RoutedEventArgs e)
			{
				StopRendering();
			}
			#endregion

			#region ======================================= CЛУЖЕБНЫЕ МЕТОДЫ СОБЫТИЙ ==================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение фонового изображения.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseBackgroundImageChanged()
			{
				if (mBackgroundImage != null)
				{
					this.Width = mBackgroundImage.PixelWidth;
					this.Height = mBackgroundImage.PixelHeight;
					this.InvalidateVisual();
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение фонового цвета.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseBackgroundColorChanged()
			{
				this.Background = XWindowsColorManager.GetBrushByColor(mBackgroundColor.ToWinColor());
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Сортировка графических объектов по глубине
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void SortByZIndex()
			{
				Elements.Sort(SortingСomparison);
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

			#region ======================================= МЕТОДЫ РЕНДЕНИНГА =========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Старт ренденинга поверхности
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void StartRendering()
			{
				if (mRenderTimer.IsRunning)
				{
					return;
				}

				CompositionTarget.Rendering += OnRendering;
				mRenderTimer.Start();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Приостановка ренденинга поверхности
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void StopRendering()
			{
				if (!mRenderTimer.IsRunning)
				{
					return;
				}

				CompositionTarget.Rendering -= OnRendering;
				mRenderTimer.Stop();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Ренденинг поверхности
			/// </summary>
			/// <param name="sender">Источник события</param>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			private void OnRendering(Object sender, EventArgs args)
			{
				if (!mRenderTimer.IsRunning)
				{
					return;
				}

				InvalidateVisual();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Основной метод рисования канвы
			/// </summary>
			/// <param name="drawing_context">Контекст рисования</param>
			//---------------------------------------------------------------------------------------------------------
			protected override void OnRender(DrawingContext drawing_context)
			{
				base.OnRender(drawing_context);

				mDrawingDevice = drawing_context;

				// Необходим для возникновения реакций мыши
				if (BackgroundImage == null)
				{
					if (this.Background == null)
					{
						drawing_context.DrawRectangle(Brushes.White, null, new Rect(0, 0, this.ActualWidth, this.ActualHeight));
					}
					else
					{
						drawing_context.DrawRectangle(this.Background, null, new Rect(0, 0, this.ActualWidth, this.ActualHeight));
					}
				}
				else
				{
					drawing_context.DrawImage(BackgroundImage, new Rect(0, 0, this.ActualWidth, this.ActualHeight));
				}

				// Рисуем элементы
				for (Int32 i = 0; i < mElements.Count; i++)
				{
					ICadObject draw_element = mElements[i];
					if (draw_element != null)
					{
						draw_element.Draw();
					}
				}

				if (mCanvasViewer != null)
				{
					if (mCanvasViewer.OperationCurrent != TViewHandling.UserOperation)
					{
						if (mCanvasViewer.OperationCurrent == TViewHandling.SelectingRegion)
						{
							if (mCanvasViewer.SelectingRightToLeft)
							{
								drawing_context.DrawRectangle(null, XCadPenManager.Blue1.WindowsPen, mCanvasViewer.SelectingRect);
							}
							else
							{
								drawing_context.DrawRectangle(null, XCadPenManager.Green1.WindowsPen, mCanvasViewer.SelectingRect);
							}
						}

						if (mCanvasViewer.OperationCurrent == TViewHandling.ZoomingRegion)
						{
							if (mCanvasViewer.ZoomingStarting)
							{
								drawing_context.DrawRectangle(XCadBrushManager.Blue.WindowsBrush, XCadPenManager.Blue1.WindowsPen, mCanvasViewer.ZoomingRect);
							}
						}
					}

					if (mCanvasViewer.SnapIsEnabled && mCanvasViewer.SnapIsExsisting)
					{
						drawing_context.DrawRectangle(null, XCadPenManager.Red1.WindowsPen, 
							mCanvasViewer.SnapRects[mCanvasViewer.SnapIndexPoint].ToWinRect());
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
				mDrawingDevice.PushTransform(transform.mWindowsTransform);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Восстановление матрицы трансформации
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void ResetTransform()
			{
				mDrawingDevice.Pop();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка прозрачности рисования графических объектов
			/// </summary>
			/// <param name="opacity">Прозрачность</param>
			//---------------------------------------------------------------------------------------------------------
			public void SetOpacity(Single opacity)
			{
				mDrawingDevice.PushOpacity(opacity);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Восстановление прозрачности рисования графических объектов
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void ResetOpacity()
			{
				mDrawingDevice.Pop();
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
				mDrawingDevice.DrawLine(mCurrentPen, start_point.ToWinPoint(), end_point.ToWinPoint());
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

				mDrawingDevice.DrawLine(stroke.WindowsPen, start_point.ToWinPoint(), end_point.ToWinPoint());
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
				Brush old_brush = mCurrentPen.Brush;
				Double old_thickness = mCurrentPen.Thickness;

				mCurrentPen.Brush = brush.WindowsBrush;
				mCurrentPen.Thickness = thickness;

				mDrawingDevice.DrawLine(mCurrentPen, start_point.ToWinPoint(), end_point.ToWinPoint());

				mCurrentPen.Brush = old_brush;
				mCurrentPen.Thickness = old_thickness;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Рисование прямоугольника с параметрами отображения по умолчанию
			/// </summary>
			/// <param name="rect">Прямоугольник</param>
			//---------------------------------------------------------------------------------------------------------
			public void DrawRectangle(ref Rect2Df rect)
			{
				mDrawingDevice.DrawRectangle(null, mCurrentPen, rect.ToWinRect());
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
				mDrawingDevice.DrawRectangle(null, stroke.WindowsPen, rect.ToWinRect());
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
				if (thickness <= 0)
				{
					mDrawingDevice.DrawRectangle(brush.WindowsBrush, null, rect.ToWinRect());
				}
				else
				{
					Brush old_brush = mCurrentPen.Brush;
					Double old_thickness = mCurrentPen.Thickness;

					mCurrentPen.Brush = brush.WindowsBrush;
					mCurrentPen.Thickness = thickness;

					mDrawingDevice.DrawRectangle(brush.WindowsBrush, mCurrentPen, rect.ToWinRect());

					mCurrentPen.Brush = old_brush;
					mCurrentPen.Thickness = old_thickness;
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Рисование эллипса с параметрами отображения по умолчанию
			/// </summary>
			/// <param name="rect">Прямоугольник</param>
			//---------------------------------------------------------------------------------------------------------
			public void DrawEllipse(ref Rect2Df rect)
			{
				mDrawingDevice.DrawEllipse(null, mCurrentPen, rect.Center.ToWinPoint(), rect.Width/2, rect.Height/2);
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
				mDrawingDevice.DrawEllipse(null, stroke.WindowsPen, rect.Center.ToWinPoint(), rect.Width / 2, rect.Height / 2);
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
				Brush old_brush = mCurrentPen.Brush;
				Double old_thickness = mCurrentPen.Thickness;

				mCurrentPen.Brush = brush.WindowsBrush;
				mCurrentPen.Thickness = thickness;

				mDrawingDevice.DrawEllipse(null, mCurrentPen, rect.Center.ToWinPoint(), rect.Width / 2, rect.Height / 2);

				mCurrentPen.Brush = old_brush;
				mCurrentPen.Thickness = old_thickness;
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
				Single size = 10 * mCanvasViewer.ZoomInverse;
				Rect handle_rect = new Rect(point.X - size / 2, point.Y - size / 2, size, size);
				if (is_selected)
				{
					mDrawingDevice.DrawRectangle(XCadBrushManager.Red.WindowsBrush, null, handle_rect);
				}
				else
				{
					mCurrentPen.Thickness = 2 * mCanvasViewer.ZoomInverse;
					mDrawingDevice.DrawRectangle(null, mCurrentPen, handle_rect);
				}
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
				Single size = 10 * mCanvasViewer.ZoomInverse;
				Rect handle_rect = new Rect(point.X - size / 2, point.Y - size / 2, size, size);
				if (is_selected)
				{
					mDrawingDevice.DrawRectangle(XCadBrushManager.Red.WindowsBrush, null, handle_rect);
				}
				else
				{
					mCurrentPen.Thickness = 2 * mCanvasViewer.ZoomInverse;
					mDrawingDevice.DrawRectangle(null, mCurrentPen, handle_rect);
				}
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
				if (is_selected)
				{
					mDrawingDevice.DrawRectangle(XCadBrushManager.Red.WindowsBrush, null, rect.ToWinRect());
				}
				else
				{
					mCurrentPen.Thickness = 2 * mCanvasViewer.ZoomInverse;
					mDrawingDevice.DrawRectangle(null, mCurrentPen, rect.ToWinRect());
				}
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
				if (is_selected)
				{
					mDrawingDevice.DrawRectangle(XCadBrushManager.Red.WindowsBrush, null, rect.ToWinRect());
				}
				else
				{
					mCurrentPen.Thickness = 2 * mCanvasViewer.ZoomInverse;
					mDrawingDevice.DrawRectangle(null, mCurrentPen, rect.ToWinRect());
				}
			}
			#endregion

			#region ======================================= ДАННЫЕ INotifyPropertyChanged =============================
			/// <summary>
			/// Событие срабатывает ПОСЛЕ изменения свойства
			/// </summary>
			public event PropertyChangedEventHandler PropertyChanged;

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вспомогательный метод для нотификации изменений свойства
			/// </summary>
			/// <param name="property_name">Имя свойства</param>
			//---------------------------------------------------------------------------------------------------------
			public void NotifyPropertyChanged(String property_name = "")
			{
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs(property_name));
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вспомогательный метод для нотификации изменений свойства
			/// </summary>
			/// <param name="args">Аргументы события</param>
			//---------------------------------------------------------------------------------------------------------
			public void NotifyPropertyChanged(PropertyChangedEventArgs args)
			{
				if (PropertyChanged != null)
				{
					PropertyChanged(this, args);
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
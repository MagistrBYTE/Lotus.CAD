//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Интерактивные графические фигуры
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADShapeTable.cs
*		Таблица.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 04.04.2021
//=====================================================================================================================
using System;
using System.Xml;
using System.IO;
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
		//! \addtogroup CadShape
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Тип данных в ячейки
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public enum TCadCellDataType
		{
			/// <summary>
			/// Простой текст
			/// </summary>
			Text,

			/// <summary>
			/// Формула
			/// </summary>
			Formula,

			/// <summary>
			/// Автотекст
			/// </summary>
			Autotext,

			/// <summary>
			/// Данные
			/// </summary>
			Data
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Тип визуального отображения строки
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public enum TCadRowModeType
		{
			/// <summary>
			/// Обычная строка
			/// </summary>
			Simple,

			/// <summary>
			/// Строка заголовка
			/// </summary>
			Header,

			/// <summary>
			/// Строка заголовка группы
			/// </summary>
			HeaderGroup,

			/// <summary>
			/// Режим строки не определен
			/// </summary>
			NoDefined
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Тип данных в столбце
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public enum TCadColumnDataType
		{
			/// <summary>
			/// Простой текст
			/// </summary>
			Text,

			/// <summary>
			/// Данные поля
			/// </summary>
			Data,

			/// <summary>
			/// Счётчик
			/// </summary>
			Counter,

			/// <summary>
			/// Расчётный
			/// </summary>
			Calculated
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Делегат события при нажатие на ячейку
		/// </summary>
		/// <param name="cell">Ячейка на которую нажали</param>
		//-------------------------------------------------------------------------------------------------------------
		public delegate void TCellEventHandler(CCadTableCell cell);

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Делегат события вызывается при обновление выбора ячеек
		/// </summary>
		/// <param name="cells">Выбранные ячейки</param>
		//-------------------------------------------------------------------------------------------------------------
		public delegate void TCellsEventHandler(List<CCadTableCell> cells);

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Ячейка таблица
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CCadTableCell : INotifyPropertyChanged
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			// Данные
			protected static PropertyChangedEventArgs PropertyArgsCellData = new PropertyChangedEventArgs(nameof(CellData));
			protected static PropertyChangedEventArgs PropertyArgsData = new PropertyChangedEventArgs(nameof(Data));

			// Оформление
			protected static PropertyChangedEventArgs PropertyArgsFontSize = new PropertyChangedEventArgs(nameof(FontSize));
			protected static PropertyChangedEventArgs PropertyArgsHorizontalAlignment = new PropertyChangedEventArgs(nameof(HorizontalAlignment));
			protected static PropertyChangedEventArgs PropertyArgsVerticalAlignment = new PropertyChangedEventArgs(nameof(VerticalAlignment));
			protected static PropertyChangedEventArgs PropertyArgsIsBorderLeft = new PropertyChangedEventArgs(nameof(IsBorderLeft));
			protected static PropertyChangedEventArgs PropertyArgsIsBorderRight = new PropertyChangedEventArgs(nameof(IsBorderRight));
			protected static PropertyChangedEventArgs PropertyArgsIsBorderTop = new PropertyChangedEventArgs(nameof(IsBorderTop));
			protected static PropertyChangedEventArgs PropertyArgsIsBorderBottom = new PropertyChangedEventArgs(nameof(IsBorderBottom));

			// Макет
			protected static PropertyChangedEventArgs PropertyArgsRowSpan = new PropertyChangedEventArgs(nameof(RowSpan));
			protected static PropertyChangedEventArgs PropertyArgsColumnSpan = new PropertyChangedEventArgs(nameof(ColumnSpan));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			// Данные
			internal TCadCellDataType mCellData;
			internal String mData = "";

			// Оформление
			internal Single mFontSize = 12;
			internal TCadTextHorizontalAlignment mHorizontalAlignment;
			internal TCadTextVerticalAlignment mVerticalAlignment;
			internal Rect2Df mBoundsRect;
			
			internal Boolean mIsDraw = false;
			internal Boolean mIsVisible = true;
			internal Boolean mIsBlocked = false;
			internal Boolean mIsBorderLeft = true;
			internal Boolean mIsBorderRight = true;
			internal Boolean mIsBorderTop = true;
			internal Boolean mIsBorderBottom = true;

			// Внутренние данные
			internal CCadShapeTable mOnwerTable;
			internal CCadTableRow mOnwerRow;
			internal CCadTableRow mOnwerRowLast;
			internal CCadTableColumn mOnwerColumn;
			internal CCadTableColumn mOnwerColumnLast;

			// Служебные данные
			internal Vector2Df mPoint;
			internal Int32 mOldRowSpan = 1;
			internal Int32 mOldColumnSpan = 1;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Уникальный идентификатор объекта
			/// </summary>
			[Browsable(false)]
			public Int32 ID
			{
				get { return (this.GetHashCode()); }
			}

			//
			// ПАРАМЕТРЫ ДАННЫХ
			//
			/// <summary>
			/// Тип данных в ячейки
			/// </summary>
			[DisplayName("Тип данных")]
			[Description("Тип данных в ячейки")]
			[Category(XInspectorGroupDesc.Data)]
			//[Display(GroupName = XResources.PropertyGroupData, Order = 0)]
			public TCadCellDataType CellData
			{
				get { return (mCellData); }
				set
				{
					if (mCellData != value)
					{
						mCellData = value;
						NotifyPropertyChanged(PropertyArgsCellData);
					}
				}
			}

			/// <summary>
			/// Вычисленные данные ячейки
			/// </summary>
			[DisplayName("Данные")]
			[Description("Вычисленные данные ячейки")]
			[Category(XInspectorGroupDesc.Data)]
			//[Display(GroupName = XResources.PropertyGroupData, Order = 1)]
			public String Data
			{
				get { return (mData); }
				set
				{
					mData = value;
					NotifyPropertyChanged(PropertyArgsData);
					
					if (mOnwerTable.IsCanvas)
					{
						//mCanvas.UpdateTextID(ID, mData);
						//mPoint = mCanvas.ComputeTextIDPoint(ID, mHorizontalAlignment, mVerticalAlignment, mBoundsRect);
						//mCanvas.Update();
					}
				}
			}

			//
			// ПАРАМЕТРЫ ГРАФИКИ
			//
			/// <summary>
			/// Размер шрифта
			/// </summary>
			[DisplayName("Размер шрифта")]
			[Description("Размер шрифта")]
			[Category(XInspectorGroupDesc.Graphics)]
			//[Display(GroupName = XInspectorGroupDesc.Graphics, Order = 0)]
			public Single FontSize
			{
				get { return (mFontSize); }
				set
				{
					mFontSize = value;
					NotifyPropertyChanged(PropertyArgsFontSize);

					if (mOnwerTable.IsCanvas)
					{
						//mCanvas.UpdateTextID(ID, mFontSize);
						//mPoint = mCanvas.ComputeTextIDPoint(ID, mHorizontalAlignment, mVerticalAlignment, mBoundsRect);
						//mCanvas.Update();
					}
				}
			}

			/// <summary>
			/// Выравнивание текста по горизонтали
			/// </summary>
			[DisplayName("Горизонталь")]
			[Description("Выравнивание текста по горизонтали")]
			[Category(XInspectorGroupDesc.Graphics)]
			//[Display(GroupName = XInspectorGroupDesc.Graphics, Order = 1)]
			public TCadTextHorizontalAlignment HorizontalAlignment
			{
				get { return (mHorizontalAlignment); }
				set
				{
					if(mHorizontalAlignment != value)
					{
						mHorizontalAlignment = value;
						NotifyPropertyChanged(PropertyArgsHorizontalAlignment);
						//mPoint = mCanvas.ComputeTextIDPoint(ID, mHorizontalAlignment, mVerticalAlignment, mBoundsRect);

						if (mOnwerTable.IsCanvas)
						{
							//mCanvas.Update();
						}
					}
				}
			}

			/// <summary>
			/// Выравнивание текста по вертикали
			/// </summary>
			[DisplayName("Вертикаль")]
			[Description("Выравнивание текста по вертикали")]
			[Category(XInspectorGroupDesc.Graphics)]
			//[Display(GroupName = XInspectorGroupDesc.Graphics, Order = 2)]
			public TCadTextVerticalAlignment VerticalAlignment
			{
				get { return (mVerticalAlignment); }
				set
				{
					if (mVerticalAlignment != value)
					{
						mVerticalAlignment = value;
						NotifyPropertyChanged(PropertyArgsVerticalAlignment);
						//mPoint = mCanvas.ComputeTextIDPoint(ID, mHorizontalAlignment, mVerticalAlignment, mBoundsRect);

						if (mOnwerTable.IsCanvas)
						{
							//mCanvas.Update();
						}
					}
				}
			}

			/// <summary>
			/// Следует ли рисовать левую границу ячейку
			/// </summary>
			[DisplayName("Левая граница")]
			[Description("Следует ли рисовать левую границу ячейку")]
			[Category(XInspectorGroupDesc.Graphics)]
			//[Display(GroupName = XInspectorGroupDesc.Graphics, Order = 3)]
			public Boolean IsBorderLeft
			{
				get { return (mIsBorderLeft); }
				set
				{
					if(mIsBorderLeft != value)
					{
						mIsBorderLeft = value;
						NotifyPropertyChanged(PropertyArgsIsBorderLeft);
						this.SetBorderLeft(value);

						if (mOnwerTable.IsCanvas)
						{
							//mCanvas.Update();
						}
					}
				}
			}

			/// <summary>
			/// Следует ли рисовать правую границу ячейку
			/// </summary>
			[DisplayName("Правая граница")]
			[Description("Следует ли рисовать правую границу ячейку")]
			[Category(XInspectorGroupDesc.Graphics)]
			//[Display(GroupName = XInspectorGroupDesc.Graphics, Order = 4)]
			public Boolean IsBorderRight
			{
				get { return (mIsBorderRight); }
				set
				{
					if(mIsBorderRight != value)
					{
						mIsBorderRight = value;
						NotifyPropertyChanged(PropertyArgsIsBorderRight);
						this.SetBorderRight(value);

						if (mOnwerTable.IsCanvas)
						{
							//mCanvas.Update();
						}
					}
				}
			}

			/// <summary>
			/// Следует ли рисовать верхнию границу ячейку
			/// </summary>
			[DisplayName("Верхняя граница")]
			[Description("Следует ли рисовать верхнию границу ячейку")]
			[Category(XInspectorGroupDesc.Graphics)]
			//[Display(GroupName = XInspectorGroupDesc.Graphics, Order = 5)]
			public Boolean IsBorderTop
			{
				get { return (mIsBorderTop); }
				set
				{
					if (mIsBorderTop != value)
					{
						mIsBorderTop = value;
						NotifyPropertyChanged(PropertyArgsIsBorderTop);
						this.SetBorderTop(value);

						if (mOnwerTable.IsCanvas)
						{
							//mCanvas.Update();
						}
					}
				}
			}

			/// <summary>
			/// Следует ли рисовать нижнию границу ячейку
			/// </summary>
			[DisplayName("Нижняя граница")]
			[Description("Следует ли рисовать нижнию границу ячейку")]
			[Category(XInspectorGroupDesc.Graphics)]
			//[Display(GroupName = XInspectorGroupDesc.Graphics, Order = 6)]
			public Boolean IsBorderBottom
			{
				get { return (mIsBorderBottom); }
				set
				{
					if(mIsBorderBottom != value)
					{
						mIsBorderBottom = value;
						NotifyPropertyChanged(PropertyArgsIsBorderBottom);
						this.SetBorderBottom(value);

						if (mOnwerTable.IsCanvas)
						{
							//mCanvas.Update();
						}
					}
				}
			}

			//
			// ПАРАМЕТРЫ МАКЕТА
			//
			/// <summary>
			/// Количество строк на которые распространяется ячейка
			/// </summary>
			[DisplayName("Кол-во строк")]
			[Description("Количество строк на которые распространяется ячейка")]
			[Category(XInspectorGroupDesc.Formats)]
			//[Display(GroupName = XResources.PropertyGroupFormats, Order = 0)]
			//[Telerik.Windows.Controls.Data.PropertyGrid.Editor(typeof(Telerik.Windows.Controls.RadNumericUpDown), "Value")]
			public Int32 RowSpan
			{
				get
				{
					// Если других строк нету то
					if (mOnwerRowLast == null || mOnwerRowLast == mOnwerRow)
					{
						return (1);
					}
					else
					{
						// Смотрим на разницу индексов
						// TODO: Строки таблицы должны быть перестроены!
						Int32 count = mOnwerRowLast.Index - mOnwerRow.Index + 1;
						return (count);
					}
				}
				set
				{
					if ((this.RowIndex + value - 1) > -1)
					{
						if (this.RowSpan != value)
						{
							// Устанавливаем объединение ячеек
							SetRowSpan(this.RowSpan, value);

							// Ищем последнию присоединеню строку
							mOnwerRowLast = mOnwerTable.mRows[this.RowIndex + value - 1];

							// Надо обновить всю таблицу
							mOnwerTable.Rebuild();
						}

						if (mOnwerTable.IsCanvas)
						{
							//mCanvas.Update();
						}
					}
				}
			}

			/// <summary>
			/// Количество столбцов на которые распространяется ячейка
			/// </summary>
			[DisplayName("Кол-во столбцов")]
			[Description("Количество столбцов на которые распространяется ячейка")]
			[Category(XInspectorGroupDesc.Formats)]
			//[Display(GroupName = XResources.PropertyGroupFormats, Order = 1)]
			//[Telerik.Windows.Controls.Data.PropertyGrid.Editor(typeof(Telerik.Windows.Controls.RadNumericUpDown), "Value")]
			public Int32 ColumnSpan
			{
				get
				{
					// Если других строк нету то
					if (mOnwerColumnLast == null || mOnwerColumnLast == mOnwerColumn)
					{
						return (1);
					}
					else
					{
						// Смотрим на разницу индексов
						// TODO: Столбцы таблицы должны быть перестроены!
						Int32 count = mOnwerColumnLast.Index - mOnwerColumn.Index + 1;
						return (count);
					}
				}
				set
				{
					if (this.ColumnSpan != value)
					{
						SetColumnSpan(this.ColumnSpan, value);

						// Ищем последнию присоединеню строку
						mOnwerColumnLast = mOnwerTable.mColumns[this.ColumnIndex + value - 1];

						// Надо обновить всю таблицу
						mOnwerTable.Rebuild();
					}

					if (mOnwerTable.IsCanvas)
					{
						//mCanvas.Update();
					}
				}
			}

			/// <summary>
			/// Видимость ячейки - перекрывающуюся ячейку не видно (и следовательно её не надо рисовать)
			/// </summary>
			[Browsable(false)]
			public Boolean IsVisible
			{
				get { return (mIsVisible); }
				set
				{
					this.SetVisible(value);
					mIsVisible = value;
				}
			}

			/// <summary>
			/// Статус блокировки ячейки - блокированную ячейку рисовать не надо
			/// </summary>
			[Browsable(false)]
			public Boolean IsBlocked
			{
				get { return (mIsBlocked); }
				set
				{
					this.SetBlocked(value);
					mIsBlocked = value;
				}
			}

			/// <summary>
			/// Статус объединённой ячейки
			/// </summary>
			[Browsable(false)]
			public Boolean Combined
			{
				get
				{
					if (this.RowSpan > 1 || this.ColumnSpan > 1)
					{
						return (true);
					}
					else
					{
						return (false);
					}
				}
			}

			/// <summary>
			/// Следует рисовать ячейку - отрисованную ячейку рисовать не надо
			/// </summary>
			[Browsable(false)]
			public Boolean IsDraw
			{
				get { return (mIsDraw); }
				set { mIsDraw = value; }
			}

			/// <summary>
			/// Строка которой принадлежит ячейка
			/// </summary>
			[Browsable(false)]
			public Int32 RowIndex
			{
				get
				{
					return (mOnwerRow.Index);
				}
			}

			/// <summary>
			/// Максимальное количество строк на которые может распространяется ячейка
			/// </summary>
			[Browsable(false)]
			public Int32 RowSpanMaximum
			{
				get { return (this.GetMaximumRowCount() + 1); }
			}

			/// <summary>
			/// Столбец которому принадлежит ячейка
			/// </summary>
			[Browsable(false)]
			public Int32 ColumnIndex
			{
				get { return (mOnwerColumn.Index); }
			}

			/// <summary>
			/// Максимальное количество столбцов на которые распространяется ячейка
			/// </summary>
			[Browsable(false)]
			public Int32 ColumnSpanMaximum
			{
				get { return (this.GetMaximumColumnCount() + 1); }
			}

			/// <summary>
			/// Таблица
			/// </summary>
			[Browsable(false)]
			public CCadShapeTable OnwerTable
			{
				get { return (mOnwerTable); }
			}

			/// <summary>
			/// Строка которой принадлежит ячейка
			/// </summary>
			[Browsable(false)]
			public CCadTableRow OnwerRow
			{
				get { return (mOnwerRow); }
			}

			/// <summary>
			/// Столбец которому принадлежит ячейка
			/// </summary>
			[Browsable(false)]
			public CCadTableColumn OnwerColumn
			{
				get { return (mOnwerColumn); }
			}

			/// <summary>
			/// Левая смежная ячейка
			/// </summary>
			[Browsable(false)]
			public CCadTableCell AdjacentLeftCell
			{
				get
				{
					if (this.ColumnIndex > 0)
					{
						return (mOnwerRow[this.ColumnIndex - 1]);
					}
					else
					{
						return (null);
					}
				}
			}

			/// <summary>
			/// Правая смежная ячейка
			/// </summary>
			[Browsable(false)]
			public CCadTableCell AdjacentRightCell
			{
				get
				{
					Int32 index = 0;

					// Получаем последний индекс столбца
					if (mOnwerColumnLast != null)
					{
						index = mOnwerColumnLast.Index;
					}
					else
					{
						index = mOnwerColumn.Index;
					}


					if (index < mOnwerTable.ColumnCount - 1)
					{
						return (mOnwerRow[index + 1]);
					}
					else
					{
						return (null);
					}
				}
			}

			/// <summary>
			/// Верхняя смежная ячейка
			/// </summary>
			[Browsable(false)]
			public CCadTableCell AdjacentTopCell
			{
				get
				{
					if (this.RowIndex > 0)
					{
						return (mOnwerColumn[this.RowIndex - 1]);
					}
					else
					{
						return (null);
					}
				}
			}

			/// <summary>
			/// Верхняя смежная ячейка
			/// </summary>
			[Browsable(false)]
			public CCadTableCell AdjacentBottomCell
			{
				get
				{
					Int32 index = 0;
					// Получаем последний индекс столбца
					if (mOnwerRowLast != null)
					{
						index = mOnwerRowLast.Index;
					}
					else
					{
						index = mOnwerRow.Index;
					}

					if (index < mOnwerTable.RowCount - 1)
					{
						return (mOnwerColumn[index + 1]);
					}
					else
					{
						return (null);
					}
				}
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Создание ячейки
			/// </summary>
			/// <param name="onwer_row">Строка которой будет принадлежат ячейка</param>
			/// <param name="onwer_column">Столбец которому будет принадлежат ячейка</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadTableCell(CCadTableRow onwer_row, CCadTableColumn onwer_column)
			{
				// Сохраняем таблицу
				mOnwerRow = onwer_row;
				mOnwerColumn = onwer_column;
				mOnwerTable = onwer_row.mOnwerTable;
				mData = "";
				mFontSize = 12;
				mHorizontalAlignment = TCadTextHorizontalAlignment.Center;
				mVerticalAlignment = TCadTextVerticalAlignment.Middle;
			}
			#endregion

			#region ======================================= СИСТЕМНЫЕ МЕТОДЫ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Преобразование к текстовому представлению
			/// </summary>
			/// <returns>Ячейка с обозначением строки и столбца</returns>
			//---------------------------------------------------------------------------------------------------------
			public override String ToString()
			{
				return ("Ячейка [" + mOnwerColumn.Index.ToString() + "][" + 
					mOnwerRow.Index.ToString() + "]");
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление размеров и позиции ячейки
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void RebuildCell()
			{
				// Считаем позицию ячейки по X
				Single pos_x = mOnwerColumn.GetX();

				// Считаем размер ячейки по X
				Single width = 0;
				for (Int32 i = 0; i < this.ColumnSpan; i++)
				{
					width += mOnwerTable.mColumns[this.ColumnIndex + i].mWidth;
				}

				// Считаем позицию ячейки по Y
				Single pos_y = mOnwerRow.GetY();

				// Считаем размер ячейки по Y
				Single height = 0;
				for (Int32 i = 0; i < this.RowSpan; i++)
				{
					height += mOnwerTable.mRows[this.RowIndex + i].mHeight;
				}

				mBoundsRect.X = pos_x;
				mBoundsRect.Y = pos_y;
				mBoundsRect.Width = width;
				mBoundsRect.Height = height;

				//mPoint = mCanvas.ComputeTextIDPoint(ID, mHorizontalAlignment, mVerticalAlignment, mBoundsRect);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Подготовка ячейки к модификации
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void PrepareToModify()
			{
				mOldRowSpan = this.RowSpan;
				mOldColumnSpan = this.ColumnSpan;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка количества объединяющихся ячеек по строкам
			/// </summary>
			/// <param name="current_row">Текущие кол-во объединеных ячеек</param>
			/// <param name="row_span">Новое кол-во объединеных ячеек</param>
			//---------------------------------------------------------------------------------------------------------
			public void SetRowSpan(Int32 current_row, Int32 row_span)
			{
				if (row_span > current_row)
				{
					// Считаем
					for (Int32 ir = current_row; ir < row_span; ir++)
					{
						for (Int32 ic = 0; ic < this.ColumnSpan; ic++)
						{
							mOnwerTable.Rows[this.RowIndex + ir][this.ColumnIndex + ic].IsVisible = false;
							mOnwerTable.Rows[this.RowIndex + ir][this.ColumnIndex + ic].mIsBlocked = true;
						}
					}
				}
				else
				{
					for (Int32 ir = row_span; ir < current_row; ir++)
					{
						for (Int32 ic = 0; ic < this.ColumnSpan; ic++)
						{
							mOnwerTable.Rows[this.RowIndex + ir][this.ColumnIndex + ic].IsVisible = true;
							mOnwerTable.Rows[this.RowIndex + ir][this.ColumnIndex + ic].mIsBlocked = false;
						}
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление статуса соединённых ячеек по строкам
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void UpdateRowSpan()
			{
				// Разница 
				Int32 current_row = this.RowSpan;

				if (current_row > 1)
				{
					// Считаем
					for (Int32 ir = 1; ir < current_row; ir++)
					{
						for (Int32 ic = 0; ic < this.ColumnSpan; ic++)
						{
							mOnwerTable.Rows[this.RowIndex + ir][this.ColumnIndex + ic].IsVisible = false;
						}
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Максимальное количество ячеек которые можно объединить по вертикали (по строкам)
			/// </summary>
			/// <returns>Количество ячеек</returns>
			//---------------------------------------------------------------------------------------------------------
			public Int32 GetMaximumRowCount()
			{
				Int32 row_end = this.RowIndex + 1;
				Int32 count = 0;

				for (Int32 i = row_end; i < mOnwerTable.RowCount; i++)
				{
					if (mOnwerTable[i, this.ColumnIndex].Combined)
					{
						break;
					}
					else
					{
						count++;
					}
				}

				return (count);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка количества объединяющихся ячеек по столбцам
			/// </summary>
			/// <param name="current_column">Текущие кол-во объединеных ячеек</param>
			/// <param name="column_span">Кол-во столбцов</param>
			//---------------------------------------------------------------------------------------------------------
			public void SetColumnSpan(Int32 current_column, Int32 column_span)
			{
				if (column_span > current_column)
				{
					// Теперь выключаем
					for (Int32 ic = current_column; ic < column_span; ic++)
					{
						for (Int32 ir = 0; ir < this.RowSpan; ir++)
						{
							mOnwerTable[this.RowIndex + ir, this.ColumnIndex + ic].IsVisible = false;
							mOnwerTable[this.RowIndex + ir, this.ColumnIndex + ic].mIsBlocked = true;
						}
					}
				}

				else
				{
					// Сначала возвращаем как было
					for (Int32 ic = column_span; ic < current_column; ic++)
					{
						for (Int32 ir = 0; ir < this.RowSpan; ir++)
						{
							mOnwerTable[this.RowIndex + ir, this.ColumnIndex + ic].IsVisible = true;
							mOnwerTable[this.RowIndex + ir, this.ColumnIndex + ic].mIsBlocked = false;
						}
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление статуса соединённых ячеек по столбцам
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void UpdateColumnSpan()
			{
				// Разница 
				Int32 current_column = this.ColumnSpan;

				if (current_column > 1)
				{
					// Теперь выключаем
					for (Int32 ic = 1; ic < current_column; ic++)
					{
						for (Int32 ir = 0; ir < this.RowSpan; ir++)
						{
							mOnwerTable[this.RowIndex + ir, this.ColumnIndex + ic].IsVisible = false;
						}
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Максимальное количество ячеек которые можно объединить по горизонтали (по столбцам)
			/// </summary>
			/// <returns>Количество ячеек</returns>
			//---------------------------------------------------------------------------------------------------------
			public Int32 GetMaximumColumnCount()
			{
				Int32 column_end = this.ColumnIndex + this.ColumnSpan;
				Int32 count = 0;

				for (Int32 i = column_end; i < mOnwerTable.ColumnCount; i++)
				{
					if (mOnwerTable[this.RowIndex, i].Combined)
					{
						break;
					}
					else
					{
						count++;
					}
				}

				return (count);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка видимости левой границы ячейки
			/// </summary>
			/// <param name="visible">Статус видимости</param>
			//---------------------------------------------------------------------------------------------------------
			public void SetBorderLeft(Boolean visible)
			{
				if (visible && mIsVisible)
				{
					if (this.ColumnIndex > 0)
					{
						// Только если смежная ячейка не блокирована
						if (this.AdjacentLeftCell.IsBlocked)
						{
							this.AdjacentLeftCell.mIsBorderRight = false;
						}
						else
						{
							this.AdjacentLeftCell.mIsBorderRight = true;
						}
					}
				}
				else
				{
		
					if (this.ColumnIndex > 0)
					{
						// Только если текущая ячейка не блокирована
						if (IsBlocked == false)
						{
							this.AdjacentLeftCell.mIsBorderRight = visible;
						}
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка видимости правой границы ячейки
			/// </summary>
			/// <param name="visible">Статус видимости</param>
			//---------------------------------------------------------------------------------------------------------
			public void SetBorderRight(Boolean visible)
			{
				CCadTableCell right_cell = this.AdjacentRightCell;

				if (visible && mIsVisible)
				{
					if (right_cell != null)
					{
						if (right_cell.IsBlocked)
						{
							right_cell.mIsBorderLeft = false;
						}
						else
						{
							right_cell.mIsBorderLeft = true;
						}
					}
				}
				else
				{

					if (right_cell != null)
					{
						// Только если текущая ячейка не блокирована
						if (IsBlocked == false)
						{
							right_cell.mIsBorderLeft = visible;
						}
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка видимости верхней границы ячейки
			/// </summary>
			/// <param name="visible">Статус видимости</param>
			//---------------------------------------------------------------------------------------------------------
			public void SetBorderTop(Boolean visible)
			{
				CCadTableCell top_cell = this.AdjacentTopCell;
				if (visible && mIsVisible)
				{

					if (top_cell != null)
					{
						// Только если смежная ячейка не блокирована
						if (top_cell.IsBlocked)
						{
							top_cell.mIsBorderBottom = false;
						}
						else
						{
							top_cell.mIsBorderBottom = true;
						}
					}
				}
				else
				{

					if (top_cell != null)
					{
						// Только если текущая ячейка не блокирована
						if (IsBlocked == false)
						{
							top_cell.mIsBorderBottom = visible;
						}
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка видимости верхней границы ячейки
			/// </summary>
			/// <param name="visible">Статус видимости</param>
			//---------------------------------------------------------------------------------------------------------
			public void SetBorderBottom(Boolean visible)
			{
				CCadTableCell bottom_cell = this.AdjacentBottomCell;
				if (visible && mIsVisible)
				{
					if (bottom_cell != null)
					{
						// Только если смежная ячейка не блокирована
						if (bottom_cell.IsBlocked)
						{
							bottom_cell.mIsBorderTop = false;
						}
						else
						{
							bottom_cell.mIsBorderTop = true;
						}
					}
				}
				else
				{
					if (bottom_cell != null)
					{
						// Только если текущая ячейка не блокирована
						if (IsBlocked == false)
						{
							bottom_cell.mIsBorderTop = visible;
						}
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка видимости ячейки
			/// </summary>
			/// <param name="is_visible">Статус видимости</param>
			//---------------------------------------------------------------------------------------------------------
			public void SetVisible(Boolean is_visible)
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка блокировки ячейки
			/// </summary>
			/// <param name="value">Статус блокировки</param>
			//---------------------------------------------------------------------------------------------------------
			public void SetBlocked(Boolean value)
			{
				if (value)
				{

				}
			}
			#endregion

			#region ======================================= МЕТОДЫ ВЫЧИСЛЕНИЙ =========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вычисление значения ячейки
			/// </summary>
			/// <param name="value">Данные</param>
			/// <returns>Вычисленное значение</returns>
			//---------------------------------------------------------------------------------------------------------
			public String CalculateData(String value)
			{
				String data = String.Empty;

				switch (mCellData)
				{
					case TCadCellDataType.Text:
						{
							//mCalcData = value;
							//mInternalData = value;

						}
						break;
					case TCadCellDataType.Formula:
						{
							try
							{
								//mInternalData = value;
								//mCalcData = this.AdjacentRightCell.Data + this.AdjacentLeftCell.Data;
							}
							catch (Exception)
							{
								//mInternalData = value;
								//mCalcData = "ERROR";
							}
						}
						break;
					case TCadCellDataType.Autotext:
						break;
					case TCadCellDataType.Data:
						break;
					default:
						break;
				}

				//mContent.Text = mCalcData;

				return (data);
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
		/// <summary>
		/// Строка таблицы
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CCadTableRow : INotifyPropertyChanged
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			protected static PropertyChangedEventArgs PropertyArgsIndex = new PropertyChangedEventArgs(nameof(Index));
			protected static PropertyChangedEventArgs PropertyArgsHeight = new PropertyChangedEventArgs(nameof(Height));
			protected static PropertyChangedEventArgs PropertyArgsRowMode = new PropertyChangedEventArgs(nameof(RowMode));
			protected static PropertyChangedEventArgs PropertyArgsGroupName = new PropertyChangedEventArgs(nameof(GroupName));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			internal List<CCadTableCell> mCells;
			internal CCadShapeTable mOnwerTable;
			internal Int32 mIndex = 0;
			internal Single mHeight = 8;
			internal TCadRowModeType mRowMode;
			internal String mGroupName;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Индекс строки по порядку
			/// </summary>
			[Browsable(false)]
			public Int32 Index
			{
				get { return mIndex; }
				set
				{
					if (mIndex != value)
					{
						mIndex = value;
						NotifyPropertyChanged(PropertyArgsIndex);
					}
				}
			}

			/// <summary>
			/// Высота строки
			/// </summary>
			[DisplayName("Высота")]
			[Description("Высота строки")]
			[Category(XInspectorGroupDesc.Params)]
			//[Display(GroupName = XInspectorGroupDesc.Params, Order = 0)]
			//[Telerik.Windows.Controls.Data.PropertyGrid.Editor(typeof(Telerik.Windows.Controls.RadNumericUpDown), "Value")]
			public Single Height
			{
				get { return mHeight; }
				set
				{
					if (mHeight != value)
					{
						mHeight = value;
						NotifyPropertyChanged(PropertyArgsHeight);
						mOnwerTable.Rebuild();

						if(mOnwerTable.IsCanvas)
						{
							//mCanvas.Update();
						}
					}
				}
			}

			/// <summary>
			/// Тип визуального отображения строки
			/// </summary>
			[DisplayName("Вид")]
			[Description("Тип визуального отображения строки")]
			[Category(XInspectorGroupDesc.Params)]
			//[Display(GroupName = XInspectorGroupDesc.Params, Order = 1)]
			public TCadRowModeType RowMode
			{
				get { return mRowMode; }
				set
				{
					if (mRowMode != value)
					{
						mRowMode = value;
						NotifyPropertyChanged(PropertyArgsRowMode);
						this.SetRowMode();

						if (mOnwerTable.IsCanvas)
						{
							//mCanvas.Update();
						}
					}
				}
			}

			/// <summary>
			/// Имя группы которой принадлежит строка
			/// </summary>
			[DisplayName("Группа")]
			[Description("Имя группы которой принадлежит строка")]
			[Category(XInspectorGroupDesc.Params)]
			//[Display(GroupName = XInspectorGroupDesc.Params, Order = 2)]
			public String GroupName
			{
				get { return mGroupName; }
				set
				{
					if (mGroupName != value)
					{
						mGroupName = value;
						NotifyPropertyChanged(PropertyArgsGroupName);
					}
				}
			}

			/// <summary>
			/// Таблица
			/// </summary>
			[Browsable(false)]
			public CCadShapeTable OnwerTable
			{
				get { return (mOnwerTable); }
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//--------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор создает строку по индексу
			/// </summary>
			/// <param name="table">Таблица</param>
			/// <param name="index">Индекс строки</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadTableRow(CCadShapeTable table, Int32 index)
			{
				mOnwerTable = table;
				mIndex = index;
				mCells = new List<CCadTableCell>();

				// Добавляем ячейки согласно количество строк
				for (Int32 i = 0; i < mOnwerTable.ColumnCount; i++)
				{
					mCells.Add(new CCadTableCell(this, mOnwerTable.Columns[i]));
				}
			}
			#endregion

			#region ======================================= ИНДЕКСАТОР ================================================
			/// <summary>
			/// Индексация ячеек строки 
			/// </summary>
			/// <param name="index">Индекс ячейки (столбца)</param>
			/// <returns>Ячейка</returns>
			//---------------------------------------------------------------------------------------------------------
			public CCadTableCell this[Int32 index]
			{
				get { return (mCells[index]); }
			}
			#endregion

			#region ======================================= СИСТЕМНЫЕ МЕТОДЫ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Преобразование к текстовому представлению
			/// </summary>
			/// <returns>Наименование графического элемента</returns>
			//---------------------------------------------------------------------------------------------------------
			public override String ToString()
			{
				return ("Строка " + mIndex.ToString());
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение координаты Y строки
			/// </summary>
			/// <returns>Суммарная координата</returns>
			//---------------------------------------------------------------------------------------------------------
			public Single GetY()
			{
				Single y = mOnwerTable.mBoundsRect.Y;

				for (Int32 i = 0; i < mIndex; i++)
				{
					y += mOnwerTable.Rows[i].Height;
				}

				return (y);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перестройка всех ячеек строки
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void RebuildRow()
			{
				for (Int32 i = 0; i < mCells.Count; i++)
				{
					mCells[i].RebuildCell();
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Подготовка строки к модификации
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void PrepareToModify()
			{
				for (Int32 i = 0; i < mCells.Count; i++)
				{
					mCells[i].PrepareToModify();
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка режима строки
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void SetRowMode()
			{
				switch (mRowMode)
				{
					case TCadRowModeType.Simple:
						{
							// Обычная строка деленная на ячейки
							for (Int32 i = 0; i < mCells.Count; i++)
							{
								mCells[i].RowSpan = 1;
								mCells[i].ColumnSpan = 1;
							}
						}
						break;
					case TCadRowModeType.Header:
						{
							// Все ячейки объединены, и первая выровненная по середине
							mCells[0].ColumnSpan = mCells[0].ColumnSpanMaximum;
							mCells[0].RowSpan = 1;
						}
						break;
					case TCadRowModeType.HeaderGroup:
						break;
					default:
						break;
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
		/// <summary>
		/// Столбец таблицы
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CCadTableColumn : INotifyPropertyChanged, IComparer<CCadTableRow>
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			protected static PropertyChangedEventArgs PropertyArgsIndex = new PropertyChangedEventArgs(nameof(Index));
			protected static PropertyChangedEventArgs PropertyArgsWidth = new PropertyChangedEventArgs(nameof(Width));
			protected static PropertyChangedEventArgs PropertyArgsColumnData = new PropertyChangedEventArgs(nameof(ColumnData));
			protected static PropertyChangedEventArgs PropertyArgsName = new PropertyChangedEventArgs(nameof(Name));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			internal CCadShapeTable mOnwerTable;
			internal Int32 mIndex = 0;
			internal Single mWidth = 30;
			internal TCadColumnDataType mColumnData;
			internal String mName;
			internal Boolean mVisible = true;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Индекс столбца по порядку
			/// </summary>
			[Browsable(false)]
			public Int32 Index
			{
				get { return mIndex; }
				set
				{
					if (mIndex != value)
					{
						mIndex = value;
						NotifyPropertyChanged(PropertyArgsIndex);
					}
				}
			}

			/// <summary>
			/// Ширина столбца
			/// </summary>
			[DisplayName("Ширина")]
			[Description("Ширина столбца")]
			[Category(XInspectorGroupDesc.Params)]
			//[Display(GroupName = XInspectorGroupDesc.Params, Order = 0)]
			//[Telerik.Windows.Controls.Data.PropertyGrid.Editor(typeof(Telerik.Windows.Controls.RadNumericUpDown), "Value")]
			public Single Width
			{
				get { return mWidth; }
				set
				{
					if (mWidth != value)
					{
						mWidth = value;
						NotifyPropertyChanged(PropertyArgsWidth);
						mOnwerTable.Rebuild();

						if (mOnwerTable.IsCanvas)
						{
							//mCanvas.Update();
						}
					}
				}
			}

			/// <summary>
			/// Тип данных в столбце
			/// </summary>
			[DisplayName("Тип данных")]
			[Description("Тип данных в столбце")]
			[Category(XInspectorGroupDesc.Params)]
			//[Display(GroupName = XInspectorGroupDesc.Params, Order = 1)]
			public TCadColumnDataType ColumnData
			{
				get { return mColumnData; }
				set
				{
					if (mColumnData != value)
					{
						mColumnData = value;
						NotifyPropertyChanged(PropertyArgsColumnData);
						mOnwerTable.Rebuild();

						if (mOnwerTable.IsCanvas)
						{
							//mCanvas.Update();
						}
					}
				}
			}

			/// <summary>
			/// Имя столбца
			/// </summary>
			[DisplayName("Имя")]
			[Description("Имя столбца")]
			[Category(XInspectorGroupDesc.Params)]
			//[Display(GroupName = XInspectorGroupDesc.Params, Order = 2)]
			public String Name
			{
				get { return mName; }
				set
				{
					if (mName != value)
					{
						mName = value;
						NotifyPropertyChanged(PropertyArgsColumnData);
					}
				}
			}

			/// <summary>
			/// Видимость столбца
			/// </summary>
			[DisplayName("Видимость")]
			[Description("Видимость столбца")]
			[Category(XInspectorGroupDesc.Params)]
			//[Display(GroupName = XInspectorGroupDesc.Params, Order = 3)]
			public Boolean Visible
			{
				get { return mVisible; }
				set
				{
					if (mVisible != value)
					{
						mVisible = value;
						mOnwerTable.Rebuild();

						if (mOnwerTable.IsCanvas)
						{
							//mCanvas.Update();
						}
					}
				}
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор создает столбец по индексу
			/// </summary>
			/// <param name="table">Таблица</param>
			/// <param name="index">Индекс</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadTableColumn(CCadShapeTable table, Int32 index = 0)
			{
				mOnwerTable = table;
				mIndex = index;
			}
			#endregion

			#region ======================================= ИНДЕКСАТОР ================================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Индексация ячеек столбца
			/// </summary>
			/// <param name="index">Индекс ячейки (строки)</param>
			/// <returns>Ячейка</returns>
			//---------------------------------------------------------------------------------------------------------
			public CCadTableCell this[Int32 index]
			{
				get
				{
					return (mOnwerTable[index, mIndex]);
				}
			}
			#endregion

			#region ======================================= СИСТЕМНЫЕ МЕТОДЫ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Преобразование к текстовому представлению
			/// </summary>
			/// <returns>Наименование графического элемента</returns>
			//---------------------------------------------------------------------------------------------------------
			public override String ToString()
			{
				if (!String.IsNullOrEmpty(mName))
				{
					return (mName + " " + mIndex.ToString());
				}
				else
				{
					return ("Столбец " + mIndex.ToString());
				}
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение координаты X столбца
			/// </summary>
			/// <returns>Суммарная координата по X</returns>
			//---------------------------------------------------------------------------------------------------------
			public Single GetX()
			{
				Single x = mOnwerTable.mBoundsRect.X;

				for (Int32 i = 0; i < mIndex; i++)
				{
					x += mOnwerTable.Columns[i].Width;
				}

				return (x);
			}
			#endregion

			#region ======================================= МЕТОДЫ СОРТИРОВКИ =========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Сравнение строк
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			/// <returns></returns>
			//---------------------------------------------------------------------------------------------------------
			public Int32 Compare(CCadTableRow x, CCadTableRow y)
			{
				return (String.Compare(x[this.Index].Data, y[this.Index].Data));
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
		/// <summary>
		/// Таблица
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		[TypeConverter(typeof(CCadShapeTableConverter))]
		public class CCadShapeTable : CCadShapeBase
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			// Основные параметры
			protected static PropertyChangedEventArgs PropertyArgsSelectedCell = new PropertyChangedEventArgs(nameof(SelectedCell));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			// Внутренние данные
			internal List<CCadTableColumn> mColumns;
			internal List<CCadTableRow> mRows;

			// Ячейки
			internal List<CCadTableCell> mSelectedCells;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			//
			// РАЗМЕРЫ И ПОЗИЦИЯ
			//
			/// <summary>
			/// Позиция геометрического центра ограничивающего прямоугольника графической фигуры
			/// </summary>
			[DisplayName("Позиция")]
			[Description("Позиция")]
			[Category(XInspectorGroupDesc.Size)]
			//[Display(GroupName = XInspectorGroupDesc.Size, Order = 9)]
			//[Telerik.Windows.Controls.Data.PropertyGrid.Editor(typeof(EditorVector2DTelerik), "Value")]
			public override Vector2Df Location
			{
				get { return (mBoundsRect.PointTopLeft); }
				set
				{
					mBoundsRect.PointTopLeft = value;
					Rebuild();

					//XManager.Presenter.Update();
				}
			}

			//
			// ОСНОВНЫЕ ПАРАМЕТРЫ
			//
			/// <summary>
			/// Список всех стобцов
			/// </summary>
			[DisplayName("Столбцы")]
			[Description("Список всех стобцов")]
			[Category(XInspectorGroupDesc.Params)]
			//[Display(GroupName = XInspectorGroupDesc.Params, Order = 0)]
			public List<CCadTableColumn> Columns
			{
				get { return (mColumns); }
			}

			/// <summary>
			/// Количество столбцов
			/// </summary>
			[DisplayName("Кол-во столбцов")]
			[Description("Количество столбцов")]
			[Category(XInspectorGroupDesc.Params)]
			//[Display(GroupName = XInspectorGroupDesc.Params, Order = 1)]
			public Int32 ColumnCount
			{
				get { return (mColumns.Count); }
			}

			/// <summary>
			/// Список всех строк
			/// </summary>
			[DisplayName("Строки")]
			[Description("Список всех строк")]
			[Category(XInspectorGroupDesc.Params)]
			//[Display(GroupName = XInspectorGroupDesc.Params, Order = 2)]
			public List<CCadTableRow> Rows
			{
				get { return (mRows); }
			}

			/// <summary>
			/// Количество строк
			/// </summary>
			[DisplayName("Кол-во строк")]
			[Description("Количество строк")]
			[Category(XInspectorGroupDesc.Params)]
			//[Display(GroupName = XInspectorGroupDesc.Params, Order = 3)]
			public Int32 RowCount
			{
				get { return (mRows.Count); }
			}

			[DisplayName("Выбранная ячейка")]
			[Description("Текущая выбранная ячейка")]
			[Category(XInspectorGroupDesc.Params)]
			//[Display(GroupName = XInspectorGroupDesc.Params, Order = 4)]
			//[Telerik.Windows.Controls.Data.PropertyGrid.Editor(typeof(EditorCadTableCellTelerik), "Value", Telerik.Windows.Controls.Data.PropertyGrid.EditorStyle.DropDown)]
			public CCadTableCell SelectedCell
			{
				get { return (mSelectedCells[0]); }
				set
				{
					// TODO Только для работы редактора
					mSelectedCells[0] = value;
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
				get { return (4); }
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Создание пустой таблицы
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public CCadShapeTable()
			{
				for (Int32 i = 0; i < 4; i++)
				{
					mHandleRects.Add(Rect2Df.Empty);
				}

				this.Init();

				// 1) Сначала добавляем столбцы
				this.AddColumns(4);

				// 1) Потом добавляем строки
				this.AddRows(4);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Создание таблицы с указанными количеством строк и столбцов
			/// </summary>
			/// <param name="column_count">Кол-во строк</param>
			/// <param name="row_count">Кол-во столбцов</param>
			/// <param name="name">Имя таблицы</param>
			/// <param name="group">Группа</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadShapeTable(Int32 column_count, Int32 row_count, String name, String group = "Без значения")
			{
				for (Int32 i = 0; i < 4; i++)
				{
					mHandleRects.Add(Rect2Df.Empty);
				}

				this.Init();

				// 1) Сначала добавляем столбцы
				this.AddColumns(column_count);

				// 1) Потом добавляем строки
				this.AddRows(row_count);

				mName = name;
				mGroup = group;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор копирования
			/// </summary>
			/// <param name="source">Источник копии</param>
			/// <param name="add_to_draft">Добавлять ли в чертеж источника</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadShapeTable(CCadShapeTable source, Boolean add_to_draft = true)
				: base(source, add_to_draft)
			{
				for (Int32 i = 0; i < 4; i++)
				{
					mHandleRects.Add(Rect2Df.Empty);
				}

				this.Init();

				// 1) Сначала добавляем столбцы
				this.AddColumns(source.ColumnCount);

				// 1) Потом добавляем строки
				this.AddRows(source.RowCount);
			}
			#endregion

			#region ======================================= ИНДЕКСАТОР ================================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Индексация ячеек
			/// </summary>
			/// <param name="ir">Индекс строки - Y</param>
			/// <param name="ic">Индекс столбца - X</param>
			/// <returns>Ячейка</returns>
			//---------------------------------------------------------------------------------------------------------
			public CCadTableCell this[Int32 ir, Int32 ic]
			{
				get { return (mRows[ir][ic]); }
			}
			#endregion

			#region ======================================= СИСТЕМНЫЕ МЕТОДЫ ==========================================
			////---------------------------------------------------------------------------------------------------------
			///// <summary>
			///// Копирование текста
			///// </summary>
			///// <returns>Дубликат текста со всеми параметрами и данными</returns>
			////---------------------------------------------------------------------------------------------------------
			//public override IBaseElement Duplicate()
			//{
			//	return (new CCadShapeTable(this, false));
			//}
			#endregion

			#region ======================================= СЛУЖЕБНЫЕ МЕТОДЫ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Очитска выбранных ячеек
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			private void ClearSelectedCells()
			{
				for (Int32 i = 0; i < mSelectedCells.Count; i++)
				{
					CCadTableCell cell = mSelectedCells[i];

					// Убираем статус выбраной ячейки
					if (cell.CellData == TCadCellDataType.Formula)
					{
						//rect.Fill = Brushes.Gold;
					}
					else
					{
						//rect.Fill = Brushes.White;
					}
				}

				mSelectedCells.Clear();
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Иннициализация внутрених данных
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void Init()
			{
				mColumns = new List<CCadTableColumn>();
				mRows = new List<CCadTableRow>();
				mSelectedCells = new List<CCadTableCell>();
				mIsCanvas = true;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Иннициализация модели данных
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void InitData()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавление указанного количества строк в конец таблицы
			/// </summary>
			/// <param name="row_count">Кол-во строк</param>
			//---------------------------------------------------------------------------------------------------------
			public void AddRows(Int32 row_count)
			{
				for (Int32 i = 0; i < row_count; i++)
				{
					CCadTableRow row = new CCadTableRow(this, mRows.Count);
					mRows.Add(row);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вставка строки сверху
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void InsertRowUp()
			{
				Int32 index = 0;
				if (mSelectedCells.Count != 0)
				{
					index = mSelectedCells[0].RowIndex;
				}

				this.InsertRow(index, 1);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вставка строки снизу
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void InsertRowDown()
			{
				Int32 index = 0;
				if (mSelectedCells.Count != 0)
				{
					index = mSelectedCells[0].RowIndex;
				}

				this.InsertRow(index + 1, 1);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Удаление строки
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void RemoveRow()
			{
				Int32 index = 0;
				if (mSelectedCells.Count != 0)
				{
					index = mSelectedCells[0].RowIndex;
				}

				this.RemoveRow(index);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вставка указанного количества строк в определенную позицию в таблице
			/// </summary>
			/// <param name="index">Позиция после которой будет производится вставка</param>
			/// <param name="row_count">Кол-во строк</param>
			//---------------------------------------------------------------------------------------------------------
			public void InsertRow(Int32 index, Int32 row_count)
			{
				// Подготовка к сохранению
				this.PrepareToModify();

				// Вставляем строки
				for (Int32 i = 0; i < row_count; i++)
				{
					CCadTableRow row = new CCadTableRow(this, mRows.Count);
					mRows.Insert(index + i, row);
				}

				// Пересобираем индексы
				for (Int32 ix = 0; ix < mRows.Count; ix++)
				{
					mRows[ix].Index = ix;
				}

				// Oбновляем
				for (Int32 ix = 0; ix < mRows.Count; ix++)
				{
					for (int ic = 0; ic < mColumns.Count; ic++)
					{
						this[ix, ic].UpdateRowSpan();
					}
				}

				// Обновляем таблицу
				this.Rebuild();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Удаление строки
			/// </summary>
			/// <param name="index">Индекс удаляемой строки</param>
			//---------------------------------------------------------------------------------------------------------
			public void RemoveRow(Int32 index)
			{
				// Подготовка к сохранению
				this.PrepareToModify();

				// Удаляем строки c канвы
				CCadTableRow table_row = mRows[index];

				// Смотрим, есть ли такая ячейка которой принадлежин данная строка
				for (Int32 ix = 0; ix < table_row.mCells.Count; ix++)
				{
					// Перекладываем ее обязаности на нижнию строку
					if (table_row.mCells[ix].RowSpan > 1)
					{
						Int32 span = table_row.mCells[ix].RowSpan - 1;
						table_row.mCells[ix].RowSpan = 1;
						table_row.mCells[ix].AdjacentBottomCell.RowSpan = span;
					}
				}

				// Удаляем со списка строк
				mRows.RemoveAt(index);

				// Пересобираем индексы
				for (Int32 ix = 0; ix < mRows.Count; ix++)
				{
					mRows[ix].Index = ix;
				}

				// Обновляем таблицу
				this.Rebuild();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Добавление указанного количества столбцов в конец таблицы.
			/// </summary>
			/// <param name="column_count">Кол-во столбцов</param>
			//---------------------------------------------------------------------------------------------------------
			public void AddColumns(Int32 column_count)
			{
				for (Int32 i = 0; i < column_count; i++)
				{
					CCadTableColumn column = new CCadTableColumn(this, mColumns.Count);
					mColumns.Add(column);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вставка столбца слева
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void InsertColumnsLeft()
			{
				Int32 index = 0;
				if (mSelectedCells.Count != 0)
				{
					index = mSelectedCells[0].ColumnIndex;
				}

				this.InsertColumns(index, 1);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вставка столбца справа
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void InsertColumnsRight()
			{
				Int32 index = 0;
				if (mSelectedCells.Count != 0)
				{
					index = mSelectedCells[0].ColumnIndex;
				}

				this.InsertColumns(index + 1, 1);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Удаление столбца
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void RemoveColumns()
			{
				Int32 index = 0;
				if (mSelectedCells.Count != 0)
				{
					index = mSelectedCells[0].ColumnIndex;
				}

				this.RemoveColumns(index);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вставка указанного количества столбцов в определенную позицию в таблице
			/// </summary>
			/// <param name="index">Позиция после которой будет производится вставка</param>
			/// <param name="column_count">Кол-во столбцов</param>
			//---------------------------------------------------------------------------------------------------------
			public void InsertColumns(Int32 index, Int32 column_count)
			{
				// Подготовка к сохранению
				this.PrepareToModify();

				// Вставляем столбцы
				for (Int32 i = 0; i < column_count; i++)
				{
					CCadTableColumn column = new CCadTableColumn(this);
					mColumns.Insert(index + i, column);

					for (Int32 ir = 0; ir < mRows.Count; ir++)
					{
						mRows[ir].mCells.Insert(index + i, new CCadTableCell(mRows[ir], column));
					}
				}

				// Пересобираем индексы
				for (Int32 ic = 0; ic < mColumns.Count; ic++)
				{
					mColumns[ic].Index = ic;
				}

				// Обновляем
				for (Int32 ix = 0; ix < mRows.Count; ix++)
				{
					for (int ic = 0; ic < mColumns.Count; ic++)
					{
						this[ix, ic].UpdateColumnSpan();
					}
				}

				// Обновляем таблицу
				this.Rebuild();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Удаление столбца
			/// </summary>
			/// <param name="index">Индекс удаляемого столбца</param>
			//---------------------------------------------------------------------------------------------------------
			public void RemoveColumns(Int32 index)
			{
				// Подготовка к сохранению
				this.PrepareToModify();

				// Удаляем строки c канвы
				CCadTableColumn table_column = mColumns[index];

				// Смотрим, есть ли такая ячейка которой принадлежин данная столбец
				for (Int32 i = 0; i < RowCount; i++)
				{
					Int32 span = this[i, table_column.Index].ColumnSpan;
					if (span > 1)
					{
						span = span - 1;
						this[i, table_column.Index].ColumnSpan = 1;
						this[i, table_column.Index].AdjacentRightCell.ColumnSpan = span;
					}
				}

				// Удаляем со списка столбцов
				mColumns.RemoveAt(index);

				// Пересобираем индексы
				for (Int32 ic = 0; ic < mColumns.Count; ic++)
				{
					mColumns[ic].Index = ic;
				}

				// Обновляем таблицу
				this.Rebuild();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перестройка всего макета таблицы
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void Rebuild()
			{
				for (Int32 ix = 0; ix < mRows.Count; ix++)
				{
					mRows[ix].RebuildRow();
				}

				Single height = 0;
				for (Int32 i = 0; i < mRows.Count; i++)
				{
					height += mRows[i].Height;
				}

				Single width = 0;
				for (Int32 i = 0; i < mColumns.Count; i++)
				{
					width += mColumns[i].Width;
				}

				mBoundsRect.Height = height;
				mBoundsRect.Width = width;

				SetHandleRects();

				if (IsCanvas)
				{
					//mCanvas.Update();
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Подготовка таблицы к модификации
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public void PrepareToModify()
			{
				for (Int32 i = 0; i < mRows.Count; i++)
				{
					mRows[i].PrepareToModify();
				}
			}
			#endregion

			#region ======================================= МЕТОДЫ ТРАНСФОРМАЦИИ ======================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перемещение графической фигуры
			/// </summary>
			/// <param name="offset">Значение смещения в условных единицах</param>
			//---------------------------------------------------------------------------------------------------------
			public override void Move(ref Vector2Df offset)
			{
				mBoundsRect.X += offset.X;
				mBoundsRect.Y += offset.Y;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перемещение графической фигуры вверх
			/// </summary>
			/// <param name="offset">Значение смещения в условных единицах</param>
			//---------------------------------------------------------------------------------------------------------
			public override void MoveUp(Single offset)
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перемещение графической фигуры вниз
			/// </summary>
			/// <param name="offset">Значение смещения в условных единицах</param>
			//---------------------------------------------------------------------------------------------------------
			public override void MoveDown(Single offset)
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перемещение графической фигуры влево
			/// </summary>
			/// <param name="offset">Значение смещения в условных единицах</param>
			//---------------------------------------------------------------------------------------------------------
			public override void MoveLeft(Single offset)
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перемещение графической фигуры вправо
			/// </summary>
			/// <param name="offset">Значение смещения в условных единицах</param>
			//---------------------------------------------------------------------------------------------------------
			public override void MoveRight(Single offset)
			{
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
				if(mBoundsRect.Contains(ref point))
				{
					mHandleIndex = 0;
				}
				else
				{
					mHandleIndex = -1;
				}

				mSelectedCells.Clear();

				for (Int32 ir = 0; ir < mRows.Count; ir++)
				{
					CCadTableRow row = mRows[ir];

					for (Int32 ic = 0; ic < row.mCells.Count; ic++)
					{
						CCadTableCell cell = row.mCells[ic];
						if (cell.IsBlocked == false && cell.IsVisible)
						{
							if (cell.mBoundsRect.Contains(ref point))
							{
								mSelectedCells.Add(cell);
							}
						}
					}
				}

				NotifyPropertyChanged(PropertyArgsSelectedCell);

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
				if(mHandleIndex == 0)
				{
					mBoundsRect.X += offset.X;
					mBoundsRect.Y += offset.Y;
					Rebuild();
				}

				// Обновляем ручки
				SetHandleRects();

				// Обновляем канву
				//mCanvas.Update();
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
				Vector2Df offset = mCanvasViewer.MouseDelta;

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
				mHandleIndex = -1;
				SetHandleCursor();

				// Обновляем канву
				//mCanvas.Update();
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
				mHandleRects[0] = mCanvas.GetHandleRect(mBoundsRect.PointTopLeft);
				mHandleRects[1] = mCanvas.GetHandleRect(mBoundsRect.PointTopRight);
				mHandleRects[2] = mCanvas.GetHandleRect(mBoundsRect.PointBottomRight);
				mHandleRects[3] = mCanvas.GetHandleRect(mBoundsRect.PointBottomLeft);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка соответствующего курсора на ручкой
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void SetHandleCursor()
			{
				//if (mHandleIndex == -1)
				//{
				//	mCanvas.SetCursor(TCursor.Arrow);
				//}
				//else
				//{
				//	if (mCanvasViewer.Selecting.EditModeMoving || mHandleIndex == 1)
				//	{
				//		mCanvas.SetCursor(TCursor.SizeAll);
				//	}
				//	else
				//	{
				//		mCanvas.SetCursor(TCursor.Cross);
				//	}
				//}
			}
			#endregion

			#region ======================================= МЕТОДЫ РИСОВАНИЯ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Рисование изображения
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void Draw()
			{
				for (Int32 ir = 0; ir < mRows.Count; ir++)
				{
					CCadTableRow row = mRows[ir];

					for (Int32 ic = 0; ic < row.mCells.Count; ic++)
					{
						CCadTableCell cell = row.mCells[ic];
						if(cell.IsBlocked == false && cell.IsVisible)
						{
							mCanvas.DrawTextID(cell.ID, cell.mPoint);

							Vector2Df lt = cell.mBoundsRect.PointTopLeft;
							Vector2Df lb = cell.mBoundsRect.PointBottomLeft;
							Vector2Df rt = cell.mBoundsRect.PointTopRight;
							Vector2Df rb = cell.mBoundsRect.PointBottomRight;

							if (cell.IsBorderLeft)
							{
								mCanvas.DrawLine(ref lt, ref lb, mStrokePen);
							}

							if (cell.IsBorderRight)
							{
								mCanvas.DrawLine(ref rt, ref rb, mStrokePen);
							}

							if (cell.IsBorderTop)
							{
								mCanvas.DrawLine(ref lt, ref rt, mStrokePen);
							}

							if (cell.IsBorderBottom)
							{
								mCanvas.DrawLine(ref lb, ref rb, mStrokePen);
							}
						}
					}
				}

				if (mIsSelect)
				{
					for (Int32 i = 0; i < HandleCount; i++)
					{
						mCanvas.DrawHandleRect(mHandleRects[i], mHandleIndex == i);
					}

					for (Int32 i = 0; i < mSelectedCells.Count; i++)
					{
						CCadTableCell cell = mSelectedCells[i];
						mCanvas.DrawRectangle(ref cell.mBoundsRect, XCadBrushManager.DarkGray);

						mCanvas.DrawTextID(cell.ID, cell.mPoint);
					}
				}
			}
			#endregion

			#region ======================================= ОБРАБОТЧИКИ СОБЫТИЙ =======================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Подготовка элемента перед сохранением
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void OnBeforeSave()
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Восстановление параметров элемента после сохранения
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void OnAfterSave()
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Подготовка элемента перед загрузкой
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void OnBeforeLoad()
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Восстановление параметров элемента после загрузки
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void OnAfterLoad()
			{
				base.OnAfterLoad();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Подготовка элемента перед печатью
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void OnBeforePrinting()
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Восстановление параметров элемента после печати
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void OnAfterPrinting()
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение масштаба представления текущего элемента
			/// </summary>
			/// <param name="scale">Масштаб представления</param>
			//---------------------------------------------------------------------------------------------------------
			public override void OnScaleChanged(Double scale)
			{

			}
			#endregion

			#region ======================================= МЕТОДЫ СЕРИАЛИЗАЦИИ =======================================
			////---------------------------------------------------------------------------------------------------------
			///// <summary>
			///// Запись свойств и данных графической фигуры в бинарный поток
			///// </summary>
			///// <param name="binary_writer">Бинарный поток открытый для записи</param>
			////---------------------------------------------------------------------------------------------------------
			//public override void WriteToStream(BinaryWriter binary_writer)
			//{

			//}

			////---------------------------------------------------------------------------------------------------------
			///// <summary>
			///// Запись свойств и данных графической фигуры в формат данных XML
			///// </summary>
			///// <param name="xml_writer">Средство записи данных в формат XML</param>
			////---------------------------------------------------------------------------------------------------------
			//public override void WriteToXml(XmlWriter xml_writer)
			//{
			//	xml_writer.WriteStartElement("CadShapeImage");

			//	WriteBaseElementToAttribute(xml_writer);
			//	WriteShapeToAttribute(xml_writer);

			//	//mPrimitive.WritePrimitivToAttribute("", xml_writer);
			//	//mTransform.WriteTransformableToAttribute("", xml_writer);

			//	xml_writer.WriteEndElement();
			//}

			////---------------------------------------------------------------------------------------------------------
			///// <summary>
			///// Чтение свойств и данных графической фигуры из бинарного потока
			///// </summary>
			///// <param name="binary_reader">Бинарный поток открытый для чтения</param>
			////---------------------------------------------------------------------------------------------------------
			//public override void ReadFromStream(BinaryReader binary_reader)
			//{

			//}

			////---------------------------------------------------------------------------------------------------------
			///// <summary>
			///// Чтение свойств и данных графической фигуры из потока данных в формате XML
			///// </summary>
			///// <param name="xml_reader">Средство чтения данных формата XML</param>
			////---------------------------------------------------------------------------------------------------------
			//public override void ReadFromXml(XmlReader xml_reader)
			//{
			//	// Читаем базовые данные
			//	ReadBaseElementFromAttribute(xml_reader);
			//	ReadShapeFromAttribute(xml_reader);

			//	//mPrimitive.ReadPrimitivFromAttribute("", xml_reader);
			//	//mTransform.ReadTransformableFromAttribute("", xml_reader);
			//}
			#endregion
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Конвертер типа таблицы для предоставления свойств
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CCadShapeTableConverter : TypeConverter
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
//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Графические примитивы
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADPrimitive.cs
*		Базовый класс для графического примитива.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 27.03.2022
//=====================================================================================================================
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
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
		//! \defgroup CadPrimitivs Графические примитивы
		//! Графических элементов являются платформо-зависимыми реализациями отображения базовых графических примитивов
		//! \ingroup CadDrawing
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Базовый графический примитив
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CCadPrimitive : IComparable<CCadPrimitive>
		{
			#region ======================================= ДАННЫЕ ====================================================
			// Основные параметры
			internal Int32 mZIndex = 0;
			internal Boolean mIsCanvas = true;
			internal Rect2Df mBoundsRect;
			internal Boolean mIsStroked;
			internal Boolean mIsFilled;
			internal CCadPen mStroke;
			internal CCadBrush mFill;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			//
			// ОСНОВНЫЕ ПАРАМЕТРЫ
			//
			/// <summary>
			/// Позиция графического примитива в Z плоскости
			/// </summary>
			public Int32 ZIndex
			{
				get { return (mZIndex); }
				set
				{
					if (mZIndex != value)
					{
						mZIndex = value;

						// Обновляем расположение
						//mCanvas.SortByZIndex();

						// Обновляем отображение примитива
						//XCadManager.Canvas.Update();
					}
				}
			}

			/// <summary>
			/// Статус расположения графического примитива на канве
			/// </summary>
			/// <remarks>
			/// Если графический примитив расположен на канве то любые изменения его свойств ведут к обновлению канвы
			/// </remarks>
			public Boolean IsCanvas
			{
				get { return (mIsCanvas); }
				set
				{
					if (mIsCanvas != value)
					{
						mIsCanvas = value;
						
						if(mIsCanvas)
						{
							// Обновляем данные
							Update();

							// Обновляем отображение примитива
							//XCadManager.Canvas.Update();
						}
					}
				}
			}

			/// <summary>
			/// Ограничивающий прямоугольник геометрии примитива
			/// </summary>
			public Rect2Df BoundsRect
			{
				get { return (mBoundsRect); }
			}

			/// <summary>
			/// Статус отображения контура примитива
			/// </summary>
			public Boolean IsStroked
			{
				get { return (mIsStroked); }
				set
				{
					if (mIsStroked != value)
					{
						mIsStroked = value;

						if (mIsCanvas)
						{
							// Обновляем данные
							UpdateIsStroked();

							// Обновляем отображение примитива
							//XCadManager.Canvas.Update();
						}
					}
				}
			}

			/// <summary>
			/// Статус отображения заливки примитива
			/// </summary>
			public Boolean IsFilled
			{
				get { return (mIsFilled); }
				set
				{
					if (mIsFilled != value)
					{
						mIsFilled = value;

						if (mIsCanvas)
						{
							// Обновляем данные
							UpdateIsFilled();

							// Обновляем отображение примитива
							//XCadManager.Canvas.Update();
						}
					}
				}
			}

			/// <summary>
			/// Перо для отображения контура примитива
			/// </summary>
			public CCadPen Stroke
			{
				get { return (mStroke); }
				set
				{
					if (mStroke != value)
					{
						mStroke = value;

						if (mIsCanvas)
						{
							// Обновляем отображение примитива
							//XCadManager.Canvas.Update();
						}
					}
				}
			}

			/// <summary>
			/// Кисть для отображения заливки примитива
			/// </summary>
			public CCadBrush Fill
			{
				get { return (mFill); }
				set
				{
					if (mFill != value)
					{
						mFill = value;

						if (mIsCanvas)
						{
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
			public CCadPrimitive()
			{
				mIsStroked = true;
				mIsFilled = false;
				mStroke = XCadPenManager.DefaultPen;
				mFill = XCadBrushManager.DefaultBrush;
			}
			#endregion

			#region ======================================= СИСТЕМНЫЕ МЕТОДЫ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Сравнение графических примитивов для упорядочивания (по Z индексу)
			/// </summary>
			/// <param name="other">Сравниваемый графический примитив</param>
			/// <returns>Статус сравнения графический примитивов</returns>
			//---------------------------------------------------------------------------------------------------------
			public Int32 CompareTo(CCadPrimitive other)
			{
				if (mZIndex > other.ZIndex)
				{
					return (1);
				}
				else
				{
					if (mZIndex < other.ZIndex)
					{
						return (-1);
					}
					else
					{
						return (0);
					}
				}
			}
			
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение хэш кода объекта
			/// </summary>
			/// <returns>Стандартный хэш код объекта</returns>
			//---------------------------------------------------------------------------------------------------------
			public override Int32 GetHashCode()
			{
				return (base.GetHashCode());
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление данных примитива
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public virtual void Update()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление данных отображения контура примитива
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public virtual void UpdateIsStroked()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление данных отображения заливки примитива
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public virtual void UpdateIsFilled()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Копирование графического примитива
			/// </summary>
			/// <returns>Дубликат графического примитива со всеми параметрами и данными</returns>
			//---------------------------------------------------------------------------------------------------------
			public virtual CCadPrimitive Duplicate()
			{
				return (null);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Копирование параметров с графического примитива
			/// </summary>
			/// <param name="primitiv">Графический примитив</param>
			//---------------------------------------------------------------------------------------------------------
			public virtual void CopyParamemtrs(CCadPrimitive primitiv)
			{
				mBoundsRect = primitiv.mBoundsRect;
				mZIndex = primitiv.mZIndex;
				mIsStroked = primitiv.mIsStroked;
				mIsFilled = primitiv.mIsFilled;
				mStroke = primitiv.mStroke;
				mFill = primitiv.mFill;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка расположения точки на контуре графического примитива
			/// </summary>
			/// <param name="point">Проверяемая точка</param>
			/// <param name="thickness">Толщина контура</param>
			/// <returns>Статус расположения</returns>
			//---------------------------------------------------------------------------------------------------------
			public virtual Boolean StrokeContains(ref Vector2Df point, Single thickness)
			{
				return (false);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка расположения точки внутри области графического примитива
			/// </summary>
			/// <param name="point">Проверяемая точка</param>
			/// <returns>Статус расположения</returns>
			//---------------------------------------------------------------------------------------------------------
			public virtual Boolean FillContains(ref Vector2Df point)
			{
				return (false);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вычисление площади графического примитива
			/// </summary>
			/// <returns>Площадь графического примитива</returns>
			//---------------------------------------------------------------------------------------------------------
			public virtual Double ComputeArea()
			{
				return (0);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Вычисление ограничивающего прямоугольника графического примитива
			/// </summary>
			/// <param name="dest_bounds_rect">Прямоугольник</param>
			//---------------------------------------------------------------------------------------------------------
			public virtual void ComputeBounds(ref Rect2Df dest_bounds_rect)
			{

			}
			#endregion

			#region ======================================= МЕТОДЫ РИСОВАНИЯ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Рисование графического примитива
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public virtual void Draw()
			{
			}
			#endregion

			#region ======================================= МЕТОДЫ СЕРИАЛИЗАЦИИ =======================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Запись свойств и данных графического примитива в формат атрибутов XML
			/// </summary>
			/// <param name="prefix">Префикс имени атрибута</param>
			/// <param name="xml_writer">Средство записи данных в формат XML</param>
			//---------------------------------------------------------------------------------------------------------
			public virtual void WritePrimitivToAttribute(String prefix, XmlWriter xml_writer)
			{
				//xml_writer.WriteRect2DToAttribute(prefix + "BoundsRect", mBoundsRect);
				xml_writer.WriteIntegerToAttribute(prefix + "ZIndex", mZIndex);
				xml_writer.WriteBooleanToAttribute(prefix + "IsStroked", mIsStroked);
				xml_writer.WriteBooleanToAttribute(prefix + "IsFilled", mIsFilled);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Чтение свойств и данных графического примитива из формата атрибутов XML
			/// </summary>
			/// <param name="prefix">Префикс имени атрибута</param>
			/// <param name="xml_reader">Средство чтения данных формата XML</param>
			//---------------------------------------------------------------------------------------------------------
			public virtual void ReadPrimitivFromAttribute(String prefix, XmlReader xml_reader)
			{
				//mBoundsRect = xml_reader.ReadMathRect2DfFromAttribute(prefix + "BoundsRect");
				mZIndex = xml_reader.ReadIntegerFromAttribute(prefix + "ZIndex", mZIndex);
				mIsStroked = xml_reader.ReadBooleanFromAttribute(prefix + "IsStroked", mIsStroked);
				mIsFilled = xml_reader.ReadBooleanFromAttribute(prefix + "IsFilled", mIsFilled);
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
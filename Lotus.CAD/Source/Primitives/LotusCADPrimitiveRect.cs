//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Графические примитивы
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADPrimitiveRect.cs
*		Графический примитив прямоугольника.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.01.2022
//=====================================================================================================================
using System;
using System.Xml;
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
		/// Графический примитив прямоугольника
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CCadPrimitiveRect : CCadPrimitive
		{
			#region ======================================= ДАННЫЕ ====================================================
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Позиция верхнего левого угла прямоугольника
			/// </summary>
			public Vector2Df Location
			{
				get { return (mBoundsRect.PointTopLeft); }
				set
				{
					mBoundsRect.PointTopLeft = value;

					if (mIsCanvas)
					{
						// Обновляем данные
						Update();

						// Обновляем отображение примитива
						//XCadManager.Canvas.Update();
					}
				}
			}

			/// <summary>
			/// Ширина прямоугольника
			/// </summary>
			public Single Width
			{
				get { return (mBoundsRect.Width); }
				set
				{
					mBoundsRect.Width = value;

					if (mIsCanvas)
					{
						// Обновляем данные
						Update();

						// Обновляем отображение примитива
						//XCadManager.Canvas.Update();
					}
				}
			}

			/// <summary>
			/// Высота прямоугольника
			/// </summary>
			public Single Height
			{
				get { return (mBoundsRect.Height); }
				set
				{
					mBoundsRect.Height = value;

					if (mIsCanvas)
					{
						// Обновляем данные
						Update();

						// Обновляем отображение примитива
						//XCadManager.Canvas.Update();
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
			public CCadPrimitiveRect()
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
			public override void WritePrimitivToAttribute(String prefix, XmlWriter xml_writer)
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
			public override void ReadPrimitivFromAttribute(String prefix, XmlReader xml_reader)
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
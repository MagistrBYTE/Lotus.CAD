//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Графические примитивы
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADPrimitiveLine.cs
*		Графический примитив линии.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 27.03.2022
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
		/// Графический примитив линии
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CCadPrimitiveLine : CCadPrimitive
		{
			#region ======================================= ДАННЫЕ ====================================================
			// Основные параметры
			internal Vector2Df mStartPoint;
			internal Vector2Df mEndPoint;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			//
			// ОСНОВНЫЕ ПАРАМЕТРЫ
			//
			/// <summary>
			/// Начальная точка
			/// </summary>
			public Vector2Df StartPoint
			{
				get { return (mStartPoint); }
				set
				{
					mStartPoint = value;

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
			/// Конечная точка
			/// </summary>
			public Vector2Df EndPoint
			{
				get { return (mEndPoint); }
				set
				{
					mEndPoint = value;

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
			public CCadPrimitiveLine()
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
				xml_writer.WriteIntegerToAttribute(prefix + "ZIndex", mZIndex);
				xml_writer.WriteBooleanToAttribute(prefix + "IsStroked", mIsStroked);
				xml_writer.WriteBooleanToAttribute(prefix + "IsFilled", mIsFilled);
				//xml_writer.WriteVector2DToAttribute(prefix + "StartPoint", mStartPoint);
				//xml_writer.WriteVector2DToAttribute(prefix + "EndPoint", mEndPoint);
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
				mZIndex = xml_reader.ReadIntegerFromAttribute(prefix + "ZIndex", mZIndex);
				mIsStroked = xml_reader.ReadBooleanFromAttribute(prefix + "IsStroked", mIsStroked);
				mIsFilled = xml_reader.ReadBooleanFromAttribute(prefix + "IsFilled", mIsFilled);
				//mStartPoint = xml_reader.ReadMathVector2DfFromAttribute(prefix + "StartPoint");
				//mEndPoint = xml_reader.ReadMathVector2DfFromAttribute(prefix + "EndPoint");
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
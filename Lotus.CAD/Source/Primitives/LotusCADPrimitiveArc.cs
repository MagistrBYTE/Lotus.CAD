//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Графические примитивы
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADPrimitiveArc.cs
*		Графический примитив дуги.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.01.2022
//=====================================================================================================================
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
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
		//! \addtogroup CadPrimitivs
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Графический примитив дуги
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CCadPrimitiveArc : CCadPrimitive
		{
			#region ======================================= ДАННЫЕ ====================================================
			// Основные параметры
			internal Vector2Df mStartPoint;
			internal Vector2Df mEndPoint;
			internal Boolean mIsLargeArc;
			internal Boolean mIsClockwiseDirection;
			internal Single mRotationAngle;
			internal Single mRadiusX;
			internal Single mRadiusY;
			internal Boolean mIsClosed;
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
					if (!Vector2Df.Approximately(ref mStartPoint, ref value, 0.01f))
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
			}

			/// <summary>
			/// Конечная точка
			/// </summary>
			public Vector2Df EndPoint
			{
				get { return (mEndPoint); }
				set
				{
					if (!Vector2Df.Approximately(ref mEndPoint, ref value, 0.01f))
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
			}

			/// <summary>
			/// Большая или малая дуга
			/// </summary>
			public Boolean IsLargeArc
			{
				get { return (mIsLargeArc); }
				set
				{
					if (mIsLargeArc != value)
					{
						mIsLargeArc = value;

						if (mIsCanvas)
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
			/// Рисование по часовой или против часовой стрелки
			/// </summary>
			public Boolean IsClockwiseDirection
			{
				get { return (mIsClockwiseDirection); }
				set
				{
					if (mIsClockwiseDirection != value)
					{
						mIsClockwiseDirection = value;

						if (mIsCanvas)
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
			/// Угол рисования дуги
			/// </summary>
			public Single RotationAngle
			{
				get { return (mRotationAngle); }
				set
				{
					if (!XMath.Approximately(mRotationAngle, value, 0.01))
					{
						mRotationAngle = value;

						if (mIsCanvas)
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
			/// Размер эллипса дуги по X
			/// </summary>
			public Single RadiusX
			{
				get { return (mRadiusX); }
				set
				{
					if (!XMath.Approximately(mRadiusX, value, 0.01))
					{
						mRadiusX = value;

						if (mIsCanvas)
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
			/// Размер эллипса дуги по Y
			/// </summary>
			public Single RadiusY
			{
				get { return (mRadiusY); }
				set
				{
					if (!XMath.Approximately(mRadiusY, value, 0.01))
					{
						mRadiusY = value;

						if (mIsCanvas)
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
			/// Статус замкнутости фигуры дуги
			/// </summary>
			public Boolean IsClosed
			{
				get { return (mIsClosed); }
				set
				{
					if (mIsClosed != value)
					{
						mIsClosed = value;

						if (mIsCanvas)
						{
							// Обновляем данные
							Update();

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
			public CCadPrimitiveArc()
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
				xml_writer.WriteBooleanToAttribute(prefix + "IsLargeArc", mIsLargeArc);
				xml_writer.WriteBooleanToAttribute(prefix + "IsClockwiseDirection", mIsClockwiseDirection);
				xml_writer.WriteSingleToAttribute(prefix + "RotationAngle", mRotationAngle);
				xml_writer.WriteSingleToAttribute(prefix + "RadiusX", mRadiusX);
				xml_writer.WriteSingleToAttribute(prefix + "RadiusY", mRadiusY);
				xml_writer.WriteBooleanToAttribute(prefix + "IsClosed", mIsClosed);
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
				mIsLargeArc = xml_reader.ReadBooleanFromAttribute(prefix + "IsLargeArc", mIsLargeArc);
				mIsClockwiseDirection = xml_reader.ReadBooleanFromAttribute(prefix + "IsClockwiseDirection", mIsClockwiseDirection);
				mRotationAngle = xml_reader.ReadSingleFromAttribute(prefix + "RotationAngle", mRotationAngle);
				mRadiusX = xml_reader.ReadSingleFromAttribute(prefix + "RadiusX", mRadiusX);
				mRadiusY = xml_reader.ReadSingleFromAttribute(prefix + "RadiusY", mRadiusY);
				mIsClosed = xml_reader.ReadBooleanFromAttribute(prefix + "IsClosed", mIsClosed);
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Графические примитивы
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADPrimitiveImage.cs
*		Графический примитив изображения.
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
		/// Тип данных изображения
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public enum TCadImageSourceType
		{
			/// <summary>
			/// Внешний файл
			/// </summary>
			File,

			/// <summary>
			/// Ресурс
			/// </summary>
			Resource
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Источник данных для изображения
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public struct TCadImageSource
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			public static TCadImageSource Empty = new TCadImageSource("Нет данных");
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			public String PathData;
			public TCadImageSourceType TypeData;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Статус скорректированного пути
			/// </summary>
			public Boolean IsCorrectPath
			{
				get
				{
					if (String.IsNullOrEmpty(PathData) || PathData == "Нет данных")
					{
						return (false);
					}

					return (true);
				}
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="path_data">Путь к данным</param>
			/// <param name="type_data">Тип данных</param>
			//---------------------------------------------------------------------------------------------------------
			public TCadImageSource(String path_data, TCadImageSourceType type_data = TCadImageSourceType.File)
			{
				PathData = path_data;
				TypeData = type_data;
			}
			#endregion
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Графический примитив изображения
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CCadPrimitiveImage : CCadPrimitiveRect
		{
			#region ======================================= ДАННЫЕ ====================================================
			// Основные параметры
			internal TCadImageSource mSource;
			internal Single mRotationAngle;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			//
			// ОСНОВНЫЕ ПАРАМЕТРЫ
			//
			/// <summary>
			/// Источник данных для изображения
			/// </summary>
			public TCadImageSource Source
			{
				get { return (mSource); }
				set
				{
					mSource = value;

					if (mIsCanvas)
					{
						// Обновляем данные
						UpdateImageSource();

						// Обновляем отображение примитива
						//XCadManager.Canvas.Update();
					}
				}
			}

			/// <summary>
			/// Путь к данным
			/// </summary>
			public String PathData
			{
				get { return (mSource.PathData); }
				set
				{
					mSource.PathData = value;

					if (mIsCanvas)
					{
						// Обновляем данные
						UpdateImageSource();

						// Обновляем отображение примитива
						//XCadManager.Canvas.Update();
					}
				}
			}

			/// <summary>
			/// Тип данных
			/// </summary>
			public TCadImageSourceType TypeData
			{
				get { return (mSource.TypeData); }
				set
				{
					mSource.TypeData = value;

					if (mIsCanvas)
					{
						// Обновляем данные
						UpdateImageSource();

						// Обновляем отображение примитива
						//XCadManager.Canvas.Update();
					}
				}
			}

			/// <summary>
			/// Угол поворота изображения в градусах
			/// </summary>
			public Single RotationAngle
			{
				get { return (mRotationAngle); }
				set
				{
					mRotationAngle = value;

					if (mIsCanvas)
					{
						// Обновляем данные
						UpdateRotationSource();

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
			public CCadPrimitiveImage()
			{
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление данных источника изображения
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public virtual void UpdateImageSource()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление данных поворота изображения
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public virtual void UpdateRotationSource()
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
				xml_writer.WriteStringToAttribute(prefix + "PathData", PathData);
				xml_writer.WriteEnumToAttribute(prefix + "TypeData", TypeData);
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
				mSource.PathData = xml_reader.ReadStringFromAttribute(prefix + "PathData", mSource.PathData);
				mSource.TypeData = xml_reader.ReadEnumFromAttribute(prefix + "TypeData", mSource.TypeData);
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
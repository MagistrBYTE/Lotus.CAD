//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Интерактивные графические фигуры
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADShapeImage.cs
*		Растровое изображения.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 04.04.2021
//=====================================================================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
		//! \addtogroup CadShape
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Растровое изображения
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		[TypeConverter(typeof(CCadShapeImageConverter))]
		public class CCadShapeImage : CCadShapeRectangular<CCadPrimitiveImage>
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			protected static PropertyChangedEventArgs PropertyArgsSource = new PropertyChangedEventArgs(nameof(Source));
			protected static PropertyChangedEventArgs PropertyArgsTypeData = new PropertyChangedEventArgs(nameof(TypeData));
			protected static PropertyChangedEventArgs PropertyArgsPathData = new PropertyChangedEventArgs(nameof(PathData));
			protected static PropertyChangedEventArgs PropertyArgsImageRotationAngle = new PropertyChangedEventArgs(nameof(ImageRotationAngle));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			//
			// ОСНОВНЫЕ ПАРАМЕТРЫ
			//
			/// <summary>
			/// Источник данных для изображения
			/// </summary>
			[DisplayName("Источник")]
			[Description("Источник данных для изображения")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 0)]
			public TCadImageSource Source
			{
				get { return (mPrimitive.mSource); }
				set
				{
					mPrimitive.Source = value;
					NotifyPropertyChanged(PropertyArgsSource);
					NotifyPropertyChanged(PropertyArgsPathData);
					NotifyPropertyChanged(PropertyArgsTypeData);
					RaiseSourceChanged();
				}
			}

			/// <summary>
			/// Путь к данным
			/// </summary>
			[DisplayName("Путь")]
			[Description("Путь к данным")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 1)]
			public String PathData
			{
				get { return (mPrimitive.PathData); }
				set
				{
					mPrimitive.PathData = value;
					NotifyPropertyChanged(PropertyArgsPathData);
					RaiseSourceChanged();
				}
			}

			/// <summary>
			/// Тип источника данных"
			/// </summary>
			[DisplayName("Тип источника")]
			[Description("Тип источника данных")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 2)]
			public TCadImageSourceType TypeData
			{
				get { return (mPrimitive.TypeData); }
				set
				{
					mPrimitive.TypeData = value;
					NotifyPropertyChanged(PropertyArgsTypeData);
					RaiseSourceChanged();
				}
			}

			/// <summary>
			/// Угол поворота изображения в градусах
			/// </summary>
			[DisplayName("Поворот")]
			[Description("Угол поворота изображения в градусах")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 3)]
			public Single ImageRotationAngle
			{
				get { return (mPrimitive.RotationAngle); }
				set
				{
					mPrimitive.RotationAngle = value;
					NotifyPropertyChanged(PropertyArgsImageRotationAngle);
					RaiseImageRotationAngleChanged();
				}
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
				get { return ("РИСУНОК"); }
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
			public CCadShapeImage()
			{
				mName = "Изображение: " + Id.ToString();
				mGroup = "Изображения";
				mPrimitive = mCanvas.CreateImage("", new Vector2Df(100, 100));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="location">Позиция изображения</param>
			/// <param name="source">Источник изображения</param>
			/// <param name="name">Имя изображения</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadShapeImage(Vector2Df location, TCadImageSource source, String name = "Изображение")
			{
				mName = name;
				mGroup = "Изображения";
				mPrimitive = mCanvas.CreateImage(source.PathData, location);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор копирования
			/// </summary>
			/// <param name="source">Источник копии</param>
			/// <param name="add_to_draft">Добавлять ли в чертеж источника</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadShapeImage(CCadShapeImage source, Boolean add_to_draft = true)
				: base(source, add_to_draft)
			{
				mPrimitive = source.mPrimitive.Duplicate() as CCadPrimitiveImage;
			}
			#endregion

			#region ======================================= СИСТЕМНЫЕ МЕТОДЫ ==========================================
			#endregion

			#region ======================================= CЛУЖЕБНЫЕ МЕТОДЫ СОБЫТИЙ ==================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение источника изображения.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseSourceChanged()
			{
				mPrimitive.Update();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение угла поворота изображения в градусах.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseImageRotationAngleChanged()
			{
				mPrimitive.Update();
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Копирование параметров с графического элемента
			/// </summary>
			/// <param name="element">Графический элемент</param>
			/// <param name="context">Контекст копирования данных</param>
			//---------------------------------------------------------------------------------------------------------
			public override void CopyParamemtrs(ICadShape element, Object context)
			{
				base.CopyParamemtrs(element, context);
				CCadShapeImage source = element as CCadShapeImage;
				if (source != null)
				{
					NotifyPropertyChanged(PropertyArgsPathData);
					NotifyPropertyChanged(PropertyArgsTypeData);
					NotifyPropertyChanged(PropertyArgsImageRotationAngle);
				}
			}
			#endregion

			#region ======================================= МЕТОДЫ ТРАНСФОРМАЦИИ ======================================
			#endregion

			#region ======================================= МЕТОДЫ УПРАВЛЕНИЯ =========================================
			#endregion

			#region ======================================= МЕТОДЫ РИСОВАНИЯ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Рисование изображения
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void Draw()
			{
				base.Draw();
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
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Запись свойств и данных графической фигуры в бинарный поток
			/// </summary>
			/// <param name="binary_writer">Бинарный поток открытый для записи</param>
			//---------------------------------------------------------------------------------------------------------
			public override void WriteToStream(BinaryWriter binary_writer)
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Запись свойств и данных графической фигуры в формат данных XML
			/// </summary>
			/// <param name="xml_writer">Средство записи данных в формат XML</param>
			//---------------------------------------------------------------------------------------------------------
			public override void WriteToXml(XmlWriter xml_writer)
			{
				xml_writer.WriteStartElement("CadShapeImage");

				//WriteBaseElementToAttribute(xml_writer);
				WriteShapeToAttribute(xml_writer);

				mPrimitive.WritePrimitivToAttribute("", xml_writer);
				mTransform.WriteTransformableToAttribute("", xml_writer);

				xml_writer.WriteEndElement();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Чтение свойств и данных графической фигуры из бинарного потока
			/// </summary>
			/// <param name="binary_reader">Бинарный поток открытый для чтения</param>
			//---------------------------------------------------------------------------------------------------------
			public override void ReadFromStream(BinaryReader binary_reader)
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Чтение свойств и данных графической фигуры из потока данных в формате XML
			/// </summary>
			/// <param name="xml_reader">Средство чтения данных формата XML</param>
			//---------------------------------------------------------------------------------------------------------
			public override void ReadFromXml(XmlReader xml_reader)
			{
				// Читаем базовые данные
				//ReadBaseElementFromAttribute(xml_reader);
				ReadShapeFromAttribute(xml_reader);

				mPrimitive.ReadPrimitivFromAttribute("", xml_reader);
				mTransform.ReadTransformableFromAttribute("", xml_reader);
			}
			#endregion
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Конвертер типа линии для предоставления свойств
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CCadShapeImageConverter : TypeConverter
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
				result.Add(pdc["Source"]);

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
				result.Add(pdc["Location"]);
				result.Add(pdc["Width"]);
				result.Add(pdc["Height"]);
				result.Add(pdc["RotationAngle"]);
				result.Add(pdc["RotationOrigin"]);

				return (new PropertyDescriptorCollection(result.ToArray(), true));
			}
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
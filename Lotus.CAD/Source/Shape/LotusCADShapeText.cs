//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Интерактивные графические фигуры
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADShapeText.cs
*		Текст.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 04.04.2021
//=====================================================================================================================
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
		/// Текст
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		[TypeConverter(typeof(CCadShapeTextConverter))]
		public class CCadShapeText : CCadShapeRectangular<CCadPrimitiveText>
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			protected static PropertyChangedEventArgs PropertyArgsOffset = new PropertyChangedEventArgs(nameof(Offset));
			protected static PropertyChangedEventArgs PropertyArgsText = new PropertyChangedEventArgs(nameof(Text));
			protected static PropertyChangedEventArgs PropertyArgsColor = new PropertyChangedEventArgs(nameof(Color));
			protected static PropertyChangedEventArgs PropertyArgsFont = new PropertyChangedEventArgs(nameof(Font));
			protected static PropertyChangedEventArgs PropertyArgsFontSize = new PropertyChangedEventArgs(nameof(FontSize));
			protected static PropertyChangedEventArgs PropertyArgsHorizontalAlignment = new PropertyChangedEventArgs(nameof(HorizontalAlignment));
			protected static PropertyChangedEventArgs PropertyArgsVerticalAlignment = new PropertyChangedEventArgs(nameof(VerticalAlignment));
			protected static PropertyChangedEventArgs PropertyArgsTrimming = new PropertyChangedEventArgs(nameof(Trimming));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			// Основные параметры
			internal Vector2Df mOffset;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			//
			// ОСНОВНЫЕ ПАРАМЕТРЫ
			//
			/// <summary>
			/// Смещение текста
			/// </summary>
			[DisplayName("Смещение")]
			[Description("Смещение текста")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 0)]
			public Vector2Df Offset
			{
				get { return (mOffset); }
				set
				{
					mOffset = value;
					NotifyPropertyChanged(PropertyArgsOffset);
					RaiseOffsetChanged();
				}
			}

			/// <summary>
			/// Текст
			/// </summary>
			[DisplayName("Текст")]
			[Description("Текст")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 1)]
			public String Text
			{
				get { return (mPrimitive.mText); }
				set
				{
					mPrimitive.Text = value;
					NotifyPropertyChanged(PropertyArgsText);
					RaiseTextChanged();
				}
			}

			/// <summary>
			/// Цвет текста
			/// </summary>
			[DisplayName("Цвет текста")]
			[Description("Цвет шрифта текста")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 2)]
			public TColor Color
			{
				get { return (mPrimitive.ColorBrush ); }
				set
				{
					mPrimitive.ColorBrush = value;
					NotifyPropertyChanged(PropertyArgsColor);
					RaiseColorChanged();
				}
			}

			/// <summary>
			/// Шрифт текста
			/// </summary>
			[DisplayName("Шрифт")]
			[Description("Шрифт текста")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 3)]
			public CCadFont Font
			{
				get { return (mPrimitive.Font); }
				set
				{
					mPrimitive.Font = value;
					NotifyPropertyChanged(PropertyArgsFont);
					RaiseFontChanged();
				}
			}

			/// <summary>
			/// Размер шрифта текста
			/// </summary>
			[DisplayName("Размер")]
			[Description("Размер шрифта текста")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 4)]
			public Single FontSize
			{
				get { return (mPrimitive.FontSize); }
				set
				{
					mPrimitive.FontSize = value;
					NotifyPropertyChanged(PropertyArgsFontSize);
					RaiseFontSizeChanged();
				}
			}

			/// <summary>
			/// Выравнивание текста по горизонтали
			/// </summary>
			[DisplayName("Горизонталь")]
			[Description("Выравнивание текста по горизонтали")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 5)]
			public TCadTextHorizontalAlignment HorizontalAlignment
			{
				get { return (mPrimitive.mHorizontalAlignment); }
				set
				{
					mPrimitive.HorizontalAlignment = value;
					NotifyPropertyChanged(PropertyArgsHorizontalAlignment);
					RaiseHorizontalAlignmentChanged();
				}
			}

			/// <summary>
			/// Выравнивание текста вертикали
			/// </summary>
			[DisplayName("Вертикаль")]
			[Description("Выравнивание текста по вертикали")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 6)]
			public TCadTextVerticalAlignment VerticalAlignment
			{
				get { return (mPrimitive.mVerticalAlignment); }
				set
				{
					mPrimitive.VerticalAlignment = value;
					NotifyPropertyChanged(PropertyArgsVerticalAlignment);
					RaiseVerticalAlignmentChanged();
				}
			}

			/// <summary>
			/// Режим обрезки текста
			/// </summary>
			[DisplayName("Обрезка")]
			[Description("Режим обрезки текста")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 6)]
			public TCadTextTrimming Trimming
			{
				get { return (mPrimitive.mTrimming); }
				set
				{
					mPrimitive.Trimming = value;
					NotifyPropertyChanged(PropertyArgsTrimming);
					RaiseTrimmingChanged();
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
				get { return ("ТЕКСТ"); }
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
			public CCadShapeText()
			{
				mName = "Текст: " + Id.ToString();
				mGroup = "Тексты";
				mPrimitive = mCanvas.CreateText("Текст", new Vector2Df(100, 100), 12);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="location">Позиция текста</param>
			/// <param name="text">Текстовые данные</param>
			/// <param name="name">Имя текста</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadShapeText(Vector2Df location, String text,  String name = "Текст")
			{
				mName = name;
				mGroup = "Тексты";

				mPrimitive = mCanvas.CreateText(text, location, 12);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор копирования
			/// </summary>
			/// <param name="source">Источник копии</param>
			/// <param name="add_to_draft">Добавлять ли в чертеж источника</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadShapeText(CCadShapeText source, Boolean add_to_draft = true)
				: base(source, add_to_draft)
			{
				mOffset = source.mOffset;
				mPrimitive = source.mPrimitive.Duplicate() as CCadPrimitiveText;
			}
			#endregion

			#region ======================================= CЛУЖЕБНЫЕ МЕТОДЫ ==========================================
			#endregion

			#region ======================================= CЛУЖЕБНЫЕ МЕТОДЫ СОБЫТИЙ ==================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение смещения текста.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseOffsetChanged()
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение текста.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseTextChanged()
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение цвета текста.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseColorChanged()
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение шрифта текста.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseFontChanged()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение размера шрифта текста.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseFontSizeChanged()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение выравнивания текста по горизонтали.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseHorizontalAlignmentChanged()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение выравнивания текста по вертикали.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseVerticalAlignmentChanged()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение режима обрезки текста.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseTrimmingChanged()
			{
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
				CCadShapeText source = element as CCadShapeText;
				if (source != null)
				{
					mOffset = source.Offset;

					NotifyPropertyChanged(PropertyArgsOffset);
					NotifyPropertyChanged(PropertyArgsText);
					NotifyPropertyChanged(PropertyArgsColor);
					NotifyPropertyChanged(PropertyArgsFont);
					NotifyPropertyChanged(PropertyArgsFontSize);
					NotifyPropertyChanged(PropertyArgsHorizontalAlignment);
					NotifyPropertyChanged(PropertyArgsVerticalAlignment);
					NotifyPropertyChanged(PropertyArgsTrimming);
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
			/// Рисование текста
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
				xml_writer.WriteStartElement("CadShapeText");

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
		/// Конвертер типа текста для предоставления свойств
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CCadShapeTextConverter : TypeConverter
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
				result.Add(pdc["Offset"]);
				result.Add(pdc["Text"]);
				result.Add(pdc["Color"]);
				result.Add(pdc["Alignment"]);
				result.Add(pdc["ParagraphAlignment"]);
				result.Add(pdc["Trimming"]);
				result.Add(pdc["FontSize"]);
				result.Add(pdc["FontStretch"]);
				result.Add(pdc["FontStyle"]);
				result.Add(pdc["FontWeight"]);

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
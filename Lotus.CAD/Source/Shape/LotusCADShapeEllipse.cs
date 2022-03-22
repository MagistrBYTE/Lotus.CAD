//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Интерактивные графические фигуры
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADShapeEllipse.cs
*		Эллипс.
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
		/// Эллипс
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		[TypeConverter(typeof(CCadShapeEllipseConverter))]
		public class CCadShapeEllipse : CCadShapeRectangular<CCadPrimitiveEllipse>
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			protected static PropertyChangedEventArgs PropertyArgsRadiusX = new PropertyChangedEventArgs(nameof(RadiusX));
			protected static PropertyChangedEventArgs PropertyArgsRadiusY = new PropertyChangedEventArgs(nameof(RadiusY));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			//
			// ОСНОВНЫЕ ПАРАМЕТРЫ
			//
			/// <summary>
			/// Радиус по X
			/// </summary>
			[DisplayName("Радиус по X")]
			[Description("Радиус эллипса по X")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 0)]
			public Single RadiusX
			{
				get { return (mPrimitive.RadiusX); }
				set
				{
					// Обновляем ширину
					mPrimitive.mBoundsRect.Width = value * 2;

					// Центр прямоугольника
					Vector2Df center = Transform.TransformPointToCanvas(mPrimitive.mBoundsRect.Center);
					mPrimitive.mBoundsRect.X = -mPrimitive.mBoundsRect.Width / 2;
					mPrimitive.mBoundsRect.Y = -mPrimitive.mBoundsRect.Height / 2;

					// Обновляем трансформацию
					mTransform.mRotationOrigin = Vector2Df.Zero;
					mTransform.Position = center;

					// Обновляем свойства
					NotifyPropertyChanged(PropertyArgsRadiusX);
					NotifyPropertyChanged(PropertyArgsWidth);
					NotifyPropertyChanged(PropertyArgsLocation);

					RaiseRadiusChanged();
				}
			}

			/// <summary>
			/// Радиус по Y
			/// </summary>
			[DisplayName("Радиус по Y")]
			[Description("Радиус эллипса по Y")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 0)]
			public Single RadiusY
			{
				get { return (mPrimitive.RadiusY); }
				set
				{
					// Обновляем высоту
					mPrimitive.mBoundsRect.Height = value * 2;

					// Центр прямоугольника
					Vector2Df center = Transform.TransformPointToCanvas(mPrimitive.mBoundsRect.Center);
					mPrimitive.mBoundsRect.X = -mPrimitive.mBoundsRect.Width / 2;
					mPrimitive.mBoundsRect.Y = -mPrimitive.mBoundsRect.Height / 2;

					// Обновляем трансформацию
					mTransform.mRotationOrigin = Vector2Df.Zero;
					mTransform.Position = center;

					// Обновляем свойства
					NotifyPropertyChanged(PropertyArgsRadiusY);
					NotifyPropertyChanged(PropertyArgsHeight);
					NotifyPropertyChanged(PropertyArgsLocation);

					RaiseRadiusChanged();
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
				get { return ("ЭЛЛИПС"); }
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
			public CCadShapeEllipse()
			{
				mName = "Эллипс: " + Id.ToString();
				mGroup = "Эллипсы";
				mPrimitive = mCanvas.CreateEllipse(new Vector2Df(100, 100), 100, 60);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="location">Позиция эллипса</param>
			/// <param name="radius_x">Радиус эллипса по X</param>
			/// <param name="radius_y">Радиус эллипса по Y</param>
			/// <param name="name">Имя эллипса</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadShapeEllipse(Vector2Df location, Single radius_x, Single radius_y, String name = "Эллипс")
			{
				mName = name;
				mGroup = "Эллипсы";
				mPrimitive = mCanvas.CreateEllipse(location, radius_x, radius_y);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор копирования
			/// </summary>
			/// <param name="source">Источник копии</param>
			/// <param name="add_to_draft">Добавлять ли в чертеж источника</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadShapeEllipse(CCadShapeEllipse source, Boolean add_to_draft = true)
				: base(source, add_to_draft)
			{
				mPrimitive = source.mPrimitive.Duplicate() as CCadPrimitiveEllipse;
			}
			#endregion

			#region ======================================= СИСТЕМНЫЕ МЕТОДЫ ==========================================
			#endregion

			#region ======================================= CЛУЖЕБНЫЕ МЕТОДЫ СОБЫТИЙ ==================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение ширины прямоугольника.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected override void RaiseWidthChanged()
			{
				base.RaiseWidthChanged();
				NotifyPropertyChanged(PropertyArgsRadiusX);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение высоты прямоугольника.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected override void RaiseHeightChanged()
			{
				base.RaiseHeightChanged();
				NotifyPropertyChanged(PropertyArgsRadiusY);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение размеров эллипса.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseRadiusChanged()
			{
				SetHandleRects();
				//mCanvas.Update();
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Окончание создания прямоугольника
			/// </summary>
			/// <param name="pos">Конечная позиция</param>
			//---------------------------------------------------------------------------------------------------------
			public override void CreateEndRect(ref Vector2Df pos)
			{
				base.CreateEndRect(ref pos);
			}

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
				CCadShapeEllipse source = element as CCadShapeEllipse;
				if (source != null)
				{
					NotifyPropertyChanged(PropertyArgsRadiusX);
					NotifyPropertyChanged(PropertyArgsRadiusY);
				}
			}
			#endregion

			#region ======================================= МЕТОДЫ ТРАНСФОРМАЦИИ ======================================
			#endregion

			#region ======================================= МЕТОДЫ УПРАВЛЕНИЯ =========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновления захвата управления от курсора
			/// </summary>
			/// <param name="point">Точка в координатах канвы</param>
			//---------------------------------------------------------------------------------------------------------
			public override void UpdateCapturePosition(ref Vector2Df point)
			{
				// Трансформируем точку из пространства канвы в пространство примитива
				Vector2Df pos = mTransform.TransformPointToLocal(ref point);

				// Смещение
				Vector2Df offset = mCanvasViewer.MouseDelta;

				// Обновляем
				UpdateCapturePosition(ref pos, ref offset);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Окончание захвата от управления курсора
			/// </summary>
			/// <param name="point">Точка в координатах канвы</param>
			//---------------------------------------------------------------------------------------------------------
			public override void EndCapturePosition(ref Vector2Df point)
			{
				base.EndCapturePosition(ref point);
				NotifyPropertyChanged(PropertyArgsRadiusX);
				NotifyPropertyChanged(PropertyArgsRadiusY);
			}
			#endregion

			#region ======================================= МЕТОДЫ РИСОВАНИЯ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Рисование эллипса
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
				xml_writer.WriteStartElement("CadShapeEllipse");

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
		/// Конвертер типа эллипса для предоставления свойств
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CCadShapeEllipseConverter : TypeConverter
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
				result.Add(pdc["RadiusX"]);
				result.Add(pdc["RadiusY"]);

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
//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Графические примитивы
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADPrimitiveText.cs
*		Графический примитив текста.
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
		/// Графический примитив текста
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CCadPrimitiveText : CCadPrimitiveRect
		{
			#region ======================================= ДАННЫЕ ====================================================
			// Основные параметры
			internal String mText = "11";
			internal CCadBrush mBrush;
			internal Single mFontSize = 12;
			internal CCadFont mFont;
			internal TCadTextHorizontalAlignment mHorizontalAlignment;
			internal TCadTextVerticalAlignment mVerticalAlignment;
			internal TCadTextTrimming mTrimming;
			internal Vector2Df mPoint = Vector2Df.Zero;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			//
			// ОСНОВНЫЕ ПАРАМЕТРЫ
			//
			/// <summary>
			/// Текст
			/// </summary>
			public String Text
			{
				get { return (mText); }
				set
				{
					if (mText != value)
					{
						mText = value;

						if (mIsCanvas)
						{
							// Обновляем данные
							UpdateText();

							// Обновляем отображение примитива
							//XCadManager.Canvas.Update();
						}
					}
				}
			}

			/// <summary>
			/// Кисть для рисования текста
			/// </summary>
			public CCadBrush Brush
			{
				get { return (mBrush); }
				set
				{
					if (mBrush != value)
					{
						mBrush = value;

						if (mIsCanvas)
						{
							// Обновляем данные
							UpdateColor();

							// Обновляем отображение примитива
							//XCadManager.Canvas.Update();
						}
					}
				}
			}

			/// <summary>
			/// Кисть для рисования текста
			/// </summary>
			public TColor ColorBrush
			{
				get { return ((mBrush as CCadBrushSolid).Color); }
				set
				{
					mBrush = XCadBrushManager.GetFromColor(value);

					if (mIsCanvas)
					{
						// Обновляем данные
						UpdateColor();

						// Обновляем отображение примитива
						//XCadManager.Canvas.Update();
					}
				}
			}

			/// <summary>
			/// Размер шрифта текста
			/// </summary>
			public Single FontSize
			{
				get { return (mFontSize); }
				set
				{
					if (!XMath.Approximately(mFontSize, value, 0.1f))
					{
						mFontSize = value;

						if (mIsCanvas)
						{
							// Обновляем данные
							UpdateFontSize();

							// Обновляем отображение примитива
							//XCadManager.Canvas.Update();
						}
					}
				}
			}

			/// <summary>
			/// Шрифт
			/// </summary>
			public CCadFont Font
			{
				get { return (mFont); }
				set
				{
					if (mFont != value)
					{
						mFont = value;

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
			/// Выравнивание текста по горизонтали
			/// </summary>
			public TCadTextHorizontalAlignment HorizontalAlignment
			{
				get { return (mHorizontalAlignment); }
				set
				{
					if (mHorizontalAlignment != value)
					{
						mHorizontalAlignment = value;

						if (mIsCanvas)
						{
							// Обновляем данные
							UpdateHorizontalAlignment();

							// Обновляем отображение примитива
							//XCadManager.Canvas.Update();
						}
					}
				}
			}

			/// <summary>
			/// Выравнивание текста вертикали
			/// </summary>
			public TCadTextVerticalAlignment VerticalAlignment
			{
				get { return (mVerticalAlignment); }
				set
				{
					if (mVerticalAlignment != value)
					{
						mVerticalAlignment = value;

						if (mIsCanvas)
						{
							// Обновляем данные
							UpdateVerticalAlignment();

							// Обновляем отображение примитива
							//XCadManager.Canvas.Update();
						}
					}
				}
			}

			/// <summary>
			/// Режим обрезки текста
			/// </summary>
			public TCadTextTrimming Trimming
			{
				get { return (mTrimming); }
				set
				{
					if (mTrimming != value)
					{
						mTrimming = value;

						if (mIsCanvas)
						{
							// Обновляем данные
							UpdateTrimming();

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
			public CCadPrimitiveText()
			{
				mBrush = XCadBrushManager.DarkGray;
				mFont = XCadFontManager.DefaultFont;
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление данных текста
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public virtual void UpdateText()
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление цвета текста
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public virtual void UpdateColor()
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление данных размера шрифта
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public virtual void UpdateFontSize()
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление выравнивание текста по горизонтали
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public virtual void UpdateHorizontalAlignment()
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление выравнивание текста по вертикали
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public virtual void UpdateVerticalAlignment()
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление режима обрезки текста
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public virtual void UpdateTrimming()
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
				xml_writer.WriteStringToAttribute(prefix + "Text", mText);
				//xml_writer.WriteColorToAttribute(prefix + "Color", ColorBrush);
				xml_writer.WriteSingleToAttribute(prefix + "FontSize", mFontSize);
				xml_writer.WriteLongToAttribute(prefix + "FontID", mFont.Id);
				xml_writer.WriteEnumToAttribute(prefix + "HorizontalAlignment", mHorizontalAlignment);
				xml_writer.WriteEnumToAttribute(prefix + "VerticalAlignment", mVerticalAlignment);
				xml_writer.WriteEnumToAttribute(prefix + "Trimming", mTrimming);
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
				mText = xml_reader.ReadStringFromAttribute(prefix + "Text", mText);
				//mBrush = XCadBrushManager.GetFromColor(xml_reader.ReadCadColorFromAttribute(prefix + "Color"));
				mFontSize = xml_reader.ReadSingleFromAttribute(prefix + "FontSize", mFontSize);
				mFont = XCadFontManager.GetFromId(xml_reader.ReadLongFromAttribute(prefix + "FontID", mFont.Id));
				mHorizontalAlignment = xml_reader.ReadEnumFromAttribute(prefix + "HorizontalAlignment", mHorizontalAlignment);
				mVerticalAlignment = xml_reader.ReadEnumFromAttribute(prefix + "VerticalAlignment", mVerticalAlignment);
				mTrimming = xml_reader.ReadEnumFromAttribute(prefix + "Trimming", mTrimming);
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
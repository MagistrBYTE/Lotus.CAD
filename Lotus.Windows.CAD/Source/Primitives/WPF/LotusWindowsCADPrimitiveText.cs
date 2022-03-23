//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Графические примитивы
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusWindowsCADPrimitiveText.cs
*		Графический примитив текста.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 27.03.2022
//=====================================================================================================================
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
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
		public class CCadWindowsPrimitiveText : CCadPrimitiveText
		{
			#region ======================================= ДАННЫЕ ====================================================
			internal FormattedText mFormattedText;
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public CCadWindowsPrimitiveText()
			{
				Update();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="text">Текст</param>
			/// <param name="start_point">Начальная точка</param>
			/// <param name="font_size">Размер шрифта текста</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadWindowsPrimitiveText(String text, Vector2Df start_point, Single font_size)
			{
				mText = text;
				mFontSize = font_size;
				mBoundsRect.PointTopLeft = start_point;

				Update();
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление данных текста
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void UpdateText()
			{
				Update();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление цвета текста
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void UpdateColor()
			{
				if (mFormattedText != null)
				{
					mFormattedText.SetForegroundBrush(mBrush.WindowsBrush);
				}
				else
				{
					Update();
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление данных размера шрифта
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void UpdateFontSize()
			{
				if(mFormattedText != null)
				{
					mFormattedText.SetFontSize(mFontSize);
				}
				else
				{
					Update();
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление выравнивание текста по горизонтали
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void UpdateHorizontalAlignment()
			{
				Single width = (Single)mFormattedText.Width;
				Single height = (Single)mFormattedText.Height;

				switch (mHorizontalAlignment)
				{
					case TCadTextHorizontalAlignment.Left:
						{
							switch (mVerticalAlignment)
							{
								case TCadTextVerticalAlignment.Top:
									mPoint = mBoundsRect.PointTopLeft;
									break;
								case TCadTextVerticalAlignment.Bottom:
									mPoint = new Vector2Df(mBoundsRect.X, mBoundsRect.Y + mBoundsRect.Height - height);
									break;
								case TCadTextVerticalAlignment.Middle:
									mPoint = new Vector2Df(mBoundsRect.X, mBoundsRect.Y + mBoundsRect.Height / 2 - height / 2);
									break;
								default:
									break;
							}
						}
						break;
					case TCadTextHorizontalAlignment.Right:
						{
							switch (mVerticalAlignment)
							{
								case TCadTextVerticalAlignment.Top:
									mPoint = new Vector2Df(mBoundsRect.X + (mBoundsRect.Width - width), mBoundsRect.Y);
									break;
								case TCadTextVerticalAlignment.Bottom:
									mPoint = new Vector2Df(mBoundsRect.X + (mBoundsRect.Width - width), mBoundsRect.Y + mBoundsRect.Height - height);
									break;
								case TCadTextVerticalAlignment.Middle:
									mPoint = new Vector2Df(mBoundsRect.X + (mBoundsRect.Width - width), mBoundsRect.Y + mBoundsRect.Height / 2 - height / 2);
									break;
								default:
									break;
							}
						}
						break;
					case TCadTextHorizontalAlignment.Center:
						{
							switch (mVerticalAlignment)
							{
								case TCadTextVerticalAlignment.Top:
									mPoint = new Vector2Df(mBoundsRect.X + (mBoundsRect.Width/2 - width/2), mBoundsRect.Y);
									break;
								case TCadTextVerticalAlignment.Bottom:
									mPoint = new Vector2Df(mBoundsRect.X + (mBoundsRect.Width / 2 - width / 2), mBoundsRect.Y + mBoundsRect.Height - height);
									break;
								case TCadTextVerticalAlignment.Middle:
									mPoint = new Vector2Df(mBoundsRect.X + (mBoundsRect.Width / 2 - width / 2), mBoundsRect.Y + mBoundsRect.Height / 2 - height / 2);
									break;
								default:
									break;
							}
						}
						break;
					case TCadTextHorizontalAlignment.Justify:
						break;
					default:
						break;
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление выравнивание текста по вертикали
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void UpdateVerticalAlignment()
			{
				UpdateHorizontalAlignment();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление режима обрезки текста
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void UpdateTrimming()
			{
				if (mFormattedText != null)
				{
					mFormattedText.Trimming = (TextTrimming)mTrimming;
				}
				else
				{
					Update();
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление данных текста
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void Update()
			{
				mFormattedText = new FormattedText(mText, 
					CultureInfo.CurrentCulture, FlowDirection.LeftToRight, 
					mFont.WindowsFont, 
					mFontSize, mBrush.WindowsBrush, 96);

				mFormattedText.Trimming = (TextTrimming)mTrimming;
				UpdateHorizontalAlignment();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Копирование графического примитива
			/// </summary>
			/// <returns>Дубликат графического примитива со всеми параметрами и данными</returns>
			//---------------------------------------------------------------------------------------------------------
			public override CCadPrimitive Duplicate()
			{
				CCadWindowsPrimitiveText text = new CCadWindowsPrimitiveText();
				text.CopyParamemtrs(this);
				text.Update();
				return (text);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Копирование параметров с графического примитива
			/// </summary>
			/// <param name="primitiv">Графический примитив</param>
			//---------------------------------------------------------------------------------------------------------
			public override void CopyParamemtrs(CCadPrimitive primitiv)
			{
				base.CopyParamemtrs(primitiv);

				CCadWindowsPrimitiveText source = primitiv as CCadWindowsPrimitiveText;

				mText = source.mText;
				mBrush = source.mBrush;
				mFontSize = source.mFontSize;
				mFont = source.mFont;
				mHorizontalAlignment = source.mHorizontalAlignment;
				mVerticalAlignment = source.mVerticalAlignment;
				mTrimming = source.mTrimming;
			}
			#endregion

			#region ======================================= МЕТОДЫ РИСОВАНИЯ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Рисование текста
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void Draw()
			{
				//if (mIsFilled)
				//{
				//	LotusCadCanvas.DrawingDevice.DrawRectangle(mFill.WindowsBrush, null, mBoundsRect.ToWinRect());
				//}

				//LotusCadCanvas.DrawingDevice.DrawText(mFormattedText, mPoint.ToWinPoint());

				//if (mIsStroked)
				//{
				//	LotusCadCanvas.DrawingDevice.DrawRectangle(null, mStroke.WindowsPen, mBoundsRect.ToWinRect());
				//}
			}
			#endregion
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Внутренний класс для отображения примитива текста
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CCadPrimitiveTextInternal
		{
			#region ======================================= ДАННЫЕ ====================================================
			//internal FormattedText mFormattedText;
			//internal Brush mBrush;
			//internal Single mFontSize = 12;
			//internal Typeface mFontTypeface;
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
﻿//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Инструменты создания и редактирования графических элементов
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADToolCreateImage.cs
*		Инструмент для создания изображения.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 04.04.2021
//=====================================================================================================================
using System;
using System.Collections.Generic;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Maths;
using Lotus.Core;
//=====================================================================================================================
namespace Lotus
{
	namespace CAD
	{
		//-------------------------------------------------------------------------------------------------------------
		//! \addtogroup CadTools
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Инструмент для создания изображения
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CCadToolCreateImage : CCadTool<CCadShapeImage>
		{
			#region ======================================= ДАННЫЕ ====================================================
			protected CCadShapeImage mCurrentImage;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public CCadToolCreateImage()
			{
			}
			#endregion

			#region ======================================= CЛУЖЕБНЫЕ МЕТОДЫ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Начало создания изображения
			/// </summary>
			/// <param name="pos">Позиция курсора в области канвы</param>
			//---------------------------------------------------------------------------------------------------------
			protected void StartCreateImage(ref Vector2Df pos)
			{
				// Создаем изображение
				//mCurrentImage = XCadManager.CreateShape("ShapeImage", "") as CCadShapeImage;

				// Если была включена привязка
				Vector2Df result = mCanvasViewer.SnapIsExsisting ? mCanvasViewer.SnapPoint : pos;
				mCurrentImage.CreateStartRect(ref result);

				// Начало рисования изображения
				mIsCreateElement = true;
				mCanvas.SetCursor(TCursor.Cross);

				// Если есть активный документ то добавляем в него (на канву добавиться автоматически)
				//if (XManager.PresenterDocument != null)
				//{
				//	//XManager.PresenterDocument.AddExistingElement(mCurrentImage);
				//}
				//else
				//{
				//	XManager.Presenter.Elements.Add(mCurrentImage);
				//}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Окончание создания изображения
			/// </summary>
			/// <param name="pos">Позиция курсора в области канвы</param>
			//---------------------------------------------------------------------------------------------------------
			protected void EndCreateImage(ref Vector2Df pos)
			{
				mIsCreateElement = false;
				mCanvas.SetCursor(TCursor.Arrow);
				//XManager.MementoManager.AddStateToHistory(new CStateMementoElementAdd(mCurrentImage));

				// Если была включена привязка
				Vector2Df result = mCanvasViewer.SnapIsExsisting ? mCanvasViewer.SnapPoint : pos;

				mCurrentImage.CreateEndRect(ref result);
				mCurrentImage = null;
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Нажатие кнопки мыши (Начало создание изображения)
			/// </summary>
			/// <param name="pos">Позиция курсора в области канвы</param>
			/// <param name="button">Кнопка мыши связанная с данным событием</param>
			//---------------------------------------------------------------------------------------------------------
			public override void OnMouseDown(ref Vector2Df pos, TMouseButton button)
			{
				// Создания изображения
				if (button == TMouseButton.Left)
				{
					if (mCanvasViewer.CreateModeIsAutoCAD)
					{
						if (mIsCreateElement == false)
						{
							StartCreateImage(ref pos);
						}
						else
						{
							EndCreateImage(ref pos);
						}
					}
					else
					{
						StartCreateImage(ref pos);
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перемещение мыши (Продолжение создание изображения)
			/// </summary>
			/// <param name="pos">Позиция курсора в области канвы</param>
			/// <param name="button">Кнопка мыши связанная с данным событием</param>
			//---------------------------------------------------------------------------------------------------------
			public override void OnMouseMove(ref Vector2Df pos, TMouseButton button)
			{
				if (mIsCreateElement)
				{
					mCurrentImage.CreateContinueRect(ref pos);
				}
				else
				{
					// Если выключен режим привязки
					if (mCanvasViewer.SnapIsEnabled)
					{
						// Перерисовываем канву
						//mCanvas.Update();
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Отпускание кнопки мыши
			/// </summary>
			/// <param name="pos">Позиция курсора в области канвы</param>
			/// <param name="button">Кнопка мыши связанная с данным событием</param>
			//---------------------------------------------------------------------------------------------------------
			public override void OnMouseUp(ref Vector2Df pos, TMouseButton button)
			{
				if (button == TMouseButton.Left && mIsCreateElement)
				{
					if (!mCanvasViewer.CreateModeIsAutoCAD)
					{
						EndCreateImage(ref pos);
					}
				}
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Инструменты создания и редактирования графических элементов
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADToolCreatePolyline.cs
*		Инструмент для создания полилинии.
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
		/// Инструмент для создания полилинии
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CCadToolCreatePolyline : CCadTool<CCadShapePolyline>
		{
			#region ======================================= ДАННЫЕ ====================================================
			private CCadShapePolyline mCurrentPolyline;
			private Int32 mCurrentIndex;
			private Boolean mIsEnter;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public CCadToolCreatePolyline()
			{
				mCurrentIndex = 1;
			}
			#endregion

			#region ======================================= CЛУЖЕБНЫЕ МЕТОДЫ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Окончание создания полилинии
			/// </summary>
			/// <param name="pos">Позиция курсора в области канвы</param>
			//---------------------------------------------------------------------------------------------------------
			protected void EndCreatePolyline(ref Vector2Df pos)
			{
				mIsCreateElement = false;
				mCanvas.SetCursor(TCursor.Arrow);
				//XManager.MementoManager.AddStateToHistory(new CStateMementoElementAdd(mCurrentPolyline));

				// Если была включена привязка
				mCurrentPolyline.EndPolyline(mCanvasViewer.SnapIsExsisting ? mCanvasViewer.SnapPoint : pos);
				mCurrentPolyline = null;
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Нажатие кнопки мыши (Начало создание полилинии)
			/// </summary>
			/// <param name="pos">Позиция курсора в области канвы</param>
			/// <param name="button">Кнопка мыши связанная с данным событием</param>
			//---------------------------------------------------------------------------------------------------------
			public override void OnMouseDown(ref Vector2Df pos, TMouseButton button)
			{
				// Создания полилинии
				if (button == TMouseButton.Left && mCanvas != null)
				{
					if (mCanvasViewer.CreateModeIsAutoCAD)
					{
						if (mIsCreateElement == false)
						{
							//mCurrentPolyline = mCanvas.PresenterDocument.AddNewElement("ShapePolyline") as CCadShapePolyline;

							// Если была включена привязка
							mCurrentPolyline.CreateStartPolyline(mCanvasViewer.SnapIsExsisting ? mCanvasViewer.SnapPoint : pos);

							// Начало рисования дуги
							mIsCreateElement = true;
							mCanvas.SetCursor(TCursor.Cross);
						}
						else
						{
							EndCreatePolyline(ref pos);
						}
					}
					else
					{
						//Debug.Print(mIsEnter.ToString());

						if (mIsEnter == false)
						{
							// Если первая точка
							if (mCurrentPolyline == null)
							{
								//mCurrentPolyline = mCanvas.PresenterDocument.AddNewElement("ShapePolyline") as CCadShapePolyline;

								// Если была включена привязка
								mCurrentPolyline.CreateStartPolyline(mCanvasViewer.SnapIsExsisting ? mCanvasViewer.SnapPoint : pos);
								mCurrentIndex = 1;

								// Начало рисования дуги
								mIsCreateElement = true;
								mCanvas.SetCursor(TCursor.Cross);
							}
							else
							{
								// Добавляем точку
								// Если была включена привязка
								mCurrentPolyline.CreateAddPoint(mCanvasViewer.SnapIsExsisting ? mCanvasViewer.SnapPoint : pos);
								mCurrentIndex++;
							}
						}

						mIsEnter = false;
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перемещение мыши (Продолжение создание полилинии)
			/// </summary>
			/// <param name="pos">Позиция курсора в области канвы</param>
			/// <param name="button">Кнопка мыши связанная с данным событием</param>
			//---------------------------------------------------------------------------------------------------------
			public override void OnMouseMove(ref Vector2Df pos, TMouseButton button)
			{
				if (mIsCreateElement)
				{
					mCurrentPolyline.CreateContinuePolyline(ref pos, mCurrentIndex);
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
			/// Отпускание кнопки мыши (Окончание создание дуги)
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
						//EndCreatePolyline(ref pos);
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Завершение операции
			/// </summary>
			/// <returns>Статус завершения операции</returns>
			//---------------------------------------------------------------------------------------------------------
			public override Boolean Enter()
			{
				mIsEnter = true;

				if (mIsCreateElement)
				{
					//EndCreatePolyline(ref pos);
					mIsCreateElement = false;
					mCanvas.SetCursor(TCursor.Arrow);
					//XManager.MementoManager.AddStateToHistory(new CStateMementoElementAdd(CurrentElement));
					CurrentElement = null;
					return (false);
				}
				else
				{
					return (true);
				}
			}
			#endregion

			#region ======================================= ОБРАБОТКА СОБЫТИЙ КОНТЕКСТНОГО МЕНЮ =======================
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
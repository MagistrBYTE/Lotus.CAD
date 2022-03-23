﻿//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Инструменты создания и редактирования графических элементов
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADToolCreate.cs
*		Базовый инструмент для создания графических элементов.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 27.03.2022
//=====================================================================================================================
using System;
using System.Collections.Generic;
//---------------------------------------------------------------------------------------------------------------------
using System.Windows;
using System.Windows.Input;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
using Lotus.Maths;
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
		/// Базовый инструмент для создания графических фигур
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public abstract class CCadToolCreate : CCadTool
		{
			#region ======================================= СВОЙСТВА ==================================================	
			/// <summary>
			/// Текущий графическая фигура
			/// </summary>
			public abstract ICadShape ICurrentElement { get; }

			/// <summary>
			/// Статус операции создания элемента
			/// </summary>
			public abstract Boolean IsCreateElement { get; }
			#endregion
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Базовый инструмент для создания графических фигур
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CCadToolCreate<TElement> : CCadToolCreate where TElement : class, ICadShape
		{
			#region ======================================= ДАННЫЕ ====================================================
			internal TElement mElement;
			internal Boolean mIsCreateElement;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Статус операции создания элемента
			/// </summary>
			public override Boolean IsCreateElement
			{
				get { return (mIsCreateElement); }
			}

			/// <summary>
			/// Текущий элемент
			/// </summary>
			public virtual TElement CurrentElement
			{
				get { return (mElement); }
			}

			/// <summary>
			/// Текущий элемент
			/// </summary>
			public override ICadShape ICurrentElement
			{
				get { return (mElement); }
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="name">Интерфейс элемента пользовательского интерфейса для управления канвой</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadToolCreate(ICadCanvasViewer canvas_viewer)
			{
				mCanvasViewer = canvas_viewer;
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Нажатие кнопки мыши
			/// </summary>
			/// <param name="pos">Позиция курсора в области канвы</param>
			/// <param name="button">Кнопка мыши связанная с данным событием</param>
			//---------------------------------------------------------------------------------------------------------
			public override void OnMouseDown(ref Vector2Df pos, TMouseButton button)
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перемещение мыши
			/// </summary>
			/// <param name="pos">Позиция курсора в области канвы</param>
			/// <param name="button">Кнопка мыши связанная с данным событием</param>
			//---------------------------------------------------------------------------------------------------------
			public override void OnMouseMove(ref Vector2Df pos, TMouseButton button)
			{

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

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка курсора
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void SetCursor()
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Завершение операции
			/// </summary>
			/// <returns>Статус завершения операции</returns>
			//---------------------------------------------------------------------------------------------------------
			public virtual Boolean Enter()
			{
				if (mIsCreateElement)
				{
					mIsCreateElement = false;
				}

				return (true);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Отмена операции
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public virtual void Cancel()
			{
				if (CurrentElement != null)
				{

				}

				mCanvasViewer.SetCursor(TCursor.Arrow);

				mIsCreateElement = false;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Окончания действия инструмента
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void ToolActionCompleted()
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка контекстного меню к инструменту
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void SetContextMenu()
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Отсоединение контекстного меню от инструмента
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public override void UnsetContextMenu()
			{
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Ресурсы для отображения графического объекта
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADVisualBrush.cs
*		Кисть для заполнения замкнутой области графического объекта и отрисовки контура графического объекта.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.01.2022
//=====================================================================================================================
using System;
using System.Collections.ObjectModel;
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
		//! \defgroup CadVisual Визуальные ресурсы
		//! Ресурсы для отображения графического объекта. К визуальным ресурсам относятся кисти - для заполнения
		//! замкнутой области графического объекта и перья - для отрисовки контура графического объекта.
		//! \ingroup CadDrawing
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Тип кисти по структуре и типе заполнения
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public enum TCadBrushFillType
		{
			/// <summary>
			/// Сплошная заливка
			/// </summary>
			[Description("Сплошная заливка")]
			Solid,

			/// <summary>
			/// Линейное градиентное заполнение
			/// </summary>
			[Description("Линейное градиентное заполнение")]
			LinearGradient,

			/// <summary>
			/// Радиальное градиентное заполнение
			/// </summary>
			[Description("Радиальное градиентное заполнение")]
			RadialGradient,

			/// <summary>
			/// Штриховка
			/// </summary>
			[Description("Штриховка")]
			Hatching,

			/// <summary>
			/// Изображение
			/// </summary>
			[Description("Изображение")]
			Image
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Кисть для заполнения замкнутой области графического объекта и контура графического объекта
		/// </summary>
		/// <remarks>
		/// Определение базового класса кисти заполнения замкнутой области графического объекта и
		/// контура графического объекта
		/// </remarks>
		//-------------------------------------------------------------------------------------------------------------
		public abstract class CCadBrush : CCadEntity, IComparable<CCadBrush>, ILotusSupportViewInspector
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			protected static PropertyChangedEventArgs PropertyArgsGroup = new PropertyChangedEventArgs(nameof(Group));
			protected static PropertyChangedEventArgs PropertyArgsOpacity = new PropertyChangedEventArgs(nameof(Opacity));
			protected static PropertyChangedEventArgs PropertyArgsWindowsBrush = new PropertyChangedEventArgs("WindowsBrush");
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			// Общие данные
			internal String mGroup;

			// Основные параметры
			internal Single mOpacity;
			internal TCadBrushFillType mBrushFill;

			// Платформенные-зависимые данные
#if USE_WINDOWS
			internal System.Windows.Media.Brush mWindowsBrush;
#endif
#if USE_GDI
			internal System.Drawing.Brush mDrawingBrush;
#endif
#if USE_SHARPDX
			internal SharpDX.Direct2D1.Brush mD2DBrush;
#endif
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			//
			// ОБЩИЕ ДАННЫЕ
			/// <summary>
			/// Логическая группа которой принадлежит кисть
			/// </summary>
			[DisplayName("Группа")]
			[Description("Логическая группа которой принадлежит кисть")]
			[Category(XInspectorGroupDesc.ID)]
			[Display(GroupName = XInspectorGroupDesc.ID, Order = 1)]
			public String Group
			{
				get { return (mGroup); }
				set
				{
					mGroup = value;

					// 1) Информируем об изменении
					NotifyPropertyChanged(PropertyArgsGroup);

					// 2) Обновляем
					RaiseGroupChanged();
				}
			}

			/// <summary>
			/// Тип сущности модуля чертежной графики
			/// </summary>
			[DisplayName("Тип объекта")]
			[Description("Тип сущности модуля чертежной графики")]
			[Category(XInspectorGroupDesc.ID)]
			[Display(GroupName = XInspectorGroupDesc.ID, Order = 3)]
			public override TCadEntityType EntityType
			{
				get { return (TCadEntityType.Brush); }
			}

			//
			// ОСНОВНЫЕ ПАРАМЕТРЫ
			//
			/// <summary>
			/// Прозрачность кисти
			/// </summary>
			[Browsable(false)]
			public Single Opacity
			{
				get { return (mOpacity); }
				set
				{
					mOpacity = value;

					// 1) Информируем об изменении
					NotifyPropertyChanged(PropertyArgsOpacity);

					// 2) Обновляем
					RaiseOpacityChanged();
				}
			}

			/// <summary>
			/// Тип кисти по структуре и типе заполнения
			/// </summary>
			[Browsable(false)]
			public TCadBrushFillType BrushFill
			{
				get { return (mBrushFill); }
			}

#if USE_WINDOWS
			/// <summary>
			/// Кисть WPF
			/// </summary>
			[Browsable(false)]
			public System.Windows.Media.Brush WindowsBrush
			{
				get { return (mWindowsBrush); }
			}
#endif
#if USE_GDI
			/// <summary>
			/// Кисть System.Drawing
			/// </summary>
			[Browsable(false)]
			public System.Drawing.Brush DrawingBrush
			{
				get { return (mDrawingBrush); }
			}
#endif
#if USE_SHARPDX
			/// <summary>
			/// Кисть SharpDX.Direct2D
			/// </summary>
			[Browsable(false)]
			public SharpDX.Direct2D1.Brush D2DBrush
			{
				get { return (mD2DBrush); }
			}
#endif

			//
			// ПОДДЕРЖКА ИНСПЕКТОРА СВОЙСТВ
			//
			/// <summary>
			/// Отображаемое имя типа в инспекторе свойств
			/// </summary>
			[Browsable(false)]
			public virtual String InspectorTypeName
			{
				get { return ("КИСТЬ"); }
			}

			/// <summary>
			/// Отображаемое имя объекта в инспекторе свойств
			/// </summary>
			[Browsable(false)]
			public virtual String InspectorObjectName
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
			public CCadBrush()
			{
			}
			#endregion

			#region ======================================= СИСТЕМНЫЕ МЕТОДЫ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Сравнение кистей для упорядочивания
			/// </summary>
			/// <param name="other">Сравниваемая кисть</param>
			/// <returns>Статус сравнения кистей</returns>
			//---------------------------------------------------------------------------------------------------------
			public Int32 CompareTo(CCadBrush other)
			{
				return (XCadDrawing.DefaultComprare(this, other));
			}
			#endregion

			#region ======================================= СЛУЖЕБНЫЕ МЕТОДЫ СОБЫТИЙ ==================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение имени кисти.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected override void RaiseNameChanged()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение логической группы кисти.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseGroupChanged()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение прозрачности кисти.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseOpacityChanged()
			{

			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Общие обновление кисти
			/// </summary>
			/// <remarks>
			/// Применяется когда надо обновить внутренние ресурсы кисти по напрямую заданным параметрам кисти, 
			/// минуя механизм свойств, например при загрузке или создании
			/// </remarks>
			//---------------------------------------------------------------------------------------------------------
			public virtual void Update()
			{
#if USE_WINDOWS
				UpdateWindowsResource();
#endif
#if USE_GDI
				UpdateDrawingResource();
#endif
#if USE_SHARPDX
				UpdateDirect2DResource(true);
#endif
			}

#if USE_WINDOWS
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление ресурса WPF
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public virtual void UpdateWindowsResource()
			{
			}
#endif
#if USE_GDI
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление ресурса System.Drawing
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public virtual void UpdateDrawingResource()
			{
			}
#endif
#if USE_SHARPDX
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление ресурса Direct2D
			/// </summary>
			/// <param name="forced">Принудительное создание ресурса Direct2D</param>
			//---------------------------------------------------------------------------------------------------------
			public virtual void UpdateDirect2DResource(Boolean forced = false)
			{
			}
#endif

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Копирования кисти
			/// </summary>
			/// <returns>Копия кисти со всеми параметрами и данными</returns>
			//---------------------------------------------------------------------------------------------------------
			public virtual CCadBrush Duplicate()
			{
				CCadBrush obj = (CCadBrush)MemberwiseClone();
#if USE_WINDOWS
				obj.mWindowsBrush = mWindowsBrush.CloneCurrentValue();
#endif
#if USE_GDI
				obj.mDrawingBrush = mDrawingBrush.Clone() as System.Drawing.Brush;
#endif
#if USE_SHARPDX
				// Обновляем ресурс Direct2D
				obj.UpdateDirect2DResource();
#endif
				return (obj);
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
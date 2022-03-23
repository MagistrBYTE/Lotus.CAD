//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Общая подсистема
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADLayerEntity.cs
*		Слой для расположения графических элементов.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 27.03.2022
//=====================================================================================================================
using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Xml;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
//=====================================================================================================================
namespace Lotus
{
	namespace CAD
	{
		//-------------------------------------------------------------------------------------------------------------
		//! \addtogroup CadDrawing
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Интерфейс для определения слоя - логического пространства на чертеже где размещаются графические элементы
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public interface ICadLayer : ICadEntity
		{
			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Логическая группа которой принадлежит слой
			/// </summary>
			String Group { get; set; }

			/// <summary>
			/// Статус печати графических элементов расположенных на слое
			/// </summary>
			Boolean IsPrint { get; set; }

			/// <summary>
			/// Режим видимости слоя
			/// </summary>
			TCadLayerVisibleMode VisibleMode { get; set; }

			/// <summary>
			/// Режим полутона отображения слоя
			/// </summary>
			TCadLayerHalftoneMode HalftoneMode { get; set; }

			/// <summary>
			/// Количество графических элементов расположенных на слое
			/// </summary>
			Int32 CountElements { get; }
			#endregion
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Слой - логическое пространство на чертеже где размещаются графические элементы
		/// </summary>
		/// <remarks>
		/// Слои используется для группирования графических элементов по критерию видимости, полутона, печати
		/// </remarks>
		//-------------------------------------------------------------------------------------------------------------
		public sealed class CCadLayer : CCadEntity, ICadLayer, IComparable<CCadLayer>
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			private static PropertyChangedEventArgs PropertyArgsGroup = new PropertyChangedEventArgs(nameof(Group));
			private static PropertyChangedEventArgs PropertyArgsIsPrint = new PropertyChangedEventArgs(nameof(IsPrint));
			private static PropertyChangedEventArgs PropertyArgsVisibleMode = new PropertyChangedEventArgs(nameof(VisibleMode));
			private static PropertyChangedEventArgs PropertyArgsHalftoneMode = new PropertyChangedEventArgs(nameof(HalftoneMode));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			// Общие данные
			internal String mGroup;

			// Основные параметры
			internal Boolean mIsPrint;
			internal TCadLayerVisibleMode mVisibleMode;
			internal TCadLayerHalftoneMode mHalftoneMode;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			//
			// ОБЩИЕ ДАННЫЕ
			//
			/// <summary>
			/// Логическая группа которой принадлежит слой
			/// </summary>
			[DisplayName("Группа")]
			[Description("Логическая группа которой принадлежит слой")]
			[Category(XInspectorGroupDesc.ID)]
			[Display(GroupName = XInspectorGroupDesc.ID, Order = 1)]
			public String Group
			{
				get { return (mGroup); }
				set
				{
					mGroup = value;
					NotifyPropertyChanged(PropertyArgsGroup);
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
				get { return (TCadEntityType.Layer); }
			}

			//
			// ОСНОВНЫЕ ПАРАМЕТРЫ
			//
			/// <summary>
			/// Статус печати графических элементов расположенных на слое
			/// </summary>
			[DisplayName("Печать элементов")]
			[Description("Статус печати графических элементов расположенных на слое")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 0)]
			public Boolean IsPrint
			{
				get { return (mIsPrint); }
				set
				{
					if (mIsPrint != value)
					{
						mIsPrint = value;

						// 1) Информируем об изменении
						NotifyPropertyChanged(PropertyArgsIsPrint);

						// 2) Информируем менеджер
						XCadLayerManager.Update(this);
					}
				}
			}

			/// <summary>
			/// Режим видимости слоя
			/// </summary>
			[DisplayName("Режим видимости")]
			[Description("Тип видимости слоя")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 1)]
			public TCadLayerVisibleMode VisibleMode
			{
				get { return (mVisibleMode); }
				set
				{
					if (mVisibleMode != value)
					{
						mVisibleMode = value;

						// 1) Информируем об изменении
						NotifyPropertyChanged(PropertyArgsVisibleMode);

						// 2) Информируем менеджер
						XCadLayerManager.Update(this);
					}
				}
			}

			/// <summary>
			/// Режим полутона отображения слоя
			/// </summary>
			[DisplayName("Режим полутона")]
			[Description("Режим полутона отображения слоя")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 2)]
			public TCadLayerHalftoneMode HalftoneMode
			{
				get
				{
					return (mHalftoneMode);
				}
				set
				{
					if (mHalftoneMode != value)
					{
						mHalftoneMode = value;

						// 1) Информируем документ
						NotifyPropertyChanged(PropertyArgsHalftoneMode);

						// 2) Информируем менеджер
						XCadLayerManager.Update(this);
					}
				}
			}

			/// <summary>
			/// Количество графических элементов расположенных на слое
			/// </summary>
			[DisplayName("Кол-во элементов")]
			[Description("Количество графических элементов расположенных на слое")]
			[Category(XInspectorGroupDesc.Params)]
			[Display(GroupName = XInspectorGroupDesc.Params, Order = 3)]
			public Int32 CountElements
			{
				get { return (0); }
			}

			//
			// ПОДДЕРЖКА ИНСПЕКТОРА СВОЙСТВ
			//
			/// <summary>
			/// Отображаемое имя типа в инспекторе свойств
			/// </summary>
			[Browsable(false)]
			public String InspectorTypeName
			{
				get { return ("СЛОЙ"); }
			}

			/// <summary>
			/// Отображаемое имя объекта в инспекторе свойств
			/// </summary>
			[Browsable(false)]
			public String InspectorObjectName
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
			public CCadLayer()
				:this("Новый слой")
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="name">Имя слоя</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadLayer(String name)
				: base(name)
			{
			}
			#endregion

			#region ======================================= СИСТЕМНЫЕ МЕТОДЫ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Сравнение слоев для упорядочивания
			/// </summary>
			/// <param name="other">Сравниваемый слой</param>
			/// <returns>Статус сравнения слоев</returns>
			//---------------------------------------------------------------------------------------------------------
			public Int32 CompareTo(CCadLayer other)
			{
				return (XCadDrawing.DefaultComprare(this, other));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Копирования слоя
			/// </summary>
			/// <returns>Дубликат слоя со всеми параметрами и данными</returns>
			//---------------------------------------------------------------------------------------------------------
			public CCadLayer Clone()
			{
				CCadLayer obj = (CCadLayer)MemberwiseClone();
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
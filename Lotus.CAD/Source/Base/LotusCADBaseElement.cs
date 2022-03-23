//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Общая подсистема
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADBaseElement.cs
*		Базовый графический элемент.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 27.03.2022
//=====================================================================================================================
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
using Lotus.Maths;
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
		/// Интерфейс для определения базового графического элемента
		/// </summary>
		/// <remarks>
		/// Базовый графический элемент – объект обязательно поддерживает параметры слоя и который может быть видим или скрыт
		/// </remarks>
		//-------------------------------------------------------------------------------------------------------------
		public interface ICadElement : ICadObject, ICadLayerSupport, IComparable<ICadElement>
		{
			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Видимость графического элемента
			/// </summary>
			Boolean IsVisible { get; set; }

			/// <summary>
			/// Полутон графического элемента
			/// </summary>
			Boolean IsHalftone { get; set; }
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение точек привязки графического элемента
			/// </summary>
			/// <remarks>
			/// Точки привязки позволяют более удобно привязываться к различным частям графического элемента
			/// </remarks>
			/// <returns>Точки привязки графического элемента</returns>
			//---------------------------------------------------------------------------------------------------------
			IList<Vector2Df> GetSnapNodes();
			#endregion
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Базовая графический элемент с базовым взаимодействием
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		[DataContract]
		public class CCadElement : CCadObject, ICadElement, IComparable<ICadElement>, IComparable<CCadElement>
		{
			#region ======================================= СТАТИЧЕСКИЕ ДАННЫЕ ========================================
			// Основные параметры
			protected static PropertyChangedEventArgs PropertyArgsIsVisible = new PropertyChangedEventArgs(nameof(IsVisible));
			protected static PropertyChangedEventArgs PropertyArgsIsHalftone = new PropertyChangedEventArgs(nameof(IsHalftone));

			// Слой графического элемента
			protected static PropertyChangedEventArgs PropertyArgsLayer = new PropertyChangedEventArgs(nameof(Layer));
			protected static PropertyChangedEventArgs PropertyArgsLayerId = new PropertyChangedEventArgs(nameof(LayerId));
			protected static PropertyChangedEventArgs PropertyArgsLayerName = new PropertyChangedEventArgs(nameof(LayerName));
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			// Основные параметры
			protected internal Boolean mIsVisible = true;
			protected internal Boolean mIsHalftone;

			// Слой графического элемента
			protected internal CCadLayer mLayer;
			protected internal Int64 mLayerId;

			// Служебные данные
			protected internal Boolean mIsVisibleElement = true;
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			//
			// ПАРАМЕТРЫ ГРАФИКИ
			//
			/// <summary>
			/// Видимость графического элемента
			/// </summary>
			[DisplayName("Видимость")]
			[Description("Видимость графического элемента")]
			[Category(XInspectorGroupDesc.Graphics)]
			[LotusCategoryOrder(2)]
			[Display(GroupName = XInspectorGroupDesc.Graphics, Order = 0)]
			[DataMember]
			public Boolean IsVisible
			{
				get { return (mIsVisible); }
				set
				{
					if (mIsVisible != value)
					{
						SavePropertyToMemory(nameof(IsVisible));

						mIsVisible = value;
						NotifyPropertyChanged(PropertyArgsIsVisible);
						RaiseIsVisibleChanged();
					}
				}
			}

			/// <summary>
			/// Полутон графического элемента
			/// </summary>
			[DisplayName("Полутон")]
			[Description("Полутон графического элемента")]
			[Category(XInspectorGroupDesc.Graphics)]
			[Display(GroupName = XInspectorGroupDesc.Graphics, Order = 1)]
			[DataMember]
			public Boolean IsHalftone
			{
				get { return (mIsHalftone); }
				set
				{
					if (mIsHalftone != value)
					{
						SavePropertyToMemory(nameof(IsHalftone));

						mIsHalftone = value;
						NotifyPropertyChanged(PropertyArgsIsHalftone);
						RaiseIsHalftoneChanged();
					}
				}
			}

			//
			// ПАРАМЕТРЫ СЛОЯ
			//
			/// <summary>
			/// Идентификатор слоя в котором расположен графический элемент
			/// </summary>
			[Browsable(false)]
			[DataMember]
			public Int64 LayerId
			{
				get { return (mLayerId); }
				set
				{
					if (mLayerId != value)
					{
						mLayer = XCadLayerManager.GetFromId(mLayerId);
						mLayerId = mLayer.Id;
						NotifyPropertyChanged(PropertyArgsLayer);
						NotifyPropertyChanged(PropertyArgsLayerId);
						NotifyPropertyChanged(PropertyArgsLayerName);
						RaiseLayerChanged();
					}
				}
			}

			/// <summary>
			/// Названия слоя в котором расположен графический элемент
			/// </summary>
			[DisplayName("Название слоя")]
			[Description("Названия слоя в котором расположен графический элемент")]
			[Category(XInspectorGroupDesc.Graphics)]
			[Display(GroupName = XInspectorGroupDesc.Graphics, Order = 4)]
			public String LayerName
			{
				get { return (mLayer.Name); }
				set
				{
					if (mLayer.Name != value)
					{
						mLayer = XCadLayerManager.GetFromName(value);
						mLayerId = mLayer.Id;
						NotifyPropertyChanged(PropertyArgsLayer);
						NotifyPropertyChanged(PropertyArgsLayerName);
						NotifyPropertyChanged(PropertyArgsLayerId);
						RaiseLayerChanged();
					}
				}
			}

			/// <summary>
			/// Слой в котором расположен графический элемент
			/// </summary>
			[DisplayName("Слой")]
			[Description("Слой в котором расположен графический элемент")]
			[Category(XInspectorGroupDesc.Graphics)]
			[Display(GroupName = XInspectorGroupDesc.Graphics, Order = 5)]
#if USE_WINDOWS
			[LotusInspectorTypeEditor(typeof(LotusEditorSelectorLayer))]
#endif
			public CCadLayer Layer
			{
				get { return (mLayer); }
				set
				{
					if (mLayer != value)
					{
						SavePropertyToMemory(nameof(Layer));

						mLayer = value;
						mLayerId = mLayer.Id;
						NotifyPropertyChanged(PropertyArgsLayer);
						NotifyPropertyChanged(PropertyArgsLayerId);
						NotifyPropertyChanged(PropertyArgsLayerName);
						RaiseLayerChanged();
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
			public CCadElement()
				:this("Новый объект")
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор инициализирует объект класса указанными параметрами
			/// </summary>
			/// <param name="name">Имя графического элемента</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadElement(String name)
				: base(name)
			{
				// Данные по умолчанию
				mLayer = XCadLayerManager.DefaultLayer;
				mLayerId = mLayer.Id;
			}
			#endregion

			#region ======================================= СИСТЕМНЫЕ МЕТОДЫ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Сравнение графических элементов для упорядочивания (по Z индексу)
			/// </summary>
			/// <param name="other">Сравниваемый графический элемент</param>
			/// <returns>Статус сравнения графических элементов</returns>
			//---------------------------------------------------------------------------------------------------------
			public Int32 CompareTo(ICadElement other)
			{
				if (ZIndex > other.ZIndex)
				{
					return (1);
				}
				else
				{
					if (ZIndex < other.ZIndex)
					{
						return (-1);
					}
					else
					{
						return (0);
					}
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Сравнение графических элементов для упорядочивания (по Z индексу)
			/// </summary>
			/// <param name="other">Сравниваемый графический элемент</param>
			/// <returns>Статус сравнения графических элементов</returns>
			//---------------------------------------------------------------------------------------------------------
			public Int32 CompareTo(CCadElement other)
			{
				return (this.CompareTo(other as ICadElement));
			}
			#endregion

			#region ======================================= CЛУЖЕБНЫЕ МЕТОДЫ СОБЫТИЙ ==================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение видимости графического элемента.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseIsVisibleChanged()
			{
				CheckVisibleElement();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение полутона графического элемента.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseIsHalftoneChanged()
			{
				CheckHalftoneElement();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение слоя графического элемента.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected virtual void RaiseLayerChanged()
			{
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение статуса расположения на канве графического элемента.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected override void RaiseIsCanvasChanged()
			{

			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Изменение статуса печати графического элемента.
			/// Метод автоматически вызывается после установки соответствующего свойства
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			protected override void RaiseIsPrintingChanged()
			{

			}
			#endregion

			#region ======================================= МЕТОДЫ ILotusCopyParameters ===============================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Копирование параметров с указанного объекта
			/// </summary>
			/// <param name="source_object">Объект-источник с которого будут скопированы параметры</param>
			/// <param name="parameters">Контекст копирования параметров</param>
			//---------------------------------------------------------------------------------------------------------
			public override void CopyParameters(System.Object source_object, CParameters parameters)
			{
				base.CopyParameters(source_object, parameters);

				if (source_object is ICadElement cad_element)
				{
					// 2) Основные параметры
					mIsVisible = cad_element.IsVisible;
					mIsHalftone = cad_element.IsHalftone;

					mLayerId = cad_element.LayerId;
					mLayer = XCadLayerManager.GetFromId(mLayerId);

					NotifyPropertyChanged(PropertyArgsIsVisible);
					NotifyPropertyChanged(PropertyArgsIsHalftone);
					NotifyPropertyChanged(PropertyArgsLayer);
					NotifyPropertyChanged(PropertyArgsLayerId);
					NotifyPropertyChanged(PropertyArgsLayerName);

					CheckHalftoneElement();
					CheckVisibleElement();
				}
			}
			#endregion

			#region ======================================= МЕТОДЫ ILotusMementoOriginator ============================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получить состояние объекта
			/// </summary>
			/// <remarks>
			/// Под наименованием состояния объекта будем подразумевать имя свойства
			/// </remarks>
			/// <param name="name_state">Наименование состояния объекта</param>
			/// <returns>Состояние объекта</returns>
			//---------------------------------------------------------------------------------------------------------
			public override System.Object GetMemento(String name_state)
			{
				System.Object result = base.GetMemento(name_state);
				if (result != null)
				{
					return (result);
				}

				switch (name_state)
				{
					case nameof(IsVisible):
						{
							result = IsVisible;
						}
						break;
					case nameof(IsHalftone):
						{
							result = IsHalftone;
						}
						break;
					case nameof(Layer):
						{
							result = Layer;
						}
						break;
					default:
						break;
				}

				return (result);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установить состояние объекта
			/// </summary>
			/// <remarks>
			/// Под наименованием состояния объекта будем подразумевать имя свойства
			/// </remarks>
			/// <param name="memento">Состояние объекта</param>
			/// <param name="name_state">Наименование состояния объекта</param>
			//---------------------------------------------------------------------------------------------------------
			public override void SetMemento(System.Object memento, String name_state)
			{
				base.SetMemento(memento, name_state);

				switch (name_state)
				{
					case nameof(IsVisible):
						{
							mIsVisible = (Boolean)memento;
							NotifyPropertyChanged(PropertyArgsIsVisible);
							CheckVisibleElement();
						}
						break;
					case nameof(IsHalftone):
						{
							mIsHalftone = (Boolean)memento;
							NotifyPropertyChanged(PropertyArgsIsHalftone);
							CheckHalftoneElement();
						}
						break;
					case nameof(Layer):
						{
							if (memento is CCadLayer layer)
							{
								mLayer = layer;
								mLayerId = layer.Id;
								NotifyPropertyChanged(PropertyArgsLayer);
								NotifyPropertyChanged(PropertyArgsLayerId);
								NotifyPropertyChanged(PropertyArgsLayerName);
							}
						}
						break;
					default:
						break;
				}
			}
			#endregion

			#region ======================================= МЕТОДЫ ИНТЕРФЕЙСОВ СОХРАНЕНИЯ/ЗАГРУЗКИ ====================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Подготовка элемента перед сохранением
			/// </summary>
			/// <param name="parameters">Параметры контекста</param>
			//---------------------------------------------------------------------------------------------------------
			public override void OnBeforeSave(CParameters parameters)
			{
				base.OnBeforeSave(parameters);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Восстановление параметров элемента после сохранения
			/// </summary>
			/// <param name="parameters">Параметры контекста</param>
			//---------------------------------------------------------------------------------------------------------
			public override void OnAfterSave(CParameters parameters)
			{
				base.OnAfterSave(parameters);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Подготовка элемента перед загрузкой
			/// </summary>
			/// <param name="parameters">Параметры контекста</param>
			//---------------------------------------------------------------------------------------------------------
			public override void OnBeforeLoad(CParameters parameters)
			{
				base.OnBeforeLoad(parameters);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Восстановление параметров элемента после загрузки
			/// </summary>
			/// <param name="parameters">Параметры контекста</param>
			//---------------------------------------------------------------------------------------------------------
			public override void OnAfterLoad(CParameters parameters)
			{
				base.OnAfterLoad(parameters);

				mLayer = XCadLayerManager.GetFromId(mLayerId);

				CheckHalftoneElement();
				CheckVisibleElement();
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Дубликат объекта
			/// </summary>
			/// <param name="context">Контекст дублирования объекта</param>
			/// <returns>Объект</returns>
			//---------------------------------------------------------------------------------------------------------
			public override ICadObject Duplicate(System.Object context)
			{
				CCadElement cad_element = new CCadElement();
				cad_element.CopyParameters(this, null);
				return (cad_element);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение точек привязки графического элемента
			/// </summary>
			/// <remarks>
			/// Точки привязки позволяют более удобно привязываться к различным частям графического элемента
			/// </remarks>
			/// <returns>Точки привязки графического элемента</returns>
			//---------------------------------------------------------------------------------------------------------
			public virtual IList<Vector2Df> GetSnapNodes()
			{
				return (null);
			}
			#endregion

			#region ======================================= МЕТОДЫ РАБОТЫ СО СЛОЕМ ====================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка на видимость графического элемента с учетом настроек слоя
			/// </summary>
			/// <returns>Видимость графического элемента </returns>
			//---------------------------------------------------------------------------------------------------------
			internal virtual Boolean CheckVisibleElement()
			{
				if ((mLayer.VisibleMode == TCadLayerVisibleMode.Hidden) ||
					(mLayer.VisibleMode == TCadLayerVisibleMode.VisibleIsHidden && mIsVisible) ||
					(mLayer.VisibleMode == TCadLayerVisibleMode.Visible && mIsVisible == false))
				{

					mIsVisibleElement = false;
				}
				else
				{
					mIsVisibleElement = true;
				}

				return (mIsVisibleElement);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Проверка на полутон графического элемента с учетом настроек слоя
			/// </summary>
			/// <returns>Полутон графического элемента </returns>
			//---------------------------------------------------------------------------------------------------------
			internal virtual Boolean CheckHalftoneElement()
			{
				if ((mLayer.HalftoneMode == TCadLayerHalftoneMode.Disable && mIsHalftone) ||
					mLayer.HalftoneMode == TCadLayerHalftoneMode.Halftone ||
					mLayer.HalftoneMode == TCadLayerHalftoneMode.HalftoneGray)
				{
					//this.Opacity = 0.5f;
					return (true);
				}
				else
				{
					//this.Opacity = 1;
					return (false);
				}
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление слоя для графического элемента
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public virtual void UpdateLayer()
			{
				CheckVisibleElement();
				CheckHalftoneElement();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка слоя для графического элемента
			/// </summary>
			/// <param name="layer">Слой</param>
			//---------------------------------------------------------------------------------------------------------
			public virtual void SetLayer(CCadLayer layer)
			{
				mLayer = layer;
				mLayerId = mLayer.Id;

				NotifyPropertyChanged(PropertyArgsLayer);
				NotifyPropertyChanged(PropertyArgsLayerId);
				NotifyPropertyChanged(PropertyArgsLayerName);

				CheckVisibleElement();
				CheckHalftoneElement();
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
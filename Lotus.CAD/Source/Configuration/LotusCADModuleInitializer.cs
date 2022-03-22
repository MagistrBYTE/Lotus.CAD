//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Элементы управления
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADModuleInitializer.cs
*		Инициализация модуля чертежной графики.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.01.2022
//=====================================================================================================================
using System;
using System.ComponentModel;
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
		//! \addtogroup CadDrawing
		/*@{*/
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Статический класс - инициализатор модуля чертежной графики
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public static class XCadModuleInitializer
		{
			#region ======================================= ДАННЫЕ ====================================================
#if USE_WINDOWS
			internal static Windows.SharedResourceDictionary mGraphicsResources;
#endif
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
#if USE_WINDOWS
			/// <summary>
			/// Глобальный кэш графических ресурсов
			/// </summary>
			public static Windows.SharedResourceDictionary GraphicsResources
			{
				get 
				{
					if (mGraphicsResources == null)
					{
						mGraphicsResources = new Windows.SharedResourceDictionary();
						mGraphicsResources.Source = new Uri(UriGraphicsResources, UriKind.Absolute);
					}
					return (mGraphicsResources);
				}
			}

			/// <summary>
			/// Словарь графических ресурсов
			/// </summary>
			public static String UriGraphicsResources
			{
				get 
				{
					return ("pack://application:,,,/Lotus.Windows;component/Themes/GraphicsResources.xaml");
				}
			}
#endif
			#endregion

			#region ======================================= МЕТОДЫ ====================================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Первоначальная инициализация диспетчера управления подсистемой чертежной графики
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public static void Init()
			{
#if USE_WINDOWS
				Windows.XWindowsColorManager.Init();

				if (mGraphicsResources == null)
				{
					mGraphicsResources = new Windows.SharedResourceDictionary();
					mGraphicsResources.Source = new Uri(UriGraphicsResources, UriKind.Absolute);
				}
#endif
				XCadBrushManager.Init();
				XCadPenStyleManager.Init();
				XCadPenManager.Init();
				XCadLayerManager.Init();
				XCadFontManager.Init();
				XCadPaperManager.Init();
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
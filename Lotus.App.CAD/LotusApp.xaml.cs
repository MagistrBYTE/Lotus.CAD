﻿//=====================================================================================================================
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
//=====================================================================================================================
namespace Lotus.CAD
{
	//-----------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	//-----------------------------------------------------------------------------------------------------------------
	public partial class App : Application
	{
		#region =========================================== ДАННЫЕ ====================================================
		private static Dictionary<String, Cursor> mCursors;
		#endregion

		#region =========================================== СВОЙСТВА ==================================================
		/// <summary>
		/// Дополнительные курсоры для проекта
		/// </summary>
		public static Dictionary<String, Cursor> Cursors
		{
			get { return (mCursors); }
		}
		#endregion

		#region =========================================== ПЕРЕГРУЖАЕМЫЕ МЕТОДЫ ======================================
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Начальная загрузка приложения
		/// </summary>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		protected override void OnStartup(StartupEventArgs args)
		{
			base.OnStartup(args);
		}
		#endregion
	}
}
//=====================================================================================================================
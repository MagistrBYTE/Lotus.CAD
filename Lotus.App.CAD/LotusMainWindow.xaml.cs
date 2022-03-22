//=====================================================================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Markup;
using System.Globalization;
//---------------------------------------------------------------------------------------------------------------------
using AvalonDock.Layout;
using Xceed.Wpf.Toolkit.PropertyGrid;
//---------------------------------------------------------------------------------------------------------------------
using Fluent;
//---------------------------------------------------------------------------------------------------------------------
using Lotus.Core;
using Lotus.Windows;
//=====================================================================================================================
namespace Lotus.CAD
{
	//-----------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	//-----------------------------------------------------------------------------------------------------------------
	public partial class MainWindow : RibbonWindow, INotifyPropertyChanged
	{
		#region =========================================== СТАТИЧЕСКИЕ ДАННЫЕ ========================================
		// Основные параметры
		protected static PropertyChangedEventArgs PropertyArgsProject = new PropertyChangedEventArgs(nameof(Project));
		protected static PropertyChangedEventArgs PropertyArgsPresentedDraft = new PropertyChangedEventArgs(nameof(PresentedDraft));
		protected static PropertyChangedEventArgs PropertyArgsCanvasViewer = new PropertyChangedEventArgs(nameof(CanvasViewer));
		protected static PropertyChangedEventArgs PropertyArgsCanvas = new PropertyChangedEventArgs(nameof(Canvas));
		protected static PropertyChangedEventArgs PropertyArgsMemento = new PropertyChangedEventArgs(nameof(Memento));
		#endregion

		#region =========================================== ДАННЫЕ ====================================================
		private CCadProject mProject;
		private CCadDraft mPresentedDraft;
		#endregion

		#region =========================================== СВОЙСТВА ==================================================
		//
		// ОСНОВНЫЕ ПАРАМЕТРЫ
		//
		/// <summary>
		/// Текущий проект
		/// </summary>
		public CCadProject Project
		{
			get { return (mProject); }
		}

		/// <summary>
		/// Текущий отображаемый чертеж
		/// </summary>
		public CCadDraft PresentedDraft
		{
			get { return (mPresentedDraft); }
		}

		/// <summary>
		/// Элемент пользовательского интерфейса для представления и управления канвой
		/// </summary>
		public LotusControlCanvasViewer ControlCanvasViewer { get; set; }

		/// <summary>
		/// Элемент для управления канвой
		/// </summary>
		public LotusCadCanvasViewer CanvasViewer 
		{ 
			get { return (ControlCanvasViewer?.Viewer); } 
		}

		/// <summary>
		/// Канва
		/// </summary>
		public LotusCadCanvas Canvas
		{
			get { return (ControlCanvasViewer?.Canvas); }
		}

		/// <summary>
		/// Менеджер для отмены/повторения действия
		/// </summary>
		public ILotusMementoManager Memento
		{
			get { return (ControlCanvasViewer?.Memento); }
		}
		#endregion

		#region =========================================== КОНСТРУКТОРЫ ==============================================
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public MainWindow()
		{
			this.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
			InitializeComponent();
		}
		#endregion

		#region =========================================== ОБРАБОТЧИКИ СОБЫТИЙ - ГЛАВНОЕ ОКНО ========================
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Загрузка основного окна и готовность к представлению
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnMainWindow_Loaded(Object sender, RoutedEventArgs args)
		{
			// Инициализируем графические ресурсы
			XCadModuleInitializer.Init();

			//// Присваиваем иконки панелям
			layoutAnchorableSolutionExplore.IconSource = XResources.Fatcow_folders_explorer_32.ToBitmapSource();
			layoutAnchorableInspectorProperties.IconSource = XResources.NuoveXT_document_properties_16.ToBitmapSource();
			layoutAnchorableLogger.IconSource = XResources.Oxygen_utilities_log_viewer_16.ToBitmapSource();

			// Устанавливаем глобальные данные по элементам управления
			XWindowManager.PropertyInspector = inspectorProperty;
			XLogger.Logger = logger;
			XFileDialog.FileDialogs = new CFileDialogsWindows();

			XSerializationDispatcher.SerializerJson = new CSerializerJson();
			XSerializationDispatcher.SerializerJson.Serializer.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto;
			XSerializationDispatcher.SerializerJson.Serializer.Formatting = Newtonsoft.Json.Formatting.Indented;
			XSerializationDispatcher.SerializerJson.Serializer.Converters.AddIfNotContains(Maths.Vector2DfConverter.Instance);


			// Присваиваем команды
			CommandBindings.Add(new CommandBinding(XCommandManager.FileNew, OnFileNew));
			CommandBindings.Add(new CommandBinding(XCommandManager.FileOpen, OnFileOpen));
			CommandBindings.Add(new CommandBinding(XCommandManager.FileSave, OnFileSave));
			CommandBindings.Add(new CommandBinding(XCommandManager.FileSaveAs, OnFileSaveAs));
			CommandBindings.Add(new CommandBinding(XCommandManager.FilePrint, OnFilePrint));
			CommandBindings.Add(new CommandBinding(XCommandManager.FileExport, OnFileExport));
			CommandBindings.Add(new CommandBinding(XCommandManager.FileClose, OnFileClose));

			dockingManager.Theme = new AvalonDock.Themes.AeroTheme();

			this.DataContext = this;
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Закрытие основного окна
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnMainWindow_Closing(Object sender, CancelEventArgs args)
		{
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Закрытие основного окна
		/// </summary>
		/// <remarks>
		/// Применяется при закрытие другим способом
		/// </remarks>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnMainWindowClose(Object sender, RoutedEventArgs args)
		{
			Close();
		}
		#endregion

		#region =========================================== ОБРАБОТЧИКИ СОБЫТИЙ - ФАЙЛ ================================
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Создание файла
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnFileNew(Object sender, RoutedEventArgs args)
		{
			// Создаем проект и чертежи
			mProject = new CCadProject("Новый проект", "Первый чертеж", "Второй чертеж");

			treeSolutionExplore.ItemTemplateSelector = CCadEntityDataSelector.Instance;
			treeSolutionExplore.ItemsSource = mProject.GetCollectionViewHierarchy();

			mPresentedDraft = mProject.FirstDraft as CCadDraft;

			AddDockingManagerPaneDraft(mPresentedDraft);

			NotifyPropertyChanged(PropertyArgsProject);
			NotifyPropertyChanged(PropertyArgsPresentedDraft);
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Открытие файла
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnFileOpen(Object sender, RoutedEventArgs args)
		{
			var project = XExtensionDocument.LoadDocument<CCadProject>();
			if (project != null)
			{
				mProject = project;
				mPresentedDraft = mProject.FirstDraft as CCadDraft;
				treeSolutionExplore.ItemTemplateSelector = CCadEntityDataSelector.Instance;
				treeSolutionExplore.ItemsSource = mProject.GetCollectionViewHierarchy();

				if (CanvasViewer == null)
				{
					AddDockingManagerPaneDraft(mPresentedDraft);
				}
				else
				{
					CanvasViewer.Draft = mPresentedDraft;
				}

				NotifyPropertyChanged(PropertyArgsProject);
				NotifyPropertyChanged(PropertyArgsPresentedDraft);
			}
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Сохранение файла
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnFileSave(Object sender, RoutedEventArgs args)
		{
			if (mProject != null)
			{
				mProject.SaveDocument();
			}
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Сохраннее файла под другим имением
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnFileSaveAs(Object sender, RoutedEventArgs args)
		{
			if (mProject != null)
			{
				mProject.SaveAsDocument();
			}
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Печать файла
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnFilePrint(Object sender, RoutedEventArgs args)
		{

		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Экспорт файла
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnFileExport(Object sender, RoutedEventArgs args)
		{

		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Закрытие файла
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnFileClose(Object sender, RoutedEventArgs args)
		{
			treeSolutionExplore.ItemsSource = null;
			CanvasViewer.Draft = null;
			mPresentedDraft = null;
			mProject = null;

			NotifyPropertyChanged(PropertyArgsProject);
			NotifyPropertyChanged(PropertyArgsPresentedDraft);
		}
		#endregion

		#region =========================================== ОБРАБОТЧИКИ СОБЫТИЙ - РЕДАКТИРОВАНИЕ ======================
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Быстрое сохранение
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnEditSave(Object sender, RoutedEventArgs args)
		{
			if (mProject != null)
			{
				mProject.SaveDocument();
			}
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Копирование в буфер обмена
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnEditCopy(Object sender, RoutedEventArgs args)
		{
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Вырезать объект в буфер обмена
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnEditCut(Object sender, RoutedEventArgs args)
		{

		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Вставка из буфера обмена
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnEditPaste(Object sender, RoutedEventArgs args)
		{

		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Отмена последнего действия
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnEditUndo(Object sender, RoutedEventArgs args)
		{
			if(Memento != null)
			{
				Memento.Undo();
			}
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Повтор отменённого действия
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnEditRedo(Object sender, RoutedEventArgs args)
		{
			if (Memento != null)
			{
				Memento.Redo();
			}
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Общие обновление
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnEditRefresh(Object sender, RoutedEventArgs args)
		{

		}
		#endregion

		#region =========================================== ОБРАБОТЧИКИ СОБЫТИЙ - СОЗДАНИЕ ФИГУР ======================
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Управление указателем
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnToolPointer(Object sender, RoutedEventArgs args)
		{
			if (ControlCanvasViewer != null)
			{
				ControlCanvasViewer.Viewer.Tool = null;
			}

			if (ribbonButtonCreateLine.IsChecked.GetValueOrDefault()) ribbonButtonCreateLine.IsChecked = false;
			if (ribbonButtonCreateRect.IsChecked.GetValueOrDefault()) ribbonButtonCreateRect.IsChecked = false;
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Создание графического элемента - Линии
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnToolCreateLine(Object sender, RoutedEventArgs args)
		{
			if (CanvasViewer != null)
			{
				ControlCanvasViewer.Viewer.Tool = new CCadToolCreateLine(ControlCanvasViewer.Viewer);
			}
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Создание графического элемента - Прямоугольника
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnToolCreateRect(Object sender, RoutedEventArgs args)
		{
			if (CanvasViewer != null)
			{
				ControlCanvasViewer.Viewer.Tool = new CCadToolCreateRect(ControlCanvasViewer.Viewer);
			}
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Создание графического элемента - Эллипса.
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnToolCreateEllipse(Object sender, RoutedEventArgs args)
		{

		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Создание графического элемента - Текста.
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnToolCreateText(Object sender, RoutedEventArgs args)
		{

		}
		#endregion

		#region =========================================== ОБРАБОТЧИКИ СОБЫТИЙ - РЕДАКТИРОВАНИЕ ФИГУР ================
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Копирование графического элемента.
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnToolEditCopy(Object sender, RoutedEventArgs args)
		{
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Перемещение графического элемента.
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnToolEditMove(Object sender, RoutedEventArgs args)
		{
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Вращение графического элемента.
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnToolEditRotate(Object sender, RoutedEventArgs args)
		{
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Масштабирование (изменение размеров) графического элемента
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnToolEditScale(Object sender, RoutedEventArgs args)
		{
		}
		#endregion

		#region =========================================== ОБРАБОТЧИКИ СОБЫТИЙ - МЕНЕДЖЕР ОКОН =======================
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Активация вкладки
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnDockingManager_ActiveContentChanged(Object sender, EventArgs args)
		{
			if (dockingManager.ActiveContent is LotusControlCanvasViewer canvas_viewer)
			{
				if(ControlCanvasViewer != canvas_viewer)
				{
					ControlCanvasViewer = canvas_viewer;
					NotifyPropertyChanged(PropertyArgsCanvasViewer);
					NotifyPropertyChanged(PropertyArgsCanvas);
					NotifyPropertyChanged(PropertyArgsMemento);
				}
			}
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Добавление докумета
		/// </summary>
		/// <param name="draft">Чертеж</param>
		//-------------------------------------------------------------------------------------------------------------
		private void AddDockingManagerPaneDraft(ICadDraft draft)
		{
			// Создаем канву
			LotusControlCanvasViewer canvasViewer = new LotusControlCanvasViewer();

			// Создаем документ
			LayoutDocument layout_document = new LayoutDocument();
			layout_document.CanFloat = true;
			layout_document.Title = draft.Name;
			layout_document.ToolTip = draft.Name;

			// Присваиваем контент
			layout_document.Content = canvasViewer;

			// Вставляем в начала
			layoutDocumentPane.InsertChildAt(0, layout_document);

			// Активируем
			layout_document.IsSelected = true;

			canvasViewer.Viewer.Draft = mPresentedDraft;
			canvasViewer.Viewer.SnapIsEnabled = true;
			canvasViewer.Viewer.CreateModeIsAutoCAD = true;
		}
		#endregion

		#region =========================================== ОБРАБОТЧИКИ СОБЫТИЙ - ОБОЗРЕВАТЕЛЬ ФАЙЛОВОЙ СИСТЕМ ========
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Выбор источника данных для обозревателя файловой системы
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnButtonFileSystemSourceOpen_Click(Object sender, RoutedEventArgs args)
		{
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Восстановление источника данных для обозревателя файловой системы
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnButtonFileSystemSourceRefresh_Click(Object sender, RoutedEventArgs args)
		{

		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Выбор источника данных для обозревателя файловой системы
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnComboFileSystemSource_SelectionChanged(Object sender, SelectionChangedEventArgs args)
		{
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Представление элемента отображения в отдельном контексте
		/// </summary>
		/// <param name="item">Элемент отображения</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnTreeExploreFileSystem_PresentedItem(ILotusViewItemHierarchy item)
		{
			if (item.DataContext is CFileSystemFile file)
			{

			}
		}
		#endregion

		#region =========================================== ОБРАБОТЧИКИ СОБЫТИЙ - ОБОЗРЕВАТЕЛЬ ПРОЕКТА ================
		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Выбор элемента в обозревателе проекта
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnSolutionExplorer_SelectedItemChanged(Object sender, RoutedPropertyChangedEventArgs<Object> args)
		{

		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Двойной шелчок по элементу в обозревателе проекта
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnSolutionExplorer_MouseDoubleClick(Object sender, MouseButtonEventArgs args)
		{
			ILotusViewItemHierarchy view_item = treeSolutionExplore.SelectedItem as ILotusViewItemHierarchy;
			if (view_item != null && view_item.DataContext != null)
			{
				if(view_item.DataContext is ICadDraft cad_draft)
				{
					//view_item.IsPresented = true;
					CanvasViewer.Draft = (CCadDraft)cad_draft;
				}
			}
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Начало перетаскивания элемента в обозревателе проекта
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnSolutionExplorer_DragStart(Object sender, DragEventArgs args)
		{
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Событие постоянно возникает при перетаскивании данных над объектом-приемником
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnSolutionExplorer_DragOver(Object sender, DragEventArgs args)
		{
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Проверка типа перетаскиваемых данных и определение типа разрешаемой операции
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnSolutionExplorer_DragEnter(Object sender, DragEventArgs args)
		{
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Событие постоянно возникает при покидании объекта-приемника
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnSolutionExplorer_DragLeave(Object sender, DragEventArgs args)
		{
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Событие возникает, когда данные сбрасываются над объектом-приемником; по умолчанию это происходит 
		/// при отпускании кнопки мыши.
		/// </summary>
		/// <param name="sender">Источник события</param>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		private void OnSolutionExplorer_Drop(Object sender, DragEventArgs args)
		{
		}
		#endregion

		#region =========================================== ДАННЫЕ INotifyPropertyChanged =============================
		/// <summary>
		/// Событие срабатывает ПОСЛЕ изменения свойства
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Вспомогательный метод для нотификации изменений свойства.
		/// </summary>
		/// <param name="property_name">Имя свойства</param>
		//-------------------------------------------------------------------------------------------------------------
		public void NotifyPropertyChanged(String property_name = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(property_name));
			}
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Вспомогательный метод для нотификации изменений свойства.
		/// </summary>
		/// <param name="args">Аргументы события</param>
		//-------------------------------------------------------------------------------------------------------------
		public void NotifyPropertyChanged(PropertyChangedEventArgs args)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, args);
			}
		}

		#endregion
	}
}
//=====================================================================================================================
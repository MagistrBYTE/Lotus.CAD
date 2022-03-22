//=====================================================================================================================
// Решение: LotusPlatform
// Раздел: Модуль чертежной графики
// Подраздел: Общая подсистема
// Автор: MagistrBYTE aka DanielDem <dementevds@gmail.com>
//---------------------------------------------------------------------------------------------------------------------
/** \file LotusCADCommonTransform.cs
*		Определения интерфейсов трансформации.
*/
//---------------------------------------------------------------------------------------------------------------------
// Версия: 1.0.0.0
// Последнее изменение от 30.01.2022
//=====================================================================================================================
using System;
using System.ComponentModel;
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
		/// Базовый интерфейс для определения трансформации графического объекта
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public interface ICadTransform
		{
			#region ======================================= МЕТОДЫ ====================================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перемещение трансформируемого объекта
			/// </summary>
			/// <param name="offset">Значение смещения в условных единицах</param>
			//---------------------------------------------------------------------------------------------------------
			void Move(ref Vector2Df offset);

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перемещение трансформируемого объекта вверх
			/// </summary>
			/// <param name="offset">Значение смещения в условных единицах</param>
			//---------------------------------------------------------------------------------------------------------
			void MoveUp(Single offset);

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перемещение трансформируемого объекта вниз
			/// </summary>
			/// <param name="offset">Значение смещения в условных единицах</param>
			//---------------------------------------------------------------------------------------------------------
			void MoveDown(Single offset);

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перемещение трансформируемого объекта влево
			/// </summary>
			/// <param name="offset">Значение смещения в условных единицах</param>
			//---------------------------------------------------------------------------------------------------------
			void MoveLeft(Single offset);

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перемещение трансформируемого объекта вправо
			/// </summary>
			/// <param name="offset">Значение смещения в условных единицах</param>
			//---------------------------------------------------------------------------------------------------------
			void MoveRight(Single offset);
			#endregion
		}

		//-------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Класс для представления трансформации объекта
		/// </summary>
		//-------------------------------------------------------------------------------------------------------------
		public class CCadTransform : ICadTransform
		{
			#region ======================================= КОНСТАНТНЫЕ ДАННЫЕ ========================================
			private static Matrix3Dx2f SubsidiaryMatrixTranslate = Matrix3Dx2f.Identity;
			private static Matrix3Dx2f SubsidiaryMatrixInvert = Matrix3Dx2f.Identity;
			#endregion

			#region ======================================= ДАННЫЕ ====================================================
			// Основные параметры
			internal Vector2Df mPosition;
			internal Single mRotationAngle;
			internal Vector2Df mRotationOrigin;
			internal Matrix3Dx2f mMatrixRotation;
			internal Matrix3Dx2f mMatrixTransform;

			// Платформенные-зависимые данные
#if USE_WINDOWS
			internal System.Windows.Media.MatrixTransform mWindowsTransform;
#endif
			#endregion

			#region ======================================= СВОЙСТВА ==================================================
			/// <summary>
			/// Позиция трансформации объекта по X
			/// </summary>
			[DisplayName("X")]
			[Description("Позиция трансформации объекта по X")]
			public Single X
			{
				get { return (mPosition.X); }
				set
				{
					mPosition.X = value;
					UpdateTransform();
				}
			}

			/// <summary>
			/// Позиция трансформации объекта по Y
			/// </summary>
			[DisplayName("Y")]
			[Description("Позиция трансформации объекта по Y")]
			public Single Y
			{
				get { return (mPosition.Y); }
				set
				{
					mPosition.Y = value;
					UpdateTransform();
				}
			}

			/// <summary>
			/// Позиция трансформации объекта
			/// </summary>
			[Browsable(false)]
			public Vector2Df Position
			{
				get { return (mPosition); }
				set
				{
					mPosition = value;
					UpdateTransform();
				}
			}

			/// <summary>
			/// Угол поворота объекта
			/// </summary>
			[DisplayName("Угол поворота")]
			[Description("Угол поворота объекта")]
			public Single RotationAngle
			{
				get { return (mRotationAngle); }
				set
				{
					mRotationAngle = value;
					Matrix3Dx2f.Rotation((Single)(XMath.DegreeToRadian_f * mRotationAngle), mRotationOrigin, out mMatrixRotation);
					UpdateTransform();
				}
			}

			/// <summary>
			/// Центр поворота
			/// </summary>
			[DisplayName("Центр поворота")]
			[Description("Центр поворота")]
			public Vector2Df RotationOrigin
			{
				get { return (mRotationOrigin); }
				set
				{
					mRotationOrigin = value;
					Matrix3Dx2f.Rotation((Single)(XMath.DegreeToRadian_f * mRotationAngle), mRotationOrigin, out mMatrixRotation);
					UpdateTransform();
				}
			}

			/// <summary>
			/// Центр поворота по X
			/// </summary>
			[DisplayName("Центр по X")]
			[Description("Центр поворота по X")]
			public Single RotationOriginX
			{
				get { return (mRotationOrigin.X); }
				set
				{
					mRotationOrigin.X = value;
					Matrix3Dx2f.Rotation((Single)(XMath.DegreeToRadian_f * mRotationAngle), mRotationOrigin, out mMatrixRotation);
					UpdateTransform();
				}
			}

			/// <summary>
			/// Центр поворота по Y
			/// </summary>
			[DisplayName("Центр по Y")]
			[Description("Центр поворота по Y")]
			public Single RotationOriginY
			{
				get { return (mRotationOrigin.Y); }
				set
				{
					mRotationOrigin.Y = value;
					Matrix3Dx2f.Rotation((Single)(XMath.DegreeToRadian_f * mRotationAngle), mRotationOrigin, out mMatrixRotation);
					UpdateTransform();
				}
			}

			/// <summary>
			/// Матрица трансформации
			/// </summary>
			[Browsable(false)]
			public Matrix3Dx2f MatrixTransform
			{
				get { return (mMatrixTransform); }
			}
			#endregion

			#region ======================================= КОНСТРУКТОРЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор по умолчанию инициализирует объект класса предустановленными значениями
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			public CCadTransform()
			{
				// Инициализируем модели перемещения
				mMatrixRotation = Matrix3Dx2f.Identity;
				mMatrixTransform = Matrix3Dx2f.Identity;

#if USE_WINDOWS
				mWindowsTransform = new System.Windows.Media.MatrixTransform();
#endif
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Конструктор копирования
			/// </summary>
			/// <param name="source">Источник копии</param>
			//---------------------------------------------------------------------------------------------------------
			public CCadTransform(CCadTransform source)
			{
#if USE_WINDOWS
				mWindowsTransform = new System.Windows.Media.MatrixTransform();
#endif

				// Инициализируем модели перемещения
				mPosition = source.Position;
				mRotationAngle = source.RotationAngle;
				mRotationOrigin = source.RotationOrigin;
				UpdateTransform();
			}
			#endregion

			#region ======================================= СЛУЖЕБНЫЕ МЕТОДЫ ==========================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Обновление параметров трансформации
			/// </summary>
			//---------------------------------------------------------------------------------------------------------
			private void UpdateTransform()
			{
				SubsidiaryMatrixTranslate.M31 = mPosition.X;
				SubsidiaryMatrixTranslate.M32 = mPosition.Y;
				Matrix3Dx2f.Multiply(ref mMatrixRotation, ref SubsidiaryMatrixTranslate, out mMatrixTransform);
#if USE_WINDOWS
				mWindowsTransform.Matrix = mMatrixTransform.ToWinMatrix();
#endif
			}
			#endregion

			#region ======================================= ОБЩИЕ МЕТОДЫ ==============================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Копирование параметров с трансформации
			/// </summary>
			/// <param name="transform">Трансформация</param>
			//---------------------------------------------------------------------------------------------------------
			public void CopyParamemtrs(CCadTransform transform)
			{
				mPosition = transform.Position;
				mRotationAngle = transform.RotationAngle;
				mRotationOrigin = transform.RotationOrigin;
				Matrix3Dx2f.Rotation((Single)(XMath.DegreeToRadian_f * mRotationAngle), mRotationOrigin, out mMatrixRotation);
				UpdateTransform();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение трансформированной точки в пространстве канвы
			/// </summary>
			/// <param name="point">Исходная точка</param>
			/// <returns>Трансформированная точка</returns>
			//---------------------------------------------------------------------------------------------------------
			public Vector2Df TransformPointToCanvas(Vector2Df point)
			{
				return (Matrix3Dx2f.TransformPoint(ref mMatrixTransform, ref point));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение трансформированной точки в пространстве канвы
			/// </summary>
			/// <param name="point">Исходная точка</param>
			/// <returns>Трансформированная точка</returns>
			//---------------------------------------------------------------------------------------------------------
			public Vector2Df TransformPointToCanvas(ref Vector2Df point)
			{
				return (Matrix3Dx2f.TransformPoint(ref mMatrixTransform, ref point));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение трансформированной точки в локальном пространстве графического элемента
			/// </summary>
			/// <param name="point">Исходная точка</param>
			/// <returns>Трансформированная точка</returns>
			//---------------------------------------------------------------------------------------------------------
			public Vector2Df TransformPointToLocal(Vector2Df point)
			{
				Matrix3Dx2f.Invert(ref mMatrixTransform, out SubsidiaryMatrixInvert);
				return (Matrix3Dx2f.TransformPoint(ref SubsidiaryMatrixInvert, ref point));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение трансформированной точки в локальном пространстве графического элемента
			/// </summary>
			/// <param name="point">Исходная точка</param>
			/// <returns>Трансформированная точка</returns>
			//---------------------------------------------------------------------------------------------------------
			public Vector2Df TransformPointToLocal(ref Vector2Df point)
			{
				Matrix3Dx2f.Invert(ref mMatrixTransform, out SubsidiaryMatrixInvert);
				return (Matrix3Dx2f.TransformPoint(ref SubsidiaryMatrixInvert, ref point));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение трансформированного вектора в локальном пространстве графического элемента
			/// </summary>
			/// <param name="vector">Исходный вектор</param>
			/// <returns>Трансформированный вектор</returns>
			//---------------------------------------------------------------------------------------------------------
			public Vector2Df TransformVectorToLocal(Vector2Df vector)
			{
				Matrix3Dx2f.Invert(ref mMatrixTransform, out SubsidiaryMatrixInvert);
				return (Matrix3Dx2f.TransformVector(ref SubsidiaryMatrixInvert, ref vector));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Получение трансформированного вектора в локальном пространстве графического элемента
			/// </summary>
			/// <param name="vector">Исходный вектор</param>
			/// <returns>Трансформированный вектор</returns>
			//---------------------------------------------------------------------------------------------------------
			public Vector2Df TransformVectorToLocal(ref Vector2Df vector)
			{
				Matrix3Dx2f.Invert(ref mMatrixTransform, out SubsidiaryMatrixInvert);
				return (Matrix3Dx2f.TransformVector(ref SubsidiaryMatrixInvert, ref vector));
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Трансформирование прямоугольника
			/// </summary>
			/// <param name="rect">Исходный прямоугольник</param>
			/// <returns>Трансформированый прямоугольник </returns>
			//---------------------------------------------------------------------------------------------------------
			public Rect2Df TransformBounds(Rect2Df rect)
			{
#if USE_WINDOWS
				System.Windows.Rect win_rect = new System.Windows.Rect(rect.X, rect.Y, rect.Width, rect.Height);
				win_rect = mWindowsTransform.TransformBounds(win_rect);
				Rect2Df result = new Rect2Df((Single)win_rect.X, (Single)win_rect.Y,
					(Single)win_rect.Width, (Single)win_rect.Height);
				return (result);
#else
				return (Rect2Df.Default);
#endif
			}
			#endregion

			#region ======================================= МЕТОДЫ ТРАНСФОРМАЦИИ ======================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка позиции
			/// </summary>
			/// <param name="x">Координата по X</param>
			/// <param name="y">Координата по Y</param>
			//---------------------------------------------------------------------------------------------------------
			public void SetPosition(Single x, Single y)
			{
				Position = new Vector2Df(x, y);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Установка позиции
			/// </summary>
			/// <param name="position">Позиция</param>
			//---------------------------------------------------------------------------------------------------------
			public void SetPosition(ref Vector2Df position)
			{
				Position = position;
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перемещение трансформируемого объекта
			/// </summary>
			/// <param name="offset">Значение смещения в условных единицах</param>
			//---------------------------------------------------------------------------------------------------------
			public void Move(ref Vector2Df offset)
			{
				mPosition.X += offset.X;
				mPosition.Y += offset.Y;
				UpdateTransform();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перемещение трансформируемого объекта вверх
			/// </summary>
			/// <param name="offset">Значение смещения в условных единицах</param>
			//---------------------------------------------------------------------------------------------------------
			public virtual void MoveUp(Single offset)
			{
				mPosition.Y += offset;
				UpdateTransform();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перемещение трансформируемого объекта вниз
			/// </summary>
			/// <param name="offset">Значение смещения в условных единицах</param>
			//---------------------------------------------------------------------------------------------------------
			public void MoveDown(Single offset)
			{
				mPosition.Y -= offset;
				UpdateTransform();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перемещение трансформируемого объекта влево.
			/// </summary>
			/// <param name="offset">Значение смещения в условных единицах</param>
			//---------------------------------------------------------------------------------------------------------
			public void MoveLeft(Single offset)
			{
				mPosition.X -= offset;
				UpdateTransform();
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Перемещение трансформируемого объекта вправо.
			/// </summary>
			/// <param name="offset">Значение смещения в условных единицах</param>
			//---------------------------------------------------------------------------------------------------------
			public void MoveRight(Single offset)
			{
				mPosition.X += offset;
				UpdateTransform();
			}
			#endregion

			#region ======================================= МЕТОДЫ СЕРИАЛИЗАЦИИ =======================================
			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Запись свойств и данных трансформируемого объекта в формат атрибутов XML
			/// </summary>
			/// <param name="prefix">Префикс имени атрибута</param>
			/// <param name="xml_writer">Средство записи данных в формат XML</param>
			//---------------------------------------------------------------------------------------------------------
			public void WriteTransformableToAttribute(String prefix, XmlWriter xml_writer)
			{
				//xml_writer.WriteVector2DToAttribute(prefix + "Position", mPosition);
				//xml_writer.WriteSingleToAttribute(prefix + "RotationAngle", mRotationAngle);
				//xml_writer.WriteVector2DToAttribute(prefix + "RotationOrigin", mRotationOrigin);
			}

			//---------------------------------------------------------------------------------------------------------
			/// <summary>
			/// Чтение свойств и данных трансформируемого объекта из формата атрибутов XML
			/// </summary>
			/// <param name="prefix">Префикс имени атрибута</param>
			/// <param name="xml_reader">Средство чтения данных формата XML</param>
			//---------------------------------------------------------------------------------------------------------
			public void ReadTransformableFromAttribute(String prefix, XmlReader xml_reader)
			{
				//mPosition = xml_reader.ReadMathVector2DfFromAttribute(prefix + "Position");
				//mRotationAngle = xml_reader.ReadSingleFromAttribute(prefix + "RotationAngle");
				//mRotationOrigin = xml_reader.ReadMathVector2DfFromAttribute(prefix + "RotationOrigin");
				UpdateTransform();
			}
			#endregion
		}
		//-------------------------------------------------------------------------------------------------------------
		/*@}*/
		//-------------------------------------------------------------------------------------------------------------
	}
}
//=====================================================================================================================
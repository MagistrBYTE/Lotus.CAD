//=====================================================================================================================
using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using Microsoft.JSInterop;
using System.ComponentModel;
using System.Reflection;
//=====================================================================================================================
namespace Lotus.Web
{
	namespace CAD
	{
		public class BoundingClientRect
		{
			public Double X { get; set; }
			public Double Y { get; set; }
			public Double Width { get; set; }
			public Double Height { get; set; }
			public Double Top { get; set; }
			public Double Right { get; set; }
			public Double Bottom { get; set; }
			public Double Left { get; set; }
		}

		class EnumDescriptionConverter<T> : JsonConverter<T> where T : IComparable, IFormattable, IConvertible
		{
			public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				String jsonValue = reader.GetString();

				foreach (var fi in typeToConvert.GetFields())
				{
					DescriptionAttribute description = (DescriptionAttribute)fi.GetCustomAttribute(typeof(DescriptionAttribute), false);

					if (description != null)
					{
						if (description.Description == jsonValue)
						{
							return (T)fi.GetValue(null);
						}
					}
				}
				throw new JsonException($"string {jsonValue} was not found as a description in the enum {typeToConvert}");
			}

			public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
			{
				FieldInfo fi = value.GetType().GetField(value.ToString());

				DescriptionAttribute description = (DescriptionAttribute)fi.GetCustomAttribute(typeof(DescriptionAttribute), false);

				writer.WriteStringValue(description.Description);
			}
		}
	}
}
//=====================================================================================================================
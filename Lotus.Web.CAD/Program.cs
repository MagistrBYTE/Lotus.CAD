using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Lotus.Web.CAD
{
	/// <summary>
	/// Основной класс для web-приложения
	/// </summary>
	public class Program
	{
		/// <summary>
		/// Точка входа в приложение
		/// </summary>
		/// <param name="args"></param>
		public static void Main(String[] args)
		{
			var host = CreateWebHostBuilder(args).Build();
			host.Run();
		}

		/// <summary>
		/// Создание web-host
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		private static IWebHostBuilder CreateWebHostBuilder(String[] args)
		{
			return WebHost.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration((context, builder) =>
				{
					builder.AddJsonFile("appsettings.json", false, true);
					builder.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true, true);
					builder.AddEnvironmentVariables();
				})
				.UseStartup<Startup>();
		}
	}
}

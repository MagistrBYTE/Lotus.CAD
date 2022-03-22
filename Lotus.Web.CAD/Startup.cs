using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Microsoft.JSInterop;

namespace Lotus.Web.CAD
{
	/// <summary>
	/// Основной класс для запуска приложения
	/// </summary>
	/// <remarks>
	/// Класс Startup является входной точкой в приложение ASP.NET Core. 
	/// Этот класс производит конфигурацию приложения, настраивает сервисы, 
	/// которые приложение будет использовать, устанавливает компоненты для обработки запроса или middleware.
	/// </remarks>
	public class Startup
	{
		/// <summary>
		/// Параметры конфигурации
		/// </summary>
		public IConfiguration Configuration { get; }

		/// <summary>
		/// Конструктор инициализирует объект параметрами конфигурации
		/// </summary>
		/// <param name="configuration"></param>
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		/// <summary>
		/// Метод регистрирует сервисы, которые используются приложением
		/// </summary>
		/// <param name="services"></param>
		public void ConfigureServices(IServiceCollection services)
		{
			//
			// Логирование
			//
			services.AddLogging();

			//
			//
			//
			services.AddRazorPages();

			//
			//
			//
			services.AddServerSideBlazor();

			//
			// добавляем сервисы CORS
			//
			services.AddCors(option => option.AddPolicy("PublicService", builder =>
			{
				builder.AllowAnyOrigin()    // принимаются запросы с любого адреса
					   .AllowAnyMethod()    // принимаются запросы любого типа (GET/POST)
					   .AllowAnyHeader();   // принимаются запросы с любыми заголовками
			}));

			//
			//
			//
			//services.ConfigureAccount(Configuration);

			services.AddTransient(sp => new Konva(sp.GetService<IJSRuntime>()));
		}

		/// <summary>
		/// Настройка конвейера обработки запросов
		/// </summary>
		/// <param name="app"></param>
		/// <param name="env"></param>
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			//
			//
			//
			app.UseHttpsRedirection();
			app.UseStaticFiles();

			//
			// Добавляем возможности маршрутизации
			//
			app.UseRouting();

			//
			//
			//
			app.UseCors("PublicService");

			//
			//
			//
			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapBlazorHub();
				endpoints.MapFallbackToPage("/_Host");
			});
		}
	}
}

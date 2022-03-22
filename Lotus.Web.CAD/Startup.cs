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
	/// �������� ����� ��� ������� ����������
	/// </summary>
	/// <remarks>
	/// ����� Startup �������� ������� ������ � ���������� ASP.NET Core. 
	/// ���� ����� ���������� ������������ ����������, ����������� �������, 
	/// ������� ���������� ����� ������������, ������������� ���������� ��� ��������� ������� ��� middleware.
	/// </remarks>
	public class Startup
	{
		/// <summary>
		/// ��������� ������������
		/// </summary>
		public IConfiguration Configuration { get; }

		/// <summary>
		/// ����������� �������������� ������ ����������� ������������
		/// </summary>
		/// <param name="configuration"></param>
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		/// <summary>
		/// ����� ������������ �������, ������� ������������ �����������
		/// </summary>
		/// <param name="services"></param>
		public void ConfigureServices(IServiceCollection services)
		{
			//
			// �����������
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
			// ��������� ������� CORS
			//
			services.AddCors(option => option.AddPolicy("PublicService", builder =>
			{
				builder.AllowAnyOrigin()    // ����������� ������� � ������ ������
					   .AllowAnyMethod()    // ����������� ������� ������ ���� (GET/POST)
					   .AllowAnyHeader();   // ����������� ������� � ������ �����������
			}));

			//
			//
			//
			//services.ConfigureAccount(Configuration);

			services.AddTransient(sp => new Konva(sp.GetService<IJSRuntime>()));
		}

		/// <summary>
		/// ��������� ��������� ��������� ��������
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
			// ��������� ����������� �������������
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

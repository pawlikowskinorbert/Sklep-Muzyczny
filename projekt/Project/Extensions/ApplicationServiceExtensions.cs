using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using Microsoft.Extensions.Options;
using Project.Helpers;

namespace Project.Extensions
{
	public static class ApplicationServiceExtensions
	{
		public static IServiceCollection AddAplicationServices(this IServiceCollection services, IConfiguration config)
		{
			return services;
		}
	}
}
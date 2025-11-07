using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace inmobiliariaApi.Models
{
	public class LoginView
	{
		[DataType(DataType.EmailAddress)]
		public string Usuario { get; set; }
		[DataType(DataType.Password)]
		public string Clave { get; set; }
	}
}
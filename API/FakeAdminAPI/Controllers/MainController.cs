using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace FakeAdminAPI.Controllers
{
	[Route("api")]
	[ApiController]

	public class MainController : ControllerBase
	{
		[HttpGet("send")]
		public IActionResult Send(string key, int id, string pc, string pass) {
			if (key == "API_KEY") {
				Console.WriteLine($"|{id}|{pc}|{pass}");
				var folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				var path = Path.Combine(folder, "data.txt");
				FileInfo fileInfo = new FileInfo(path);

				if (!fileInfo.Exists) { 
				
					FileStream fs = fileInfo.Create();
					fs.Close();
				}

				StreamWriter writer = fileInfo.AppendText();
				writer.WriteLine($"{id}|{pc}|{pass}");
				writer.Close();

				return Ok();
			}
			return BadRequest("Wrong authentication key!");
		}

	}
}

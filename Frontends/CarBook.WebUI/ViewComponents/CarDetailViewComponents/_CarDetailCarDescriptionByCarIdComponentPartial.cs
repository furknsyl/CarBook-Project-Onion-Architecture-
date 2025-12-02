using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using CarBook.Dto.CarDescriptionDtos;

namespace CarBook.WebUI.ViewComponents.CarDetailViewComponents
{
	public class _CarDetailCarDescriptionByCarIdComponentPartial : ViewComponent
	{
		private readonly IHttpClientFactory _httpClientFactory;
		public _CarDetailCarDescriptionByCarIdComponentPartial(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		public async Task<IViewComponentResult> InvokeAsync(int id)
		{
            ViewBag.carid = id;
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync($"https://localhost:7158/api/CarDescriptions?id=" + id);

            ResultCarDescriptionByCarIdDto values = null;

            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(jsonData))
                {
                    values = JsonConvert.DeserializeObject<ResultCarDescriptionByCarIdDto>(jsonData);
                }
            }

            // Eğer values hala null ise, default değer ata
            if (values == null)
            {
                values = new ResultCarDescriptionByCarIdDto
                {
                    CarDescriptionID = 0,
                    CarID = id,
                    Details = "Araç açıklaması bulunamadı."
                };
            }

            return View(values);
        }
	}
}

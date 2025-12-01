using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using CarBook.Dto.CarFeatureDtos;
using CarBook.Dto.FeatureDtos;

namespace CarBook.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/AdminCarFeatureDetail")]
    public class AdminCarFeatureDetailController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public AdminCarFeatureDetailController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [Route("Index/{id}")]
        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync("https://localhost:7158/api/CarFeatures?id=" + id);
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultCarFeatureByCarIdDto>>(jsonData);
                return View(values);
            }
            return View();
        }

        [HttpPost]
        [Route("Index/{id}")]
        public async Task<IActionResult> Index(List<ResultCarFeatureByCarIdDto> resultCarFeatureByCarIdDto)
        {

            foreach(var item in  resultCarFeatureByCarIdDto)
            {
                if (item.Available)
                {
                    var client = _httpClientFactory.CreateClient();
                    await client.GetAsync("https://localhost:7158/api/CarFeatures/CarFeatureChangeAvailableToTrue?id=" + item.CarFeatureID);
                    
                }
                else
                {
                    var client = _httpClientFactory.CreateClient();
                    await client.GetAsync("https://localhost:7158/api/CarFeatures/CarFeatureChangeAvailableToFalse?id=" + item.CarFeatureID);
                }
            }
            return RedirectToAction("Index", "AdminCar");
        }

        [HttpGet]
        [Route("CreateCarFeatureByCarID/{id}")]

        public async Task<ActionResult> CreateCarFeatureByCarID(int id)

        {

            ViewBag.CarId = id;



            var client = _httpClientFactory.CreateClient();



            // 1. Tüm özellikler

            var response = await client.GetAsync("https://localhost:7158/api/Features");

            if (!response.IsSuccessStatusCode)

                return View(new List<ResultFeatureDto>());



            var json = await response.Content.ReadAsStringAsync();

            var allFeatures = JsonConvert.DeserializeObject<List<ResultFeatureDto>>(json);



            // 2. CarFeature tablosundan zaten atanmış olan featureId’ler

            var responseCarFeatures = await client.GetAsync($"https://localhost:7158/api/CarFeatures?carId={id}");

            var carFeaturesJson = await responseCarFeatures.Content.ReadAsStringAsync();

            var assignedFeatures = JsonConvert.DeserializeObject<List<ResultCarFeatureByCarIdDto>>(carFeaturesJson);



            var assignedFeatureIds = assignedFeatures.Select(x => x.FeatureID).ToHashSet();



            // 3. Sadece atanmamış olan özellikleri gönder

            var featuresToShow = allFeatures.Where(f => !assignedFeatureIds.Contains(f.FeatureID)).ToList();



            return View(featuresToShow);

        }



        [HttpPost]
        [Route("CreateCarFeatureByCarID/{id}")]

        public async Task<IActionResult> CreateCarFeatureByCarID(int carId, List<int> selectedFeatureIds)

        {

            foreach (var fid in selectedFeatureIds)

            {

                var dto = new CreateCarFeatureDto

                {

                    CarID = carId,

                    FeatureID = fid,

                    Available = false

                };

                var client = _httpClientFactory.CreateClient();

                var json = JsonConvert.SerializeObject(dto);

                var content = new StringContent(json, Encoding.UTF8, "application/json");



                await client.PostAsync("https://localhost:7158/api/CarFeatures", content);

            }



            return RedirectToAction("Index", "AdminCar");

        }
    }
}

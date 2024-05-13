using BarterPlatform.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Common;
using System.Net.Http.Headers;

namespace BarterPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgriculturalProductOriginPricesController : ControllerBase
    {


        private readonly string apiUri = "https://data.moa.gov.tw/Service/OpenData/DataFileService.aspx?UnitId=652&IsTransData=1";

        public AgriculturalProductOriginPricesController()
        {

        }

        [HttpGet]
        public async Task<IActionResult> GetData()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUri);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.GetAsync(apiUri);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        var agriculturalProductOriginPrices = JsonConvert.DeserializeObject<List<Rootobject>>(data);
                        return Ok(agriculturalProductOriginPrices);
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode, response.ReasonPhrase);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}

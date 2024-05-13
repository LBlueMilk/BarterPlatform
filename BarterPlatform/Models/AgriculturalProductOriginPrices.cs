using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BarterPlatform.Models
{

    public class Rootobject
    {
        public string 作物 { get; set; }
        public string 年份 { get; set; }

        [JsonProperty("1月價格")]
        public string _1月價格 { get; set; }

        [JsonProperty("2月價格")]
        public string _2月價格 { get; set; }

        [JsonProperty("3月價格")]
        public string _3月價格 { get; set; }

        [JsonProperty("4月價格")]
        public string _4月價格 { get; set; }

        [JsonProperty("5月價格")]
        public string _5月價格 { get; set; }

        [JsonProperty("6月價格")]
        public string _6月價格 { get; set; }

        [JsonProperty("7月價格")]
        public string _7月價格 { get; set; }

        [JsonProperty("8月價格")]
        public string _8月價格 { get; set; }

        [JsonProperty("9月價格")]
        public string _9月價格 { get; set; }

        [JsonProperty("10月價格")]
        public string _10月價格 { get; set; }

        [JsonProperty("11月價格")]
        public string _11月價格 { get; set; }

        [JsonProperty("12月價格")]
        public string _12月價格 { get; set; }
    }

}

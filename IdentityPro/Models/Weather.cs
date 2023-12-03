

namespace IdentityPro.Models
{
    public class Weather
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }

        public string Season { get; set; }
        public int Humidity { get; set; }
        public float Temp { get; set; }

    }
}
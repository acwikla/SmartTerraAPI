using SmartTerraAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartTerraAPI.DTO
{
    public class DevicePropertiesDTO
    {
        public DateTime Date { get; set; }

        public int Id { get; set; }

        public bool isLiquidLevelSufficient { get; set; }

        public double Temperature { get; set; }

        public double Humidity { get; set; }

        public double HeatIndex { get; set; }

        public double SoilMoisturePercentage { get; set; }

        public string LEDHexColor { get; set; }

        public double LEDBrightness { get; set; }
    }
}

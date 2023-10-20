using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionService.DTOs
{
    public class UpdateAuctionDTO
    {
        [Required]
        public string Make { get; set; }
        [Required]
        public string Model { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public string Color { get; set; }
        [Required]
        public int Mileage { get; set; }
    }
}
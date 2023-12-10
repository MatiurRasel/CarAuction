using AutoMapper;
using BiddingService.DTOs;
using Contracts;

namespace BiddingService.RequestHelpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles(){
            CreateMap<Bid,BidDTO>();
            CreateMap<Bid,BidPlaced>();
        }
    }
}
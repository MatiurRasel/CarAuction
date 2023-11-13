using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("api/auctions")]
    public class AuctionsController : ControllerBase
    {
        private readonly AuctionDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public AuctionsController(AuctionDbContext context,IMapper mapper,
        IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }
        [HttpGet]
        public async Task<ActionResult<List<AuctionDTO>>> GetAllAuctions(string date)
        {
            var query = _context.Auctions.OrderBy(x=>x.Item.Make).AsQueryable();

            if(!string.IsNullOrEmpty(date))
            {
                query = query.Where(x=>x.UpdatedAt
                .CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
            }
            
            return await query.ProjectTo<AuctionDTO>
                (_mapper.ConfigurationProvider).ToListAsync();    
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDTO>> GetAuctionById(Guid id)
        {
            var auction = await _context.Auctions
                .Include(x=>x.Item)
                .FirstOrDefaultAsync(x=>x.Id == id);

            if(auction == null) return NotFound();

            return _mapper.Map<AuctionDTO>(auction);    
        }
        [HttpPost]
        public async Task<ActionResult<AuctionDTO>> CreateAuction(CreateAuctionDTO auctionDTO)
        {
            var auction = _mapper.Map<Auction>(auctionDTO);
            //TODO:  Add current user as seller
            auction.Seller = "test";
            _context.Auctions.Add(auction);
           
            var newAuction = _mapper.Map<AuctionDTO>(auction);
            await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));
            
            var result = await _context.SaveChangesAsync() > 0;
            if(!result) return BadRequest("Could not save changes to the database");

           

            return CreatedAtAction(nameof(GetAuctionById), new {auction.Id},
            _mapper.Map<AuctionDTO>(newAuction));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAuction(Guid id,UpdateAuctionDTO updateAuctionDTO)
        {
            var auction =  await _context.Auctions
                .Include(x =>x.Item)
                .FirstOrDefaultAsync(x =>x.Id == id);

            if(auction == null) return NotFound();

            //TODO: check seller == userName

            auction.Item.Make = updateAuctionDTO.Make ?? auction.Item.Make;
            auction.Item.Model = updateAuctionDTO.Model ?? auction.Item.Model;
            auction.Item.Color = updateAuctionDTO.Color ?? auction.Item.Color;
            auction.Item.Mileage = updateAuctionDTO.Mileage ?? auction.Item.Mileage;
            auction.Item.Year = updateAuctionDTO.Year ?? auction.Item.Year;

            await _publishEndpoint.Publish(_mapper.Map<AuctionUpdated>(auction));

            var result = await _context.SaveChangesAsync() > 0;
            if(result) return Ok();
            return BadRequest("Problem saving changes");
        } 
    
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuction(Guid id)
        {
            var auction = await _context.Auctions.FindAsync(id);
            if(auction == null) return NotFound();

            //TODO: check seller == userName

            _context.Auctions.Remove(auction);
            await _publishEndpoint.Publish(_mapper.Map<AuctionDeleted>(new 
            {
                Id = auction.Id.ToString()
            }));

            var result = await _context.SaveChangesAsync() >0;
            if(!result) return BadRequest("Could not update  in database");
            return Ok();
        }
    
    }
}
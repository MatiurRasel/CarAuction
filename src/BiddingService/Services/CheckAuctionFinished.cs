

using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace BiddingService.Services
{
    public class CheckAuctionFinished : BackgroundService 
    // BackgroundService this going to run as singleton.
    //Masstransit service lifetime is scoped  to the scoped of the request..cannot inject different lifetime into BackgroundService
    {
        private readonly ILogger<CheckAuctionFinished> _logger;
        private readonly IServiceProvider _services;

        public CheckAuctionFinished(ILogger<CheckAuctionFinished> logger,IServiceProvider services)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting check for finished auctions");
            
            stoppingToken.Register(() => _logger.LogInformation("==> Auction check is stopping"));

            while(!stoppingToken.IsCancellationRequested)
            {
                await CheckAuctions(stoppingToken);

                await Task.Delay(5000,stoppingToken);
            }
        }

        private async Task CheckAuctions(CancellationToken stoppingToken)
        {
            var finishedAuctions = await DB.Find<Auction>()
                    .Match(x=>x.AuctionEnd <= DateTime.UtcNow)
                    .Match(x=>!x.Finished)
                    .ExecuteAsync(stoppingToken);

            if(finishedAuctions.Count == 0) return;

            _logger.LogInformation("==> Found {count} auctions that have completed",finishedAuctions.Count);

            // BackgroundService this going to run as singleton.
            //MassTransit service lifetime is scoped  to the scoped of the request..cannot inject different lifetime into BackgroundService
            //So need to create a scope inside this class.

            using var scope = _services.CreateScope();
            var endpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

            foreach(var auction in finishedAuctions)
            {
                auction.Finished = true;
                await auction.SaveAsync(null,stoppingToken);

                var winningBid = await DB.Find<Bid>()
                        .Match(a=>a.AuctionId == auction.ID)
                        .Match(b=>b.BidStatus == BidStatus.Accepted)
                        .Sort(x=>x.Descending(s=>s.Amount))
                        .ExecuteFirstAsync(stoppingToken);
                
                await endpoint.Publish(new AuctionFinished
                {
                    //ItemSold = winningBid
                });
            }
        }
    }
}
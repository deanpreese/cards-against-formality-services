using System.Collections.Concurrent;
using System.Threading.Channels;
using OMS.Core.Models;

namespace OMS.Services.Queue;

public class ClosedOrderQueue
{
    private readonly Channel<ClosedTrade> _closedTradeChannel;
    private ILogger<NewOrderQueue> _logger;

    private readonly ConcurrentBag<ClosedTrade> _concurrentCollection = new ConcurrentBag<ClosedTrade>();


    public ClosedOrderQueue(ILogger<NewOrderQueue> logger)
    {
        _logger = logger;

        // Create a bounded channel with a capacity limit to prevent out-of-memory issues in case of high load
        _closedTradeChannel= Channel.CreateBounded<ClosedTrade>(new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true, // Set to true if only one consumer will read from the channel
            SingleWriter = false  // Set to true if only one producer will write to the channel
        });
    }

    public async Task WriteAsync(ClosedTrade order, CancellationToken cancellationToken = default)
    {
        await _closedTradeChannel.Writer.WriteAsync(order, cancellationToken);
    }

    public IAsyncEnumerable<ClosedTrade> ReadAllAsync(CancellationToken cancellationToken = default)
    {
        return _closedTradeChannel.Reader.ReadAllAsync(cancellationToken);
    }

    public void AddToList(ClosedTrade order)
    {
        ArgumentNullException.ThrowIfNull(order);
        _concurrentCollection.Add(order);
    }


    public List<ClosedTrade> GetOrdersList(int userId, int userGroup)
    {
        List<ClosedTrade> query = (from distinct_order in _concurrentCollection
                 where distinct_order.UserID == userId
                 && distinct_order.GroupID== userGroup
                            select distinct_order).ToList().ToList();

        return query;
    }


    
}

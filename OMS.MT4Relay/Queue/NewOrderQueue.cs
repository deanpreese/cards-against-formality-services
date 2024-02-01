using System.Collections.Concurrent;
using System.Threading.Channels;
using OMS.Core.Models;

namespace OMS.Services.Queue;

public class NewOrderQueue
{
    private readonly Channel<NewOrder> _channel;
    private ILogger<NewOrderQueue> _logger;
    private readonly ConcurrentBag<NewOrder> _concurrentCollection = new ConcurrentBag<NewOrder>();
       

    public NewOrderQueue(ILogger<NewOrderQueue> logger)
    {
        _logger = logger;

        // Create a bounded channel with a capacity limit to prevent out-of-memory issues in case of high load
        _channel = Channel.CreateBounded<NewOrder>(new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true, // Set to true if only one consumer will read from the channel
            SingleWriter = false  // Set to true if only one producer will write to the channel
        });
    }

    public async Task WriteAsync(NewOrder order, CancellationToken cancellationToken = default)
    {
        AddToList(order);
        await _channel.Writer.WriteAsync(order, cancellationToken);
    }

    public IAsyncEnumerable<NewOrder> ReadAllAsync(CancellationToken cancellationToken = default)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }

    public void AddToList(NewOrder order)
    {
        ArgumentNullException.ThrowIfNull(order);
        _concurrentCollection.Add(order);
    }


    public List<NewOrder> GetOrdersList(NewOrder order)
    {
        IEnumerable<NewOrder> query = (from distinct_order in _concurrentCollection
                 where distinct_order.UserName == order.UserName 
                 && distinct_order.UserGroup == order.UserGroup
                            select distinct_order).ToList();

        return query.ToList();
    }


    public void RemoveFromList(int platformOrderID)
    {
        var order = _concurrentCollection.FirstOrDefault(o => o.PlatformOrderID == platformOrderID);

        if (order != null)
        {
            _concurrentCollection.TakeWhile(o => o.PlatformOrderID != platformOrderID).ToList();
        }
    }

    
}

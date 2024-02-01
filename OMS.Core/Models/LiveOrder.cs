using System.ComponentModel.DataAnnotations;
using OMS.Core.Common;

namespace OMS.Core.Models;


[GenerateSerializer]
[Alias("LiveOrder")]
public class LiveOrder
{
    [Id(15)]
    private DateTime _orderTime;

    [Key]
    [Id(18)]
    public int LiveOrderID { get; set; }    
    
    [Id(0)]
    // PlatformOrderID generate by platform of record
    public int PlatformOrderID { get; set; }

    // In the case of NinjaTrader as the execution platformn - this would be the filled order ID returned on execution
    [Id(1)]
    public int ExecutedOrderID { get; set; }
    [Id(2)]
    public int UserID { get; set; }
    [Id(3)]
    public int UserGroup { get; set; }
    [Id(4)]
    public double Leverage { get; set; }
    [Id(5)]
    public int Opposite { get; set; }

    // Token for successful trader authentication
    [Id(6)]
    public int AuthToken { get; set; }

    // ID generated by the order manager
    [Id(7)]
    public int OrderManagerID { get; set; }

    // Related orders are available to handle split orders  ex- your 5 long and you sell 3 
    [Id(8)]
    public int RelatedOrderID { get; set; }
    [Id(9)]
    public DateTime OrderTime
    {
        get => _orderTime;
        set => _orderTime = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }
    [Id(10)]
    public string Instrument { get; set; } = PlatformConstants.InvalidSymbol;
    [Id(11)]
    public double OrderPX { get; set; }

    // Market, Limit, Stop
    [Id(12)]
    public OrderType OrderType { get; set; }

    //Buy Sell
    [Id(13)]
    public OrderAction OrderAction { get; set; }
    [Id(14)]
    public int Quantity { get; set; }

    [Id(16)]
    public double MAE { get; set; }
    [Id(17)]
    public double MFE { get; set; }

    [Timestamp]
    public byte[]? Version { get; set; }

}
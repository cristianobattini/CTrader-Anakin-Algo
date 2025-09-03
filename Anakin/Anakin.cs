using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Collections;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;

namespace cAlgo.Robots
{
    [Robot(AccessRights = AccessRights.None)]
    public class Anakin : Robot
    {
        // Input parameters
        [Parameter("Order Type", DefaultValue = TradeType.Buy)]
        public TradeType OrderType { get; set; }

        [Parameter("Entry Price", DefaultValue = 1.10000)]
        public double EntryPrice { get; set; }

        [Parameter("Take Profit 1 (Pips)", DefaultValue = 50)]
        public int TakeProfit1Pips { get; set; }

        [Parameter("Take Profit 2 (Pips)", DefaultValue = 100)]
        public int TakeProfit2Pips { get; set; }

        [Parameter("Stop Loss (Pips)", DefaultValue = 30)]
        public int StopLossPips { get; set; }

        [Parameter("Volume (% of Balance)", DefaultValue = 2.0)]
        public double VolumePercent { get; set; }

        [Parameter("TP1 Order Weight (%)", DefaultValue = 60.0, MinValue = 0, MaxValue = 100)]
        public double TP1WeightPercent { get; set; }

        [Parameter("Order Expiration (Hours)", DefaultValue = 24)]
        public int OrderExpirationHours { get; set; }

        protected override void OnStart()
        {
            Print("=== Anakin Started ===");

            Positions.Opened += OnPositionOpened;
            Positions.Closed += OnPositionClosed;
            PendingOrders.Created += OnPendingOrdersCreated;

            PrintParametersSummary();
            
            // Calculate volumes for each order
            double totalVolume = CalculateVolume(VolumePercent);
            double volumeTP1 = totalVolume * (TP1WeightPercent / 100.0);
            double volumeTP2 = totalVolume * ((100 - TP1WeightPercent) / 100.0);
            
            Print($"Volume Calculation: Total={totalVolume:F2} units, TP1={volumeTP1:F2} units ({TP1WeightPercent}%), TP2={volumeTP2:F2} units ({100-TP1WeightPercent}%)");
            
            // Calculate take profit and stop loss prices
            double takeProfitPrice1 = CalculateTakeProfitPrice(OrderType, TakeProfit1Pips);
            double takeProfitPrice2 = CalculateTakeProfitPrice(OrderType, TakeProfit2Pips);
            double stopLossPrice = CalculateStopLossPrice(OrderType, StopLossPips);
            
            PrintProfitLossAnalysis(totalVolume, takeProfitPrice1, takeProfitPrice2, stopLossPrice);
            
            // Place the two pending orders
            PlaceOrder(volumeTP1, TakeProfit1Pips, stopLossPrice, "TP1 Order");
            PlaceOrder(volumeTP2, TakeProfit2Pips, stopLossPrice, "TP2 Order");
            
            Print("=== Orders Placed Successfully ===");
        }

        private void PlaceOrder(double volume, double takeProfitPips, double stopLossPips, string label)
        {
            var result = PlaceLimitOrder(
                OrderType,
                Symbol.Name,
                Symbol.NormalizeVolumeInUnits(volume),
                EntryPrice,
                label,
                stopLossPips,   
                takeProfitPips, 
                ProtectionType.Relative,
                // Server.Time.AddHours(OrderExpirationHours),
                null,
                $"TP (pips): {takeProfitPips}, SL (pips): {stopLossPips}"
            );
            
            if (result.IsSuccessful)
            {
                Print($"✅ {label} placed: Volume={volume:F2}, Entry={EntryPrice}, TP={takeProfitPips:F5}, SL={stopLossPips:F5}");
            }
            else
            {
                Print($"❌ Failed to place {label}: {result.Error}");
            }
        }

        private double CalculateVolume(double percent)
        {
            double balance = Account.Balance;
            double riskAmount = balance * (percent / 100.0);
            double pipValue = Symbol.PipValue;
            
            if (pipValue == 0 || StopLossPips == 0)
            {
                Print("Error: PipValue or StopLossPips cannot be zero");
                return 0;
            }
            
            double volume = riskAmount / (StopLossPips * pipValue);
            
            return volume;
        }

        private double CalculateTakeProfitPrice(TradeType tradeType, int pips)
        {
            if (tradeType == TradeType.Buy)
                return EntryPrice + (pips * Symbol.PipSize);
            else
                return EntryPrice - (pips * Symbol.PipSize);
        }

        private double CalculateStopLossPrice(TradeType tradeType, int pips)
        {
            if (tradeType == TradeType.Buy)
                return EntryPrice - (pips * Symbol.PipSize);
            else
                return EntryPrice + (pips * Symbol.PipSize);
        }

        private void PrintParametersSummary()
        {
            Print("📋 Parameters Summary:");
            Print($"   Order Type: {OrderType}");
            Print($"   Entry Price: {EntryPrice}");
            Print($"   Take Profit 1: {TakeProfit1Pips} pips");
            Print($"   Take Profit 2: {TakeProfit2Pips} pips");
            Print($"   Stop Loss: {StopLossPips} pips");
            Print($"   Volume: {VolumePercent}% of balance");
            Print($"   TP1 Weight: {TP1WeightPercent}%");
            Print($"   Current Price: {Symbol.Bid}");
            Print($"   Account Balance: ${Account.Balance:F2}");
        }

        private void PrintProfitLossAnalysis(double totalVolume, double tp1Price, double tp2Price, double slPrice)
        {
            Print("💰 Profit & Loss Analysis:");
            
            double pipValue = Symbol.PipValue;
            double balance = Account.Balance;
            
            // Calculate potential profits
            double profitTP1 = (TakeProfit1Pips * pipValue) * (totalVolume * (TP1WeightPercent / 100.0));
            double profitTP2 = (TakeProfit2Pips * pipValue) * (totalVolume * ((100 - TP1WeightPercent) / 100.0));
            double totalProfit = profitTP1 + profitTP2;
            double profitPercent = (totalProfit / balance) * 100;
            
            // Calculate potential loss
            double lossAmount = (StopLossPips * pipValue) * totalVolume;
            double lossPercent = (lossAmount / balance) * 100;
            
            Print($"   Potential Profit TP1: ${profitTP1:F2} ({((TakeProfit1Pips * pipValue * (totalVolume * (TP1WeightPercent / 100.0))) / balance) * 100:F2}%)");
            Print($"   Potential Profit TP2: ${profitTP2:F2} ({((TakeProfit2Pips * pipValue * (totalVolume * ((100 - TP1WeightPercent) / 100.0))) / balance) * 100:F2}%)");
            Print($"   Total Potential Profit: ${totalProfit:F2} ({profitPercent:F2}% of balance)");
            Print($"   Potential Loss: ${lossAmount:F2} ({lossPercent:F2}% of balance)");
            
            if (lossAmount != 0)
                Print($"   Risk/Reward Ratio: {Math.Abs(totalProfit/lossAmount):F2}:1");
            else
                Print("   Risk/Reward Ratio: undefined (zero risk)");
        }

        protected override void OnTick()
        {
            // Optional: Add tick-based logic if needed
        }

        protected override void OnStop()
        {
            Print("=== Anakin Stopped ===");
        }

        protected override void OnError(Error error)
        {
            Print($"❌ Error: {error.Code} - {error}");
        }

        private void OnPendingOrdersCreated(PendingOrderCreatedEventArgs pendingOrder)
        {
            Print($"📋 Pending Order Created");
        }

        private void OnPositionOpened(PositionOpenedEventArgs args)
        {
            Print($"🎯 Position Opened: {args.Position.Label} - P&L: ${args.Position.NetProfit:F2}");
        }

        private void OnPositionClosed(PositionClosedEventArgs args)
        {
            var position = args.Position;
            Print($"🔚 Position Closed: {position.Label} - P&L: ${position.NetProfit:F2} ({((position.NetProfit/Account.Balance)*100):F2}%)");
        }
    }
}
# Anakin cTrader Algo 🥷🏻
A sophisticated cTrader algorithmic trading solution that provides precise control over trade entries, multiple take profit levels, and advanced risk management features. Named after the legendary Jedi Anakin Skywalker, this algo brings power and precision to your trading strategy.

## 🚀 Features
- Precise Entry Control: Set exact entry prices or use market orders
- Dual Take Profit System: Configure two separate take profit levels with customizable volume allocation
- Advanced Risk Management: Stop loss protection with optional break-even functionality
- Flexible Position Sizing: Choose between risk percentage or fixed lots sizing
- Visual Trading Levels: Clear chart drawings for entry, stop loss, and take profit levels
- Notification System: Email and sound alerts for trade events
- Pending Order Support: Both limit and stop order types available

## ⚙️ Installation
- Download the Anakin.algo file
- Open cTrader platform
- Navigate to Algo tab
- Click Import and select the downloaded file
- The Anakin algo will appear in your algorithms list

## 🎮 How to Use
Basic Setup:
- Drag and drop the Anakin algo onto your desired chart
- Configure the parameters in the settings panel
- Trade Setup Parameters:
- Trade Type: Select Buy or Sell direction
- Entry Price (0 = market): Specify your desired entry price (0 for market order)
- Stop Loss (pips): Risk management level in pips
- Take Profit 1 (pips): First profit target in pips
- Take Profit 2 (pips): Second profit target in pips
- TP1 Share (%): Percentage of volume allocated to TP1 (remainder goes to TP2)

Break-Even Features:
- Enable Break-Even: Toggle break-even functionality
- BE Trigger (pips): Pips in profit before activating break-even
- BE Offset (pips): Pips from entry price for break-even stop

Money Management:
- Position Sizing Mode: Choose between RiskPercent or FixedLots
- Risk % (of Balance): Risk percentage per trade (if using RiskPercent mode)
- Fixed Lots: Fixed lot size (if using FixedLots mode)

Visual Customization:
- Customize colors for entry, stop loss, and take profit lines
- Adjust line thickness for chart objects

Notifications:
- Enable/disable email notifications
- Enable/disable sound alerts

## 📊 Strategy Implementation
The Anakin algo supports multiple trading approaches:
- Precision Entry Trading: Set exact entry prices for technical levels
- Multiple Profit Targets: Take partial profits at TP1 and let remainder run to TP2
- Break-Even Protection: Automatically move stop loss to break-even plus offset
- Risk-Adjusted Position Sizing: Automatically calculate position size based on account risk

## ⚠️ Risk Management
Important Considerations:
- Always test strategies in demo accounts first
- The break-even feature only activates when enabled in parameters
- Ensure sufficient account margin for intended trades
- Market conditions may affect execution prices for market orders
- Slippage may occure during high volatility periods

Default Risk Parameters:
- Stop Loss: 20 pips
- Take Profit 1: 6 pips (50% of position)
- Take Profit 2: 25 pips (50% of position)
- Risk Per Trade: 1% of account balance
Adjust these parameters according to your risk tolerance and trading strategy.

## 🔧 Technical Requirements
- cTrader platform version 4.0 or higher
- .NET framework support
- Stable internet connection
- Sufficient account margin for intended trades

📝 License
This algorithm is provided for educational purposes. Users are responsible for their trading decisions and should understand the risks involved in algorithmic trading.

🤝 Support
For questions or issues:
- Check parameter settings for validity
- Ensure sufficient account balance
- Verify market hours and liquidity
  
Troubleshooting:
- If the algorithm doesn't start, check that all required parameters are set correctly
- Ensure stop loss and take profit values are greater than 0
- Verify that the symbol is available for trading
- Check that your account has sufficient funds for the requested position size

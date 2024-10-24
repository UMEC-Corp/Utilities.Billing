using TonSdk.Contracts;
using TonSdk.Core;
using TonSdk.Core.Block;
using TonSdk.Core.Boc;

namespace Utilities.Billing.TonWallets.SmartContracts.Multisig;

internal class MultiSigOrderBuilder
{
    private readonly uint _walletId;
    private CellBuilder _messages;
    private long _queryId;

    public MultiSigOrderBuilder(uint wallet_id, int lifetime = 7200, long? query_id = null)
    {
        _walletId = wallet_id;
        _messages = new CellBuilder();
        _queryId = query_id ?? GenerateQueryId(lifetime);
    }

    public MultiSigOrderBuilder AddTransferMessage(Address toAddr, Coins amount, object? payload = null, StateInit? stateInit = null, int sendMode = 3)
    {
        Cell payloadCell = null;
        if (payload != null)
        {
            if (payload is string)
            {
                var payloadStr = payload as string;
                if (payloadStr.Length > 0)
                {
                    payloadCell = new CellBuilder()
                        .StoreUInt(0, 32)
                        .StoreString(payloadStr)
                        .Build();
                }
            }
            else if (payload is Cell)
            {
                payloadCell = payload as Cell;
            }
            else
            {
                payloadCell = new CellBuilder().StoreBytes(payload as byte[]).Build();
            }
        }

        var orderHeader = new IntMsgInfo(new IntMsgInfoOptions
        {
            Dest = toAddr,
            Value = amount
        });

        var order = new MessageX(new MessageXOptions
        {
            Info = orderHeader,
            StateInit = stateInit,
            Body = payloadCell,
        });

        AddMessageFromCell(order.Cell, sendMode);
        return this;
    }


    public Cell AddMessageFromCell(Cell message, int mode = 3)
    {
        if (_messages.Refs.Count() >= 4)
        {
            throw new Exception("only 4 refs are allowed");
        }

        _messages.StoreUInt(mode, 8);
        _messages.StoreRef(new CellBuilder().StoreCellSlice(message.Parse()).Build());

        return message;
    }

    public void ClearMessages()
    {
        _messages = new CellBuilder();
    }

    public MultiSigOrder Build()
    {
        // TODO: query_id нужно возвращить и отдавать всем, кто должен подписать эту мультитранзакцию
        Console.WriteLine($"query_id = {_queryId}");

        return new MultiSigOrder(new CellBuilder()
                             .StoreUInt(_walletId, 32)
                             .StoreUInt(_queryId, 64)
                             .StoreCellSlice(_messages.Build().Parse())
                             .Build());
    }

    private long GenerateQueryId(int lifetime)
    {
        return DateTimeOffset.Now.ToUnixTimeSeconds() + lifetime << 32;
    }
}

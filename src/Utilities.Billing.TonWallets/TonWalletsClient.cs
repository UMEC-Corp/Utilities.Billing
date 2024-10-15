using TonSdk.Client;
using TonSdk.Contracts.Jetton;
using TonSdk.Contracts.Wallet;
using TonSdk.Core.Block;
using TonSdk.Core.Boc;
using TonSdk.Core.Crypto;
using TonSdk.Core;
using System.Security.Cryptography.X509Certificates;
using TonSdk.Connect;
using System.Xml.Linq;
using System.Linq.Expressions;
using TonSdk.Adnl.LiteClient;
using Utilities.Billing.Contracts;

namespace Utilities.Billing.TonWallets;

public class TonWalletsClient : IPaymentSystem
{
    private readonly TonWalletsSettings _settings;

    public TonWalletsClient(TonWalletsSettings settings)
    {
        _settings = settings;
    }

    private TonClient GetClient()
    {
        var tonClientParams = new HttpParameters
        {
            Endpoint = _settings.Endpoint,
            ApiKey = _settings.ApiKey
        };

        return new TonClient(TonClientType.HTTP_TONCENTERAPIV2, tonClientParams);
    }



    public async Task<string> CreateWallet(CreateWalletCommand command)
    {
        var tonClient = GetClient();

        var (masterWallet, masterKey) = GetMasteWallet();

        var (newWallet, newKey) = CreateNewWallet();

        var initialBalance = new Coins(1);
        var fromMasterToNew = await MakeTransferMessage(tonClient, masterWallet, masterKey, newWallet, initialBalance, "Wallet init", false);
        await SendMessage(tonClient, fromMasterToNew);

        // На выполнение транзакции в TON нужно некоторое время.
        // Чтобы понять, что транзакция выполнилась, периодически проверям баланс нового кошелька.
        // Как только баланс станет равен переведенной сумме (initialBalance), можно счиать, что транзакция завершилась.
        var newBalance = await tonClient.GetBalance(newWallet.Address);
        var timeout = 2 * 60 * 1000;
        var delay = 5 * 1000;
        var time = 0;
        while (!newBalance.Eq(initialBalance))
        {
            await Task.Delay(5000);
            newBalance = await tonClient.GetBalance(newWallet.Address);
            time += delay;
            if (time > timeout)
            {
                throw new InvalidOperationException("Wallet creation operation has timed out.");
            }
        }

        var fromNewToMaster = await MakeTransferMessage(tonClient, newWallet, newKey, masterWallet, new Coins(0.9), "Wallet init completion", false);
        await SendMessage(tonClient, fromNewToMaster);



        return newWallet.Address.ToString();
    }

    private async Task<SendBocResult> SendMessage(TonClient tonClient, Cell cellMessage)
    {
        // send this message via TonClient,
        var result = await tonClient.SendBoc(cellMessage);
        if (result == null || result.Value.Type != "ok")
        {
            throw new InvalidOperationException($"Faild send message to TON: {result?.Type}");
        }

        return result.Value;
    }

    private static async Task<Cell> MakeTransferMessage(TonClient tonClient, WalletV4 fromWallet, byte[] fromKey, WalletV4 toWallet, Coins amount, string comment, bool isBounsable)
    {
        // create transaction body query + comment
        Cell body = new CellBuilder().StoreUInt(0, 32).StoreString(comment).Build();

        // getting seqno using tonClient
        uint? seqno = await tonClient.Wallet.GetSeqno(fromWallet.Address);

        // create transfer message
        var message = fromWallet.CreateTransferMessage(new[]
        {
                new WalletTransfer
                {
                    Message = new InternalMessage(new InternalMessageOptions
                    {
                        Info = new IntMsgInfo(new IntMsgInfoOptions
                        {
                            Dest = toWallet.Address,
                            Value = amount,
                            Bounce = isBounsable, // make bounceable message
                        }),
                        Body = body
                    }),
                    Mode = 1 // message mode
                }
            }, seqno ?? 0); // if seqno is null we pass 0, wallet will auto deploy on message send

        // sign transfer message
        message.Sign(fromKey);

        // get message cell
        Cell cell = message.Cell;
        return cell;
    }

    private (WalletV4, byte[]) GetMasteWallet()
    {
        var mnemonic = new Mnemonic(_settings.MasterSecret.Split(" "));
        return GetWalletByMnemonic(mnemonic);
    }

    private (WalletV4, byte[]) CreateNewWallet()
    {
        var mnemonic = new Mnemonic();
        var (w, pk) = GetWalletByMnemonic(mnemonic);

        Console.WriteLine($"newWallet mnemonic: {string.Join(" ", mnemonic.Words)}\n address: {w.Address}");

        return (w, pk);
    }

    private (WalletV4, byte[]) GetWalletByMnemonic(Mnemonic mnemonic)
    {
        var options = new WalletV4Options()
        {
            PublicKey = mnemonic.Keys.PublicKey,
        };
        var wallet = new WalletV4(options, 2);

        return (wallet, mnemonic.Keys.PrivateKey);
    }

    public async Task ReturnMoneyToMaster(Mnemonic fromWalletMnemonic)
    {
        var tonClient = GetClient();

        var (toWallet, toKey) = GetMasteWallet();

        var (fromWallet, fromKey) = GetWalletByMnemonic(fromWalletMnemonic);

        var cellMessage = await MakeTransferMessage(tonClient, fromWallet, fromKey, toWallet, new Coins(0.9), "Wallet init completion", false);

        await SendMessage(tonClient, cellMessage);
    }

    public Task AddAssetAsync(Contracts.AddStellarAssetCommand command)
    {
        throw new NotImplementedException();
    }

    public Task AddPaymentAsync(Contracts.AddPaymentCommand command)
    {
        throw new NotImplementedException();
    }

    public Task<string> CreateInvoiceXdr(Contracts.CreateInvoiceXdrCommand command)
    {
        throw new NotImplementedException();
    }

    public Task<string> CreateWalletAsync(Contracts.CreateWalletCommand command)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetMasterAccountAsync()
    {
        throw new NotImplementedException();
    }

    public Task<ICollection<InvoiceInfomation>> GetInvoicesInformationAsync(IEnumerable<long> invoiceIds)
    {
        throw new NotImplementedException();
    }
}





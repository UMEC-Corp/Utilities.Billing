using TonSdk.Client;
using TonSdk.Contracts.Jetton;
using TonSdk.Contracts.Wallet;
using TonSdk.Core;
using TonSdk.Core.Block;
using TonSdk.Core.Boc;
using TonSdk.Core.Crypto;
using Utilities.Billing.Contracts;
using Utilities.Billing.TonWallets.Extensions;
using Utilities.Billing.TonWallets.SmartContracts.Multisig;

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


        var muiltisigWallet = await CreateMultisigWallet(tonClient, masterWallet, masterKey, newWallet);


        return muiltisigWallet.Address.ToString();
    }

    private async Task<MultisigWallet> CreateMultisigWallet(TonClient tonClient, WalletV4 masterWallet, byte[] masterKey, WalletV4 newWallet)
    {
        var wallet = new MultisigWallet(new MultisigWalletOptions
        {
            PublicKeys = [masterWallet.PublicKey, newWallet.PublicKey],
            RequiredSigners = 1,
        });

        Console.WriteLine($"multisig: {wallet.Address}");

        Coins amount = new Coins(0.01); // 0.01 TON
        string comment = "Init multisig";

        // create transaction body query + comment
        Cell body = new CellBuilder().StoreUInt(0, 32).StoreString(comment).Build();

        // getting seqno using tonClient
        uint? seqno = await tonClient.Wallet.GetSeqno(masterWallet.Address);
        ExternalInMessage message = masterWallet.CreateTransferMessage(new[]
        {
                new WalletTransfer
                {
                    Message = new InternalMessage(new InternalMessageOptions
                    {
                        Info = new IntMsgInfo(new IntMsgInfoOptions
                        {
                            Dest = wallet.Address,
                            Value = amount,
                            Bounce = false // make bounceable message
                        }),
                        Body = body
                    }),
                    Mode = 1 // message mode
                }
            }, seqno ?? 0); // if seqno is null we pass 0, wallet will auto deploy on message send

        // sign transfer message
        message.Sign(masterKey);
        await SendMessage(tonClient, message.Cell);



        // На выполнение транзакции в TON нужно некоторое время.
        // Чтобы понять, что транзакция выполнилась, периодически проверям баланс нового кошелька.
        // Как только баланс станет равен переведенной сумме (initialBalance), можно счиать, что транзакция завершилась.
        var walletBalance = await tonClient.GetBalance(wallet.Address);
        var timeout = 2 * 60 * 1000;
        var delay = 5 * 1000;
        var time = 0;
        while (!walletBalance.Eq(amount))
        {
            await Task.Delay(5000);
            walletBalance = await tonClient.GetBalance(wallet.Address);
            time += delay;
            if (time > timeout)
            {
                throw new InvalidOperationException("Wallet creation operation has timed out.");
            }
        }




        //TODO: деплой кошелька тоже занимает некоторое время. Нужно добавить проверку, что контракт задеплоился
        var deployMsg = wallet.CreateDeployMessage();
        await SendMessage(tonClient, deployMsg.Cell);

        return wallet;
    }

    // Пример перевода кастомных токенов
    public async Task TransferJetton()
    {
        var tonClient = GetClient();

        var options = new JettonTransferOptions()
        {
            Amount = new Coins(100), // jetton amount to send, for ex 100 jettons
            Destination = new Address("0QAubBpp44X8ebbN_ZmmWMfcB-uvOsZ1r5wL_x71CEDgnzon"), // receiver
            ForwardPayload = GetStringPayload("jetton transfer test")
        };
        var jettonTransfer = JettonWallet.CreateTransferRequest(options);

        var jettonWallet = new Address("kQDvSc1s31GiXRDIbgDzrcLkdQ1FJnBAdQNG9ybKQVIDs7A8");
        var (masterW, masterK) = GetMasteWallet();

        uint? seqno = await tonClient.Wallet.GetSeqno(masterW.Address);
        var message = masterW.CreateTransferMessage(new[]
        {
                new WalletTransfer
                {
                    Message = new InternalMessage(new()
                    {
                        Info = new IntMsgInfo(new()
                        {
                            Dest = jettonWallet,
                            Value = new Coins(0.01) // amount in TONs to send
                        }),
                        Body = jettonTransfer
                    }),
                    Mode = 1
                }
            }, seqno ?? 0).Sign(masterK);

        await SendMessage(tonClient, message.Cell);
    }


    // Создает транзакцию с нескольиим операциями в кастомных токенах
    // Используется JettonWalletContract
    public async Task CreateJettonOrder(string walletAddr)
    {
        var tonClient = GetClient();
        var multisigWallet = await tonClient.GetExistsMultisigWallet(walletAddr);
        var (masterW, masterK) = GetMasteWallet();
        var jettonWalletAddress = new Address("kQBssgnUtQOpJWvIOiBT6A8tSB46P_aY6ULhCS62FeNJ2wfl");

        var options = new JettonTransferOptions()
        {
            Amount = new Coins(100), // jetton amount to send, for ex 100 jettons
            Destination = new Address("0QBjgZpwfQhuFd409R9elG8J0kut9WF9O9BmcYxGK6fh8sRo"), // receiver
            ForwardPayload = GetStringPayload("jetton transfer from multisig test")
        };
        var jettonTransfer = JettonWallet.CreateTransferRequest(options);

        var order = new MultiSigOrderBuilder(multisigWallet.WalletId)
          .AddTransferMessage(jettonWalletAddress, new Coins(0.1), jettonTransfer)
          .Build();
        order.Sign(0, masterK);

        var msg = multisigWallet.CreateTransferMessage(order, masterW.PublicKey);
        msg.Sign(masterK);

        await SendMessage(tonClient, msg.Cell);
        return;
    }

    private Cell GetStringPayload(string data)
    {
        return new CellBuilder().StoreUInt(0, 32).StoreString(data).Build();
    }

    // Создает транзакцию с нескольиим перациями в валюте TON
    // TODO: требуется доработка. Вместо "test payment from multisig" нужно отправлять нормальное сообщение типа WalletTransfer
    public async Task CreateOrder(string walletAddr)
    {
        var tonClient = GetClient();
        var multisig = await tonClient.GetExistsMultisigWallet(walletAddr);
        var (master, key) = GetMasteWallet();

        var orderBuiler = new MultiSigOrderBuilder(multisig.WalletId);
        orderBuiler.AddTransferMessage(master.Address, new Coins(0.001), "test payment from multisig");
        var order = orderBuiler.Build();

        var msg = multisig.CreateTransferMessage(order, master.PublicKey);
        msg.Sign(key);

        await SendMessage(tonClient, msg.Cell);
        return;
    }


    // Пример создания обычного кошелька v4r2
    public async Task<string> CreateWallet2(CreateWalletCommand command)
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

    public Task AddAssetAsync(AddStellarAssetCommand command)
    {
        throw new NotImplementedException();
    }

    public Task AddPaymentAsync(AddPaymentCommand command)
    {
        throw new NotImplementedException();
    }

    public Task<string> CreateInvoiceXdr(CreateInvoiceXdrCommand command)
    {
        throw new NotImplementedException();
    }

    public Task<string> CreateWalletAsync(CreateWalletCommand command)
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





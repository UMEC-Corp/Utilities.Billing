﻿using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using TonSdk.Client;
using TonSdk.Contracts;
using TonSdk.Contracts.Wallet;
using TonSdk.Core;
using TonSdk.Core.Block;
using TonSdk.Core.Boc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Utilities.Billing.TonWallets.SmartContracts.Multisig;

public class MultisigWallet : ContractBase
{
    // код смарт-контракта multisig-wallet
    // https://github.com/ton-blockchain/multisig-contract
    private string _codeHexStr = "B5EE9C7241022B01000418000114FF00F4A413F4BCF2C80B010201200203020148040504DAF220C7008E8330DB3CE08308D71820F90101D307DB3C22C00013A1537178F40E6FA1F29FDB3C541ABAF910F2A006F40420F90101D31F5118BAF2AAD33F705301F00A01C20801830ABCB1F26853158040F40E6FA120980EA420C20AF2670EDFF823AA1F5340B9F2615423A3534E202321220202CC06070201200C0D02012008090201660A0B0003D1840223F2980BC7A0737D0986D9E52ED9E013C7A21C2125002D00A908B5D244A824C8B5D2A5C0B5007404FC02BA1B04A0004F085BA44C78081BA44C3800740835D2B0C026B500BC02F21633C5B332781C75C8F20073C5BD0032600201200E0F02012014150115BBED96D5034705520DB3C82A020148101102012012130173B11D7420C235C6083E404074C1E08075313B50F614C81E3D039BE87CA7F5C2FFD78C7E443CA82B807D01085BA4D6DC4CB83E405636CF0069006027003DAEDA80E800E800FA02017A0211FC8080FC80DD794FF805E47A0000E78B64C00017AE19573FC100D56676A1EC40020120161702012018190151B7255B678626466A4610081E81CDF431C24D845A4000331A61E62E005AE0261C0B6FEE1C0B77746E10230189B5599B6786ABE06FEDB1C6CA2270081E8F8DF4A411C4A05A400031C38410021AE424BAE064F6451613990039E2CA840090081E886052261C52261C52265C4036625CCD8A30230201201A1B0017B506B5CE104035599DA87B100201201C1D020399381E1F0111AC1A6D9E2F81B60940230015ADF94100CC9576A1EC1840010DA936CF0557C160230015ADDFDC20806AB33B50F6200220DB3C02F265F8005043714313DB3CED54232A000AD3FFD3073004A0DB3C2FAE5320B0F26212B102A425B3531CB9B0258100E1AA23A028BCB0F269820186A0F8010597021110023E3E308E8D11101FDB3C40D778F44310BD05E254165B5473E7561053DCDB3C54710A547ABC242528260020ED44D0D31FD307D307D33FF404F404D1005E018E1A30D20001F2A3D307D3075003D70120F90105F90115BAF2A45003E06C2121D74AAA0222D749BAF2AB70542013000C01C8CBFFCB0704D6DB3CED54F80F70256E5389BEB198106E102D50C75F078F1B30542403504DDB3C5055A046501049103A4B0953B9DB3C5054167FE2F800078325A18E2C268040F4966FA52094305303B9DE208E1638393908D2000197D3073016F007059130E27F080705926C31E2B3E630062A2728290060708E2903D08308D718D307F40430531678F40E6FA1F2A5D70BFF544544F910F2A6AE5220B15203BD14A1236EE66C2232007E5230BE8E205F03F8009322D74A9802D307D402FB0002E83270C8CA0040148040F44302F0078E1771C8CB0014CB0712CB0758CF0158CF1640138040F44301E201208E8A104510344300DB3CED54925F06E22A001CC8CB1FCB07CB07CB3FF400F400C9B99895F4";
    private uint _walletId;
    private byte _requiredSigners;
    private long _lastCleaned;
    private Dictionary<uint, byte[]> _publicKeys = new Dictionary<uint, byte[]>();
    private Dictionary<uint, byte[]> _pendingQueries = new Dictionary<uint, byte[]>();

    public uint WalletId => _walletId;
    public Dictionary<uint, byte[]> PublicKeys => _publicKeys;

    public MultisigWallet(MultisigWalletOptions opt)
    {
        _code = Cell.From(_codeHexStr);
        _walletId = opt.WalletId.GetValueOrDefault(698983191u);
        _requiredSigners = opt.RequiredSigners;
        _lastCleaned = opt.LastCleaned;

        var keys = opt.PublicKeys;
        for (uint i = 0; i < keys.Count(); i++)
        {
            PublicKeys.Add(i, keys[i]);
        }

        var quries = opt.PendingQueries;
        for (uint i = 0; i < quries.Count(); i++)
        {
            _pendingQueries.Add(i, quries[i]);
        }

        _stateInit = BuildStateInit();
        _address = opt.Address == null ? new Address(opt.Workchain.GetValueOrDefault(), _stateInit) : new Address(opt.Address);
    }


    public static HashmapOptions<uint, byte[]> GetOwnersDictOptions()
    {
        return new HashmapOptions<uint, byte[]>
        {
            KeySize = 8,

            Serializers = new HashmapSerializers<uint, byte[]>
            {
                Key = k => new BitsBuilder(8).StoreInt(k, 8).Build(),

                // Для понимания как хранятся данные владельца кошелька, см. ф-ию pack_owner_info(...) в исходном коде смарт-контракта 
                Value = v => new CellBuilder()
                    .StoreBytes(v)      //.store_uint(public_key, 256)  
                    .StoreUInt(0, 8)    //.store_uint(flood, 8);        
                    .Build()
            },

            Deserializers = new HashmapDeserializers<uint, byte[]>
            {
                Key = k => (uint)k.Parse().LoadUInt(8),
                Value = v => v.Parse().LoadBytes(32)
            }
        };
    }

    protected override StateInit BuildStateInit()
    {
        var ownerInfos = new HashmapE<uint, byte[]>(GetOwnersDictOptions());
        foreach (var pk in _publicKeys)
        {
            ownerInfos.Set(pk.Key, pk.Value);
        }

        // Для понимания как хранятся данные кошелька, см. ф-ию pack_state(...) в исходном коде смарт-контракта
        var data = new CellBuilder()
            .StoreUInt(_walletId, 32)            //.store_uint(wallet_id, 32)
            .StoreUInt(_publicKeys.Count, 8)     //.store_uint(n, 8)
            .StoreUInt(_requiredSigners, 8)      //.store_uint(k, 8)
            .StoreUInt(_lastCleaned, 64)         //.store_uint(last_cleaned, 64)
            .StoreDict(ownerInfos)               //.store_dict(owner_infos)
            .StoreBit(b: false)                  //.store_dict(pending_queries) 
            .Build();

        var opt = new StateInitOptions
        {
            Code = _code,
            Data = data
        };

        return new StateInit(opt);
    }

    public ExternalInMessage CreateDeployMessage()
    {
        var opt = default(ExternalInMessageOptions);
        opt.Info = new ExtInMsgInfo(new ExtInMsgInfoOptions
        {
            Dest = _address,
        });
        opt.StateInit = _stateInit;
        return new ExternalInMessage(opt);
    }

    public ExternalInMessage CreateTransferMessage(MultiSigOrder order, byte[] publicKey)
    {
        var ownerId = GetOwnerIdByPublicKey(publicKey);
        var body = order.ToCell(ownerId);

        var opt = default(ExternalInMessageOptions);
        opt.Info = new ExtInMsgInfo(new ExtInMsgInfoOptions
        {
            Dest = _address,
        });
        opt.Body = body;
        return new ExternalInMessage(opt);
    }

    private uint GetOwnerIdByPublicKey(byte[] publicKey)
    {
        var query = _publicKeys.Where(x => x.Value.SequenceEqual(publicKey));
        if (!query.Any())
        {
            throw new Exception("public key is not an owner");
        }

        return query.First().Key;
    }

}

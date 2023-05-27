using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using stellar_dotnet_sdk;
using Utilities.Billing.Contracts;
using Utilities.Billing.StellarWallets;

namespace Utilities.Billing.Tests.StellarWallets;
class WalletsManagementTests
{
    private const string HorizonUrl = "https://horizon-testnet.stellar.org";

    [Test]
    public async Task Should_Create_Wallet()
    {
        var services = new ServiceCollection();
        services.UseStellarWallets(new StellarWalletsSettings
        {
            HorizonUrl = HorizonUrl,
            SecretSeed = KeyPair.Random().SecretSeed
        });

        var provider = services.BuildServiceProvider();
        var paymentSystem = provider.GetRequiredService<IPaymentSystem>();

        var wallet =
            await paymentSystem.CreateWalletAsync(new CreateWalletCommand(TenantId: Guid.NewGuid(), Token: "TEST"));

        Assert.That(wallet, Is.Not.Empty);
    }

    [Test]
    public async Task Should_Create_Asset()
    {
        Assert.Inconclusive();
    }
}

# Utilities.Billing

This system provides a decentralized and transparent platform for tracking energy consumption and facilitating automated payments using the Stellar blockchain. It is designed for energy service providers, such as utility companies or smart building operators, who require precise metering, seamless payment processing, and auditability across customer devices.



## 📌 How does onboarding start?

1. **Registering a Management Company**

   * An energy service provider (the "tenant") is registered in the system using the `AddTenant` method.
   * As part of the process, a **Stellar account (wallet)** must be created in advance and linked to the company.
   * The company provides the **public address** of the Stellar account that will be used for issuing assets and managing balances.

2. **Creating a Custom Asset**

   * The company must create a **custom asset** on the Stellar blockchain. This token will represent the energy consumption value.
   * This is done either via the system's `addAsset` method or directly through the Stellar SDK or Horizon API.
   * The asset is issued from the company's Stellar account, which becomes the **asset issuer**.

---

## 📌 How are new customer devices added?

1. **Creating a Customer Account**

   * Each metering device (e.g., electricity meter) is linked to a dedicated **Stellar account**, created via the `CreateCustomerAccount` method.
   * The **serial number** of the device is passed as a parameter to associate it with its account.

2. **Activating the Account**

   * A minimum balance of **3 XLM** must be transferred to the account to make it active (per [Stellar's base reserve requirements](https://developers.stellar.org/docs/encyclopedia/minimum-balance/)).

3. **Account Ownership**

   * The system discards the original secret key of the device account and assumes full control by signing all transactions using its **master signer** (system account).
   * Effectively, the **customer account is custodial**, owned and managed by the platform.

---

## 📌 How is consumption recorded?

1. **Receiving Meter Readings**

   * The system listens for messages from devices (e.g., via MQTT or another messaging protocol).
   * Each new reading is interpreted as energy consumed and converted into an amount of the custom asset.

2. **Crediting the Account**

   * The consumption value is tokenized and **credited to the customer’s Stellar account** in the form of the issued asset.

3. **Storing Outstanding Balance**

   * The balance of the asset on the customer’s account represents the **accrued energy debt**.
   * Balances can be viewed publicly via any Stellar blockchain explorer.

---

## 📌 How is consumption paid for?

1. **Creating an Invoice**

   * The system provides a `CreateInvoice` method that:

     * Takes the **customer account** (i.e., metering account),
     * A **payer account** (usually the user who will pay),
     * And the **amount to be paid**.
   * The method returns an **XDR-formatted transaction** (Stellar transaction envelope), ready to be signed.

2. **Signing and Submitting**

   * The payer signs the XDR with their Stellar keypair.
   * Once signed and submitted, the system executes a two-step operation:

     * Transfer from the **customer account** to the **platform master account** (internal consumption recognition),
     * Transfer from the **payer account** to the **management company’s account** (actual payment).

3. **Trustline Requirements**

   * Both the customer and the payer must **create trustlines** (`ChangeTrust` operation) to accept the custom asset.
   * This is a mandatory prerequisite in Stellar for holding non-native assets.

---

## Additional Notes

* **Muxed (multiplexed) accounts** are not supported at this time.
* All device accounts are **custodial**, with secret keys discarded and operations signed by the platform.
* Users must use external Stellar-compatible wallets to sign and submit XDR transactions returned by the system.

namespace Utilities.Billing.Grains
{
    public interface IDeviceGrain : IGrainWithStringKey
    {
        Task DeleteInputState(string inputCode);
        Task<InputInfo?> GetInputState(string code);
        Task UpdateInputState(string code, InputInfo info);
    }
}
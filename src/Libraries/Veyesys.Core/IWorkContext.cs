using System.Threading.Tasks;
using Veyesys.Core.Domain.Customers;
using Veyesys.Core.Domain.Directory;
using Veyesys.Core.Domain.Localization;

namespace Veyesys.Core
{
    /// <summary>0
    /// Represents work context
    /// </summary>
    public interface IWorkContext
    {
        /// <summary>
        /// Gets the current customer
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task<Customer> GetCurrentCustomerAsync();

        /// <summary>
        /// Sets the current customer
        /// </summary>
        /// <param name="customer">Current customer</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task SetCurrentCustomerAsync(Customer customer = null);

        /// <summary>
        /// Gets the original customer (in case the current one is impersonated)
        /// </summary>
        Customer OriginalCustomerIfImpersonated { get; }


        /// <summary>
        /// Gets current user working language
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task<Language> GetWorkingLanguageAsync();

        /// <summary>
        /// Sets current user working language
        /// </summary>
        /// <param name="language">Language</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task SetWorkingLanguageAsync(Language language);

        /// <summary>
        /// Gets or sets current user working currency
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task<Currency> GetWorkingCurrencyAsync();

        /// <summary>
        /// Sets current user working currency
        /// </summary>
        /// <param name="currency">Currency</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task SetWorkingCurrencyAsync(Currency currency);
  
    }
}

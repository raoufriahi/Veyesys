using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Veyesys.Core;
using Veyesys.Core.Domain.Catalog;
using Veyesys.Core.Domain.Common;
using Veyesys.Core.Domain.Customers;
using Veyesys.Core.Domain.Directory;
using Veyesys.Core.Domain.Localization;
using Veyesys.Core.Domain.Logging;
using Veyesys.Core.Domain.Seo;
using Veyesys.Core.Domain.Stores;
using Veyesys.Core.Infrastructure;
using Veyesys.Data;

namespace Veyesys.Services.Installation
{
    /// <summary>
    /// Installation service
    /// </summary>
    public partial class InstallationService : IInstallationService
    {
        #region Fields

        private readonly IVeDataProvider _dataProvider;
        private readonly IVeFileProvider _fileProvider;
        private readonly IRepository<ActivityLogType> _activityLogTypeRepository;
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<Currency> _currencyRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Language> _languageRepository;
        private readonly IRepository<MeasureDimension> _measureDimensionRepository;
        private readonly IRepository<MeasureWeight> _measureWeightRepository;
        private readonly IRepository<StateProvince> _stateProvinceRepository;
        private readonly IRepository<Store> _storeRepository;
        private readonly IRepository<UrlRecord> _urlRecordRepository;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public InstallationService(IVeDataProvider dataProvider,
            IVeFileProvider fileProvider,
            IRepository<ActivityLogType> activityLogTypeRepository,
            IRepository<Address> addressRepository,
            IRepository<Country> countryRepository,
            IRepository<Currency> currencyRepository,
            IRepository<Customer> customerRepository,
            IRepository<Language> languageRepository,
            IRepository<MeasureDimension> measureDimensionRepository,
            IRepository<MeasureWeight> measureWeightRepository,
            IRepository<ProductAttribute> productAttributeRepository,
            IRepository<StateProvince> stateProvinceRepository,
            IRepository<Store> storeRepository,

            IRepository<UrlRecord> urlRecordRepository,
            IWebHelper webHelper)
        {
            _dataProvider = dataProvider;
            _fileProvider = fileProvider;
            _activityLogTypeRepository = activityLogTypeRepository;
            _addressRepository = addressRepository;
            _countryRepository = countryRepository;
            _currencyRepository = currencyRepository;
            _customerRepository = customerRepository;
            _languageRepository = languageRepository;
            _measureDimensionRepository = measureDimensionRepository;
            _measureWeightRepository = measureWeightRepository;
            _stateProvinceRepository = stateProvinceRepository;
            _storeRepository = storeRepository;

            _urlRecordRepository = urlRecordRepository;
            _webHelper = webHelper;
        }

        #endregion

        public virtual async Task InstallRequiredDataAsync(string defaultUserEmail, string defaultUserPassword,
           (string languagePackDownloadLink, int languagePackProgress) languagePackInfo, RegionInfo regionInfo, CultureInfo cultureInfo)

        {
            return;

        }
        public virtual async Task InstallSampleDataAsync(string defaultUserEmail)
        {

            return;
        }

    }
}
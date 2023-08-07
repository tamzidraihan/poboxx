using SmartBox.Infrastructure.Data.Repository.ApplicationSettings;
using SmartBox.Infrastructure.Data.Repository.Locker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.OTP
{
    public class OTPService : IOTPService
    {
        private readonly IApplicationSettingRepository _appSettingRepository;
        private readonly ILockerRepository lockerRepository;
        public OTPService(IApplicationSettingRepository appSettingRepository, ILockerRepository lockerRepository)
        {
            _appSettingRepository = appSettingRepository;
            this.lockerRepository = lockerRepository;
        }

        public async Task<string> GenerateOTP()
        {
            var appSetting = _appSettingRepository.GetApplicationSetting().Result;
            var otp = Shared.SharedServices.GenerateOTP(appSetting.OTPLength, appSetting.IsOTPMixCharacters);
            var existingBooking = await lockerRepository.GetOTP(otp);
            if (existingBooking == null) return otp;
            return await GenerateOTP();
        }

        public DateTime OTPExpiration()
        {
            var appSetting = _appSettingRepository.GetApplicationSetting().Result;
            return DateTime.Now.AddMinutes(appSetting.OTPMinutes);
        }
    }
}

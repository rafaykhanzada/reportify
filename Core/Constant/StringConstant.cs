using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Constant
{
    public static class LogString
    {
        public const string Error = "Error";

    }
    public static class MessageString
    {
        public const string Deleted = "Record Deleted Successfully.";
        public const string Updated = "Record Updated Successfully.";
        public const string Created = "Record Insert Successfully.";
        public const string Success = "Operation Completed Successfully.";
        public const string Verified = "Verification Completed Successfully.";
        public const string ServerError = "Internal Server Error Please Contact Adminstrator.";
        public const string LoginError = "Invalid Credentials";
        public const string LockError = "User account locked out.";
        public const string InvalidLogin = "Invalid login attempt.";
        public const string Invalid = "Invalid attempt.";
        public const string NotFound = "Record Not Found";
        public const string CreateError = "Error While Creating Record";
        public const string AlreadyExist = "Record Already Exist!";
        public const string AlreadyExistDAWSM = "Dear Customer! This mobile No. is already registered as Salesperson in our record, Consumer Discount is not applicable here.";
        public const string DeleteError = "Error While Deleting Record";
        public const string ValidationError = "Please provide all fields!";
        public const string LocationError = "Invalid Location Error!";
        public const string LimitError = "Max Limit Excuted!";
        /* QRCODE */
        public const string InvalidQR = "Dear Customer!\r\nWe’re sorry, but this QR code doesn’t seem to be working. Please try scanning it again, or reach out for any assistance.\r\n";
        public const string Expired = InvalidQR;
        public const string LineExpired = "Dear Customer!\r\nWe’re sorry, but this QR code doesn’t seem to be working. Please try scanning it again, or reach out for any assistance.\r\n";
        public const string InvalidLocation = "Dear Customer!\r\nYour current location doesn't match the dealer’s QR code. Please ensure you're at the right dealer and scan again.\r\n";
        //public const string InvalidCnic = "Dear Customer!\r\nYour CNIC, mobile number, and email combination doesn’t match our records. Please check your details and try again.\r\n";
        public const string InvalidCnic = "Dear Customer!\r\nYour CNIC, mobile number combination doesn’t match our records. Please check your details and try again.\r\n";
        public const string PolicyNotFound = "Dear Customer!\r\nWe’re sorry, but there’s no discount on this product at the moment.\r\n";
        public const string ProductLimited = "Dear Customer!\r\nYou've reached the maximum purchase limit for this product {0}. Unfortunately! There is no discount available.";

    }
    public static class CacheSuffix
    {
        public const string LocaleStringResourcesSuffix = "Localized.Property";
        public const string LocalizedPropertiesPrefix = "Localized.Property.Resource";
        public const string AppFileSuffix = "AppFile";
        public const string AppDealer = "AppDealer";
        public const string AppDealerShop = "AppDealerShop";
        public const string AppProduct = "AppProduct";
        public const string AppCategory = "AppCategory";
        public const string AppModel = "AppModel";

    }
    public static class ConstantStrings
    {
        public const string StripeHeaderText = "Stripe Header text";
        public const string DateTimeFormat = "dd/MM/yyyy HH:mm";
    }
    public static class TemplateStrings
    {
        public const string OTP = "Verification code\nCODE:{0}\nThank you for choosing Dawlance! Your OTP is {0}. Enter this code to avail special cash discounts on your purchase This OTP is valid for {1} minutes.";

        public const string Voucher = "Hi {0},\r\nCongratulations! Your OTP has been verified successfully!\r\nUse voucher code: {1} to get Rs. {2}/- Cash Discount\r\nHappy shopping!\r\nwww.dawlance.com.pk";

        public const string Campaign = "Hi {0},\r\nCongratulations! Your OTP has been verified successfully!\r\nUse voucher code: {1} to get deal price of Rs. {2}/- on this Product \r\nHappy shopping!\r\nwww.dawlance.com.pk";

    }
    public static class ApplicationRole
    {
        public const string Guest = "Guest";
        public const string RegisterUser = "User";
        public const string Admin = "Admin";
    }
    public static class DiscountPolicyStrings
    {
        public const string L1 = "L1";
        public const string L2 = "L2";
        public const string L3 = "L3";
        public const string L4 = "L4";
        public const string C1 = "C1";
    }
    public static class TaxMethods
    {
        public const string Exclusive = "Exclusive";
        public const string Inclusive = "Inclusive";
    }
    public static class ProductType
    {
        public const string Physical = "Physical";
        public const string Virtual = "Virtual";
    }
    public static class DiscountType
    {
        public const string Campagin = "Campagin";
        public const string Policy = "Discount Policy";
        public const string NoPolicy = "No Discount";
    }
    public static class ExportType
    {
        public const string Excel = "xlsx";
        public const string CSV = "csv";
        public const string MSWORD = "docx";
    }
    public static class Settings
    {
        public const string StoreCacheOnStartup = "StoreCacheOnstartp";
    }
    public static class SiteUrl
    {
        public const string Referer = "Referer";
        public static string FilePath { get; set; }
    }
    public static class EnvirmenttStrings
    {
        public const string IsDevelopment = "Development";
        public const string IsProduction = "Production";
    }
}

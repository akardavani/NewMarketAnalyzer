using MarketAnalyzer.Abstractions;
using System;

namespace MarketAnalyzer.SharedKernel
{
    public class PersonUpsertedNotification : INotification
    {
        /// <summary>
        /// شناسه شخص
        /// </summary>
        public required long PersonId { get; init; }
        /// <summary>
        /// نام کامل شخص
        /// </summary>
        public required string Name { get; init; }
        /// <summary>
        /// کد ملی/شناسه ملی شرکت/پم کد سبد/شناسه اتباع خارجه
        /// </summary>
        public required string NationalCode { get; init; }
        /// <summary>
        /// توضیحات
        /// </summary>
        public required string Description { get; init; }
        /// <summary>
        /// کد حساب تفصیلی
        /// </summary>
        public required string DetailAccountCode { get; init; }
        /// <summary>
        /// شناسه نوع شخص
        /// </summary>
        public required int PersonTypeId { get; init; }
        /// <summary>
        /// نوع شخص
        /// </summary>
        public required string PersonTypeName { get; init; }
        /// <summary>
        /// نام پدر
        /// </summary>
        public required string FatherName { get; init; }
        /// <summary>
        /// تاریخ تولد شخص حقیقی/تاریخ ثبت شرکت
        /// </summary>
        public required DateOnly BirthDate { get; init; }
        /// <summary>
        /// شماره شناسنامه شخص حقیقی/شماره ثبت حقوقی
        /// </summary>
        public required string IdentityNumber { get; init; }
        /// <summary>
        /// محل صدور شخص حقیقی/محل ثبت حقوقی
        /// </summary>
        public required string IdentityIssuancePlace { get; init; }
        /// <summary>
        /// تلفن تماس
        /// </summary>
        public required string PhoneNumber { get; init; }
        /// <summary>
        /// ایمیل
        /// </summary>
        public required string Email { get; init; }
        /// <summary>
        /// مجوز صدور/ابطال اینترنتی
        /// </summary>
        public required bool RequestPermit { get; init; }
        /// <summary>
        /// صدور از محل سود تقسیمی
        /// </summary>
        public required bool ReinvestmentPermit { get; init; }
        /// <summary>
        /// درصد صدور از محل سود تقسیمی
        /// </summary>
        public required decimal ReinvestmentPercentage { get; init; }
        /// <summary>
        /// ایجاد حساب کاربری در پورتال
        /// </summary>
        public required bool CreatePortalUser { get; init; }
        /// <summary>
        /// وضعیت سجام
        /// </summary>
        public required bool SejamAuthenticated { get; init; }
        /// <summary>
        /// شناسه کاربر
        /// </summary>
        public required int OperatorId { get; init; }
    }
}

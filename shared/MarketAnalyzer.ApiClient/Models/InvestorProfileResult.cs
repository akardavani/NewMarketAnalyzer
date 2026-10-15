using System.Collections.Generic;
using System;

namespace MarketAnalyzer.ApiClient.Models
{
  public class InvestorProfileResult
  {
    /// <summary>
    /// شخص حقیقی
    /// </summary>
    public required bool IsNaturalPerson { get; init; }
    /// <summary>
    /// نوع شخص
    /// </summary>
    public required string PersonType { get; init; }
    /// <summary>
    /// کد ملی/شناسه ملی شرکت/پم کد سبد/شناسه اتباع خارجه
    /// </summary>
    public required string NationalCode { get; init; }
    /// <summary>
    /// تبعه خارجی
    /// </summary>
    public required bool Foreigner { get; init; }
    /// <summary>
    /// نام
    /// </summary>
    public required string FirstName { get; init; }
    /// <summary>
    /// نام خانوادگی
    /// </summary>
    public required string LastName { get; init; }
    /// <summary>
    /// نام پدر
    /// </summary>
    public required string FatherName { get; init; }
    /// <summary>
    /// کد اقتصادی
    /// </summary>
    public required string EconomicCode { get; init; }
    /// <summary>
    /// تاریخ تولد/تاریخ ثبت
    /// </summary>
    public required DateOnly RegistrationOrBirthDate { get; init; }
    /// <summary>
    /// محل تولد
    /// </summary>
    public required string BirthPlace { get; init; }
    /// <summary>
    /// شماره شناسنامه
    /// </summary>
    public required string IdentityNumber { get; init; }
    /// <summary>
    /// سری شناسنامه
    /// </summary>
    public required string IdentitySequnceNumber { get; init; }
    /// <summary>
    /// سریال شناسنامه
    /// </summary>
    public required string IdentitySerialNumber { get; init; }
    /// <summary>
    /// محل صدور شناسنامه/محل ثبت
    /// </summary>
    public required string RegistrationOrIdentityIssuancePlace { get; init; }
    /// <summary>
    /// جنسیت
    /// </summary>
    public required string Gender { get; init; }
    /// <summary>
    /// نام شرکت
    /// </summary>
    public required string Name { get; init; }
    /// <summary>
    /// شماره ثبت
    /// </summary>
    public required string RegistrationNumber { get; init; }
    /// <summary>
    /// نوع شرکت یا موسسه
    /// </summary>
    public required string CompanyType { get; init; }
    /// <summary>
    /// تماس ها
    /// </summary>
    public required IReadOnlyCollection<Contact> Contacts { get; init; }
    /// <summary>
    /// نشانی ها
    /// </summary>
    public required IReadOnlyCollection<Address> Addresses { get; init; }
    /// <summary>
    /// حسابهای بانکی
    /// </summary>
    public required IReadOnlyCollection<BankAccount> BankAccounts { get; init; }

    public class Address
    {
      public required string Content { get; init; }
      public required string Number { get; init; }
      public required string PostalCode { get; init; }
      public required string Region { get; init; }
      public required string AddressType { get; init; }
    }

    public class BankAccount
    {
      public required string AccountNumber { get; init; }
      public required string ShebaNumber { get; init; }
      public required string Bank { get; init; }
      public required string Branch { get; init; }
      public required string AccountType { get; init; }
      public required bool IsDefault { get; init; }
    }

    public class Contact
    {
      public required string Content { get; init; }
      public required string ContactType { get; init; }
    }
  }
}


declare global {
  interface AppSettings {
    captchaUrl: string;
    captchaLifetime: number;
    apiUrl: string;
    appName: string;
    version: string;
    /** عنوان محصول (مثلاً «صندوق مبین سرمایه»). اختیاری؛ پیش‌فرض: appName. */
    productName?: string;
    /** نام کامل محصول برای صفحات ورود (اختیاری؛ پیش‌فرض: productName). */
    productFullName?: string;
    /** نام مدیر/سبدگردان زیر عنوان صفحات ورود (اختیاری؛ خالی = نمایش داده نمی‌شود). */
    managerName?: string;
    /** نام شرکت سازنده. اختیاری. */
    companyName?: string;
    /** آدرس لوگو. اختیاری. */
    logoUrl?: string;
    /** دکمهٔ میان‌بر هدر (مثلاً «درخواست جدید»). اگر path خالی باشد دکمه نمایش داده نمی‌شود. */
    quickActionLabel?: string;
    quickActionPath?: string;
  }
  const appSettings: AppSettings;
}

export interface PlainObject<TData = any> {
  [key: string]: TData;
}

interface SucceededApiResult<T = void> {
  successed: true;
  data: T;
  errors?: string[];
}

interface FailedApiResult<T = void> {
  successed: false;
  data?: T;
  errors: string[];
}

export type ApiResult<T = void> = SucceededApiResult<T> | FailedApiResult<T>;

export function isApiResult<T = any>(obj: any): obj is ApiResult<T> {
  return !!obj &&
    typeof (obj) == 'object' &&
    typeof (obj['successed']) == 'boolean' &&
    ('errors' in obj || 'data' in obj);
}

export interface AuthResult {
  accessToken: string;
  lifetime: number;
  requiredTfa: boolean;
}

export interface AuthData {
  accessToken: string;
  expireDate: number;
}

export type Nullable<T> = T | null;

export type NumericKeys<T> = { [K in keyof T]: T[K] extends number ? K : never; }[keyof T];

export interface QueryResult<T = void> {
  itemsCount: number;
  rows: T[];
}

export interface SelectOption<T = number> {
  value: T;
  text: string;
}

export interface QueryParam<T = void> {
  rows: number;
  page: number;
  getTotals: boolean;
  filter: FilterRule<T>;
  orderBy?: PlainObject<SortType>;
}

export interface FilterRule<T> {
  /** The name of the field that the filter applies to. */
  field?: keyof T;
  /** The condition type of @see ConditionType */
  condition?: ConditionType;
  /** The filter operator of @see FilterOperator */
  operator?: FilterOperator;
  /** Gets or sets nested filter rules. */
  rules?: FilterRule<T>[];
  /** Gets or sets the value of the filter. */
  value?: any;
  /** Apply not operator to rule. */
  not?: boolean;
}

export type Func<TInput, TOutput> = (input: TInput) => TOutput;

export interface DateRange {
  startDate: Date;
  endDate: Date;
}

export interface FiscalYearOption extends SelectOption<number> {
  readonly value: number;
  readonly startDate: Date;
  readonly endDate: Date;
  readonly text: string;
  readonly status: FiscalYearStatus;
}

export interface PaymentRequestBankAccount {
  id: number;
  title: string;
  detailAccountCode: string;
  paymentTypes: SelectOption[];
}


//#region [Enums]

export enum AppState {
  RequiredSettings,
  RequiredFiscalYear,
  Ready
}

export enum LoginType {
  StaticPassword = 1,
  OneTimePassword = 2
}

export enum SortType {
  ASC = 0,
  DESC = 1
}

export enum ConditionType {
  AndAlso = 1,
  OrElse = 2
}

export enum FilterOperator {
  /** Equals */
  Eq = 1,
  /** Not Equals */
  Neq,
  /** Greater than */
  Gt,
  /** Less than */
  Lt,
  /** Less than or equal to */
  Lte,
  /** Greater than or equal to */
  Gte,
  /** Between */
  Between,
  /** In */
  In,
  /** Contain (only for string) */
  Contains,
  /** Starts with (only for string) */
  StartsWith,
  /** Ends with (only for string) */
  EndsWith,
  /** Is null */
  IsNull,
  /** Is not null */
  IsNotNull
}

export enum ReportFormat {
  Html = 0,
  Pdf = 1,
  Excel = 2
}

export enum FiscalYearStatus {
  Active = 1,
  Closed = 2
}

export enum AccountEntryLevel {
  /**گروه*/
  Group = 1,
  /**کل*/
  Ledger = 2,
  /**معین*/
  Subsidiary = 3,
  /**معین سطح 1*/
  Level1Subsidiary = 4,
  /**تفصیلی*/
  Detail = 5
}

export enum OrderSide {
  /** خرید */
  Buy = 1,
  /** فروش */
  Sell = 2,
  /** تسویه فیزیکی */
  PhysicalSettlement = 3,
  /** تسویه نقدی */
  CashSettlement = 4,
  /** عدم اعمال اختیار */
  OptionNonExercise = 5,
  /** فروش حق تقدم استفاده نشده */
  SellNotAppliedRight = 6,
  /** فروش آپشن متناظر */
  SellCounterpartOptions = 7
}

export enum TradeSettlementAccountType {
  /** کارگزار */
  Broker = 0,
  /** بانک */
  Bank = 1,
  /** سایر */
  Other = 2
}

export enum BbsFileType {
  /** DDN فایل اکسل دریافتی از سامانه  */
  DdnExcel = 0,
  /** فایل اکسل با فرمت سفارشی شده */
  CustomExcel = 1,
  /** Json فایل */
  Json = 2
}

export enum OptionSettlementType {
  /** تسویه فیزیکی */
  PhysicalSettlement = 1,
  /** تسویه نقدی  */
  CashSettlement = 2
}

export enum EmbeddedsIssuingPurpose {
  /** حمایتی */
  Supportive = 1,
  /** تامین مالی */
  Financing = 2
}

export enum InstrumentTypeCategory {
  /** سهام و حق تقدم */
  StocksAndRights = 1,
  /** اوراق با درآمد ثابت */
  Bonds = 2,
  /** اختیار معامله و اختیار تبعی */
  EmbeddedsAndOptions = 3
}

export enum InvestmentType {
  /** قابل معامله (ETF) */
  Negotiable = 1,
  /** صدور و ابطال */
  IssuanceAndCancellation = 2,
}

export enum ProfitLossResportInstrumentType {
  /** سهام و حق تقدم */
  StockRight = 0,
  /** غیر از سهام و حق تقدم*/
  Other = 1
}

export enum ProfitLossResportGroup {
  /** سهم  */
  Instrument = 0,
  /** صنعت */
  Sector = 1
}

export enum OptionType {
  /** اختیار فروش */
  Put = 1,
  /** اختیار خرید */
  Call = 2
}

export enum OptionPosition {
  /** موقعیت فروش */
  Short = 1,
  /** موقعیت خرید */
  Long = 2
}

export enum OptionClearingType {
  /** نقدی */
  Cashly = 0,
  /** عدم اعمال */
  NonExercise = 1,
  /** خرید ناشی از اعمال اختیار */
  BuyResultOfExercise = 2,
  /** فروش ناشی از اعمال اختیار */
  SellResultOfExercise = 3
}

export enum BatchRequestsFileType {
  /** فایل اکسل سامانه */
  InternalExcel = 0,
  /** فایل اکسل سایر صندوق‌ها */
  ExternalExcel = 1
}

export enum PersonType {
  /** حقیقی */
  Natural = 1,
  /** حقیقی خارجی */
  ForeignNatural = 2,
  /** حقوقی */
  Legal = 3,
  /** حقوقی خارجی */
  ForeignLegal = 4,
  /** سبد */
  Basket = 5
}

/** منشأ اعتبار درخواست صدور */
export enum IssuanceRequestCreditOrigin {
  /** بانک */
  Bank = 1,
  /** حساب */
  Account = 2
}

/** شیوه واریز */
export enum PaymentMethod {
  /** واریز اینترنتی */
  OnlineDeposit = 1,
  /** فیش واریزی */
  DepositSlip = 2,
  /** درگاه پرداخت آنلاین */
  OnlineGateway = 3,
  /** نامعین */
  Unknown = 4,
  /** حساب صندوق */
  FundAccount = 5
}

export enum Gender {
  /** مرد */
  Male = 1,
  /** زن */
  Female = 2,
}

export enum RegionType {
  /** کشور */
  Country = 1,
  /** استان */
  Province = 2,
  /** شهرستان */
  County = 3,
  /** شهر */
  City = 4,
  /** بخش */
  District = 5,
  /** دهستان */
  RuralDistrict = 6,
  /** روستا */
  Village = 7,
  /** آبادی */
  Hamlet = 8,
  /** محله */
  Neighborhood = 9,
  /** بزرگراه */
  Highway = 10,
  /** خیابان */
  Street = 11,
  /** کوچه */
  Alley = 12,
  /** بن بست، بلوک، محوطه */
  Block = 13,
  /** منطقه */
  Area = 14
}

//#endregion




//#region [Setup]

export interface UpsertFundSettings {
  name: string;
  englishName: string;
  bourseCode: string;
  webSite: string;
  email: string;
  postalCode: string;
  phoneNumber: string;
  address: string;
  sizeId: number;
  typeId: number;
  fixedIncomeFundTypeId: number | null;
  issuanceRequestConfirmationTypeId: number;
  cancelRequestConfirmationTypeId: number;
  alternativeTypeName: string;
  seoRegistrationNumber: string;
  registrationNumber: string;
  registrationDate: Date;
  registrationPlace: string;
  profitDividePeriodId: number;
  dividendProfitDay: number;
  minDayForProfit: number;
  establishmentLicenseDate: Date;
  portfolioManagementLicenseDate: Date;
  activityStartDate: Date;
  activityEndDate: Date;
  ipoStartDate: Date;
  ipoEndDate: Date;
  auditorStratDate: Date;
  auditorEndDate: Date;
  maxEstablishmentCost: number;
  maxMembershipCost: number;
  maxMeetingsCost: number;
  nominalValue: number;
  maxFundCapital: number;
  maxFoundersUnits: number;
  minInvestorsUnits: number;
  maxInvestorsUnits: number;
  minRequestUnits: number;
  superUnitsCount: number;
  ordinaryUnitsCount: number;
  minUnitsCount: number;
  settlementFeeCoefficient: number;
  settlementFee: number;
  settlementFeeFromDate: Date;
  settlementFeeToDate: Date;
  applySettlementTax: boolean;
  considerMaxForSettleWage: boolean;
  maxNavForSettlement: number;
  maxSettlementAmount: number;
  maxSettlementExtraAmount: number;
  marketMakerMaxTradesCount: number;
  investingInBankThreshold: number;
  fixedIssuanceFee: number;
  fixedCancellationFee: number;
  maxStockPriceAdjustment: number;
  maxBoundPriceAdjustment: number;
  boundInterestRate: number;
  bbsUploadLimit: number;
  decription: number;
  issuanceFeeVariableCoefficient: number;
  maxIssuanceFeeVariable: number;
  validateVouchersDaily: boolean;
  calculateStockValueChanges: boolean;
  efficiencyOrForecastRate: number;
  notification12020162EffectiveDate: number;
  upperThresholdRequestTime: string;
  vatFee: number;
  tollFee: number;
  fundOperatingFixedCostDetailAccountCode: string;
  positiveIndexAdjustmentCoefficient: number;
  negativeIndexAdjustmentCoefficient: number;
  pillars: UpsertPillarRequest[];
  cancellationPenalties: UpsertCancellationPenalty[];
}

export interface UpsertPillarRequest {
  roleId: number;
  personId: number;
  fixedAnnual: number;
  performanceBasedFee: number;
  minPerformanceBasedRate: number;
  stockAndRightFee: number;
  bankDepositInterestFee: number;
  netAssetValueFee: number;
  fixedIncomeBondsFee: number;
  fixedIncomeDepositCertificateFee: number;
  bondUnderwritingObligationFee: number;
  stockFundFee: number;
  fixedFundFee: number;
  mixedFundFee: number;
  tradesValueFee: number;
  allocatedProfitFee: number;
  applyTax: boolean;
  maxPillarsUnits: number;
  minPillarsUnits: number;
  maxCommission: number | null;
  minCommission: number;
  assetsOtherThanStocksFee: number;
}

export interface UpsertCancellationPenalty {
  fromDay: number;
  toDay: number;
  percent: number;
}

//#endregion

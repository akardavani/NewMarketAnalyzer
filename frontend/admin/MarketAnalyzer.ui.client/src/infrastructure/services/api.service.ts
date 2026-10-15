import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { RemoteDataSource } from '@components/autocomplete/autocomplete.component';
import { AccountEntryLevel, ApiResult, BatchRequestsFileType, IssuanceRequestCreditOrigin, LoginType, PaymentMethod, PaymentRequestBankAccount, PersonType, PlainObject, RegionType, SelectOption } from '@typings';
import { apiUrl, buildUrl, saveFile } from '@utils';
import { map, Observable, of, shareReplay } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ApiService {

  constructor(private readonly _client: HttpClient) {

  }

  get manualvoucherTypes(): Observable<ReadonlyArray<SelectOption>> {
    return this._client
      .get<ApiResult<ReadonlyArray<SelectOption>>>(apiUrl('Voucher/GetVoucherTypeSelectOptions'))
      .pipe(map(this.mapSelectOptions));
  }

  get rightStates(): Observable<ReadonlyArray<SelectOption>> {
    return this._client
      .get<ApiResult<ReadonlyArray<SelectOption>>>(apiUrl('Portfolio/RightInstrumentStateOptions'))
      .pipe(map(this.mapSelectOptions));
  }

  get accountLevels(): ReadonlyArray<SelectOption> {
    return [
      { value: AccountEntryLevel.Group, text: 'گروه' },
      { value: AccountEntryLevel.Ledger, text: 'کل' },
      { value: AccountEntryLevel.Subsidiary, text: 'معین' },
      { value: AccountEntryLevel.Detail, text: 'تفصیلی' }
    ];
  }

  get accountTypes(): ReadonlyArray<SelectOption> {
    return [
      { value: 0, text: 'نامعین' },
      { value: 1, text: 'دائمی' },
      { value: 2, text: 'موقت' },
      { value: 3, text: 'حسا‌ب‌های انتظامی' }
    ];
  }

  get accountNatures(): ReadonlyArray<SelectOption> {
    return [
      { value: 0, text: 'نامعین' },
      { value: 1, text: 'بدهکار' },
      { value: 2, text: 'بستانکار' },
      { value: 3, text: 'خنثی' },
      { value: 4, text: 'بدهکار/بستانکار' }
    ];
  }

  get voucherTypes(): Observable<ReadonlyArray<SelectOption>> {
    return this._client
      .get<ApiResult<ReadonlyArray<SelectOption>>>(apiUrl('Voucher/GetVoucherTypeSelectOptions'))
      .pipe(map(this.mapSelectOptions));
  }

  get voucherStates(): Observable<ReadonlyArray<SelectOption>> {
    return this._client
      .get<ApiResult<ReadonlyArray<SelectOption>>>(apiUrl('Voucher/GetVoucherStateSelectOptions'))
      .pipe(map(this.mapSelectOptions));
  }

  get creationTypes(): Observable<ReadonlyArray<SelectOption>> {
    return this._client
      .get<ApiResult<ReadonlyArray<SelectOption>>>(apiUrl('Voucher/GetVoucherCreationTypeSelectOptions'))
      .pipe(map(this.mapSelectOptions));
  }

  get loginTypes(): ReadonlyArray<SelectOption> {
    return [
      { value: LoginType.StaticPassword, text: 'رمز ایستا' },
      { value: LoginType.OneTimePassword, text: 'رمز موقت' },
    ];
  }

  get applications(): Observable<ReadonlyArray<SelectOption>> {
    return this._client
      .get<ApiResult<ReadonlyArray<SelectOption>>>(apiUrl('Application/QueryOptions'))
      .pipe(map(this.mapSelectOptions), shareReplay(1));
  }

  get roles(): Observable<ReadonlyArray<SelectOption>> {
    return this._client
      .get<ApiResult<ReadonlyArray<SelectOption>>>(apiUrl('Role/QueryOptions'))
      .pipe(map(this.mapSelectOptions), shareReplay(1));
  }

  get loginResults(): Observable<ReadonlyArray<SelectOption>> {
    return this._client
      .get<ApiResult<ReadonlyArray<SelectOption>>>(apiUrl('Application/LoginResults'))
      .pipe(map(this.mapSelectOptions), shareReplay(1));
  }

  get banks(): Observable<ReadonlyArray<SelectOption>> {
    return this._client
      .get<ApiResult<ReadonlyArray<SelectOption>>>(apiUrl('Bank/QueryOptions'))
      .pipe(map(this.mapSelectOptions), shareReplay(1));
  }

  get fixedCostTypes(): Observable<ReadonlyArray<SelectOption>> {
    return this._client
      .get<ApiResult<ReadonlyArray<SelectOption>>>(apiUrl('FixedCostType/QueryOptions'))
      .pipe(map(this.mapSelectOptions), shareReplay(1));
  }

  get bankDeposits(): Observable<ReadonlyArray<SelectOption>> {
    return this._client
      .get<ApiResult<ReadonlyArray<SelectOption>>>(apiUrl('CompanyBankAccount/HasDepositInterestQueryOptions'))
      .pipe(map(this.mapSelectOptions), shareReplay(1));
  }

  get bankAccountTypes(): Observable<ReadonlyArray<SelectOption>> {
    return this._client
      .get<ApiResult<ReadonlyArray<SelectOption>>>(apiUrl('BankAccountType/QueryOptions'))
      .pipe(map(this.mapSelectOptions), shareReplay(1));
  }

  get interestPeriods(): Observable<ReadonlyArray<SelectOption>> {
    return this._client
      .get<ApiResult<ReadonlyArray<SelectOption>>>(apiUrl('CompanyBankAccount/InterestPeriodsQueryOptions'))
      .pipe(map(this.mapSelectOptions), shareReplay(1));
  }

  get maritalStatuses(): Observable<ReadonlyArray<SelectOption>> {
    return this._client
      .get<ApiResult<ReadonlyArray<SelectOption>>>(apiUrl('MaritalStatus/QueryOptions'))
      .pipe(map(this.mapSelectOptions), shareReplay(1));
  }

  get militaryStatuses(): Observable<ReadonlyArray<SelectOption>> {
    return this._client
      .get<ApiResult<ReadonlyArray<SelectOption>>>(apiUrl('MilitaryStatus/QueryOptions'))
      .pipe(map(this.mapSelectOptions), shareReplay(1));
  }

  get genders(): Observable<ReadonlyArray<SelectOption>> {
    return this._client
      .get<ApiResult<ReadonlyArray<SelectOption>>>(apiUrl('Gender/QueryOptions'))
      .pipe(map(this.mapSelectOptions), shareReplay(1));
  }

  get companyTypes(): Observable<ReadonlyArray<SelectOption>> {
    return this._client
      .get<ApiResult<ReadonlyArray<SelectOption>>>(apiUrl('LegalPersonType/QueryOptions'))
      .pipe(map(this.mapSelectOptions), shareReplay(1));
  }

  get addressTypes(): Observable<ReadonlyArray<SelectOption>> {
    return this._client
      .get<ApiResult<ReadonlyArray<SelectOption>>>(apiUrl('AddressType/QueryOptions'))
      .pipe(map(this.mapSelectOptions), shareReplay(1));
  }

  get contactTypes(): Observable<ReadonlyArray<SelectOption>> {
    return this._client
      .get<ApiResult<ReadonlyArray<SelectOption>>>(apiUrl('ContactType/QueryOptions'))
      .pipe(map(this.mapSelectOptions), shareReplay(1));
  }

  get fundSizes(): Observable<ReadonlyArray<SelectOption>> {
    return this._client
      .get<ApiResult<ReadonlyArray<SelectOption>>>(apiUrl('FundSize/QueryOptions'))
      .pipe(map(this.mapSelectOptions), shareReplay(1));
  }

  get fundTypes(): Observable<ReadonlyArray<SelectOption>> {
    return this._client
      .get<ApiResult<ReadonlyArray<SelectOption>>>(apiUrl('FundType/QueryOptions'))
      .pipe(map(this.mapSelectOptions), shareReplay(1));
  }

  get requestConfirmationTypes(): Observable<ReadonlyArray<SelectOption>> {
    return this._client
      .get<ApiResult<ReadonlyArray<SelectOption>>>(apiUrl('RequestConfirmationType/QueryOptions'))
      .pipe(map(this.mapSelectOptions), shareReplay(1));
  }

  get requestTypes(): Observable<ReadonlyArray<SelectOption>> {
    return this._client
      .get<ApiResult<ReadonlyArray<SelectOption>>>(apiUrl('RequestType/QueryOptions'))
      .pipe(map(this.mapSelectOptions), shareReplay(1));
  }

  get collateralOrigins(): Observable<ReadonlyArray<SelectOption>> {
    return this._client
      .get<ApiResult<ReadonlyArray<SelectOption>>>(apiUrl('CollateralRequest/CollateralOriginsQueryOptions'))
      .pipe(map(this.mapSelectOptions), shareReplay(1));
  }

  get collateralRequestTypes(): Observable<ReadonlyArray<SelectOption>> {
    return this._client
      .get<ApiResult<ReadonlyArray<SelectOption>>>(apiUrl('CollateralRequest/CollateralRequestTypesQueryOptions'))
      .pipe(map(this.mapSelectOptions), shareReplay(1));
  }

  get profitDividePeriods(): Observable<ReadonlyArray<SelectOption>> {
    return this._client
      .get<ApiResult<ReadonlyArray<SelectOption>>>(apiUrl('ProfitDividePeriod/QueryOptions'))
      .pipe(map(this.mapSelectOptions), shareReplay(1));
  }

  get pillarRoles(): Observable<ReadonlyArray<SelectOption>> {
    return this._client
      .get<ApiResult<ReadonlyArray<SelectOption>>>(apiUrl('PillarRole/QueryOptions'))
      .pipe(map(this.mapSelectOptions), shareReplay(1));
  }

  get rollbackNetAssetValueStates(): Observable<ReadonlyArray<SelectOption>> {
    return of([
      { value: 1, text: 'در انتظار' },
      { value: 2, text: 'رد شده' },
      { value: 3, text: 'تایید شده' }
    ]);
  }

  get personTypes(): ReadonlyArray<SelectOption> {
    return [
      { value: PersonType.Natural, text: 'حقیقی' },
      { value: PersonType.ForeignNatural, text: 'حقیقی خارجی' },
      { value: PersonType.Legal, text: 'حقوقی' },
      { value: PersonType.ForeignLegal, text: 'حقوقی خارجی' },
      { value: PersonType.Basket, text: 'سبد' }
    ];
  }

  get certificateTypes(): ReadonlyArray<SelectOption> {
    return [
      { value: 1, text: 'اول' },
      { value: 2, text: 'دوم' },
    ];
  }

  get bondTypes(): ReadonlyArray<SelectOption> {
    return [
      { value: 1, text: 'گواهی سپرده عام' },
      { value: 2, text: 'اوراق مشارکت' },
    ];
  }

  get calculationMethodTypes(): ReadonlyArray<SelectOption> {
    return [
      { value: 1, text: 'ساده' },
      { value: 2, text: 'صعودی' },
    ];
  }

  get periodReportTypes(): ReadonlyArray<SelectOption> {
    return [
      { value: 3, text: 'سه ماهه' },
      { value: 4, text: 'شش ماهه' },
      { value: 5, text: 'نه ماهه' },
      { value: 6, text: 'یک ساله' },
    ];
  }

  get companyBankAccountConfirmationStates(): Observable<ReadonlyArray<SelectOption>> {
    return of([
      { value: 1, text: 'در انتظار' },
      { value: 2, text: 'رد' },
      { value: 3, text: 'تایید' }
    ]);
  }

  get assetUnitTypes(): Observable<ReadonlyArray<SelectOption>> {
    return this._client
      .get<ApiResult<ReadonlyArray<SelectOption>>>(apiUrl('AssetUnitType/QueryOptions'))
      .pipe(map(this.mapSelectOptions))
  }

  get investorPaymentReasons(): Observable<ReadonlyArray<SelectOption>> {
    return this._client
      .get<ApiResult<ReadonlyArray<SelectOption>>>(apiUrl('PaymentReason/PaymentToInvestorQueryOptions'))
      .pipe(map(this.mapSelectOptions))
  }

  get paymentReasonTypes(): Observable<ReadonlyArray<SelectOption>> {
    return this._client
      .get<ApiResult<ReadonlyArray<SelectOption>>>(apiUrl('PaymentReason/PaymentReasonTypesQueryOptions'))
      .pipe(map(this.mapSelectOptions))
  }

  get paymentTypes(): Observable<ReadonlyArray<SelectOption>> {
    return this._client
      .get<ApiResult<ReadonlyArray<SelectOption>>>(apiUrl('PaymentOrder/PaymentTypesQueryOptions'))
      .pipe(map(this.mapSelectOptions))
  }

  get paymentReasonCategories(): Observable<ReadonlyArray<SelectOption>> {
    return this._client
      .get<ApiResult<ReadonlyArray<SelectOption>>>(apiUrl('PaymentReason/CategoryQueryOptions'))
      .pipe(map(this.mapSelectOptions))
  }

  get paymentRequestBankAccounts(): Observable<ReadonlyArray<PaymentRequestBankAccount>> {
    return this._client
      .get<ApiResult<PaymentRequestBankAccount[]>>(apiUrl('PaymentRequest/GetBankAccounts'))
      .pipe(map(response => response.successed ? response.data : []));
  }

  get batchRequestsFileTypes(): ReadonlyArray<SelectOption> {
    return [
      { value: BatchRequestsFileType.InternalExcel, text: 'فایل اکسل سامانه' },
      { value: BatchRequestsFileType.ExternalExcel, text: 'فایل اکسل سایر صندوق‌ها' },
    ];
  }

  get requestStates(): Observable<ReadonlyArray<SelectOption>> {
    return this._client
      .get<ApiResult<ReadonlyArray<SelectOption>>>(apiUrl('Request/RequestStatesQueryOptions'))
      .pipe(map(this.mapSelectOptions));
  }

  get confirmableRequestStates(): Observable<ReadonlyArray<SelectOption>> {
    return this._client
      .get<ApiResult<ReadonlyArray<SelectOption>>>(buildUrl(apiUrl('Request/RequestStatesQueryOptions'), { onlyConfirmable: true }))
      .pipe(map(this.mapSelectOptions), shareReplay(1));
  }

  get issuanceRequestCreditOrigins(): ReadonlyArray<SelectOption> {
    return [
      { value: IssuanceRequestCreditOrigin.Bank, text: 'بانک' },
      { value: IssuanceRequestCreditOrigin.Account, text: 'حساب' },
    ];
  }

  get issuancePaymentMethods(): ReadonlyArray<SelectOption> {
    return [
      { value: PaymentMethod.OnlineDeposit, text: 'واریز اینترنتی' },
      { value: PaymentMethod.DepositSlip, text: 'فیش واریزی' },
      { value: PaymentMethod.OnlineGateway, text: 'درگاه پرداخت آنلاین' },
      { value: PaymentMethod.FundAccount, text: 'حساب صندوق' },
    ];
  }

  get paymentMethods(): ReadonlyArray<SelectOption> {
    return [
      { value: PaymentMethod.OnlineDeposit, text: 'واریز اینترنتی' },
      { value: PaymentMethod.DepositSlip, text: 'فیش واریزی' },
      { value: PaymentMethod.OnlineGateway, text: 'درگاه پرداخت آنلاین' },
      { value: PaymentMethod.Unknown, text: 'نامعین' },
      { value: PaymentMethod.FundAccount, text: 'حساب صندوق' },
    ];
  }

  paymentOrderStates(onlyTrusteeConfirmation: boolean): Observable<ReadonlyArray<SelectOption>> {
    const url = buildUrl(apiUrl('PaymentOrder/StateQueryOptions'), {
      onlyTrusteeConfirmation: onlyTrusteeConfirmation
    });
    return this._client
      .get<ApiResult<ReadonlyArray<SelectOption>>>(url)
      .pipe(map(this.mapSelectOptions));
  }

  companyBankAccounts(valueMember: 'value'): Observable<ReadonlyArray<SelectOption<number>>>;
  companyBankAccounts(valueMember: 'detailAccountCode'): Observable<ReadonlyArray<SelectOption<string>>>;
  companyBankAccounts(valueMember: 'value' | 'detailAccountCode'): Observable<ReadonlyArray<SelectOption<string | number>>> {
    return this._client
      .get<ApiResult<CompanyBankAccountSelectOption[]>>(apiUrl('CompanyBankAccount/QueryOptions'))
      .pipe(map(response => {
        if (response.successed) {
          return valueMember == 'value' ?
            response.data :
            response.data.map(item => ({
              value: item.detailAccountCode,
              text: item.text
            }))
        }

        return []
      }));
  }

  buildBankBranchsSource(bankId?: number): RemoteDataSource<number> {
    return {
      query: 'keyword',
      url: apiUrl('Bank/SearchBranchs'),
      params: {
        rows: 50,
        bankId: bankId
      }
    };
  }

  buildAccountsSource(dataProperty: 'accountId', level?: AccountEntryLevel, accountId?: number): RemoteDataSource<number>;
  buildAccountsSource(dataProperty: 'accountCode', level?: AccountEntryLevel, accountCode?: string): RemoteDataSource<string>;
  buildAccountsSource(dataProperty: DataProperty, level?: AccountEntryLevel, accountIdOrCode?: number | string): RemoteDataSource<number | string> {
    return {
      query: 'keyword',
      url: apiUrl('Accounting/Search'),
      transform: (response: ApiResult<AccountInfo[]>) => {
        if (response.successed) {
          return response.data.map(item => ({
            value: item[dataProperty],
            text: `${item.accountTitle} (${item.accountCode})`,
            src: item
          }));
        }
        else {
          return [];
        }
      },
      params: {
        rows: 50,
        level: level,
        accountId: typeof (accountIdOrCode) === 'number' ? accountIdOrCode : null,
        accountCode: typeof (accountIdOrCode) === 'string' ? accountIdOrCode : null,
      }
    };
  }

  buildInvestorsSource(personTypes?: PersonType[]): RemoteDataSource<number> {
    return {
      query: 'keyword',
      url: apiUrl('Person/Search'),
      transform: (response: ApiResult<ReadonlyArray<SelectOption>>) => {
        return response.successed ? response.data : [];
      },
      params: (!personTypes || !personTypes.length) ? undefined : { personTypes: personTypes }
    };
  };

  buildRegionSource(regionType?: RegionType, parentId?: number): RemoteDataSource<number> {
    return {
      query: 'keyword',
      url: apiUrl('Region/Search'),
      transform: (response: ApiResult<ReadonlyArray<SelectOption>>) => {
        return response.successed ? response.data : [];
      },
      params: {
        rows: 50,
        parentId: parentId,
        regionType: regionType,
      }
    };
  }


  downloadFile(path: string, data?: PlainObject<any>): void {
    this._client
      .get<any>(apiUrl(path), { observe: 'response', responseType: 'arraybuffer' as any, params: data })
      .subscribe(saveFile);
  }

  private mapSelectOptions(response: ApiResult<ReadonlyArray<SelectOption>>): ReadonlyArray<SelectOption> {
    return response.successed ? response.data : [];
  }

}

export type DataProperty = 'accountId' | 'accountCode';

interface AccountInfo {
  accountId: number;
  accountTitle: string;
  accountCode: string;
}

interface CompanyBankAccountSelectOption extends SelectOption {
  detailAccountCode: string;
}

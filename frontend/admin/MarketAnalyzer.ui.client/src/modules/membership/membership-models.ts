export interface UnknownLoginHistory {
  userName: string;
  loginType: string;
  ipAddress: string;
  userAgent: string;
  application: string;
  eventDateTime: Date;
}

export interface UnknownLoginHistoryFilter {

}

export interface UsersLoginHistory {
  id: number;
  userId: number;
  userName: string;
  fullName: string;
  loginType: string;
  ipAddress: string;
  userAgent: string;
  loginResult: string;
  application: string;
  eventDateTime: Date;
}

export interface UsersLoginHistoryFilter {

}

export interface Users {
  id: number;
  fullName: string;
  userName: string;
  phoneNumber: string;
  email: string;
  twoFactorAuthentication: boolean;
  isActive: boolean;
}

export interface UsersFilter {

}

export interface Roles {
  id: number;
  name: string;
  displayName: string;
  isActive: boolean;
  editable: boolean;
  usersCount: number;
}

export interface RolesFilter {

}

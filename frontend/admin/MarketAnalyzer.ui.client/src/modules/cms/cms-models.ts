export interface StatuteRequest {
  id: number;
  description: string;
  attachmentId: string;
  date: Date;
}
export interface StatuteRequestFilter {}

export interface ProspectusRequest {
  id: number;
  description: string;
  attachmentId: string;
  date: Date;
}
export interface ProspectusRequestFilter {}

export interface InvestmentPolicyStatementRequest {
  id: number;
  description: string;
  attachmentId: string;
  isActive: boolean;
  date: Date;
}
export interface InvestmentPolicyStatementRequestFilter {}

import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ApiResult } from '@typings';
import { apiUrl } from '@utils';
import { Observable } from 'rxjs';

export interface LearningCollection {
  id: number;
  title: string;
  description?: string | null;
  status: number;
  itemCount: number;
}

export interface LearningFile { fileName: string; content: string; }
export interface PreviewItem { externalNumber?: string | null; title: string; originalContent: string; contentHash: string; }
export interface ImportPreview { items: PreviewItem[]; files: { fileName: string; discoveredItems: number; newItems: number; updatedItems: number; duplicateItems: number; warnings: string[] }[]; }
export interface LearningItem { id: number; sectionId?: number | null; externalNumber?: string | null; title: string; displayContent?: string | null; displayOrder: number; isStudied: boolean; }
export interface CollectionContent { collection: LearningCollection; sections: { id: number; title: string; displayOrder: number }[]; items: LearningItem[]; }
export interface LearningPlan { id: number; collectionId: number; mode: number; status: number; batchCount: number; createdAtUtc: string; }
export interface LearnOverview { collectionCount: number; activePlanCount: number; studiedItemCount: number; dueReviewCount: number; overdueReviewCount: number; }
export interface DueLearningReview { collectionId: number; collectionTitle: string; learningItemId: number; title: string; externalNumber?: string | null; nextReviewAtUtc: string; }

@Injectable({ providedIn: 'root' })
export class LearnApiService {
  private readonly http = inject(HttpClient);

  collections(): Observable<ApiResult<LearningCollection[]>> {
    return this.http.get<ApiResult<LearningCollection[]>>(apiUrl('Learn/Collections'));
  }
  createCollection(title: string, description: string): Observable<ApiResult<number>> {
    return this.http.post<ApiResult<number>>(apiUrl('Learn/CreateCollection'), { title, description });
  }
  archiveCollection(collectionId: number): Observable<ApiResult<number>> {
    return this.http.delete<ApiResult<number>>(apiUrl('Learn/ArchiveCollection'), { params: { collectionId } });
  }
  content(collectionId: number): Observable<ApiResult<CollectionContent>> {
    return this.http.get<ApiResult<CollectionContent>>(apiUrl('Learn/CollectionContent'), { params: { collectionId } });
  }
  preview(collectionId: number, files: LearningFile[]): Observable<ApiResult<ImportPreview>> {
    return this.http.post<ApiResult<ImportPreview>>(apiUrl('Learn/PreviewImport'), { collectionId, files });
  }
  import(collectionId: number, files: LearningFile[]): Observable<ApiResult<unknown>> {
    return this.http.post<ApiResult<unknown>>(apiUrl('Learn/Import'), { collectionId, files, confirmImport: true });
  }
  createPlan(collectionId: number, itemsPerBatch: number): Observable<ApiResult<number>> {
    return this.http.post<ApiResult<number>>(apiUrl('Learn/CreateStudyPlan'), {
      collectionId, mode: 2, itemsPerBatch, targetDaysPerBatch: null, dailyTimeBudgetMinutes: null
    });
  }
  plans(collectionId: number): Observable<ApiResult<LearningPlan[]>> {
    return this.http.get<ApiResult<LearningPlan[]>>(apiUrl('Learn/StudyPlans'), { params: { collectionId } });
  }
  setPlanStatus(studyPlanId: number, status: number): Observable<ApiResult<number>> {
    return this.http.put<ApiResult<number>>(apiUrl('Learn/SetStudyPlanStatus'), null, { params: { studyPlanId, status } });
  }
  addBatch(studyPlanId: number, scheduledDate: string, learningItemIds: number[]): Observable<ApiResult<number>> {
    return this.http.post<ApiResult<number>>(apiUrl('Learn/AddStudyBatch'), { studyPlanId, scheduledDate, learningItemIds });
  }
  overview(): Observable<ApiResult<LearnOverview>> {
    return this.http.get<ApiResult<LearnOverview>>(apiUrl('Learn/Overview'));
  }
  dueReviews(): Observable<ApiResult<DueLearningReview[]>> {
    return this.http.get<ApiResult<DueLearningReview[]>>(apiUrl('Learn/DueReviews'), { params: { take: 50 } });
  }
  startSession(collectionId: number): Observable<ApiResult<number>> {
    return this.http.post<ApiResult<number>>(apiUrl('Learn/StartSession'), { collectionId, source: 1 });
  }
  recordProgress(sessionId: number, learningItemId: number): Observable<ApiResult<number>> {
    return this.http.put<ApiResult<number>>(apiUrl('Learn/RecordProgress'), { sessionId, learningItemId, status: 3 });
  }
  review(collectionId: number, learningItemId: number, rating: number): Observable<ApiResult<unknown>> {
    return this.http.post<ApiResult<unknown>>(apiUrl('Learn/Review'), { collectionId, learningItemId, rating, reviewedAtUtc: new Date().toISOString() });
  }
  export(collectionId: number, learningItemIds: number[]): Observable<Blob> {
    return this.http.post(apiUrl('Learn/Export'), { collectionId, learningItemIds }, { responseType: 'blob' });
  }
}

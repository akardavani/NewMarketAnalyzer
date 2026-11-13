namespace MarketAnalyzer.Learn.Domain;

public enum LearningCollectionStatus { Active = 1, Archived = 2 }
public enum LearningItemType { Question = 1, Lesson = 2, Article = 3 }
public enum StudyPlanMode { Sequential = 1, ItemsPerBatch = 2, TargetDaysPerBatch = 3, TimeBudget = 4, Manual = 5 }
public enum StudyPlanStatus { Active = 1, Paused = 2, Completed = 3, Archived = 4 }
public enum StudyBatchStatus { Pending = 1, InProgress = 2, Completed = 3, Skipped = 4 }
public enum StudySessionSource { Web = 1, Offline = 2, Manual = 3 }
public enum StudyItemStatus { NotStarted = 1, InProgress = 2, Studied = 3 }
public enum ReviewRating { Again = 1, Hard = 2, Good = 3, Easy = 4 }
public enum LearningContentFormat { Html = 1, Markdown = 2, PlainText = 3 }

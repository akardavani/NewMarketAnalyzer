using System;
using System.Collections.Generic;

namespace MarketAnalyzer.ApiClient.Models
{
  public class NavHistory
  {
    public required IReadOnlyList<Item> Items { get; init; }
    public required decimal FundReturn { get; init; }

    public class Item
    {
      /// <summary>
      /// تاریخ
      /// </summary>
      public required DateOnly Date { get; init; }
      /// <summary>
      /// صدور NAV
      /// </summary>
      public required decimal Issuance { get; init; }
      /// <summary>
      /// ابطال NAV
      /// </summary>
      public required decimal Cancellation { get; init; }
      /// <summary>
      /// آماری NAV
      /// </summary>
      public required decimal Statistical { get; init; }
    }
  }
}

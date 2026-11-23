# Codal provider POC — batch 02

**State: not started.** The task brief requires batch 02 only after batch 01 passes. No second batch is designated until then. Do not reuse batch 01 symbols.

After batch 01 acceptance:

1. Re-read the current `MarketAnalyzer` catalog with a read-only query.
2. Use a new recorded seed and deterministic selection; exclude all batch 01 SymbolIds.
3. Prefer different sector/subsector codes while requiring active, listed company shares.
4. Record the database snapshot date, algorithm, seed, SymbolIds, SymbolKeys, InsCodes, Mnemonics, and issuer/report identity verified against Codal.
5. Repeat the exact same provider/report/period matrix and accuracy checks as batch 01.

Do not mark this POC complete or choose a production provider until measurements are recorded.


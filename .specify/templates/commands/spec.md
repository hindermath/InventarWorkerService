# Command Template: `/speckit.specify`

Use this command to create or update a feature specification.

## Required Actions

1. Create a branch for the feature (never work directly on `main`).
2. Fill `spec.md` with prioritized, independently testable user stories.
3. Define measurable outcomes and explicit edge cases.
4. Fill the Constitution Alignment section with concrete impacts:
   - .NET 10 + C# 14.0 toolchain impact
   - NuGet dependency currency impact
   - coverage thresholds (`>=70%`, target `>=80%`)
   - layering/shared logic placement
   - bilingual documentation impact (German first, English second, CEFR B2)
   - A11Y evidence and language-tagged code-block impact
   - iSAQB/arc42 architecture evidence impact under `docs/architecture/`
   - XML documentation and DocFX impact
   - Red-Green-Refactor test impact
   - secure-development standards, MSL applicability, CRA applicability, and
     supply-chain evidence impact
   - data contract implications

## Validation Checklist

- Each story can be tested independently.
- Requirements are implementation-agnostic.
- Constitution alignment items are complete and non-empty.
- Toolchain/dependency/coverage/security/architecture/A11Y constraints are
  explicit and measurable.

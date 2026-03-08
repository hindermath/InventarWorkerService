# Command Template: `/speckit.plan`

Use this command to produce an implementation plan from an approved specification.

## Required Actions

1. Populate technical context with real stack details.
2. Execute the Constitution Check gates explicitly:
   - branching and PR flow
   - architecture/layer boundaries
   - bilingual CEFR B2 documentation scope
   - XML documentation + DocFX regeneration scope
   - Red-Green-Refactor testing scope
   - serialization/data conventions
3. Document concrete project structure for this feature.
4. Record justified exceptions in Complexity Tracking.

## Validation Checklist

- No gate is left unresolved without rationale.
- Test and documentation impacts are planned before implementation.

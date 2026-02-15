# User Context & Permissions Checklist: Create Taskify MVP

**Purpose**: Validate requirements quality for user identity model, current user context, task assignment permissions, and comment ownership rules
**Created**: 2026-02-15
**Feature**: [spec.md](../spec.md)
**Depth**: Lightweight Pre-Review (focusing on major gaps and critical ambiguities)

## User Identity & Context Establishment

- [x] CHK001 - Are requirements defined for how user identity persists across browser refresh/reload? [Gap] → **RESOLVED**: Added FR-017 specifying sessionStorage persistence
- [x] CHK002 - Is the mechanism for establishing "current user" context explicitly specified? [Clarity, Spec §FR-000a] → **RESOLVED**: Expanded User entity description with UserContextService details
- [x] CHK003 - Are requirements defined for displaying current user identity in the UI (beyond task color highlighting)? [Completeness, Spec §FR-000e] → **RESOLVED**: Added FR-000g requiring name and role display in header

## User Context Switching

- [x] CHK004 - Are requirements defined for handling unsaved edits (task description, comments) when user switches identity? [Gap, Edge Case] → **RESOLVED**: Added FR-017a with confirmation dialog for unsaved changes
- [x] CHK005 - Is the atomicity of user switching specified (immediate vs. confirmation required)? [Clarity, Spec §FR-000f] → **RESOLVED**: Clarified FR-000f as "immediate and synchronous unless unsaved edits exist"
- [x] CHK006 - Are requirements consistent between initial user selection (Spec §FR-000) and in-session switching (Spec §FR-000e)? [Consistency] → **RESOLVED**: Verified consistency - both use same 5 predefined users, same context mechanism

## Task Assignment Permissions

- [x] CHK007 - Are permission requirements defined for who can reassign tasks (any user, or only current assignee)? [Gap, Spec §FR-014c] → **RESOLVED**: Added FR-014c-1 stating any user can reassign any task
- [x] CHK008 - Are requirements defined for handling tasks with no assignee (creation default, permissions, visibility)? [Gap, Edge Case] → **RESOLVED**: Added FR-006a specifying optional assignee with "Unassigned" label
- [x] CHK009 - Are permission differences between Product Manager and Engineer roles explicitly stated or intentionally excluded? [Clarity, Assumptions] → **RESOLVED**: Already specified in Assumptions - all users have same capabilities

## Comment Authorship & Permissions

- [x] CHK010 - Is the mechanism for tracking comment authorship (linking comments to users) explicitly specified? [Completeness, Entity: Comment] → **RESOLVED**: Expanded Comment entity with AuthorId foreign key details
- [x] CHK011 - Are requirements defined for comment authorship persistence when user identity is switched? [Gap, Edge Case] → **RESOLVED**: Added Edge Case clarifying AuthorId persists, only permissions update
- [x] CHK012 - Are permission check requirements specified (client-side only, timing, enforcement points)? [Gap, Spec §FR-011b-d] → **RESOLVED**: Added FR-011d-1 (client-side) and FR-011d-2 (server-side) permission checks

## Visual Differentiation & Permission Indicators

- [x] CHK013 - Are the specific colors/tints for "my tasks" differentiation defined, or is "distinct accent color" measurable? [Measurability, Spec §FR-000c] → **RESOLVED**: Created UI Specifications section with hex colors, contrast ratios, WCAG compliance
- [x] CHK014 - Are requirements defined for how edit/delete controls are visually presented on comments? [Completeness, Spec §FR-011d] → **RESOLVED**: Expanded FR-011d with icon types, sizes, colors, positioning details

## Edge Cases & Boundary Conditions

- [x] CHK015 - Are requirements defined for multi-tab/multi-window scenarios with same user identity? [Gap, Coverage] → **RESOLVED**: Added Edge Case specifying independent connections with SignalR synchronization

## Notes

- Items marked with [Gap] indicate missing requirements that should be added to spec
- Items marked with [Clarity] indicate ambiguous requirements that need quantification
- Items marked with [Consistency] indicate potential conflicts between requirements
- Items marked with [Completeness] indicate incomplete requirement coverage
- Check items off `[x]` as requirements are validated/updated
- Add findings or spec section updates inline as comments

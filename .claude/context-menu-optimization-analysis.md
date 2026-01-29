# Context Menu Optimization Analysis

## Problem
The `PopulatePropertyMenu` and `PopulateNavigationPropertyMenu` methods rebuild the entire menu on every right-click, creating unnecessary memory pressure.

## What's Actually Dynamic

### Property Menu (`PopulatePropertyMenu`)
Only **two conditional differences**:

1. **Entity Key toggle** (scalar properties only):
   - Item only added for scalar properties
   - `IsChecked = scalarProperty.EntityKey` - the **only truly dynamic value**

2. **"Move to New Complex Type"** (scalar properties only):
   - Only added for scalar properties (structural, not runtime)

Everything else (Cut, Copy, Paste, Rename, Delete, Move operations, Show commands) is **completely static**.

### Navigation Property Menu (`PopulateNavigationPropertyMenu`)
- **Nothing is dynamic** - completely static menu structure

### Association Menu (`PopulateAssociationMenu`)
- **Legitimately dynamic** - entity/property names in labels like "Select {CustomerName}"
- Rebuild is justified here

## Memory Impact of Current Approach
Each right-click creates:
- ~15 new `MenuCommandDefinition` objects
- Each with strings, `ObservableCollection<object>` for children, event handlers
- All become garbage immediately after menu closes
- GC pressure accumulates with frequent context menu usage

## Recommended Solutions

### Option 1: Two Cached Menus (for Property Menu)
- One for scalar properties, one for complex properties
- Only update `IsChecked` on the Entity Key command before showing

### Option 2: Single Menu with Visibility Toggling
- Build once with all items
- Toggle `IsVisible` on Entity Key and "Move to New Complex Type" based on property type
- Update `IsChecked` on Entity Key before showing

### For Navigation Property Menu
- Build once on first use
- No updates needed - completely static

## Summary Table

| Menu | Truly Dynamic? | Rebuild Needed? |
|------|---------------|-----------------|
| `PopulatePropertyMenu` | Only `IsChecked` on Entity Key | No |
| `PopulateNavigationPropertyMenu` | Nothing | No |
| `PopulateAssociationMenu` | Entity/property names in labels | **Yes** |

## Files to Modify
- `src/Microsoft.Data.Entity.Design.Package/CustomCode/DiagramSurfaceContextMenuService.cs`

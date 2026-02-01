# Microsoft's Entity Framework Designer is dead.

Yet it still ships inside Visual Studio 2026, broken and out of date. Unable to work with moderm projects or SQL providers. And [not accepting new PRs](https://github.com/dotnet/ef6tools/issues/83).

So the EDMXperts at CloudNimble have taken matters into our own hands.

# Welcome to the EasyAF Entity Designer

[**EasyAF**](https://easyaf.dev) is CloudNimble's platform for warp-speed application development with .NET, powered by EDMX.

**EasyAF.EntityDesigner** is a modern evolution of Microsoft's EDMX experience that works with EF 6.5 and SDK-style projects targeting .NET Framework _AND_ .NET 8+.

We've stripped it down and removed all the legacy code, dependencies, and ugly WinForms UI that bogged it down.

It's now refreshed with a lighter codebase, modern UI enhanced by WPF, and a bevy of new features.

| Feature Supported            | Microsoft Entity Frame  | EasyAF.EntityDesigner |
| ---------------------------- | -------------------------- | --------------------- |
| .NET Framework 4.8           | ✅                         | ✅                   |
| .NET 8 / 9 / 10 / 11         | ❌                         | ✅                   |
| `Microsoft.Data.SqlProvider` | ❌                         | ✅                   |
| SDK-Style Projects           | ❌                         | ✅                   |
| Intuitive User Experience    | ❌                         | ✅                   |
| High-Res Image Export        | ❌                         | ✅                   |
| SVG Export                   | ❌                         | ✅                   |
| Mermaid Diagram Export       | ❌                         | ✅                   |

## Screenshots



# Why Now?

The Age of AI makes it easy to keep just about anything up-to-date. We recently took the time to make some minor improvements to Microsoft's still-shipping EF6Tools codebase, and were rejected.

The thing is, EDMX is still a very powerful language for describing a database schema. We use it to power data access and code generation in both EF6 and EFCore applications across multiple cloud providers.

In fact, Entity Framework 6.5 rung great on modern .NET, It's a valid compatibility path to get legacy apps onto .NET 8+, and there are thousands of EDMX-based applications still being maintained.

But Microsoft's corporate calculus is leaving developers high and dry.
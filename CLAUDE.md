# Working in this fork

This is a **fork** of MapLibre Native MAUI, on branch `rhike-patches`, consumed
by `RHike/Rhike` through `RHike/vendor-packages`.

**Read [FORK.md](FORK.md) first.** It is the only file that describes what is
different here and why.

## The rule that matters most

**`README.md` and `CHANGELOG.md` are upstream's. Do not edit them.** Every edit
there turns the next upstream sync from a merge into a conflict resolution. The
fork's own story goes in `FORK.md`.

Same for `release.yml`: the stock workflow stays untouched and
`release-rhike.yml` sits next to it.

## Versioning

`VERSION` carries a `-rhike.N` prerelease suffix so it can never collide with
an upstream release. Bump it for every change that produces a package.

**Never push to NuGet.org.** These packages are published as GitHub releases on
this fork and consumed from `vendor-packages` through `nuget.config`. A
collision with an upstream version is the one mistake that cannot be recalled,
because it is already in someone's package cache.

## Before adding a patch

Ask whether it belongs upstream. This fork is a list of debts, not a feature
set: every patch here is something to carry, sync and eventually re-apply. A
patch that upstream would take should be offered to them, and when it lands,
the entry leaves `FORK.md` with a note saying which upstream version has it.

Both current functional patches are upstream candidates. The `GeoJsonSource`
fix in particular is a plain bug, not something RHike-specific.

## Adding to FORK.md

A new patch means a new section in [FORK.md](FORK.md) with the commit, what
broke or was missing, and what would have to be true to drop it. A patch nobody
wrote down is a patch that gets silently dropped at the next sync, or worse,
carried forever because nobody dares remove it.

## Writing

English, matching upstream. Never use an em dash: plain hyphen with spaces, or
rewrite the sentence.

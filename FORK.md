# This is a fork

`README.md` and `CHANGELOG.md` in this repository are **upstream's** and are
deliberately left as they are, so syncing with upstream stays a merge rather
than a conflict resolution. This file is the only one that describes the fork.

What is here that upstream does not have, why, and what would have to be true
to drop it.

| | |
| --- | --- |
| Branch | `rhike-patches` |
| Version | `4.5.1-rhike.3`, a prerelease suffix so it can never collide with an upstream release |
| Consumed by | `RHike/Rhike` through `RHike/vendor-packages` and its `nuget.config` |
| Published to | GitHub releases on this fork only. **Never NuGet.org.** |

Five commits on top of upstream `main`. Three are build and release plumbing,
two are functional.

---

## Functional changes

### `GeoJsonSource` pushes `FeatureCollection` changes into the style

`770ccbc`, in `4.5.1-rhike.3`.

Data set after `StyleLoaded` never reached the map. The `StyleView`
materialise pass runs before app-side `StyleLoaded` handlers and bailed on a
null `FeatureCollection`, and the property-changed callback was empty. Every
layer referencing the source silently drew nothing until a style reload
happened to re-materialise the element with data.

"Silently drew nothing" is what makes this expensive: there is no error, the
map simply has no route on it.

A `FeatureCollection` change now adds or updates the source in place, in shared
handler code, on all platforms.

The same commit installs the existing `mbgl_install_log_callback` on Android,
so native style, source, glyph and sprite errors reach logcat under the tag
`MapLibre` instead of disappearing into stderr. That is how the above was found
in the first place.

### `AddSourceJson` / `AddLayerJson` on `IMapLibreMapController`

`7107f2b`.

`MbglStyle` has carried both since the 3.x generic-JSON work, and the Windows
and macOS/iOS controllers already exposed them, but they were never on the
interface and Android lacked them entirely. Added to the interface and
implemented on Android with the usual `_styleReady`/`_style` guard.

**This is the only route to style expressions** while the typed layer API has
none. RHike needs it for a `SymbolLayer` with `icon-rotate: [get, bearing]` and
`icon-rotation-alignment: map`, which is what makes direction markers rotate
with the map.

---

## Build and release plumbing

### 16 KB page-size alignment for Android natives

`a253840`, in `4.5.1-rhike.2`.

Android 15 and later can use 16 KB memory pages, and the Play Store requires
16 KB-aligned native libraries for apps targeting Android 15+. NDK 27 still
defaults to 4 KB, and .NET Android warns `XA0141` on the 4 KB-aligned
`libmln-cabi.so`.

`-Wl,-z,max-page-size=16384` is passed at CMake configure time in
`native-android.yml` and `native-android-vulkan.yml`.

**This one is a store-submission requirement, not a nicety.**

### Stripped native libraries

`f57bddb`.

The release `.so` shipped with full debug info: 106.8 MB per ABI against about
8 MB for official MapLibre Android builds. `llvm-strip --strip-unneeded` now
runs from the same NDK 27 toolchain after the build, and the unstripped copy is
uploaded as a separate `-debugsyms` artifact so native crashes stay
symbolicatable.

### Fork release workflow

`a493d05`, in `4.5.1-rhike.1`.

`release-rhike.yml` is a `workflow_dispatch` pipeline that builds only the
Android OpenGL natives, packs `MapLibreNative.Maui` and `.Handlers` with those
natives, and publishes a GitHub release on this fork with the nupkgs and the
debug-symbol zips.

**The stock `release.yml` is left untouched**, on purpose, so upstream sync does
not conflict over it. There is no NuGet.org push and there should not be one:
these packages carry a `-rhike` prerelease suffix and belong only to RHike.

---

## Keeping this fork honest

- **Do not edit `README.md` or `CHANGELOG.md`.** They are upstream's. Every
  edit there is a conflict on the next sync, and the fork's own story belongs
  in this file.
- **Bump `VERSION` with a `-rhike.N` suffix** for every change that produces a
  package. A version that could collide with an upstream release is the one
  mistake that is hard to undo, because it is already in someone's cache.
- **A patch that upstream accepts should leave this file**, with a note saying
  which upstream version it landed in. This document is a list of debts, not a
  feature set.
- Both functional patches are candidates for upstream. The `GeoJsonSource` one
  in particular is a plain bug fix that is not RHike-specific.

---

*Last verified: 2026-09-13 against `rhike-patches` at commit `770ccbc`.*

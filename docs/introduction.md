# Introduction

dotPDFium is a wrapper around the Chromium project's PDFium library.

PDFium itself is released under the Apache 2.0 license, while the dotPDFium wrapper is released under the MIT license.

dotPDFium includes binaries for Win x64/ARM64, Linux x64/ARM64 and MacOS x64/ARM64 (as a universal dylib).

This is a very early stage library and we do expect some growing pains. The first step was to get the native code covered via dllimport statements so we could build out the wrapper functionality. That is at around 98% completion with the remaining functions being for Javascript and XLA forms, which we do not intend to support at this time.

If you find the library interesting, we encourage feedback (both good & bad), and would like to have a v1.0 release sometime around the end of Summer 2025. 

The goal is to completely wrap the underlying library in a user-friendly and in a way that is idiomatic to the .NET ecosystem. The truth is that we're going to dogfood it for a few months and see where that takes us.

Documentation is light, but will be forthcoming in stages as we move along.

Help and PRs are encouraged and welcome.

Please enjoy.
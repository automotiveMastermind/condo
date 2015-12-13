---
layout: docs
title: History
group: about
redirect_from: "/about/"
slug: history
---

Condo was born out of the frustration that all of us feel when trying to enable continous integration for projects across
different technologies and platforms.

When [Microsoft](http://www.microsoft.com) first announced that .NET was going open source and would also become cross-platform,
[@dmccaffery](https://github.com/dmccaffery), a C# guy that loves his Mac, was immediately smitten with the idea. While tinkering
with all things CoreCLR and DNX in the early days, he soon realized that the introduction of this new stack would be the perfect opportunity
to bring sanity back into the build system. From that idea, Condo was born.

At its heart, Condo is based on the build used by [Microsoft](http://www.microsoft.com) called [KoreBuild](https://github.com/aspnet/universe).
While this build system is a great starting point, it is specialized to Microsofts needs. Condo seeks to fill in the gaps for web developers with
its native support for package managers like Gem, npm, and bower as well as task runners like Grunt and Gulp.

In addition, Condo is designed to work equally well on any continous integration service, including Visual Studio Team Services Build, AppVeyor,
and Travis-CI. Each of these are fully supported out of the box, and condo can be made to work with any CI system that can set environment variables.
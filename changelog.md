# CHANGE LOG

> All notable changes to this project will be documented in this file.

<a name="2.0.0-beta-00356"></a>
## [2.0.0-beta-00356](https://github.com/pulsebridge/condo/compare/2.0.0-beta-00354...2.0.0-beta-00356) (2017-01-30)


### Bug Fixes

* **changelog:** resolve issue with tags (#32) ([774651c](https://github.com/pulsebridge/condo/commits/774651c)), closes [#32](https://github.com/pulsebridge/condo/issues/32)


<a name="2.0.0-beta-00354"></a>
## [2.0.0-beta-00354](https://github.com/pulsebridge/condo/compare/2.0.0-beta-00287...2.0.0-beta-00354) (2017-01-30)


### Features

* **log:** add support for conventional changelog (#31) ([8f27d5a](https://github.com/pulsebridge/condo/commits/8f27d5a)), closes [#31](https://github.com/pulsebridge/condo/issues/31)


### BREAKING CHANGE

* **log:** 
Any existing bootstrap scripts *MUST* be updated due to some changes in how condo itself is retrieved and built. Replace the bootstrap scripts you rely on (`condo.ps1`, `condo.cmd`, and `condo.ps1`) from [here](https://github.com/pulsebridge/condo/tree/develop/template).

BREAKING CHANGE:
* **log:** 
Condo no longer uses the ```<SemanticVersion>``` tag found in `condo.build`. The version is now based on git tags.


<a name="2.0.0-beta-00287"></a>
## [2.0.0-beta-00287](https://github.com/pulsebridge/condo/compare/2.0.0-beta-00247...2.0.0-beta-00287) (2016-11-17)


### Features

* **dotnet:** add support for dotnet core 1.1 (#30) ([b74275c](https://github.com/pulsebridge/condo/commits/b74275c)), closes [#30](https://github.com/pulsebridge/condo/issues/30)


<a name="2.0.0-beta-00247"></a>
## [2.0.0-beta-00247](https://github.com/pulsebridge/condo/compare/2.0.0-beta-00214...2.0.0-beta-00247) (2016-11-01)


### Bug Fixes

* **dotnet:** pin the version of dotnet as dotnet-test is broken in preview3 (#28) ([1e64e33](https://github.com/pulsebridge/condo/commits/1e64e33)), closes [#28](https://github.com/pulsebridge/condo/issues/28)


<a name="2.0.0-beta-00214"></a>
## [2.0.0-beta-00214](https://github.com/pulsebridge/condo/compare/2.0.0-beta-00196...2.0.0-beta-00214) (2016-10-29)


### Bug Fixes

* do not force push tags (#27) ([b53fb7f](https://github.com/pulsebridge/condo/commits/b53fb7f)), closes [#27](https://github.com/pulsebridge/condo/issues/27)


<a name="2.0.0-beta-00196"></a>
## [2.0.0-beta-00196](https://github.com/pulsebridge/condo/compare/1.0.0-rc-176...2.0.0-beta-00196) (2016-10-29)


### Features

* **git-tag:** add support for version tagging in git repo (#26) ([2c0abce](https://github.com/pulsebridge/condo/commits/2c0abce)), closes [#26](https://github.com/pulsebridge/condo/issues/26)
* **nuget:** add support for nuget push of vsts protected feeds (#18) ([75a7d41](https://github.com/pulsebridge/condo/commits/75a7d41)), closes [#18](https://github.com/pulsebridge/condo/issues/18)
* **windows:** add support for building on windows (#17) ([961090d](https://github.com/pulsebridge/condo/commits/961090d)), closes [#17](https://github.com/pulsebridge/condo/issues/17)
* **dotnet-cli:** replace dnx support with dotnet-cli using msbuild (#16) ([c97c190](https://github.com/pulsebridge/condo/commits/c97c190)), closes [#16](https://github.com/pulsebridge/condo/issues/16) [#12](https://github.com/pulsebridge/condo/issues/12) [#13](https://github.com/pulsebridge/condo/issues/13)


### Bug Fixes

* **dotnet:** support projects where dotnet is not present (#24) ([bf55425](https://github.com/pulsebridge/condo/commits/bf55425)), closes [#24](https://github.com/pulsebridge/condo/issues/24)
* resolve issue where build quality could be incorrect ([2341c71](https://github.com/pulsebridge/condo/commits/2341c71))
* bug in expand when downloading condo from src (#22) ([0a59505](https://github.com/pulsebridge/condo/commits/0a59505)), closes [#22](https://github.com/pulsebridge/condo/issues/22)


<a name="1.0.0-rc-176"></a>
## 1.0.0-rc-176 (2016-07-29)



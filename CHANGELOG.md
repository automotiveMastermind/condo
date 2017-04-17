# CHANGE LOG

> All notable changes to this project will be documented in this file.

<a name="2.0.1-alpha-02825"></a>
## 2.0.1-alpha-02825 (2017-04-17)


### Bug Fixes

* **releases:** detect PR after agent is imported 68eee00
* **nuget-push:** remove trailing slash on windows (#64) 2692f5d, closes #64
* ensure master branch always uses next version (#63) 0777c6b, closes #63


<a name="2.0.0-beta-02716"></a>
## 2.0.0-beta-02716 (2017-04-07)


### Bug Fixes

* **test:** add configuration to dotnet-test (#62) 9c46c5f, closes #62


<a name="2.0.0-beta-02709"></a>
## 2.0.0-beta-02709 (2017-04-07)


### Performance Improvements

* **build:** improve build performance (#61) 693dbb4, closes #61


<a name="2.0.0-beta-02656"></a>
## 2.0.0-beta-02656 (2017-04-04)


### Bug Fixes

* **windows:** fix bootstrapping on windows (#60) b3ce495, closes #60 #59


<a name="2.0.0-beta-02611"></a>
## 2.0.0-beta-02611 (2017-03-28)


<a name="2.0.0-beta-02596"></a>
## 2.0.0-beta-02596 (2017-03-27)


### Bug Fixes

* **install:** fix install on windows (#46) 98993f1, closes #46
* **git:** checkout branch task was missing (#45) 57462c7, closes #45
* **release:** checkout branch due to detached head (#41) 05b4c26, closes #41
* **release:** set author name/email (#40) df032a0, closes #40
* **project-json:** set prerelease tag when appropriate (#36) 671843f, closes #36
* **version:** fix recommended version for initial builds (#34) 7820374, closes #34
* **version:** issue with missing branch properties (#33) bdf2a74, closes #33
* **changelog:** resolve issue with tags (#32) 774651c, closes #32
* **dotnet:** pin the version of dotnet as dotnet-test is broken in preview3 (#28) 1e64e33, closes #28
* do not force push tags (#27) b53fb7f, closes #27
* **dotnet:** support projects where dotnet is not present (#24) bf55425, closes #24
* resolve issue where build quality could be incorrect 2341c71
* bug in expand when downloading condo from src (#22) 0a59505, closes #22


### Features

* **package:** use dotnet nuget in place of custom task (#48) 0e46525, closes #48 #50 dotnet/cli/#6123
* **msbuild:** add support for msbuild project system (#44) 86c588a, closes #44
* **logging:** add msbuild logging everywhere (#38) 3fa4633, closes #38
* **project-json:** update semver in project.json (#35) d874638, closes #35
* **log:** add support for conventional changelog (#31) 8f27d5a, closes #31
* **dotnet:** add support for dotnet core 1.1 (#30) b74275c, closes #30
* **git-tag:** add support for version tagging in git repo (#26) 2c0abce, closes #26
* **nuget:** add support for nuget push of vsts protected feeds (#18) 75a7d41, closes #18
* **windows:** add support for building on windows (#17) 961090d, closes #17
* **dotnet-cli:** replace dnx support with dotnet-cli using msbuild (#16) c97c190, closes #16 #12 #13


### BREAKING CHANGES

* **log:** 
Any existing bootstrap scripts *MUST* be updated due to some changes in how condo itself is retrieved and built. Replace the bootstrap scripts you rely on (`condo.ps1`, `condo.cmd`, and `condo.ps1`) from [here](https://github.com/pulsebridge/condo/tree/develop/template).
* **log:** 
Condo no longer uses the ```<SemanticVersion>``` tag found in `condo.build`. The version is now based on git tags.



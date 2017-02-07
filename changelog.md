# CHANGE LOG

> All notable changes to this project will be documented in this file.

<a name="2.0.0-beta-00384"></a>
## 2.0.0-beta-00384 (2017-02-03)


<a name="2.0.0-beta-00382"></a>
## 2.0.0-beta-00382 (2017-02-02)


<a name="2.0.0-beta-00379"></a>
## 2.0.0-beta-00379 (2017-02-02)


### Bug Fixes

* **release:** checkout branch due to detached head (#41) 05b4c26, closes #41


<a name="2.0.0-beta-00377"></a>
## 2.0.0-beta-00377 (2017-02-02)


### Bug Fixes

* **release:** set author name/email (#40) df032a0, closes #40


<a name="2.0.0-beta-00371"></a>
## 2.0.0-beta-00371 (2017-01-31)


### Features

* **logging:** add msbuild logging everywhere (#38) 3fa4633, closes #38


<a name="2.0.0-beta-00368"></a>
## 2.0.0-beta-00368 (2017-01-30)


<a name="2.0.0-beta-00366"></a>
## 2.0.0-beta-00366 (2017-01-30)


### Features

* **project-json:** update semver in project.json (#35) d874638, closes #35


### Bug Fixes

* **project-json:** set prerelease tag when appropriate (#36) 671843f, closes #36


<a name="2.0.0-beta-00362"></a>
## 2.0.0-beta-00362 (2017-01-30)


### Bug Fixes

* **version:** fix recommended version for initial builds (#34) 7820374, closes #34
* **version:** issue with missing branch properties (#33) bdf2a74, closes #33


<a name="2.0.0-beta-00354"></a>
## 2.0.0-beta-00354 (2017-01-30)


### Features

* **log:** add support for conventional changelog (#31) 8f27d5a, closes #31


### BREAKING CHANGE

* **log:** 
Any existing bootstrap scripts *MUST* be updated due to some changes in how condo itself is retrieved and built. Replace the bootstrap scripts you rely on (`condo.ps1`, `condo.cmd`, and `condo.ps1`) from [here](https://github.com/pulsebridge/condo/tree/develop/template).

BREAKING CHANGE:
* **log:** 
Condo no longer uses the ```<SemanticVersion>``` tag found in `condo.build`. The version is now based on git tags.


<a name="2.0.0-beta-00287"></a>
## 2.0.0-beta-00287 (2016-11-17)


### Features

* **dotnet:** add support for dotnet core 1.1 (#30) b74275c, closes #30


<a name="2.0.0-beta-00247"></a>
## 2.0.0-beta-00247 (2016-11-01)


### Bug Fixes

* **dotnet:** pin the version of dotnet as dotnet-test is broken in preview3 (#28) 1e64e33, closes #28


<a name="2.0.0-beta-00214"></a>
## 2.0.0-beta-00214 (2016-10-29)


### Bug Fixes

* do not force push tags (#27) b53fb7f, closes #27


<a name="2.0.0-beta-00196"></a>
## 2.0.0-beta-00196 (2016-10-29)


### Features

* **git-tag:** add support for version tagging in git repo (#26) 2c0abce, closes #26
* **nuget:** add support for nuget push of vsts protected feeds (#18) 75a7d41, closes #18
* **windows:** add support for building on windows (#17) 961090d, closes #17
* **dotnet-cli:** replace dnx support with dotnet-cli using msbuild (#16) c97c190, closes #16 #12 #13


### Bug Fixes

* **dotnet:** support projects where dotnet is not present (#24) bf55425, closes #24
* resolve issue where build quality could be incorrect 2341c71
* bug in expand when downloading condo from src (#22) 0a59505, closes #22


<a name="2.0.0-alpha-02030"></a>
## 2.0.0-alpha-02030 (2017-01-30)


### Bug Fixes

* **changelog:** resolve issue with tags (#32) 774651c, closes #32


<a name="2.0.0-alpha-00398"></a>
## 2.0.0-alpha-00398 (2017-02-06)


<a name="2.0.0-alpha-00397"></a>
## 2.0.0-alpha-00397 (2017-02-06)


<a name="2.0.0-alpha-00396"></a>
## 2.0.0-alpha-00396 (2017-02-06)


<a name="2.0.0-alpha-00395"></a>
## 2.0.0-alpha-00395 (2017-02-06)


<a name="2.0.0-alpha-00394"></a>
## 2.0.0-alpha-00394 (2017-02-06)


<a name="2.0.0-alpha-00389"></a>
## 2.0.0-alpha-00389 (2017-02-03)


### Features

* **release:** add a unique remote for push 7a06dbe


<a name="1.0.0-rc-176"></a>
## 1.0.0-rc-176 (2016-07-29)



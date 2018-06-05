# CHANGE LOG

> All notable changes to this project will be documented in this file.
# [2.0.0](https://github.com/automotiveMastermind/condo.git/compare/1.0.0...2.0.0) (2018-06-04)


### Bug Fixes

* **build:** remove legacy msbuild myget feed (#183) ([4d6f9a2](https://github.com/automotiveMastermind/condo.git/commits/4d6f9a2)), closes [#183](https://github.com/automotiveMastermind/condo.git/issues/183)
* **changelog:** resolve issue with tags (#32) ([774651c](https://github.com/automotiveMastermind/condo.git/commits/774651c)), closes [#32](https://github.com/automotiveMastermind/condo.git/issues/32)
* **changelog:** update changelog generation (#186) ([d35ee76](https://github.com/automotiveMastermind/condo.git/commits/d35ee76)), closes [#186](https://github.com/automotiveMastermind/condo.git/issues/186) [#109](https://github.com/automotiveMastermind/condo.git/issues/109)
* **cli:** capture exit after build (#154) ([3bf2d30](https://github.com/automotiveMastermind/condo.git/commits/3bf2d30)), closes [#154](https://github.com/automotiveMastermind/condo.git/issues/154)
* **detect-pr:** emit detection of pull requests (#68) ([e791845](https://github.com/automotiveMastermind/condo.git/commits/e791845)), closes [#68](https://github.com/automotiveMastermind/condo.git/issues/68)
* **docker:** resolve issue with required build quality (#139) ([9aa0a19](https://github.com/automotiveMastermind/condo.git/commits/9aa0a19)), closes [#139](https://github.com/automotiveMastermind/condo.git/issues/139) [#138](https://github.com/automotiveMastermind/condo.git/issues/138)
* **dotnet:** support projects where dotnet is not present (#24) ([bf55425](https://github.com/automotiveMastermind/condo.git/commits/bf55425)), closes [#24](https://github.com/automotiveMastermind/condo.git/issues/24)
* **dotnet:** pin the version of dotnet as dotnet-test is broken in preview3 (#28) ([1e64e33](https://github.com/automotiveMastermind/condo.git/commits/1e64e33)), closes [#28](https://github.com/automotiveMastermind/condo.git/issues/28)
* **dotnet:** save assembly info after prepare (#171) ([b3a3ff3](https://github.com/automotiveMastermind/condo.git/commits/b3a3ff3)), closes [#171](https://github.com/automotiveMastermind/condo.git/issues/171)
* **dotnet:** download and build on windows (#180) ([f93cdc3](https://github.com/automotiveMastermind/condo.git/commits/f93cdc3)), closes [#180](https://github.com/automotiveMastermind/condo.git/issues/180)
* **git:** checkout branch task was missing (#45) ([57462c7](https://github.com/automotiveMastermind/condo.git/commits/57462c7)), closes [#45](https://github.com/automotiveMastermind/condo.git/issues/45)
* **install:** fix install on windows (#46) ([98993f1](https://github.com/automotiveMastermind/condo.git/commits/98993f1)), closes [#46](https://github.com/automotiveMastermind/condo.git/issues/46)
* **nuget-push:** remove trailing slash on windows (#64) ([2692f5d](https://github.com/automotiveMastermind/condo.git/commits/2692f5d)), closes [#64](https://github.com/automotiveMastermind/condo.git/issues/64)
* **prepare:** set docker required properly (#167) ([238ab76](https://github.com/automotiveMastermind/condo.git/commits/238ab76)), closes [#167](https://github.com/automotiveMastermind/condo.git/issues/167)
* **project-json:** set prerelease tag when appropriate (#36) ([671843f](https://github.com/automotiveMastermind/condo.git/commits/671843f)), closes [#36](https://github.com/automotiveMastermind/condo.git/issues/36)
* **publish:** improve publish target detection (#69) ([42b1c9c](https://github.com/automotiveMastermind/condo.git/commits/42b1c9c)), closes [#69](https://github.com/automotiveMastermind/condo.git/issues/69) [#66](https://github.com/automotiveMastermind/condo.git/issues/66)
* **publish:** issue where publish was always true (#151) ([63d5074](https://github.com/automotiveMastermind/condo.git/commits/63d5074)), closes [#151](https://github.com/automotiveMastermind/condo.git/issues/151)
* **publish:** non-netstandard projects should publish (#152) ([9acad0d](https://github.com/automotiveMastermind/condo.git/commits/9acad0d)), closes [#152](https://github.com/automotiveMastermind/condo.git/issues/152)
* **release:** set author name/email (#40) ([df032a0](https://github.com/automotiveMastermind/condo.git/commits/df032a0)), closes [#40](https://github.com/automotiveMastermind/condo.git/issues/40)
* **release:** checkout branch due to detached head (#41) ([05b4c26](https://github.com/automotiveMastermind/condo.git/commits/05b4c26)), closes [#41](https://github.com/automotiveMastermind/condo.git/issues/41)
* **release:** resolve issue with release tags and changelog (#143) ([53694a7](https://github.com/automotiveMastermind/condo.git/commits/53694a7)), closes [#143](https://github.com/automotiveMastermind/condo.git/issues/143)
* **release:** do not push assets before tagging (#164) ([e40a095](https://github.com/automotiveMastermind/condo.git/commits/e40a095)), closes [#164](https://github.com/automotiveMastermind/condo.git/issues/164)
* **restore:** ignore failed sources on initial restore (#72) ([ff79b2f](https://github.com/automotiveMastermind/condo.git/commits/ff79b2f)), closes [#72](https://github.com/automotiveMastermind/condo.git/issues/72)
* **scripts:** remove unnecessary docker daemon parameter (#146) ([afaf452](https://github.com/automotiveMastermind/condo.git/commits/afaf452)), closes [#146](https://github.com/automotiveMastermind/condo.git/issues/146)
* **scripts:** skip install of dotnet v1 when host running ubuntu 16.10 or greater (#179) ([e9bb432](https://github.com/automotiveMastermind/condo.git/commits/e9bb432)), closes [#179](https://github.com/automotiveMastermind/condo.git/issues/179)
* **targets:** default build quality override (#190) ([7e4287b](https://github.com/automotiveMastermind/condo.git/commits/7e4287b)), closes [#190](https://github.com/automotiveMastermind/condo.git/issues/190)
* **targets:** detect pull requests correctly (#193) ([95a6cac](https://github.com/automotiveMastermind/condo.git/commits/95a6cac)), closes [#193](https://github.com/automotiveMastermind/condo.git/issues/193)
* **tasks:** properly generate random folder on all .net OS's (#187) ([c159965](https://github.com/automotiveMastermind/condo.git/commits/c159965)), closes [#187](https://github.com/automotiveMastermind/condo.git/issues/187)
* **test:** add configuration to dotnet-test (#62) ([9c46c5f](https://github.com/automotiveMastermind/condo.git/commits/9c46c5f)), closes [#62](https://github.com/automotiveMastermind/condo.git/issues/62)
* **version:** issue with missing branch properties (#33) ([bdf2a74](https://github.com/automotiveMastermind/condo.git/commits/bdf2a74)), closes [#33](https://github.com/automotiveMastermind/condo.git/issues/33)
* **version:** fix recommended version for initial builds (#34) ([7820374](https://github.com/automotiveMastermind/condo.git/commits/7820374)), closes [#34](https://github.com/automotiveMastermind/condo.git/issues/34)
* **version:** include changelog in gitignore (#147) ([b10ec2c](https://github.com/automotiveMastermind/condo.git/commits/b10ec2c)), closes [#147](https://github.com/automotiveMastermind/condo.git/issues/147)
* **vs:** make condo work from vs on windows (#150) ([76613bf](https://github.com/automotiveMastermind/condo.git/commits/76613bf)), closes [#150](https://github.com/automotiveMastermind/condo.git/issues/150)
* **windows:** fix bootstrapping on windows (#60) ([b3ce495](https://github.com/automotiveMastermind/condo.git/commits/b3ce495)), closes [#60](https://github.com/automotiveMastermind/condo.git/issues/60) [#59](https://github.com/automotiveMastermind/condo.git/issues/59)
* **windows:** fix condo script on windows (#162) ([feccbdb](https://github.com/automotiveMastermind/condo.git/commits/feccbdb)), closes [#162](https://github.com/automotiveMastermind/condo.git/issues/162)
* bug in expand when downloading condo from src (#22) ([0a59505](https://github.com/automotiveMastermind/condo.git/commits/0a59505)), closes [#22](https://github.com/automotiveMastermind/condo.git/issues/22)
* resolve issue where build quality could be incorrect ([2341c71](https://github.com/automotiveMastermind/condo.git/commits/2341c71))
* do not force push tags (#27) ([b53fb7f](https://github.com/automotiveMastermind/condo.git/commits/b53fb7f)), closes [#27](https://github.com/automotiveMastermind/condo.git/issues/27)
* ensure master branch always uses next version (#63) ([0777c6b](https://github.com/automotiveMastermind/condo.git/commits/0777c6b)), closes [#63](https://github.com/automotiveMastermind/condo.git/issues/63)
* set execution bit of docker.sh (#161) ([73d9d7d](https://github.com/automotiveMastermind/condo.git/commits/73d9d7d)), closes [#161](https://github.com/automotiveMastermind/condo.git/issues/161)
* do not run bower/poly when unnecessary (#166) ([9d2109b](https://github.com/automotiveMastermind/condo.git/commits/9d2109b)), closes [#166](https://github.com/automotiveMastermind/condo.git/issues/166)
* print dotnet projects (#168) ([24da620](https://github.com/automotiveMastermind/condo.git/commits/24da620)), closes [#168](https://github.com/automotiveMastermind/condo.git/issues/168)
* dotnet detection (#169) ([6c20ee8](https://github.com/automotiveMastermind/condo.git/commits/6c20ee8)), closes [#169](https://github.com/automotiveMastermind/condo.git/issues/169)
* dotnet project print list (#170) ([b7f59b4](https://github.com/automotiveMastermind/condo.git/commits/b7f59b4)), closes [#170](https://github.com/automotiveMastermind/condo.git/issues/170)
* fix poject directory in polymer metadata (#172) ([2c0d43a](https://github.com/automotiveMastermind/condo.git/commits/2c0d43a)), closes [#172](https://github.com/automotiveMastermind/condo.git/issues/172)
* msbuildversion should be buildversion in csproj (#181) ([d5cecb3](https://github.com/automotiveMastermind/condo.git/commits/d5cecb3)), closes [#181](https://github.com/automotiveMastermind/condo.git/issues/181)
* release vs publish semantics (#191) ([872cc9c](https://github.com/automotiveMastermind/condo.git/commits/872cc9c)), closes [#191](https://github.com/automotiveMastermind/condo.git/issues/191)
* continue to include 2.1.105 sdk for now (#200) ([47c643d](https://github.com/automotiveMastermind/condo.git/commits/47c643d)), closes [#200](https://github.com/automotiveMastermind/condo.git/issues/200)


### Features

* **build:** add dind for circle-ci (#158) ([7c86947](https://github.com/automotiveMastermind/condo.git/commits/7c86947)), closes [#158](https://github.com/automotiveMastermind/condo.git/issues/158)
* **build:** publish dockerized condo (#160) ([f071df7](https://github.com/automotiveMastermind/condo.git/commits/f071df7)), closes [#160](https://github.com/automotiveMastermind/condo.git/issues/160)
* **clean:** allow skip clean (#178) ([71ce39b](https://github.com/automotiveMastermind/condo.git/commits/71ce39b)), closes [#178](https://github.com/automotiveMastermind/condo.git/issues/178)
* **cli:** builds can now be done through docker (#153) ([8782781](https://github.com/automotiveMastermind/condo.git/commits/8782781)), closes [#153](https://github.com/automotiveMastermind/condo.git/issues/153)
* **docfx:** add support for docfx (#65) ([ae95308](https://github.com/automotiveMastermind/condo.git/commits/ae95308)), closes [#65](https://github.com/automotiveMastermind/condo.git/issues/65)
* **docker:** add support for building docker containers (#115) ([ef38da4](https://github.com/automotiveMastermind/condo.git/commits/ef38da4)), closes [#115](https://github.com/automotiveMastermind/condo.git/issues/115) [#112](https://github.com/automotiveMastermind/condo.git/issues/112)
* **docker:** add docker push support (#130) ([bdc6fa6](https://github.com/automotiveMastermind/condo.git/commits/bdc6fa6)), closes [#130](https://github.com/automotiveMastermind/condo.git/issues/130)
* **docs:** add support for github pages (#71) ([2863792](https://github.com/automotiveMastermind/condo.git/commits/2863792)), closes [#71](https://github.com/automotiveMastermind/condo.git/issues/71) [#82](https://github.com/automotiveMastermind/condo.git/issues/82)
* **docs:** make docfx great again (#137) ([167373a](https://github.com/automotiveMastermind/condo.git/commits/167373a)), closes [#137](https://github.com/automotiveMastermind/condo.git/issues/137) [#136](https://github.com/automotiveMastermind/condo.git/issues/136)
* **dotnet:** add support for dotnet core 1.1 (#30) ([b74275c](https://github.com/automotiveMastermind/condo.git/commits/b74275c)), closes [#30](https://github.com/automotiveMastermind/condo.git/issues/30)
* **dotnet:** update dotnet to latest (#86) ([dce3e99](https://github.com/automotiveMastermind/condo.git/commits/dce3e99)), closes [#86](https://github.com/automotiveMastermind/condo.git/issues/86)
* **dotnet:** add support for legacy .net framework (#133) ([d0aea10](https://github.com/automotiveMastermind/condo.git/commits/d0aea10)), closes [#133](https://github.com/automotiveMastermind/condo.git/issues/133) [#132](https://github.com/automotiveMastermind/condo.git/issues/132)
* **dotnet:** use latest sdk for .NET (#135) ([ad4af64](https://github.com/automotiveMastermind/condo.git/commits/ad4af64)), closes [#135](https://github.com/automotiveMastermind/condo.git/issues/135) [#134](https://github.com/automotiveMastermind/condo.git/issues/134)
* **dotnet:** upgrade to core 2.1 (#199) ([4e5fc2f](https://github.com/automotiveMastermind/condo.git/commits/4e5fc2f)), closes [#199](https://github.com/automotiveMastermind/condo.git/issues/199)
* **dotnet-cli:** replace dnx support with dotnet-cli using msbuild (#16) ([c97c190](https://github.com/automotiveMastermind/condo.git/commits/c97c190)), closes [#16](https://github.com/automotiveMastermind/condo.git/issues/16) [#12](https://github.com/automotiveMastermind/condo.git/issues/12) [#13](https://github.com/automotiveMastermind/condo.git/issues/13)
* **git:** add support for auth headers for clones (#145) ([77f18e9](https://github.com/automotiveMastermind/condo.git/commits/77f18e9)), closes [#145](https://github.com/automotiveMastermind/condo.git/issues/145)
* **git-tag:** add support for version tagging in git repo (#26) ([2c0abce](https://github.com/automotiveMastermind/condo.git/commits/2c0abce)), closes [#26](https://github.com/automotiveMastermind/condo.git/issues/26)
* **log:** add support for conventional changelog (#31) ([8f27d5a](https://github.com/automotiveMastermind/condo.git/commits/8f27d5a)), closes [#31](https://github.com/automotiveMastermind/condo.git/issues/31)
* **logging:** add msbuild logging everywhere (#38) ([3fa4633](https://github.com/automotiveMastermind/condo.git/commits/3fa4633)), closes [#38](https://github.com/automotiveMastermind/condo.git/issues/38)
* **metadata:** add support for skipping test projects in condo (#114) ([f56ccce](https://github.com/automotiveMastermind/condo.git/commits/f56ccce)), closes [#114](https://github.com/automotiveMastermind/condo.git/issues/114)
* **metadata:** support msbuild imports (#149) ([75a6271](https://github.com/automotiveMastermind/condo.git/commits/75a6271)), closes [#149](https://github.com/automotiveMastermind/condo.git/issues/149)
* **msbuild:** add support for msbuild project system (#44) ([86c588a](https://github.com/automotiveMastermind/condo.git/commits/86c588a)), closes [#44](https://github.com/automotiveMastermind/condo.git/issues/44)
* **msbuild:** add task to clone repo (#141) ([a2bbf39](https://github.com/automotiveMastermind/condo.git/commits/a2bbf39)), closes [#141](https://github.com/automotiveMastermind/condo.git/issues/141) [#140](https://github.com/automotiveMastermind/condo.git/issues/140)
* **node:** add node build/test support (#165) ([d830d25](https://github.com/automotiveMastermind/condo.git/commits/d830d25)), closes [#165](https://github.com/automotiveMastermind/condo.git/issues/165)
* **nuget:** add support for nuget push of vsts protected feeds (#18) ([75a7d41](https://github.com/automotiveMastermind/condo.git/commits/75a7d41)), closes [#18](https://github.com/automotiveMastermind/condo.git/issues/18)
* **package:** use dotnet nuget in place of custom task (#48) ([0e46525](https://github.com/automotiveMastermind/condo.git/commits/0e46525)), closes [#48](https://github.com/automotiveMastermind/condo.git/issues/48) [#50](https://github.com/automotiveMastermind/condo.git/issues/50) [dotnet/cli/#6123](https://github.com/automotiveMastermind/condo.git/issues/6123)
* **polymer:** add polymer install and build (#118) ([d8bcbe5](https://github.com/automotiveMastermind/condo.git/commits/d8bcbe5)), closes [#118](https://github.com/automotiveMastermind/condo.git/issues/118) [#105](https://github.com/automotiveMastermind/condo.git/issues/105)
* **project-json:** update semver in project.json (#35) ([d874638](https://github.com/automotiveMastermind/condo.git/commits/d874638)), closes [#35](https://github.com/automotiveMastermind/condo.git/issues/35)
* **publish:** do not push during pull request (#157) ([8b33681](https://github.com/automotiveMastermind/condo.git/commits/8b33681)), closes [#157](https://github.com/automotiveMastermind/condo.git/issues/157)
* **targets:** allow build quality to be set (#189) ([2b76d36](https://github.com/automotiveMastermind/condo.git/commits/2b76d36)), closes [#189](https://github.com/automotiveMastermind/condo.git/issues/189)
* **tasks:** allow custom encoding in save file (#188) ([3cb5012](https://github.com/automotiveMastermind/condo.git/commits/3cb5012)), closes [#188](https://github.com/automotiveMastermind/condo.git/issues/188)
* **test:** add filtering by category (#70) ([3b75a8e](https://github.com/automotiveMastermind/condo.git/commits/3b75a8e)), closes [#70](https://github.com/automotiveMastermind/condo.git/issues/70)
* **versioning:** add envars for release versions (#111) ([04452c8](https://github.com/automotiveMastermind/condo.git/commits/04452c8)), closes [#111](https://github.com/automotiveMastermind/condo.git/issues/111)
* **windows:** add support for building on windows (#17) ([961090d](https://github.com/automotiveMastermind/condo.git/commits/961090d)), closes [#17](https://github.com/automotiveMastermind/condo.git/issues/17)
* update dotnet sdk versions (#163) ([9f30070](https://github.com/automotiveMastermind/condo.git/commits/9f30070)), closes [#163](https://github.com/automotiveMastermind/condo.git/issues/163)


### Performance Improvements

* **build:** improve build performance (#61) ([693dbb4](https://github.com/automotiveMastermind/condo.git/commits/693dbb4)), closes [#61](https://github.com/automotiveMastermind/condo.git/issues/61)
* **dotnet:** opt out of dotnet cli telemetry (#92) ([3822d5f](https://github.com/automotiveMastermind/condo.git/commits/3822d5f)), closes [#92](https://github.com/automotiveMastermind/condo.git/issues/92)
* **dotnet:** disable xml generation on nuget restore (#93) ([75e37b9](https://github.com/automotiveMastermind/condo.git/commits/75e37b9)), closes [#93](https://github.com/automotiveMastermind/condo.git/issues/93)
* **dotnet:** opt out of dotnet first run experience on build (#97) ([4ca1ca7](https://github.com/automotiveMastermind/condo.git/commits/4ca1ca7)), closes [#97](https://github.com/automotiveMastermind/condo.git/issues/97)


### BREAKING CHANGES

* **log:** 
Condo no longer uses the ```<SemanticVersion>``` tag found in `condo.build`. The version is now based on git tags.
* **log:** 
Any existing bootstrap scripts *MUST* be updated due to some changes in how condo itself is retrieved and built. Replace the bootstrap scripts you rely on (`condo.ps1`, `condo.cmd`, and `condo.ps1`) from [here](https://github.com/pulsebridge/condo/tree/develop/template).


# 1.0.0 (2016-07-08)



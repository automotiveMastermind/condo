# Condo lifecycle
Condo's lifecycle is comprised of a series of tasks that get executed sequentially, given success of previous tasks.
When the first task target is called, the dependencies are checked.

## Default implementation
The default implementation of the condo lifecycle is as follows:

1. Clean
2. Bootstrap
3. Initialize
4. Version
5. Prepare
6. Compile
7. Test
8. Package
9. Verify
10. Document
11. Publish

Task         | Description                                                               | Depends on
-------------|---------------------------------------------------------------------------|----------------------------
Clean        | Default target for clean (removes artifacts and intermediate artifacts)   | N/A
Bootstrap    | Developer bootstrapping                                                   | N/A
Initialize   | Initializes dynamic properties                                            | Clean
Version      | Semantic versioning                                                       | Initialize
Prepare      | Prepare for compilation: usually for executing restore operations <br>or executing task runners | Version
Compile      | Compile the project                                                       | Prepare
Test         | Execute tests for the project                                             | Compile
Package      | Perform post test packaging of the project                                | Test
Verify       | Verify build output and test results                                      | Package
Document     | Generate documentation from build output                                  | Version
Publish      | Publish final build artifacts                                             | Verify, Document


## Build lifecycle
When you call ```./condo.sh``` on your condo project, you will run the configurable ```condo.build``` file. If you are running the default build, as specified in ```lifecycle.targets```, the first target that gets called is the ```Build``` target, which in turn depends on the ```Package``` task.

An execution stack is created, and can be represented as such:

|Stack Position | Target            | Dependency Task |
|:-------------:|-------------------|-----------------|
|1              |Build              | Package         |


The ```Package``` task's dependencies are checked next. ```Package``` depends on ```Test```, and that dependency is consequently pushed onto the stack.

The stack can be represented as such:

|Stack Position | Target            | Dependency Task |
|:-------------:|-------------------|-----------------|
|2              |Package            | Test            |
|1              |Build              | Package         |


```Test``` in turn depends on ```Compile```, which depends on ```Prepare```, which depends on ```Version```, which depends on ```Initialize```, which depends on ```Clean```.

The resulting stack can be represented as such:

|Stack Position | Target            | Dependency Task |
|:-------------:|-------------------|-----------------|
|8              |Clean              | N/A             |
|7              |Initialize         | Clean           |
|6              |Version            | Initialize      |
|5              |Prepare            | Version         |
|4              |Compile            | Prepare         |
|3              |Test               | Compile         |
|2              |Package            | Test            |
|1              |Build              | Package         |


Given LIFO, the stack unravels as each task is completed successfully. ```Clean``` is called, followed by ```Initialize```, followed by ```Version```, followed by ```Prepare```, followed by ```Compile```, followed by ```Test```, followed by ```Package```, followed by ```Build```.


## Publish lifecycle
The ```Publish``` lifecycle closely follows the ```Build``` lifecycle. ```Publish``` first depends on ```Verify``` and ```Document```. ```Verify``` depends on ```Package```, and ```Document``` depends on ```Version```.

The resulting stack can be represented as such:

|Stack Position | Target            | Dependency Task |
|:-------------:|-------------------|-----------------|
|10             |Clean              | N/A             |
|9              |Initialize         | Clean           |
|8              |Version            | Initialize      |
|7              |Prepare            | Version         |
|6              |Compile            | Prepare         |
|5              |Test               | Compile         |
|4              |Package            | Test            |
|3              |Verify             | Package         |
|2              |Document           | Version         |
|1              |Publish            | Verify, Document|


## Copright and License

&copy; automotiveMastermind and contributors. Distributed under the MIT license. See [LICENSE][] for details.

[license-image]: https://img.shields.io/badge/license-MIT-blue.svg
[license]: LICENSE

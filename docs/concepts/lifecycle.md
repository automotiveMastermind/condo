# Condo lifecycle
Condo's lifecycle is comprised of a series of tasks that get executed sequentially, given success of previous tasks.
When the first task target is called, the dependencies are checked. If the dependency has not been been resolved
successfully, it is p

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
Clean        | default target for clean (removes artifacts and intermediate artifacts)   | N/A
Bootstrap    | developer bootstrapping                                                   | N/A
Initialize   | initializes dynamic properties                                            | Clean
Version      | semantic versioning                                                       | Initialize
Prepare      | prepare for compilation: usually for executing restore operations <br>or executing task runners | Version
Compile      | compile the project                                                       | Prepare
Test         | execute tests for the project                                             | Version
Package      | perform post test packaging of the project                                | Test
Verify       | verify build output and test results                                      | Package
Document     | generate documentation from build output                                  | Version
Publish      | publish final build artifacts                                             | Verify, Document


## Build lifecycle
When you call ```./condo.sh``` on your condo project, you will run the configurable condo.build file.

The first task that gets called is the ```build``` task, which depends on the ```Package``` task.


## Publish lifecycle



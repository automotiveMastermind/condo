# Getting started with condo (setup and configuration)

1. Get the necessary files:

    Copy the four files in the [`template`][template] folder and add them to the root folder of your project.
    ```
    condo.build
    condo.cmd
    condo.ps1
    condo.sh
    ```

    `condo.build` imports the targets to be executed.
    `condo.sh` and `condo.cmd` will set path variables and run the build and execute the condo shell with developer
    specified arguments. Depending on specified parameters (outlined and set in `condo.ps1`), condo will execute the
    target task.

    If the target is not specified, the [default implementation][lifecycle] of the lifecycle will be executed.


2. Edit the `condo.build` config file:

    Configure some stuff here. Learn more about condo's default implementation outlined in the
    [lifecycle docs][lifecycle].

3. Run the build:

	OS X / Linux:

	```bash
	./condo.sh
	```

	Windows (CLI):

	```cmd
	condo
	```

	Windows (PoSH):
	```posh
	./condo.ps1
	```


[template]: (../../template)
[lifecycle]: (../concepts/lifecycle.md)

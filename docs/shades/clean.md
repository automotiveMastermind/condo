---
layout: docs
title: clean
group: shades
---

@{/*

clean
    Cleans a project.

default clean_project=''
    Path to the project.json file to clean.

default clean_path=''
    Path to the folder containing the project to clean.

*/}

use namespace = 'System'
use namespace = 'System.IO'

default clean_project = ''
default clean_path = ''

@{
    Build.Log.Header("clean");

    var final_clean_path = !string.IsNullOrEmpty(clean_project) ? Path.GetDirectoryName(clean_project) : clean_path;

    if (string.IsNullOrEmpty(final_clean_path))
    {
        throw new ArgumentException("clean: no project or path was specified.", "clean_path");
    }

    Build.Log.Argument("path", final_clean_path);
    Build.Log.Header();
}

directory delete='${ Path.Combine(final_clean_path, "bin") }'
directory delete='${ Path.Combine(final_clean_path, "obj") }'